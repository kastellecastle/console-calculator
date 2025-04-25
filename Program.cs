using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class Program
{
    private static string[] operators = { "+", "-", "*", "/", "^", "(", ")", "root", "sin", "cos", "tan" };
    private static string[] twoOperandOperators = { "+", "-", "*", "/", "^" };

    private static bool showSteps = true;
    private static bool debug = false;

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
                    Console.WriteLine(solveExpression(split));
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

        string lastComponent = "";
        foreach (string s in expression)
        {
            if (s == "(" || s == ")") 
            {
                lastComponent = s;
                continue;
            }

            if (operators.Contains(s))
            {
                if (lastComponent == "operator" || lastComponent == "(") 
                {
                    Console.WriteLine("invalid syntax: operator into operator " + s);
                    check = false;
                    break;
                }
                lastComponent = "operator";
                continue;
            }

            if (double.TryParse(s, out z))
            {
                if (lastComponent == "number")
                {
                    Console.WriteLine("invalid syntax: number into number");
                    check = false;
                    break;
                }
                lastComponent = "number";
                continue;
            }
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

    private static List<string> subExpression(List<string> expression, int start, int end) 
    {
        List<string> result = new List<string>();
        for (int i = start; i <= end; i++) 
        {
            result.Add(expression[i]);
        }
        return result;
    }

    private static double solveExpression(List<string> split)
    {
        if (debug)
        {
            printExpression(split);
        }
        // FIRST: BRACKETS
        // SOLVE EXPRESSION IS RECURSIVELY CALLED ON THE EXPRESSION WITHIN BRACKETS

        while (split.Contains("(")) 
        {
            int lBracket = 0;
            int rBracket = split.Count - 1;
            int layer = 0;

            for (int i = 0; i < split.Count; i++) 
            {
                if (split[i] == "(") 
                {
                    if (layer == 0)
                    {
                        lBracket = i;
                    }
                    layer += 1;
                }

                if (split[i] == ")")
                {
                    rBracket = i;
                    layer -= 1;
                    if (layer == 0) 
                    {
                        break;
                    }
                }
            }

            List<string> inBrackets = subExpression(split, lBracket + 1, rBracket - 1);
            split[lBracket] = solveExpression(inBrackets).ToString();

            split.RemoveRange(lBracket + 1, rBracket - lBracket);

            if (lBracket != 0) 
            {
                if (split.Count > 1 && !operators.Contains(split[lBracket - 1]))
                {
                    split.Insert(lBracket, "*");
                    lBracket++;
                }
            }

            if (lBracket != split.Count - 1)
            {
                if (!operators.Contains(split[lBracket + 1]))
                {
                    split.Insert(lBracket + 1, "*");
                }
            }
        }

        // SECOND: POWERS
        // DOES MATH.POW
        for (int i = 0; i < split.Count; i++) 
        {
            if (split[i] == "^") 
            {
                twoOperandExpression(split, i);
                i -= 1;
            }
        }

        // THIRD: MULTIPLICATION/DIVISION
        for (int i = 0; i < split.Count; i++)
        {
            if (split[i] == "/" || split[i] == "*")
            {
                twoOperandExpression(split, i);
                i -= 1;
            }
        }


        // FOURTH: ADDITION/SUBTRACTION
        for (int i = 0; i < split.Count; i++)
        {
            if (split[i] == "+" || split[i] == "-")
            {
                twoOperandExpression(split, i);
                i -= 1;
            }
        }

        if (split.Count == 1) 
        {
            return double.Parse(split[0]);
        }

        return 0;
    }

    private static void twoOperandExpression(List<string> expression, int operatorIndex) 
    {
        if (operatorIndex == 0)
        {
            expression.Insert(0, "0");
            operatorIndex += 1;
        }
        if (operatorIndex == expression.Count - 1)
        {
            expression.Add("0");
        }

        double a;
        double b;

        if (double.TryParse(expression[operatorIndex - 1], out a) && double.TryParse(expression[operatorIndex + 1], out b))
        {
            string operatorStr = expression[operatorIndex];
            if (operatorStr == "+") 
            {
                expression[operatorIndex - 1] = (a + b).ToString();
            }
            if (operatorStr == "-")
            {
                expression[operatorIndex - 1] = (a - b).ToString();
            }
            if (operatorStr == "*")
            {
                expression[operatorIndex - 1] = (a * b).ToString();
            }
            if (operatorStr == "/")
            {
                expression[operatorIndex - 1] = (a / b).ToString();
            }
            if (operatorStr == "^")
            {
                expression[operatorIndex - 1] = (Math.Pow(a, b)).ToString();
            }

            if (showSteps)
            {
                Console.WriteLine(a + " " + operatorStr + " " + b + " = " + expression[operatorIndex - 1]);
            }

            expression.RemoveRange(operatorIndex, 2);
        }
    }
}