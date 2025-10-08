namespace Par5er.Core;

public class Parser
{
    private readonly List<Token> tokens;
    private int pos = 0;
    private Token Current => tokens[pos];

    public Parser(List<Token> tokens) => this.tokens = tokens;

    private Token Kill(TokenType type)  //a safer version of pos++, to see if brackets actually bracket
    {
        if (Current.Type == type)
            return tokens[pos++];
        throw new Exception($"Expected {type} but got {Current.Type}");
    }

    public Expression Parse() => ParseExpression();

    // expression = term ("+" || "-") term, X amount of times
    private Expression ParseExpression()
    {
        Expression expr = ParseTerm();
        while (Current.Type == TokenType.Plus || Current.Type == TokenType.Minus)
        {
            Token op = Current;
            pos++;
            Expression right = ParseTerm();
            expr = new BinaryExpr(expr, op, right);
        }
        return expr;
    }

    // term = power ("*" || "/") power, X amount of times
    private Expression ParseTerm()
    {
        Expression expr = ParsePower();
        while (Current.Type == TokenType.Multiply || Current.Type == TokenType.Divide)
        {
            Token op = Current;
            pos++;
            Expression right = ParsePower();
            expr = new BinaryExpr(expr, op, right);
        }
        return expr;
    }

    // power = unary, with optionally another ("^" power)
    private Expression ParsePower()
    {
        Expression left = ParseUnary();
        if (Current.Type == TokenType.Power)
        {
            Token op = Current;
            pos++;
            Expression right = ParsePower(); // right-associative
            return new BinaryExpr(left, op, right);
        }
        return left;
    }

    // unary = primary || ( "-" unary )
    // potentially multple "-" *mark*
    private Expression ParseUnary()
    {
        if (Current.Type == TokenType.Minus)
        {
            Token op = Current;
            pos++;
            Expression right = ParseUnary();
            return new UnaryExpr(op, right);
        }
        return ParsePrimary();
    }

    // primary = NUMBER || (  "(" expression ")"  )
    private Expression ParsePrimary()
    {
        if (Current.Type == TokenType.Number)
        {
            var val = double.Parse(Current.Value);
            pos++;
            return new NumberExpr(val);
        }
        if (Current.Type == TokenType.LBracket)
        {
            Kill(TokenType.LBracket);
            Expression expr = ParseExpression();
            Kill(TokenType.RBracket);
            return expr;
        }
        throw new Exception($"Unexpected token: {Current.Type}");
    }
}
