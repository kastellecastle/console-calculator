using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class Program
{
    private static string[] operators = { "+", "-", "*", "/", "^", "(", ")", "root", "sin", "cos", "tan" };
    private static string[] functions = { "root", "sin", "cos", "tan", "ln", "arcsin", "arccos", "arctan" };
    private static string[] twoOperandOperators = { "+", "-", "*", "/", "^" };
    private static string[] oneArgFunctions = { "root", "sin", "cos", "tan", "ln", "arcsin", "arccos", "arctan" };
    private static string[] constants = { "pi", "e", "ans" };

    private static double ans = 0;
    private static Dictionary<string, double> variables = new Dictionary<string, double>();

    private static bool useRadians = false;

    private static bool showSteps = true;
    private static bool debug = false;

    public static void Main(string[] args)
    {
        Console.WriteLine("console calculator - kastellecastle");
        while (true) 
        {
            string? line = Console.ReadLine();
            if (line != null && line != "") 
            {
                List<string> split = splitExpression(line);
                string type = getExpressionType(split);

                if (type == "config") 
                {
                    doConfigExpression(split);
                }
                else if (type == "solve") 
                {
                    ans = solveExpression(split);
                    Console.WriteLine(ans);
                }
                else if (type == "assign")
                {
                    doAssignExpression(split);
                }
            }
        }
    }

    private static void doConfigExpression(List<string> expression)
    {
        if (expression.Count < 2) 
        {
            return;
        }

        // ONE COMPONENT COMMANDS GO HERE

        if (expression[1] == "use") 
        {
            if (expression[2] == "help")
            {
                Console.WriteLine("selects the metric to be used for angles. syntax:");
                Console.WriteLine("use radians || use degrees");
            }
            else if (expression[2] == "radians")
            {
                useRadians = true;
            }
            else if (expression[2] == "degrees")
            {
                useRadians = false;
            }
            else 
            {
                Console.WriteLine("unrecognised argument: " + expression[2]);
            }
        }

        return;
    }

    private static void doAssignExpression(List<string> expression)
    {
        if (expression.Count < 3) 
        {
            return;
        }

        if (constants.Contains(expression[0]))
        {
            Console.WriteLine("cannot assign value to " + expression[0] + " because it is already a constant");
            return;
        }

        if (functions.Contains(expression[0]))
        {
            Console.WriteLine("cannot assign value to " + expression[0] + " because it is already a function");
            return;
        }

        if (functions.Contains(expression[0]))
        {
            Console.WriteLine("cannot assign value to " + expression[0] + " because it is already an operator");
            return;
        }

        string variable = expression[0];
        double value = 0;

        expression.RemoveRange(0, 2);
        value = solveExpression(expression);

        variables[variable] = value;
    }

    private static bool checkExpression(List<string> expression)
    {
        bool check = true;
        /*
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


        */
        return check;
    }

    private static string getExpressionType(List<string> expression) 
    {
        if (expression[0] == "config") 
        {
            return "config";
        }

        if (expression.Count > 1 && expression[1] == "=")
        {
            return "assign";
        }

        return "solve";
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
                if (split.Count > 1 && (!operators.Contains(split[lBracket - 1]) && !functions.Contains(split[lBracket - 1])))
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

            printExpression(split);
        }

        // SECOND: CONSTANTS
        // REPLACES CONSTANTS WITH THEIR ACTUAL VALUES
        for (int i = 0; i < split.Count; i++)
        {
            if (constants.Contains(split[i]))
            {
                if (split[i] == "pi")
                {
                    split[i] = Math.PI.ToString();
                }

                if (split[i] == "e")
                {
                    split[i] = Math.E.ToString();
                }

                if (split[i] == "ans")
                {
                    split[i] = ans.ToString();
                }
            }
            if (variables.ContainsKey(split[i]))
            {
                split[i] = variables[split[i]].ToString();
            }
        }

        for (int i = split.Count - 1; i >= 0; i--)
        {
            if (functions.Contains(split[i]))
            {
                doFunction(split, i);
                i = split.Count - 1;
            }
        }

        // THIRD: POWERS
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
                if (i == 0)
                {
                    if (split[i] == "-")
                    {
                        split[1] = "-" + split[1];
                    }
                    split.RemoveAt(0);
                    i -= 1;
                }
                else
                {
                    twoOperandExpression(split, i);
                    i -= 1;
                }
            }
        }

        if (variables.ContainsKey(split[0])) 
        {
            split[0] = variables[split[0]].ToString();
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

    private static void doFunction(List<string> expression, int funcIndex)
    {
        if (funcIndex == expression.Count - 1)
        {
            expression.Add("0");
        }

        if (oneArgFunctions.Contains(expression[funcIndex])) { doOneArgFunction(expression, funcIndex); }

    }

    private static void doOneArgFunction(List<string> expression, int funcIndex) 
    {
        int argStart = funcIndex + 1;
        string func = expression[funcIndex];

        if (expression[funcIndex + 1] == "^")
        {
            argStart += 2;
        }

        double arg;
        double result = 0;

        if (double.TryParse(expression[argStart], out arg)) 
        {
            if (func == "root") 
            {
                result = Math.Sqrt(arg);
            }
            if (func == "sin")
            {
                if (!useRadians) 
                {
                    arg = toRadians(arg);
                }
                result = Math.Sin(arg);
            }
            if (func == "cos")
            {
                if (!useRadians)
                {
                    arg = toRadians(arg);
                }
                result = Math.Cos(arg);
            }
            if (func == "tan")
            {
                if (!useRadians)
                {
                    arg = toRadians(arg);
                }
                result = Math.Tan(arg);
            }
            if (func == "arcsin")
            {
                result = Math.Asin(arg);
                if (!useRadians)
                {
                    result = toDegrees(result);
                }
            }
            if (func == "arccos")
            {
                result = Math.Acos(arg);
                if (!useRadians)
                {
                    result = toDegrees(result);
                }
            }
            if (func == "arctan")
            {
                result = Math.Atan(arg);
                if (!useRadians)
                {
                    result = toDegrees(result);
                }
            }
            if (func == "ln")
            {
                Console.WriteLine("logged " + arg);
                result = Math.Log(arg);
            }
        }

        if (showSteps) 
        {
            Console.WriteLine(func + " " + arg + " = " + result);
        }

        expression[funcIndex] = result.ToString();
        expression.RemoveAt(argStart);
    }

    private static double toRadians(double num) 
    {
        return num * Math.PI / 180;
    }

    private static double toDegrees(double num)
    {
        return num / Math.PI * 180;
    }
}