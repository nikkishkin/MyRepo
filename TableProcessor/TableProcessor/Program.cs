using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TableProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] tableProps = Console.ReadLine().Split(' ');

            int rowsNumber = Convert.ToInt32(tableProps[0]);
            int colsNumber = Convert.ToInt32(tableProps[1]);

            // Fill the table with data received from standard input
            string[,] table = new string[rowsNumber,colsNumber];
            for (int i = 0; i < rowsNumber; i++)
            {
                string[] currentRow = Console.ReadLine().Split('\t');
                for (int j = 0; j < colsNumber; j++)
                {
                    table[i, j] = currentRow[j];
                }
            }

            Spreadsheet spreadsheet = new Spreadsheet {Data = table};
            spreadsheet.Calculate();

            Console.WriteLine("----------------------");

            for (int i = 0; i < rowsNumber; i++)
            {
                for (int j = 0; j < colsNumber; j++)
                {
                    Console.Write(spreadsheet.Data[i, j] + "\t");
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }

    }
}
