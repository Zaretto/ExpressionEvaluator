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
            eval.SetSymbol("ENV.MAIN",20);
            eval.SetSymbol("ENV.SECONDAY",30);
            Assert.IsTrue(eval.Evaluate("ENV.MAIN+ENV.SECONDAY") == 50);
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
