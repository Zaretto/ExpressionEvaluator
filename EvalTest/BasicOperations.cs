using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Evaluator;

namespace EvalTest
{
    [TestClass]
    public class BasicOperations
    {
        [TestMethod]
        public void Symbols()
        {
            var eval = new Eval();
            eval.SetSymbol("TVAR.MAIN", 20);
            eval.SetSymbol("TVAR.SECONDAY", 30);
            Assert.IsTrue(eval.Evaluate("TVAR.MAIN+TVAR.SECONDAY") == 50);
        }
        [TestMethod]
        public void QuotedSymbols()
        {
            var eval = new Eval();
            eval.SetSymbol("TVAR.MAIN", 20);
            eval.SetSymbol("TVAR.SECONDAY", 30);
            Assert.IsTrue(eval.Evaluate("'TVAR.MAIN'+'TVAR.SECONDAY'") == 50);

            eval.SetSymbol("TVAR MAIN", 20);
            eval.SetSymbol("TVAR SECONDAY", 30);
            Assert.IsTrue(eval.Evaluate("'TVAR MAIN'+'TVAR SECONDAY'") == 50);

            eval.SetSymbol("TVAR-MAIN", 20);
            eval.SetSymbol("TVAR-SECONDAY", 30);
            Assert.IsTrue(eval.Evaluate("'TVAR-MAIN'+'TVAR-SECONDAY'") == 50);

        }
        [TestMethod]
        public void ExtraOperatorError()
        {
            var s = "('Var1' 'Var2')";
            try
            {
                var eval = new NoSymEval();
                eval.Evaluate(s);
                Assert.Fail("Expression should produce symbol error");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is InvalidOperationException, "Unexpected " + e.ToString());
            }
        }
        [TestMethod]
        public void CurrentValueTest()
        {
            var s = "(1.2+'V2')";
            try
            {
                var eval = new NoSymEval();
                eval.Evaluate(s);
                Assert.AreEqual(eval.currentValueList.Count,1);
                Assert.AreEqual(eval.currentValueList[0], 1.2);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is InvalidOperationException, "Unexpected " + e.ToString());
            }
        }
        [TestMethod]
        public void MissingOperatorError()
        {
            var s = "('Var1'+'Var2')- ";
            try
            {
                var eval = new NoSymEval();
                eval.Evaluate(s);
                Assert.Fail("Expression should produce symbol error");
            }
            catch(Exception e)
            {
                Assert.IsTrue(e is InvalidOperationException, "Unexpected "+e.ToString());
            }

        }
        [TestMethod]
        public void Whitespace()
        {
            var eval = new Eval();
            eval.SetSymbol("TVAR MAIN", 20);
            eval.SetSymbol("TVAR SECONDAY", 30);
            Assert.IsTrue(eval.Evaluate("'TVAR MAIN' +'TVAR SECONDAY'") == 50);
            Assert.IsTrue(eval.Evaluate("'TVAR MAIN'+ 'TVAR SECONDAY'") == 50);
            Assert.IsTrue(eval.Evaluate("'TVAR MAIN' + 'TVAR SECONDAY'") == 50);
            Assert.IsTrue(eval.Evaluate("'TVAR MAIN' +  'TVAR SECONDAY'") == 50);
        }
        [TestMethod]
        public void AdditionAndSubtraction()
        {
            var eval = new Eval();
            Assert.IsTrue(eval.Evaluate("-10+20") ==10);
            Assert.IsTrue(eval.Evaluate("-10--30") ==20);
            Assert.IsTrue(eval.Evaluate("-10-20") ==-30);
            Assert.IsTrue(eval.Evaluate("10-20+30") ==20);
            Assert.IsTrue(eval.Evaluate("100+200") ==300);
        }
        [TestMethod]
        public void Brackets()
        {
            var eval = new Eval();
            Assert.IsTrue(eval.Evaluate("(10-20)+30") == 20);
            Assert.IsTrue(eval.Evaluate("10-(20+30)") == -40);
            Assert.IsTrue(eval.Evaluate("(20+30)/2") == 25);
        }
        [TestMethod]
        public void Multiplication()
        {
            var eval = new Eval();
            Assert.IsTrue(eval.Evaluate("2*3") == 6);
            Assert.IsTrue(eval.Evaluate("2*2.4") == 4.8);
        }
        [TestMethod]
        public void Division()
        {
            var eval = new Eval();
            Assert.IsTrue(eval.Evaluate("12/3") == 4);
        }
        [TestMethod]
        public void OrderOfPrecedence()
        {
            var eval = new Eval();
            Assert.IsTrue(eval.Evaluate("2*6/3") == 4);
            Assert.IsTrue(eval.Evaluate("2*3+4") == 10);
            Assert.IsTrue(eval.Evaluate("2^2+10") == 14);
        }
        [TestMethod]
        public void RaiseToThePower()
        {
            var eval = new Eval();
            Assert.IsTrue(eval.Evaluate("2^2") == 4);
        }
    }
}
