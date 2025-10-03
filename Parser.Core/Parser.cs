using System.Diagnostics.Contracts;
using System.Xml;

namespace Paarser;

public class Parser
{
    public List<string> Content { get; set; }

    public List<string> Operators = ["+", "-", "*", "/", "^"];

    public Parser(string input)
    {
        Content = Lexer.Tokenize(input);

        // validation
        var check = Content;
        check.RemoveAll(c => c == "(");
        check.RemoveAll(c => c == ")");
        if (!int.TryParse(check.Last(), out _))
        {
            throw new ArgumentException("Input cannot end on an operator.");
        }
        bool isValid = check
        .Select((item, index) => new { item, index })
        .All(x => x.index % 2 == 0 ? int.TryParse(x.item, out _) : "+-*/".Contains(x.item));
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
        }
        return result;
    }

    public ASTnode TreeTime(List<ASTnode> list)
    {
        List<ASTnode> newList = [];

        if (list.Count == 1) { return list[0]; }

        else
        {
            for (int pos = 0; pos < list.Count; pos++)
            {
                if (list[pos].GetType() == typeof(BinaryOperator))  //if operator
                {
                    newList.Add(new Expression(list[pos - 1], list[pos], list[pos + 1]));  // number operator number
                    pos += 2; //go to next operator

                    if (pos < list.Count)
                    {
                        newList.Add(list[pos]);  //next operator added to list
                    }
                }
            }
        }

        if (newList.Count > 1) { return TreeTime(newList); }
        else { return newList[0]; }
    }
}