﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Evaluator
{
    class Program
    {
        static Eval x = new Eval();
        static void teval(string v, int expected)
        {
            var res = x.Evaluate(v);
            System.Console.WriteLine("{0} = {1}", v, res);
            if (res != expected)
                System.Console.WriteLine(" Expected {0}", expected);

        }

        static void Main(string[] args)
        {
            x.SetSymbol("ENV.MAIN", 20);
            x.SetSymbol("ENV.SECONDAY", 30);
            teval("ENV.MAIN+ENV.SECONDAY", 50);
            teval("-10+20", 10);
            teval("-10--30", 20);
            teval("-10-20", -30);
            teval("10-20+30", 20);
            teval("100+200", 300);
            teval("(10-20)+30", 20);
            teval("10-(20+30)", -40);
            teval("(20+30)/2", 25);
            teval("2*3+4", 10);
            teval("2*6/3", 4);
            teval("2^2", 4);
            teval("2^2+10", 14);
        }
    }
}
