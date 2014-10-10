using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortgageChallenge
{
    /// <summary>
    /// Holds the three inputs for our PaymentTable: Principal, TermMonths, and AnnualRate
    /// Includes constructor with definitions, ReadCsv function, and getters & setters for data
    /// Note that TermMonths is in months (not years) and AnnualRate is in real-space (ie 0.037, not 3.7%)
    /// </summary>
    public class Mortgage
    {
        // the amount of the mortgage
        public double Principal { get; set; }
        // how long the mortgage is (in months)
        public int TermMonths { get; set; }
        // what the quoted rate on the mortgage is
        public double AnnualRate { get; set; }

        // token indexes for csv lines
        const int TOKEN_AMOUNT = 0;
        const int TOKEN_TERM = 1;
        const int TOKEN_RATE = 2;
        const int NUM_TOKENS = 3;

        public Mortgage() { }

        public Mortgage(double principal, int termMonths, double annualRate)
        {
            this.Principal = principal;
            this.TermMonths = termMonths;
            this.AnnualRate = annualRate;
        }

        public Mortgage(String s)
        {
            ReadCsv(s);
        }

        /// <summary>
        /// Reads a character-delimited string into this mortgage 
        /// </summary>
        /// <param name="s">String in the format "Principal,Years,Rate" (eg "200000,30,3.7")</param>
        /// <returns>True if the string was successfully read</returns>
        public Boolean ReadCsv(string s)
        {
            Boolean anyErrorsYet = false;

            // get the mortgage amount in dollars (ie "200000" is a mortgage of $200k)
            string[] data;
            data = s.Split(',');
            if (data.Length < NUM_TOKENS)
            {
                this.Principal = 0; // season to taste
                this.TermMonths = 0; // season to taste
                this.AnnualRate = 0; // season to taste

                anyErrorsYet = true;
            }
            else
            {

                double amountDollars;
                if (Double.TryParse(data[TOKEN_AMOUNT], out amountDollars))
                {
                    this.Principal = amountDollars;
                }
                else
                {
                    this.Principal = 0; // season to taste
                    anyErrorsYet = true;
                }

                // get the mortgage term in years and convert to months
                int termYears;
                if (int.TryParse(data[TOKEN_TERM], out termYears))
                {
                    this.TermMonths = termYears * 12;
                }
                else
                {
                    this.TermMonths = 0; // season to taste
                    anyErrorsYet = true;
                }

                // get tge mortgage rate in percent and convert it to a real
                double RatePercent;
                if (double.TryParse(data[TOKEN_RATE], out RatePercent))
                {
                    this.AnnualRate = RatePercent / 100.0;
                }
                else
                {
                    this.AnnualRate = 0; // season to taste
                    anyErrorsYet = true;
                }
            }
            return !anyErrorsYet;
        }
    }
}
