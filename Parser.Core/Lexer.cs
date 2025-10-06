using System.Text.RegularExpressions;

namespace Paarser;

public abstract class Lexer
{
    public static List<string> Tokenize(string input)
    {
        var result1 = input.Replace(" ", "");
        List<string> parts = Regex.Split(result1, @"([+\-*/^()])")
        .Where(p => !string.IsNullOrWhiteSpace(p))
        .ToList();
        return parts;
    }
}