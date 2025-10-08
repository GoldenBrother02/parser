namespace Par5er.Core;

public class Lexer
{
    public List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        for (int i = 0; i < input.Length;)
        {
            char c = input[i];
            if (char.IsWhiteSpace(c))
            {
                i++;
            }
            else if (char.IsDigit(c))
            {
                int start = i;
                int dotCount = 0;
                while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
                {
                    if (input[i] == '.')
                    {
                        dotCount++;
                        if (dotCount > 1)
                            throw new Exception($"Invalid number: {input[start..(i + 2)]}");
                    }
                    i++;
                }
                tokens.Add(new Token(TokenType.Number, input[start..i])); //I used a *range* operator, wow
            }
            else
            {
                switch (c)
                {
                    case '+': tokens.Add(new Token(TokenType.Plus, "+")); break;
                    case '-': tokens.Add(new Token(TokenType.Minus, "-")); break;
                    case '*': tokens.Add(new Token(TokenType.Multiply, "*")); break;
                    case '/': tokens.Add(new Token(TokenType.Divide, "/")); break;
                    case '^': tokens.Add(new Token(TokenType.Power, "^")); break;
                    case '(': tokens.Add(new Token(TokenType.LBracket, "(")); break;
                    case ')': tokens.Add(new Token(TokenType.RBracket, ")")); break;
                    default: throw new Exception($"Unexpected character: {c}");
                }
                i++;
            }
        }
        tokens.Add(new Token(TokenType.END, ""));

        for (int i = 0; i < tokens.Count - 1; i++)  //No number after number
        {
            if (tokens[i].Type == TokenType.Number && tokens[i + 1].Type == TokenType.Number)
                throw new Exception($"Unexpected token sequence: {tokens[i].Value} {tokens[i + 1].Value}");
        }

        return tokens;
    }
}