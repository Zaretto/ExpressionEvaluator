using System;
using System.Collections.Generic;
namespace Evaluator
{
    public class Eval
    {
        /* Expression evaluator
         * Takes a string expression and return either a double value.
         * --------------------------------
         *  This is originally based on the C version I wrote on my Amiga in 1987, 
         *  which ended up in symon and this version was based on a BBC Basic version
         *  which in turn was based on an understanding gained from disassembling
         *  the BBC Basic ROM (6502) in 1981, so credit is due to Richard Florance 
         *  who figured it out first and explained it to me and also to Roger Wilson
         *  who built it in the first place.
         *  ---
         *  For the C# version I've finally fixed the bug that broke the evaluation order
         *  for operators at the same precedence, originally it was simply recursing back to
         *  the start so 10-20+30 would be evaulated as 10-(20+30). 
         * 
         *	Author:	R.J.Harrison		Date	15-December-1987 
         *	        R.J.Harrison		Date	01-May-2013 - ported from C to C#
        */

        /// <summary>
        /// Floating point accumulator - i.e current value
        /// </summary>
        /// <remarks>following BBC basic naming conventions.</remarks>
        double fac = 0;

        /// <summary>
        /// the recursion depth - which generally is the same as the bracket level i.e.
        /// X+(A-B)
        /// depth = 1 then (2) whilst evaluating A-B. Again for context in the GetSymbol
        /// virtual method.
        /// </summary>
        protected int CurrentDepth;

        /// <summary>
        /// the string being parseed.
        /// </summary>
        string Expression;

        /// <summary>
        /// position within instring of the current parse location
        /// </summary>
        int ExpressionPosition = 0;

        /// <summary>
        /// The operator symbol for the current operation; can be useful in GetSymbol to ascertain context.
        /// This is only valid after level6 has been reached
        /// </summary>
        protected char CurrentOperator;

        /// <summary>
        /// Symbol dictionary, i.e. variables and their value. 
        /// </summary>
        /// <remarks>
        /// This is also what we used to call it at Link-Miles, although 
        /// at Rediffusion they called the same thing DATAPOOL. I think symbol dictionary
        /// is more elegant a name, and maps nicely onto the C# world because we're using 
        /// a Dictionary to store it.
        /// </remarks>
        Dictionary<string, double> SymbolDictionary = new Dictionary<string, double>();

        public virtual void SetSymbol(string name, double val)
        {
            SymbolDictionary[name] = val;
        }

        public virtual double GetSymbol(string name, char Operator)
        {
            return SymbolDictionary[name];
        }

        /// <summary>
        /// main entry point
        /// </summary>
        /// <remarks>
        /// This works by having different levels each of which is a set of operators at the same
        /// precedence. The Operator that is currently being parsed is returned, it starts at null
        /// and is reset to null when a bracketed expression has been parsed.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public double Evaluate(String input)
        {
            Expression = input;
            ExpressionPosition = 0;
            CurrentDepth = 0;
            level1('\0');
            return fac;
        }

        /// <summary>
        /// no idea why we have two blank levels at the beginning - but it's always been like
        /// that since 1987, so I'll have to figure out what's missing and then either implement
        /// them or remove.
        /// </summary>
        /// <param name="Operator"></param>
        /// <returns></returns>
        char level1(char Operator)
        {
            CurrentDepth++;
            do
            {
                Operator = level2(Operator);
            }
            while (Operator > 0);
            return Operator;
        }

        char level2(char Operator)
        {
            return level3(Operator);
        }

        char level3(char Operator)
        {
            if (Operator == '+')
            {
                var cur_fac = fac;
                Operator = level4(Operator);
                fac = cur_fac + fac;
                return Operator;
            }
            if (Operator == '-')
            {
                var cur_fac = fac; ;
                Operator = level4(Operator);
                fac = cur_fac - fac;
                return Operator;
            }
            return level4(Operator);
        }

        char level4(char Operator)
        {
            if (Operator == '*')
            {
                var cur_fac = fac; ;
                Operator = level5(Operator);
                fac = cur_fac * fac;
                return Operator;
            }
            if (Operator == '/')
            {
                var cur_fac = fac; ;
                Operator = level5(Operator);
                fac = cur_fac / fac;
                return Operator;
            }
            return level5(Operator);
        }

        char level5(char Operator)
        {
            if (Operator == '^')
            {
                var cur_fac = fac; ;
                Operator = level6(Operator);
                fac = Math.Pow(cur_fac, fac);
                return Operator;
            }
            return level6(Operator);
        }

        char nextOperator()
        {
            char Operator;
            while (ExpressionPosition < Expression.Length && Char.IsWhiteSpace(Expression[ExpressionPosition]))
                ExpressionPosition++;
            if (ExpressionPosition < Expression.Length)
                Operator = Expression[ExpressionPosition++];
            else
            {
                Operator = '\0';
            }
            return Operator;
        }
        char level6(char Operator)
        {
            /*
             * see if end of bracketed expression
             */
            CurrentOperator = Operator;
            if (Operator == ')')
                return '\0';

            // at this point we clear the fac as this level will hopefully find a new value for it
            fac = 0;


            if (ExpressionPosition >= Expression.Length)
                return '\0';

            while (ExpressionPosition < Expression.Length && Char.IsWhiteSpace(Expression[ExpressionPosition]))
                ExpressionPosition++;

            // the current Operator (symbol).
            Operator = Expression[ExpressionPosition];

            /*
             * handle variables
             */
            if (Operator == '\'')
            {
                int end = ++ExpressionPosition;
                while (end < Expression.Length && Expression[end] != '\'')
                {
                    end++;
                }
                var sv = Expression.Substring(ExpressionPosition, end - ExpressionPosition);
                ExpressionPosition = end + 1;
                // this will throw an exception if not found. 
                Operator = nextOperator();
                fac = GetSymbol(sv, Operator);
            }
            else if (Char.IsLetter(Operator))
            {
                int end = ExpressionPosition;
                while (end < Expression.Length && (Expression[end] == '.' || Char.IsLetterOrDigit(Expression[end])))
                {
                    end++;
                }
                var sv = Expression.Substring(ExpressionPosition, end - ExpressionPosition);
                ExpressionPosition = end;
                // this will throw an exception if not found. 
                Operator = nextOperator();
                fac = GetSymbol(sv, Operator);
            }
            else
            {
                // handle any as yet unprocessed Operators; the aim of this
                // is to get a value and store it in the FAC.
                switch (Operator)
                {
                    case '(':
                        ExpressionPosition++;
                        level1(Expression[ExpressionPosition]);
                        break;

                    default:
                        {
                            var end = ExpressionPosition;
                            bool can_negate = true;
                            while (end < Expression.Length
                                   && (Char.IsDigit(Expression[end]) || Expression[end] == '.' || (can_negate && Expression[end] == '-')))
                            {
                                can_negate = false;
                                end++;
                            }
                            var sv = Expression.Substring(ExpressionPosition, end - ExpressionPosition);
                            ExpressionPosition = end;
                            fac = Double.Parse(sv);
                            break;
                        }
                }
                Operator = nextOperator();
            }
            return Operator;
        }
    }

}