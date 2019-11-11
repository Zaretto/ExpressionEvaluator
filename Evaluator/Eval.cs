using System;
using System.Collections.Generic;
namespace Evaluator
{
    public class Eval
    {
        public Eval()
        {
            RegisterFunction("abs", Math.Abs);
            RegisterFunction("cos", Math.Cos);
            RegisterFunction("sin", Math.Sin);
            RegisterFunction("acos", Math.Acos);
            RegisterFunction("asin", Math.Asin);
            RegisterFunction("atan", Math.Atan);
            RegisterFunction("sinh", Math.Sinh);
            RegisterFunction("sqrt", Math.Sqrt);
            RegisterFunction("tan", Math.Tan);
            RegisterFunction("tanh", Math.Tanh);
            RegisterFunction("truncate", Math.Truncate);
        }
        delegate double TransformDelegate(double v);
        Dictionary<string, TransformDelegate> Functions = new Dictionary<string, TransformDelegate>();

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
         *	        R.J.Harrison        Date    19-Feb-2015 - Make properties protected
        */

        /// <summary>
        /// Floating point accumulator - i.e current value
        /// </summary>
        /// <remarks>following BBC basic naming conventions.</remarks>
        protected double fac = 0;

        protected Stack<double> stack = new Stack<double>();
        /// <summary>
        /// current value of fac
        /// </summary>
        protected double cur_fac;

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
        protected string Expression;

        /// <summary>
        /// position within instring of the current parse location
        /// </summary>
        protected int ExpressionPosition = 0;

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
        protected Dictionary<string, double> SymbolDictionary = new Dictionary<string, double>();
        
        public virtual void SetSymbol(string name, double val)
        {
            SymbolDictionary[name] = val;
        }

        public virtual double GetSymbol(string name, char Operator)
        {
            return SymbolDictionary[name];
        }

        private void RegisterFunction(string name, TransformDelegate function)
        {
            Functions[name] = function;
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
            try
            {
                Expression = input;
                ExpressionPosition = 0;
                CurrentDepth = 0;
                level1('\0');
            }
            catch (FormatException e)
            {
                throw new InvalidOperationException(string.Format("{0} in value starting at character {1}", e.Message, ExpressionPosition), e);
            }
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
            return level2(Operator);
        }

        char level2(char Operator)
        {
            return level3(Operator);
        }

        char level3(char Operator)
        {
            var nOperator = level4(Operator);
            do
            {
                if (nOperator == '+')
                {
                    stack.Push(fac);
                    nOperator = level4(Operator);
                    fac = stack.Pop() + fac;
                }
                else if (nOperator == '-')
                {
                    stack.Push(fac);
                    nOperator = level4(Operator);
                    fac = stack.Pop() - fac;
                }
                else
                    break;
            } while (nOperator == '-' || nOperator == '+');
            return nOperator;
        }

        char level4(char Operator)
        {
            var nOperator = level5(Operator);
            do
            {
                if (nOperator == '*')
                {
                    stack.Push(fac);
                    nOperator = level5(Operator);
                    fac = stack.Pop() * fac;
                }
                else if (nOperator == '/')
                {
                    stack.Push(fac);
                    nOperator = level5(Operator);
                    fac = stack.Pop() / fac;
                }
                else
                    break;
            } while (nOperator == '*' || nOperator == '/');

            return nOperator;
        }

        char level5(char Operator)
        {
            Operator = level6(Operator);
            if (Operator == '^')
            {
                stack.Push(fac);
                Operator = level5(Operator);
                fac = Math.Pow(stack.Pop(), fac);
            }
            return Operator;
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
            {
                return CurrentOperator;
            }

            cur_fac = fac;
            // at this point we clear the fac as this level will hopefully find a new value for it
            fac = 0;


            if (ExpressionPosition >= Expression.Length)
                return '\0';

            while (ExpressionPosition < Expression.Length && Char.IsWhiteSpace(Expression[ExpressionPosition]))
                ExpressionPosition++;

            // the current Operator (symbol).
            if (ExpressionPosition >= Expression.Length)
                throw new InvalidOperationException("Expression incomplete at end of expression");
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
                if (Operator == '\'' || Char.IsLetterOrDigit(Operator))
                    throw new InvalidOperationException("Unexpected symbol at character " + ExpressionPosition.ToString());
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
                if (Operator == '(')
                {
                    if (!Functions.ContainsKey(sv))
                        throw new InvalidOperationException("Unknown function " + sv);

                    TransformDelegate func = Functions[sv];
                    var newop = level1('\0'); //level1(Expression[ExpressionPosition]);
                    if (newop != ')')
                        throw new InvalidOperationException(String.Format("Missing close bracket for function {0}", sv));

                    fac = func(fac);
                }
                else
                {
                    if (Operator == '\'' || Char.IsLetterOrDigit(Operator))
                        throw new InvalidOperationException("Unexpected symbol at character " + ExpressionPosition.ToString());
                    fac = GetSymbol(sv, Operator);
                }
            }
            else
            {
                // handle any as yet unprocessed Operators; the aim of this
                // is to get a value and store it in the FAC.
                switch (Operator)
                {
                    case '(':
                        {
                            ExpressionPosition++;
                            Operator = Expression[ExpressionPosition];
                            CurrentDepth++;
                            var newop = level1('\0'); //level1(Expression[ExpressionPosition]);
                            if (newop == '\0')
                                if (CurrentDepth > 1)
                                    throw new InvalidOperationException(String.Format("There are {0} missing closing brackets", CurrentDepth));
                                else
                                    throw new InvalidOperationException("Missing closing bracket");
                            CurrentDepth--;
                        }
                        break;
                    case ')':
                        throw new InvalidOperationException("Unexpected closing bracket at character " + ExpressionPosition.ToString());

                    default:
                        {
                            var end = ExpressionPosition;
                            bool can_negate = true;
                            bool can_posate = false;
                            while (end < Expression.Length
                                   && (Char.IsWhiteSpace(Expression[end]) || Char.IsDigit(Expression[end]) || Expression[end] == '.' 
                                       || Expression[end] == 'E' || Expression[end] == 'e' 
                                       || (can_negate && Expression[end] == '-')
                                       || (can_posate && Expression[end] == '+')
                                       )
                                   )
                            {
                                if (Expression[end] == 'E' || Expression[end] == 'e')
                                {
                                    can_negate = true;
                                    can_posate = true;
                                }
                                else
                                    can_negate = false;
                                end++;
                            }
                            var sv = Expression.Substring(ExpressionPosition, end - ExpressionPosition);
                            fac = Double.Parse(sv);
                            ExpressionPosition = end;
                            break;
                        }
                }
                Operator = nextOperator();
            }
            return Operator;
        }

        public double ProcessEquation(string exp)
        {
            var lines = exp.Split('\n');
            foreach (var _l in lines)
            {
                var l = _l.Replace(" ", "");
                if (l.Contains("="))
                {
                    var parts = l.Split('=');
                    if (parts.Length == 2)
                    {
                        if (parts[0].Length > 0)
                        {
                            var dv = Evaluate(parts[1]);
                            double dd = 0;
                            Double.TryParse(parts[1], out dd);
                            var ee = dv - dd;
                            SetSymbol(parts[0], dv);
                        }
                        else
                        {
                            var evalulatedValue = Evaluate(parts[1]);
//                            System.Console.WriteLine("Calculate {0}", evalulatedValue);
                            return evalulatedValue;
                            //System.Console.WriteLine("ERROR: bad numeber format {0}", parts[1]);
                        }
                    }
                }
            }
            return 0;
        }
    }
}   