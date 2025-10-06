using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Paarser;

public class Parser
{
    public List<string> Content { get; set; }

    public List<string> Operators = ["+", "-", "*", "/", "^"];

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
                if (prev != null && int.TryParse(prev, out _))
                {
                    throw new ArgumentException($"Invalid expression: number before '(' at position {i}");
                }
                // '(' cannot be followed by an operator or ')'
                if (next == ")" || (next != null && Operators.Contains(next)))
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
                if (prev != null && Operators.Contains(prev))
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
        if (Operators.Contains(tokens.Last()) || tokens.Last() == "(")
        {
            throw new ArgumentException("Expression cannot end with operator or '('");
        }

        // 3. Validate sequence of tokens (alternating number/operator, ignoring parentheses)
        var check = tokens.Where(c => c != "(" && c != ")").ToList();
        bool isValid = check
            .Select((item, index) => new { item, index })
            .All(x => x.index % 2 == 0 ? int.TryParse(x.item, out _) : Operators.Contains(x.item));

        if (!isValid)
        {
            throw new ArgumentException("The expression you have entered is invalid.");
        }
    }

    public List<ASTnode> Nodenize(List<string> content)
    {
        List<ASTnode> result = [];
        foreach (var token in content)
        {
            if (int.TryParse(token, out _))
            {
                result.Add(new Number(int.Parse(token)));
            }
            else if (Operators.Contains(token))
            {
                result.Add(new BinaryOperator(token));
            }
            else if (token == "(")
            {
                result.Add(new OpenBracket(token));
            }
            else if (token == ")")
            {
                result.Add(new ClosedBracket(token));
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

    public ASTnode TreeTime(List<ASTnode> list)
    {
        var flat = BracketTime(list);

        if (flat.Count == 0)
            throw new ArgumentException("Empty expression.");
        if (flat.Count == 1)
            return flat[0];

        // Sanity check: must alternate operand/operator/operand
        if (flat.Count % 2 == 0)
            throw new ArgumentException("Invalid number of tokens.");

        ASTnode current = flat[0];

        // Fold left-to-right
        for (int i = 1; i < flat.Count; i += 2)
        {
            if (i + 1 >= flat.Count)
                throw new ArgumentException("Dangling operator at end of expression.");

            var op = (BinaryOperator)flat[i];
            var right = flat[i + 1];
            current = new Expression(current, op, right);
        }

        return current;
    }

}