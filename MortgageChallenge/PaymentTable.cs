using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortgageChallenge
{
    /// <summary>
    /// A payment table (monthly/yearly) for a mortgag
    /// Instantiate a PaymentTable with a constructor using the mortgage for which the PaymentTable will describe
    /// Get the entire table with either GetAllMonths or GetAlYears; or get a specific month with GetMonth(n) or GetYear(n)
    /// The table is zero-indexed; so the first year is year #0.
    /// </summary>
    public class PaymentTable
    {
        TableRow[] tblMonths;
        TableRow[] tblYears;
        Mortgage mortgage;

        public class TableRow
        {
            public int index {get; set;}
            public double MonthlyPayment { get; set; }
            public double StartingBalance { get; set; }
            public double EndingBalance { get; set; }
            public double PrincipalPaid { get; set; }
            public double InterestPaid { get; set; }
        }


        public PaymentTable( Mortgage m )
        {
            this.mortgage = m;
            this.CalculateYears();
        }


        /// <summary>
        /// Calculates the regular monthly payment for a standard american-style amortizing mortgage
        /// </summary>
        /// <returns>The amount of the regular monthly payment</returns>
        public double CalculatePayment()
        {
            return CalculatePayment(mortgage);
        }


        /// <summary>
        /// Calculates the regular monthly payment for a standard american-style amortizing mortgage
        /// </summary>
        /// <param name="m">A mortgage for which to calculate the regular monthly payment</param>
        /// <returns>The amount of the regular monthly payment</returns>
        public static double CalculatePayment(Mortgage m)
        {
            // based on the simplest US mortgage conventions (no baloon payments, adjustable rates, interest-only, none of that tricky stuff)
            // http://www.bretwhissel.net/amortization/amortize2col.pdf

            double periodicRate = m.AnnualRate / 12.0;
            // discountFactor = (1 + periodicRate) ^ (double)this.OriginalTermMonths;
            double discountFactor = Math.Pow(1 + periodicRate, m.TermMonths);
            double payment = m.Principal * periodicRate * discountFactor / (discountFactor - 1.0);

            return payment;
        }

        public TableRow GetMonth(int idx)
        {
            return (tblMonths[idx]);
        }

        public TableRow[] GetAllMonths()
        {
            return (tblMonths);
        }

        public TableRow GetYear(int idx)
        {
            return (tblYears[idx]);
        }

        public TableRow[] GetAllYears()
        {
            return (tblYears);
        }

        /// <summary>
        /// Calculates the monthly table
        /// </summary>
        public void CalculateMonths()
        {
            tblMonths = new TableRow[this.mortgage.TermMonths];
            tblMonths[0] = CalculateNextMonth(this.mortgage, null);
            for (int idx = 1; idx < this.mortgage.TermMonths; idx++)
            {
                tblMonths[idx] = CalculateNextMonth(this.mortgage, tblMonths[idx - 1]);
            }
        }

        /// <summary>
        /// Calculates the yearly & monthly ables
        /// </summary>
        public void CalculateYears()
        {
            CalculateMonths();
            tblYears = new TableRow[(int)Math.Ceiling(this.mortgage.TermMonths / 12.0)];
            for( int i = 0; i < tblYears.Length; i++ )
            {
                tblYears[i] = new TableRow();
            }

            foreach (TableRow r in tblMonths)
            {
                int idxMonth = r.index;
                int mo = r.index % 12;
                int idxYear = (idxMonth - mo ) / 12;

                tblYears[idxYear].index = idxYear;
                tblYears[idxYear].MonthlyPayment = r.MonthlyPayment;    // assign
                tblYears[idxYear].InterestPaid += r.InterestPaid;       // sum
                tblYears[idxYear].PrincipalPaid += r.PrincipalPaid;     // sum
                switch (mo)
                {
                    case 0: tblYears[idxYear].StartingBalance = r.StartingBalance; break;     // assign
                    case 11: tblYears[idxYear].EndingBalance = r.EndingBalance; break;     // assign
                }

            }
        }


        /// <summary>
        /// Calculates the next month of a payment table for a mortgage
        /// </summary>
        /// <param name="m">The Mortgage definition</param>
        /// <param name="previousMonth">The previous month TableRowon the payment table</param>
        /// <returns>the TableRow for the month following the given previousMonth</returns>
        private TableRow CalculateNextMonth(Mortgage m, TableRow previousMonth)
        {
            TableRow nextMonth;

            if (null == previousMonth)
            {
                nextMonth = new TableRow();
                nextMonth.index = 0;
                nextMonth.MonthlyPayment = CalculatePayment();
                nextMonth.StartingBalance = m.Principal;
            }
            else
            {
                nextMonth = new TableRow();
                nextMonth.index = previousMonth.index +1;
                // nextMonth.MonthlyPayment = CalculatePayment(m);  
                nextMonth.MonthlyPayment = previousMonth.MonthlyPayment;        // assume that the previous month has it correctly (and we're in trouble already if it isn't!)
                nextMonth.StartingBalance = previousMonth.EndingBalance;
            }
            nextMonth.InterestPaid = nextMonth.StartingBalance * m.AnnualRate / 12;
            nextMonth.PrincipalPaid = nextMonth.MonthlyPayment - nextMonth.InterestPaid;
            nextMonth.EndingBalance = nextMonth.StartingBalance - nextMonth.PrincipalPaid;

            return nextMonth;
        }
    }
}
