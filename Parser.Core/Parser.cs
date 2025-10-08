using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Paarser;

public class Parser
{
    public List<string> Content { get; set; }

    public List<string> BinaryOperators = ["+", "-", "*", "/", "^"];

    public List<string> UnaryOperators = ["-"];

    public Parser(string input)
    {
        Content = Lexer.Tokenize(input);

        var tokens = Content.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        // 1. Validate parentheses balance and order
        int balance = 0;
        for (int i = 0; i < tokens.Count; i++)
        {
            string current = tokens[i];
            string? prev = i > 0 ? tokens[i - 1] : null;
            string? next = i < tokens.Count - 1 ? tokens[i + 1] : null;

            if (current == "(")
            {
                balance++;
                // '(' cannot come after a number
                if (prev != null && decimal.TryParse(prev, out _))
                {
                    throw new ArgumentException($"Invalid expression: number before '(' at position {i}");
                }
                // '(' cannot be followed by an operator or ')'
                if (next == ")" || (next != null && BinaryOperators.Contains(next) && !UnaryOperators.Contains(next)))
                {
                    throw new ArgumentException($"Invalid expression: '(' followed by invalid token '{next}'");
                }
            }
            else if (current == ")")
            {
                balance--;
                if (balance < 0)
                {
                    throw new ArgumentException($"Invalid expression: ')' before matching '(' at position {i}");
                }
                // ')' cannot directly follow an operator
                if (prev != null && BinaryOperators.Contains(prev))
                {
                    throw new ArgumentException($"Invalid expression: operator before ')' at position {i}");
                }
            }
        }

        if (balance != 0)
        {
            throw new ArgumentException("Mismatched parentheses in expression.");
        }

        // 2. Validate last token is not operator or '('
        if (BinaryOperators.Contains(tokens.Last()) || tokens.Last() == "(")
        {
            throw new ArgumentException("Expression cannot end with operator or '('");
        }

        // 3. Validate sequence of tokens (alternating number/operator, ignoring parentheses)
        var check = tokens.Where(c => c != "(" && c != ")").ToList();
        // bool isValid = check
        //     .Select((item, index) => new { item, index })
        //     .All(x => x.index % 2 == 0 ? decimal.TryParse(x.item, out _) : BinaryOperators.Contains(x.item));
        bool isValid = true;
        string? previous = null;
        foreach (var token in check)
        {
            if (BinaryOperators.Contains(token) && BinaryOperators.Contains(previous!) && !UnaryOperators.Contains(token))
            {
                isValid = false;
                break;
            }
            previous = token;
        }

        if (!isValid)
        {
            throw new ArgumentException("The expression you have entered is invalid.");
        }
    }

    public List<ASTnode> Nodenize(List<string> content)
    {
        ASTnode? previous = null;
        List<ASTnode> result = [];
        foreach (var token in content)
        {
            if (token == "-" && (previous == null || previous is BinaryOperator || previous is OpenBracket))
            {
                result.Add(new UnaryOperator(token));
                previous = new UnaryOperator(token);
                continue;
            }
            if (decimal.TryParse(token, out _))
            {
                result.Add(new Number(decimal.Parse(token)));
                previous = new Number(decimal.Parse(token));
            }
            else if (BinaryOperators.Contains(token))
            {
                result.Add(new BinaryOperator(token));
                previous = new BinaryOperator(token);
            }
            else if (token == "(")
            {
                result.Add(new OpenBracket(token));
                previous = new OpenBracket(token);
            }
            else if (token == ")")
            {
                result.Add(new ClosedBracket(token));
                previous = new ClosedBracket(token);
            }
        }
        return result;
    }

    public List<ASTnode> BracketTime(List<ASTnode> list)
    {
        List<ASTnode> resultlist = [];
        for (int pos = 0; pos < list.Count; pos++)
        {
            if (list[pos] is OpenBracket)
            {
                var balance = 1;
                var start = pos;
                while (balance != 0)
                {
                    pos++;
                    if (list[pos] is OpenBracket)
                    {
                        balance++;
                    }
                    if (list[pos] is ClosedBracket)
                    {
                        balance--;
                    }
                    if (balance == 0)
                    {
                        int stop = pos;
                        List<ASTnode> sublist = list.GetRange(start + 1, stop - start - 1);
                        resultlist.Add(TreeTime(sublist));
                        break;
                    }
                }
            }
            else
            {
                resultlist.Add(list[pos]);
            }
        }
        return resultlist;
    }

    // public List<ASTnode> Power(List<ASTnode> flat)
    // {
    //     var checkedList = new List<ASTnode>();

    //     // Start from right
    //     for (int i = flat.Count - 1; i >= 0; i--)
    //     {
    //         if (flat[i] is BinaryOperator binary && binary.Type == "^")
    //         {
    //             // Right operand from checkedList[0]
    //             var right = checkedList[0];
    //             var left = flat[i - 1];
    //             checkedList[0] = new Expression(left, flat[i], right);
    //             i--; // Skip the left operand we just consumed
    //         }
    //         else
    //         {
    //             checkedList.Insert(0, flat[i]);
    //         }
    //     }

    //     return checkedList;
    // }

    public List<ASTnode> Power(List<ASTnode> flat)
    {
        var checkedList = new List<ASTnode>();

        // Start from right
        for (int i = flat.Count - 1; i >= 0; i--)
        {
            if (flat[i] is BinaryOperator binary && binary.Type == "^")
            {
                // Right operand from checkedList[0]
                var right = checkedList[0];
                if (right is UnaryOperator unop)
                {
                    if (checkedList.Count < 2)
                        throw new ArgumentException("Invalid exponent: unary operator missing operand.");

                    right = new Expression(new Number(-1), new BinaryOperator("*"), checkedList[1]);
                    checkedList.RemoveAt(1); // Remove the operand we just consumed
                }
                var left = flat[i - 1];
                checkedList[0] = new Expression(left, flat[i], right);
                i--; // Skip the left operand we just consumed
            }
            else
            {
                checkedList.Insert(0, flat[i]);
            }
        }

        return checkedList;
    }

    public List<ASTnode> Multiplication(List<ASTnode> power)
    {
        List<ASTnode> Checkedlist = [];

        for (int i = 0; i < power.Count; i++)
        {
            if (power[i] is BinaryOperator binary && (binary.Type == "*" || binary.Type == "/"))
            {
                var left = Checkedlist.Last();
                var right = power[i + 1];
                Checkedlist[Checkedlist.Count - 1] = new Expression(left, power[i], right);
                i++;
            }
            else
            {
                Checkedlist.Add(power[i]);
            }
        }
        return Checkedlist;
    }

    public List<ASTnode> UnaryTime(List<ASTnode> list)
    {
        List<ASTnode> resultlist = [];
        for (int pos = 0; pos < list.Count; pos++)
        {
            if (list[pos] is UnaryOperator)
            {
                var op = list[pos];
                var right = list[pos + 1];
                resultlist.Add(new Expression(new Number(-1), new BinaryOperator("*"), right));
                pos++;
            }
            else
            {
                resultlist.Add(list[pos]);
            }
        }
        return resultlist;
    }

    public ASTnode TreeTime(List<ASTnode> list)
    {
        // First handle parentheses
        var flat = BracketTime(list);

        var DoneList = Multiplication(UnaryTime(Power(flat)));

        if (DoneList.Count == 0)
            throw new ArgumentException("Empty expression.");
        if (DoneList.Count == 1)
            return DoneList[0];

        // Sanity check: must alternate operand/operator/operand
        if (DoneList.Count % 2 == 0)
            throw new ArgumentException("Invalid number of tokens.");

        ASTnode current = DoneList[0];

        // Fold left-to-right
        for (int i = 1; i < DoneList.Count; i += 2)
        {
            var op = (BinaryOperator)DoneList[i];
            var right = DoneList[i + 1];
            current = new Expression(current, op, right);
        }

        return current;
    }

}