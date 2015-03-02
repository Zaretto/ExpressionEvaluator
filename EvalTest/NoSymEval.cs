using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalTest
{
    public class NoSymEval : Evaluator.Eval
    {
        static double tv = 0;
        public override double GetSymbol(string name, char Operator)
        {
            currentValueList.Add(cur_fac); 
            return tv++;
        }

        public List<double> currentValueList = new List<double>();
    }
}
