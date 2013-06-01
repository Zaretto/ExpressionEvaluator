using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluator
{
    class Eval
    {
        /*		Expression evaluator for symon. Will take expression and  
         *	return either a double, or a 4 byte unsigned integer (for hex or   
         *      ASCII input of values                                            
         * 
         *	Author:	R.J.Harrison		Date	15-December-1987 
         *	        R.J.Harrison		Date	01-May-2013 - ported from C to C#
        */

        Stack<double> stack;

        int txtptr = 0;
        static double fac = 0;
        string instring;

        public double eval(String input)
        {
            instring = input;
            txtptr = 0;
            stack = new Stack<double>();
            
            level1('\0');
            return fac;
            //            return type;
        }

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
                stack.Push(fac);
                sign = level4(sign);
                fac = stack.Pop() + fac;
                return sign;
            }
            if (sign == '-')
            {
                stack.Push(fac);
                sign = level4(sign);
                fac = stack.Pop() - fac;
                return sign;
            }
            return level4(sign);
        }

        char level4(char sign)
        {
            if (sign == '*')
            {
                stack.Push(fac);
                sign = level5(sign);
                fac = stack.Pop() * fac;
                return sign;
            }
            if (sign == '/')
            {
                stack.Push(fac);
                sign = level5(sign);
                fac = stack.Pop() / fac;
                return sign;
            }
            return level5(sign);
        }

        char level5(char sign)
        {
            if (sign == '^')
            {
                stack.Push(fac);
                sign = level6(sign);
                fac = Math.Pow(stack.Pop(), fac);
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

            fac = 0;

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
                Console.WriteLine("Found var {0}", sv);
                fac = 0;
            }
            else
            {
                switch (sign)
                {
                    //case '-':
                    //    txtptr++;
                    //    negate = !negate;
                    //    goto restart;
                    //    break;

                    //case '&' :
                    //    txtptr++;
                    //    strg = chex();
                    //    break;

                    //case '+':
                    //    txtptr++; // leading + is a no-op.
                    //    goto restart;
                    //    //fac = cint();
                    //break;

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
                            //          fac = cint();
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