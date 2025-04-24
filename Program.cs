using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class Program
{
    private static string[] operators = { "+", "-", "*", "/", "^", "(", ")", "root", "sin", "cos", "tan" };
    public static void Main(string[] args)
    {
        Console.WriteLine("console calculator - kastellecastle");
        while (true) 
        {
            string? line = Console.ReadLine();
            if (line != null) 
            {
                List<string> split = splitExpression(line);

                if (split[0] == "config") 
                {
                    executeConfigExpression(split);
                }
                else if (checkExpression(split)) 
                {
                    solveExpression(split);
                }
            }
        }
    }

    private static void executeConfigExpression(List<string> expression)
    {
        return;
    }

    private static bool checkExpression(List<string> expression)
    {
        bool check = true;
        double z;

        // CHECKS FOR INVALID PARTS OF THE EXPRESSION
        foreach (string s in expression) 
        {
            if (operators.Contains(s) || double.TryParse(s, out z)) 
            {
                continue;
            }

            check = false;
            Console.WriteLine("invalid syntax: " + s);
            break;
        }

        int lBrackets = 0;
        int rBrackets = 0;
        for (int i = 0; i < expression.Count; i++) 
        {
            if (expression[i] == "(") 
            {
                lBrackets += 1;
            }
            if (expression[i] == ")") 
            {
                rBrackets += 1;
            }
        }

        if (lBrackets != rBrackets) 
        {
            Console.WriteLine("invalid brackets");
            check = false;
        }

        return check;
    }

    private static List<string> splitExpression(string line) 
    {
        int start = 0;
        int end = 0;
        List<string> result = new List<string>();

        while (start < line.Length)
        {
            while (start < line.Length && line[start] == ' ')
            {
                start += 1;
            }

            end = start + 1;

            // HANDLES ALL ONE-SYMBOL OPERATORS
            // PLUS OR MINUS CAN BE NUMBER PREFIXES, AND SO ARE EXCLUDED HERE
            if (operators.Contains(line[start].ToString()))
            {
                string sub2 = line.Substring(start, end - start);
                result.Add(sub2);
                start = end;
                continue;
            }

            bool hasUsedPoint = false;
            while (end < line.Length)
            {
                // HANDLES ALL WORD OPERATORS
                if (char.IsLetter(line[start]) && char.IsLetter(line[end])) 
                {
                    end += 1;
                    continue;
                }

                if (char.IsDigit(line[start]))
                {
                    if (line[end] != ' ') 
                    {
                        if (line[end] == '.' && !hasUsedPoint) 
                        {
                            hasUsedPoint = true;
                            Console.WriteLine("point used");
                            end += 1;
                            continue;
                        }
                        if (char.IsDigit(line[end])) 
                        {
                            end += 1;
                            continue;
                        }
                    }
                }

                break;
            }

            string sub = line.Substring(start, end - start);
            result.Add(sub);
            start = end;
        }

        printExpression(result);
        return result;
    }

    private static void printExpression(List<string> expression) 
    {
        if (expression.Count == 0) 
        {
            return;
        }
        Console.Write(expression[0]);
        for (int i = 1; i < expression.Count; i++) 
        {
            Console.Write(", " + expression[i]);
        }
        Console.WriteLine("");
    }

    private static double solveExpression(List<string> split)
    {
        Console.WriteLine("answer");

        // FIRST: BRACKETS
        // SOLVE EXPRESSION IS RECURSIVELY CALLED ON THE EXPRESSION WITHIN BRACKETS

        // SECOND: POWERS
        // DOES MATH.POW

        // THIRD: MULTIPLICATION/DIVISION

        // FOURTH: ADDITION/SUBTRACTION

        return 0;
    }
}