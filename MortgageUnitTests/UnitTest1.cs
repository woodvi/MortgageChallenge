
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MortgageChallenge;


namespace MortgageUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        const double delta = 0.0000001;
        const double toTheNearestPenny = 0.005;
        const double toTheNearestDollar = 0.5;

        [TestMethod]
        public void TestDefinitionConstructor()
        {
            Mortgage m = new Mortgage(200000, 360, 0.037);

            Assert.AreEqual(200000, m.Principal, "Wrong Amount");
            Assert.AreEqual(360, m.TermMonths, "Wrong Term");
            Assert.AreEqual(0.037, m.AnnualRate, delta, "Wrong Rate");
        }

        [TestMethod]
        public void TestReadString()
        {
            string mortgageData = "200000, 30, 3.7";    // 200k mortgage, 30 year term (360 months), and 3.7% interest
            Mortgage m = new Mortgage();
            if (
                m.ReadCsv(mortgageData)
                )
            {

                Assert.AreEqual(200000, m.Principal, "Wrong Amount");
                Assert.AreEqual(360, m.TermMonths, "Wrong Term");                Assert.AreEqual(0.037, m.AnnualRate, delta, "Wrong Rate");
            }
            else
            {
                Assert.Fail("Couldn't read string data.");
            }

        }

        [TestMethod]
        public void TestReadBad()
        {
            Mortgage m = new Mortgage();

            Assert.IsFalse(m.ReadCsv( "Hello World"), "Wrong Number of Tokens");
            Assert.IsFalse(m.ReadCsv("200k, 30, 3.7"), "Invalid Principal Amount");
            Assert.IsFalse(m.ReadCsv("200000, 12th of never, 3.7"), "Invalid Term");
            Assert.IsFalse(m.ReadCsv("200000, 30, LIBOR+250"), "Invalid Rate");
        }

        [TestMethod]
        public void TestCalculatePayment()
        {
            Mortgage m = new Mortgage(200000, 360, 0.037);
            double payment = PaymentTable.CalculatePayment( m );

            Assert.AreEqual(920.57, payment, toTheNearestPenny, "Wrong Payment");
        }


        [TestMethod]
        public void TestMonthTable()
        {
            string mortgageData = "200000, 30, 3.7";    // 200k mortgage, 30 year term (360 months), and 3.7% interest
            Mortgage m = new Mortgage(mortgageData);
            PaymentTable.TableRow t;
            PaymentTable tbl = new PaymentTable(m);
            tbl.CalculateMonths();

            // check the very start
            t = tbl.GetMonth(0);
            Assert.AreEqual(920.57, t.MonthlyPayment, toTheNearestPenny, "Wrong Payment (month 0)");
            Assert.AreEqual(200000, t.StartingBalance, toTheNearestDollar, "Wrong StartingBalance (month 0)");
            Assert.AreEqual(616.67, t.InterestPaid, toTheNearestPenny, "Wrong Interest Payment (month 0)");
            Assert.AreEqual(303.90, t.PrincipalPaid, toTheNearestPenny, "Wrong Principal Payment (month 0)");

            // check the end of the first year
            t = tbl.GetMonth(12);
            Assert.AreEqual(920.57, t.MonthlyPayment, toTheNearestPenny, "Wrong Payment (month 12)");
            Assert.AreEqual(196291, t.StartingBalance, toTheNearestDollar, "Wrong Ending Balance (month 12)");
            Assert.AreEqual(605.23, t.InterestPaid, toTheNearestPenny, "Wrong Interest Payment (month 12)");
            Assert.AreEqual(315.34, t.PrincipalPaid, toTheNearestPenny, "Wrong Principal Payment (month 12)");

            // check the end of year #5
            t = tbl.GetMonth(60);
            Assert.AreEqual(920.57, t.MonthlyPayment, toTheNearestPenny, "Wrong Payment (month 60)");
            Assert.AreEqual(180004, t.StartingBalance, toTheNearestDollar, "Wrong Ending Balance (month 60)");
            Assert.AreEqual(555.01, t.InterestPaid, toTheNearestPenny, "Wrong Interest Payment (month 60)");
            Assert.AreEqual(365.55, t.PrincipalPaid, toTheNearestPenny, "Wrong Principal Payment (month 60)");

            // check the very last month
            t = tbl.GetMonth(359);
            Assert.AreEqual(920.57, t.MonthlyPayment, toTheNearestPenny, "Wrong Payment (month 360)");
            Assert.AreEqual(0, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (month 360)");
            Assert.AreEqual(2.83, t.InterestPaid, toTheNearestPenny, "Wrong Interest Payment (month 360)");
            Assert.AreEqual(917.74, t.PrincipalPaid, toTheNearestPenny, "Wrong Principal Payment (month 360)");

            // check that it stops there
            try
            {
                t = tbl.GetMonth(360);
                Assert.Fail("Missing ArgumentOutOfRangeException (year 0)");
            } catch (System.IndexOutOfRangeException) {
                // this is what should happen
            } catch (Exception e) {
                Assert.Fail("Unexpected Exception (year 30)");
            } 
        }

        [TestMethod]
        public void TestYearTable()
        {
            string mortgageData = "200000, 30, 3.7";    // 200k mortgage, 30 year term (360 months), and 3.7% interest
            Mortgage m = new Mortgage();
            PaymentTable.TableRow t;

            m.ReadCsv(mortgageData);
            PaymentTable tbl = new PaymentTable(m);
            tbl.CalculateYears();

            // check the very start
            t = tbl.GetYear(0);
            Assert.AreEqual(200000, t.StartingBalance, toTheNearestDollar, "Wrong StartingBalance (year 0)");

            // check the first year
            t = tbl.GetYear(0);
            Assert.AreEqual(920.57, t.MonthlyPayment, toTheNearestPenny, "Wrong Payment (year 0)");
            Assert.AreEqual(196291, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (year 0)");
            Assert.AreEqual(7338, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (year 0)");
            Assert.AreEqual(3709, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (year 0)");

            // check the end of year #4 (zero-indexed, so this is actually the fifth year)
            t = tbl.GetYear(4);
            Assert.AreEqual(920.57, t.MonthlyPayment, toTheNearestPenny, "Wrong Payment (year 4)");
            Assert.AreEqual(180004, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (year 4)");
            Assert.AreEqual(6747, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (year 4)");
            Assert.AreEqual(4300, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (year 4)");

            // check the last year
            t = tbl.GetYear(29);
            Assert.AreEqual(920.57, t.MonthlyPayment, toTheNearestPenny, "Wrong Payment (year 29)");
            Assert.AreEqual(0, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (year 29)");
            Assert.AreEqual(218, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (year 29)");
            Assert.AreEqual(10829, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (year 29)");

            // check that it stops there
            try
            {
                t = tbl.GetYear(30);
                Assert.Fail("Missing ArgumentOutOfRangeException (year 0)");
            }
            catch (System.IndexOutOfRangeException)
            {
                // this is what should happen
            } catch (Exception e) {
                Assert.Fail("Unexpected Exception (year 30)");
            } 
        }


        [TestMethod]
        public void TestCalculatePayment2()
        {
            Mortgage m = new Mortgage(100000, 180, 0.05);
            double payment = PaymentTable.CalculatePayment(m);

            Assert.AreEqual(791, payment, toTheNearestDollar, "Wrong Payment");
        }


        [TestMethod]
        public void TestMonthTable2()
        {
            Mortgage m = new Mortgage(100000, 180, 0.05);

            PaymentTable.TableRow t;
            PaymentTable tbl = new PaymentTable(m);
            tbl.CalculateMonths();

            // check the very start
            t = tbl.GetMonth(0);
            Assert.AreEqual(791, t.MonthlyPayment, toTheNearestDollar, "Wrong Payment (month 0)");
            Assert.AreEqual(100000, t.StartingBalance, toTheNearestDollar, "Wrong StartingBalance (month 0)");
            Assert.AreEqual(417, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (month 0)");
            Assert.AreEqual(374, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (month 0)");

            // check the end of the first year
            t = tbl.GetMonth(12);
            Assert.AreEqual(791, t.MonthlyPayment, toTheNearestDollar, "Wrong Payment (month 12)");
            Assert.AreEqual(95406, t.StartingBalance, toTheNearestDollar, "Wrong Ending Balance (month 12)");
            Assert.AreEqual(398, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (month 12)");
            Assert.AreEqual(393, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (month 12)");

            // check the end of year #5
            t = tbl.GetMonth(60);
            Assert.AreEqual(791, t.MonthlyPayment, toTheNearestDollar, "Wrong Payment (month 60)");
            Assert.AreEqual(74557, t.StartingBalance, toTheNearestDollar, "Wrong Ending Balance (month 60)");
            Assert.AreEqual(311, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (month 60)");
            Assert.AreEqual(480, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (month 60)");

            // check the very last month
            t = tbl.GetMonth(179);
            Assert.AreEqual(791, t.MonthlyPayment, toTheNearestDollar, "Wrong Payment (month 360)");
            Assert.AreEqual(0, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (month 360)");
            Assert.AreEqual(3.28, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (month 360)");
            Assert.AreEqual(788, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (month 360)");

            // check that it stops there
            try
            {
                t = tbl.GetMonth(180);
                Assert.Fail("Missing ArgumentOutOfRangeException (month 180)");
            }
            catch (System.IndexOutOfRangeException)
            {
                // this is what should happen
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected Exception (month180)");
            }
        }

        [TestMethod]
        public void TestYearTable2()
        {
            Mortgage m = new Mortgage(100000, 180, 0.05);

            PaymentTable.TableRow t;
            PaymentTable tbl = new PaymentTable(m);
            tbl.CalculateYears();

            // check the very start
            t = tbl.GetYear(0);
            Assert.AreEqual(100000, t.StartingBalance, toTheNearestDollar, "Wrong StartingBalance (year 0)");

            // check the first year
            t = tbl.GetYear(0);
            Assert.AreEqual(791, t.MonthlyPayment, toTheNearestDollar, "Wrong Payment (year 0)");
            Assert.AreEqual(95406, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (year 0)");
            Assert.AreEqual(4896, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (year 0)");
            Assert.AreEqual(4594, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (year 0)");

            // check the end of year #4 (zero-indexed, so this is actually the fifth year)
            t = tbl.GetYear(4);
            Assert.AreEqual(791, t.MonthlyPayment, toTheNearestDollar, "Wrong Payment (year 4)");
            Assert.AreEqual(74557, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (year 4)");
            Assert.AreEqual(3881, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (year 4)");
            Assert.AreEqual(5609, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (year 4)");

            // check the last year
            t = tbl.GetYear(14);
            Assert.AreEqual(791, t.MonthlyPayment, toTheNearestDollar, "Wrong Payment (year 29)");
            Assert.AreEqual(0, t.EndingBalance, toTheNearestDollar, "Wrong Ending Balance (year 29)");
            Assert.AreEqual(252, t.InterestPaid, toTheNearestDollar, "Wrong Interest Payment (year 29)");
            Assert.AreEqual(9237, t.PrincipalPaid, toTheNearestDollar, "Wrong Principal Payment (year 29)");

            // check that it stops there
            try
            {
                t = tbl.GetYear(15);
                Assert.Fail("Missing ArgumentOutOfRangeException (year 15)");
            }
            catch (System.IndexOutOfRangeException)
            {
                // this is what should happen
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected Exception (year 15)");
            }
        }
    }
}
