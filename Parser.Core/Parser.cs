using System.Diagnostics.Contracts;

namespace Paarser;

public class Parser
{
    public List<string> Content { get; set; }

    public List<string> Operators = ["+", "-", "*", "/", "^"];

    public Parser(string input)
    {
        Content = Lexer.Tokenize(input);
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
        }
        return result;
    }

    public Expression TreeTime(List<ASTnode> list)
    {
        return new Expression(list[0], list[1], list[2]);
    }
}