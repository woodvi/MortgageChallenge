using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortgageChallenge
{
    class MortgageChallenge
    {
        /// <summary>
        /// Displays a tab-delimited payment table on StdOut
        /// most of the magic happens in PaymentTable
        /// 
        /// call withuot parameters for an example (Demo Mode)
        /// or call with [Principal Years Rate] from the command line, eg:
        /// MortgageChallenge.exe 200000 30 3.7
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            const int TOKEN_AMOUNT = 0;
            const int TOKEN_TERM = 1;
            const int TOKEN_RATE = 2;
            const int NUM_TOKENS = 3;

            if (args.Length == NUM_TOKENS)
            {
                // enter [principal years rate] on the command line to use your own parameters
                double principal;
                int years;
                double rate;

                Boolean anyErrorsYet = false;

                anyErrorsYet |= !Double.TryParse(args[TOKEN_AMOUNT], out principal);
                anyErrorsYet |= !int.TryParse(args[TOKEN_TERM], out years);
                anyErrorsYet |= !Double.TryParse(args[TOKEN_RATE], out rate);

                if (!anyErrorsYet)
                {
                    CalcGivenMortgage(principal, years * 12, rate /100.0);
                }
            }
            else
            {
                // Demo mode
                DemoMode();
                //PressEnterToContinue();
            }
        }


        /// <summary>
        /// Sample morgage of 200k principal, 30 years, and 3.7% interest rate
        /// </summary>
        private static void DemoMode()
        {
            // 200k mortgage, 30 year term (360 months), and 3.7% interest
            CalcGivenMortgage(200000, 360, 0.037);
        }

        /// <summary>
        /// user defined mortgage
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="months"></param>
        /// <param name="rate"></param>
        private static void CalcGivenMortgage(double principal, int months, double rate)
        {
            Mortgage m = new Mortgage(principal, months, rate);
            PaymentTable tbl = new PaymentTable(m);
            tbl.CalculateYears();

            WriteTable(System.Console.Out, tbl);
        }

        private static void WriteTable(System.IO.TextWriter writer, PaymentTable tbl)
        {
            foreach (PaymentTable.TableRow r in tbl.GetAllYears())
            {
                writer.WriteLine(
                String.Format("{0})\t{1:C0}\t{2:C0}\t{3:C0}\t{4:C0}", r.index + 1, r.MonthlyPayment, r.EndingBalance, r.PrincipalPaid, r.InterestPaid)
                );

            }
        }

        private static void PressEnterToContinue()
        {
            System.Console.Out.WriteLine("Press Enter to Continue");
            System.Console.In.Read();
        }

    }
}

