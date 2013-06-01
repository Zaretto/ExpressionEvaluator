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
        /// the string being parseed.
        /// </summary>
        string instring;

        /// <summary>
        /// position within instring of the current parse location
        /// </summary>
        int txtptr = 0;

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

        public void SetSymbol(string name, double val)
        {
            SymbolDictionary[name] = val;
        }

        /// <summary>
        /// main entry point
        /// </summary>
        /// <remarks>
        /// This works by having different levels each of which is a set of operators at the same
        /// precedence. The sign that is currently being parsed is returned, it starts at null
        /// and is reset to null when a bracketed expression has been parsed.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public double Evaluate(String input)
        {
            instring = input;
            txtptr = 0;
            
            level1('\0');
            return fac;
        }

        /// <summary>
        /// no idea why we have two blank levels at the beginning - but it's always been like
        /// that since 1987, so I'll have to figure out what's missing and then either implement
        /// them or remove.
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        char level1(char sign)
        {
            do
            {
                sign = level2(sign);
            }
            while (sign > 0);
            return sign;
        }

        char level2(char sign)
        {
            return level3(sign);
        }

        char level3(char sign)
        {
            if (sign == '+')
            {
                var cur_fac = fac;
                sign = level4(sign);
                fac = cur_fac + fac;
                return sign;
            }
            if (sign == '-')
            {
                var cur_fac = fac;;
                sign = level4(sign);
                fac = cur_fac - fac;
                return sign;
            }
            return level4(sign);
        }

        char level4(char sign)
        {
            if (sign == '*')
            {
                var cur_fac = fac;;
                sign = level5(sign);
                fac = cur_fac * fac;
                return sign;
            }
            if (sign == '/')
            {
                var cur_fac = fac;;
                sign = level5(sign);
                fac = cur_fac / fac;
                return sign;
            }
            return level5(sign);
        }

        char level5(char sign)
        {
            if (sign == '^')
            {
                var cur_fac = fac;;
                sign = level6(sign);
                fac = Math.Pow(cur_fac, fac);
                return sign;
            }
            return level6(sign);
        }

        char level6(char sign)
        {
            /*
             * see if end of bracketed expression
             */
            if (sign == ')')
                return '\0';

            // at this point we clear the fac as this level will hopefully find a new value for it
            fac = 0;

            // the current sign (symbol).
            sign = instring[txtptr];

            /*
             * handle variables
             */
            if (Char.IsLetter(sign))
            {
                int end = txtptr;
                while (end < instring.Length && (instring[end] == '.' || Char.IsLetterOrDigit(instring[end])))
                {
                    end++;
                }
                var sv = instring.Substring(txtptr, end - txtptr);
                txtptr = end;
                // this will throw an exception if not found. 
                fac = SymbolDictionary[sv];
            }
            else
            {
                // handle any as yet unprocessed signs; the aim of this
                // is to get a value and store it in the FAC.
                switch (sign)
                {
                    case '(':
                        txtptr++;
                        level1(instring[txtptr]);
                        break;

                    default:
                        {
                            var end = txtptr;
                            bool can_negate = true;
                            while (end < instring.Length
                                   && (Char.IsDigit(instring[end]) || instring[end] == '.' || (can_negate && instring[end] == '-')))
                            {
                                can_negate = false;
                                end++;
                            }
                            var sv = instring.Substring(txtptr, end - txtptr);
                            txtptr = end;
                            fac = Double.Parse(sv);
                            break;
                        }
                }
            }
            if (txtptr < instring.Length)
                sign = instring [txtptr++];
            else
            {
                sign = '\0';
            }
            return sign;
        }
    }
}