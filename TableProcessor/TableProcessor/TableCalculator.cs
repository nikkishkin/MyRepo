using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TableProcessor
{
    class Spreadsheet
    {
        public string[,] Data { get; set; }

        private bool _wereAnyChanges;

        private const int LETTER_TO_NUMBER_CONVERSION_OFFSET = 96;

        private const string INCORRECT_CELL_DATA_MESSAGE = "#The cell data is in incorrect format";
        private const string BAD_CELL_REFERENCE_MESSAGE = "#Referenced cell cannot be part of expression";

        private const char EXPRESSION_PREFIX = '=';
        private const char TEXT_LINE_PREFIX = '\'';
        private const char ERROR_MESSAGE_PREFIX = '#';

        public void Calculate()
        {
            do
            {
                _wereAnyChanges = false;
                for (int i = 0; i < Data.GetLength(0); i++)
                {
                    for (int j = 0; j < Data.GetLength(1); j++)
                    {
                        ProcessCell(i, j);
                    }
                }
            } while (_wereAnyChanges);

            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    Data[i, j] = GetAdjustedValue(i, j);
                }
            }
        }

        private string GetAdjustedValue(int i, int j)
        {
            // Remove all redundant symbols which were intended only for parsing
            return UnfoldNumericTerm(Data[i, j]).TrimStart(TEXT_LINE_PREFIX);
        }

        private bool IsValidCell(string value)
        {
            return IsCalculatedValue(value) || String.IsNullOrWhiteSpace(value) || IsTextLine(value) ||
                   IsErrorMessage(value) || IsExpression(value);
        }

        private bool IsExpression(string value)
        {
            return !String.IsNullOrEmpty(value) && value[0] == EXPRESSION_PREFIX;
        }

        private bool IsErrorMessage(string value)
        {
            return value[0] == ERROR_MESSAGE_PREFIX;
        }

        private bool IsTextLine(string value)
        {
            return value[0] == TEXT_LINE_PREFIX;
        }

        private bool IsCalculatedValue(string value)
        {
            int number;
            return int.TryParse(UnfoldNumericTerm(value), out number);
        }

        private void ProcessCell(int i, int j)
        {
            string cellData = Data[i, j];

            // Firstly, let's check if this cell is incorrect

            if (!IsValidCell(cellData))
            {
                // Cell is incorrect
                Data[i, j] = INCORRECT_CELL_DATA_MESSAGE;
                _wereAnyChanges = true;
                return;
            }

            // Ok, cell is valid

            if (!IsExpression(cellData))
            {
                return;
            }

            // So, this is expression

            // Now, let's see if there are some cell references in this expression
            // If yes, let's try to replace them with values from referenced cells

            bool readyForCalculation = true;
            Match cellReferenceMatch = Regex.Match(cellData, @"[A-Za-z][0-9]");
            while (cellReferenceMatch.Success)
            {
                // Ok, we've found cell reference. Let's try to replace it with value

                string cellReference = cellReferenceMatch.Value;

                int referencedColumnNumber = Convert.ToInt32(cellReference.ToLower()[0]) -
                             LETTER_TO_NUMBER_CONVERSION_OFFSET - 1;
                int referencedRowNumber = Convert.ToInt32(cellReference.Substring(1)) - 1;

                if (IsCalculatedValue(Data[referencedRowNumber, referencedColumnNumber]))
                {
                    // There is a numeric value in referenced cell, so we can replace our reference with this value
                    cellData = cellData.Replace(cellReference, Data[referencedRowNumber, referencedColumnNumber]);
                    Data[i, j] = cellData;
                    _wereAnyChanges = true;
                }
                else
                {
                    // We couldn't replace the reference, so we couldn't calculate the expression
                    readyForCalculation = false;

                    // So, referenced cell's data is not a calculated value
                    // If it is also not an expression, then it can't be a term of expression 

                    if (!IsExpression(Data[referencedRowNumber, referencedColumnNumber]))
                    {
                        Data[i, j] = BAD_CELL_REFERENCE_MESSAGE;
                        _wereAnyChanges = true;
                        break;
                    }
                }

                cellReferenceMatch = cellReferenceMatch.NextMatch();
            }

            // Now we've replaced references with numeric values when it was possible, or we've added error message
            // If expression is ready for calculation, let's calculate it

            if (readyForCalculation)
            {
                Data[i, j] = CalculateExpression(cellData);
                
                // We've replaced expression with the calculated value or with error meaasge, so we should iterate over the table again
                _wereAnyChanges = true;
            }
        }

        private string CalculateExpression(string cellData)
        {
            string termPattern = @"\d+|{.*?}";
            string[] terms =
                Regex.Matches(cellData, termPattern)
                    .OfType<Match>()
                    .Select(m => UnfoldNumericTerm(m.Value))
                    .ToArray();
            string allExceptTerms = Regex.Replace(cellData, termPattern, String.Empty);
            string[] operations =
                Regex.Matches(allExceptTerms, @"\*|/|\+|-")
                    .OfType<Match>()
                    .Select(m => m.Value)
                    .ToArray();

            int currentResult = GetTermValue(terms[0]);

            for (int k = 1; k < terms.Length; k++)
            {
                currentResult = GetOperationResult(currentResult, GetTermValue(terms[k]),
                    operations[k - 1]);
            }

            return currentResult < 0 ?
                FoldNumericTerm(currentResult) :
                currentResult.ToString();
        }

        private int GetTermValue(string term)
        {
            return Convert.ToInt32(UnfoldNumericTerm(term));
        }

        private string FoldNumericTerm(int currentResult)
        {
            return String.Format("{{{0}}}", currentResult);
        }

        private string UnfoldNumericTerm(string value)
        {
            return value.Trim('{', '}');
        }

        private string AdjustTextLine(string line)
        {
            return line.TrimStart(TEXT_LINE_PREFIX);
        }

        private int GetOperationResult(int a, int b, string operation)
        {
            switch (operation)
            {
                case "+":
                    return a + b;
                case "-":
                    return a - b;
                case "*":
                    return a * b;
                case "/":
                    return a / b;
            }

            throw new ArgumentException(operation);
        }
    }
}
