using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Evaluator;
using System.Linq;

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
        public void SpaceInNumber()
        {
            var s = "202+ 1 1 + 2";
            try
            {
                var eval = new NoSymEval();
                eval.Evaluate(s);
                Assert.AreEqual(eval.currentValueList.Count,1);
                Assert.AreEqual(eval.currentValueList[0], 1.2);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Assert.IsTrue(e is InvalidOperationException, "Unexpected " + e.ToString());
            }
        }

        [TestMethod]
        public void ExtraBracket()
        {
            var s = "((202)+11+2";
            try
            {
                var eval = new NoSymEval();
                eval.Evaluate(s);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Assert.IsTrue(e is InvalidOperationException, "Unexpected " + e.ToString());
                Assert.IsTrue(e.Message.Contains("Missing closing"), "Error message should contain 'Missing closing'");
                return;
            }
            Assert.Fail("Extra bracket not detected");
        }
        [TestMethod]
        public void ExtraBrackets()
        {
            var s = "(((202)+11+2";
            try
            {
                var eval = new NoSymEval();
                eval.Evaluate(s);
                Assert.Fail("Extra bracket not detected");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Assert.IsTrue(e is InvalidOperationException, "Unexpected " + e.ToString());
                Assert.IsTrue(e.Message.Contains("2 missing"), "Error message should contain '2 missing'");
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
            Assert.AreEqual(10 ,eval.Evaluate("-10+20")   );
            Assert.AreEqual(20 ,eval.Evaluate("-10--30")  );
            Assert.AreEqual(-30 ,eval.Evaluate("-10-20")   );
            Assert.AreEqual(40, eval.Evaluate("10-20+50"), "10-20+50");
            Assert.AreEqual(300, eval.Evaluate("100+200") );
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
            Assert.AreEqual(18, eval.Evaluate("12*3/2"));

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
            Assert.IsTrue(eval.Evaluate("2^2*2") == 8);
        }

        [TestMethod]
        public void Equation()
        {
            var eval = new Eval();
            var ev = @"a =	-1.90348642570E-002
              b =	2.59124284126E+003
              c =	1.74712818469E+000
              d =	2.19476267227E+000
              x =  10
              =(a*b+c*x^d)/(b+x^d)";
            var expectedValue = 0.0816126249369512;
            Assert.AreEqual(Math.Round(expectedValue, 3), Math.Round(eval.ProcessEquation(ev), 3));
        }
        [TestMethod]
        public void Equation_1()
        {
            var eval = new Eval();
            var ev = @"a =	-1.65798712337E-002
b =	6.91771818666E-003
c =	9.16997584955E-004
d =	-1.47185697480E-005
e =	6.23910630630E-008
                x = 10
                =a+b*x+c*x^2+d*x^3+e*x^4";
            var expectedValue = 0.13;
            Assert.AreEqual(Math.Round(expectedValue, 3), Math.Round(eval.ProcessEquation(ev), 3));
            var ev_neg = @"a =	-1.65798712337E-002
b =	6.91771818666E-003
c =	9.16997584955E-004
d =	-1.47185697480E-005
e =	6.23910630630E-008
                x = -10
                =a+b*x+c*x^2+d*x^3+e*x^4";
            expectedValue = 0.0212;
            Assert.AreEqual(Math.Round(expectedValue, 3), Math.Round(eval.ProcessEquation(ev_neg), 3));
        }

        [TestMethod]
        public void Equation_SYM()
        {
            var eval = new Eval();
            var ev = @"a =	4.42907255764E-002
b =	4.28646004865E-003
c =	6.79041100634E-004
d =	-6.24669695062E-006
                x = iv.alpha
                =a+b*x+c*x^2+d*x^3";
            var expectedValue = 0.149;
            eval.SetSymbol("iv.alpha", 10);
            Assert.AreEqual(Math.Round(expectedValue, 3), Math.Round(eval.ProcessEquation(ev), 3));
        }
        [TestMethod]
        public void Function_ABS()
        {
            var eval = new Eval();
            Assert.AreEqual(10, eval.Evaluate("abs(-10)"));
        }
        [TestMethod]
        public void Function_cos()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("cos(11.112)"), Math.Cos(11.112));
        }

        [TestMethod]
        public void Function_sin()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("sin(11.112)"), Math.Sin(11.112));
        }

        [TestMethod]
        public void Function_acos()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("acos(11.112)"), Math.Acos(11.112));
        }

        [TestMethod]
        public void Function_asin()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("asin(11.112)"), Math.Asin(11.112));
        }

        [TestMethod]
        public void Function_atan()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("atan(11.112)"), Math.Atan(11.112));
        }

        [TestMethod]
        public void Function_sinh()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("sinh(11.112)"), Math.Sinh(11.112));
        }

        [TestMethod]
        public void Function_sqrt()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("sqrt(11.112)"), Math.Sqrt(11.112));
        }

        [TestMethod]
        public void Function_tan()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("tan(11.112)"), Math.Tan(11.112));
        }

        [TestMethod]
        public void Function_tanh()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("tanh(11.112)"), Math.Tanh(11.112));
        }

        [TestMethod]
        public void Function_truncate()
        {
            var eval = new Eval();

            Assert.AreEqual(eval.Evaluate("truncate(11.112)"), Math.Truncate(11.112));
        }


    }
}
