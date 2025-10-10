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

    // term = unary (("*" || "/") unary), X amount of times
    //now unary in stead of power
    private Expression ParseTerm()
    {
        Expression expr = ParseUnary();
        while (Current.Type == TokenType.Multiply || Current.Type == TokenType.Divide)
        {
            Token op = Current;
            pos++;
            Expression right = ParseUnary();
            expr = new BinaryExpr(expr, op, right);
        }
        return expr;
    }

    // unary = ("+" || "-") unary || power
    // unary before power
    private Expression ParseUnary()
    {
        if (Current.Type == TokenType.Plus || Current.Type == TokenType.Minus)  //also allowed positve unary
        {
            Token op = Current;
            pos++;
            Expression right = ParseUnary();
            return new UnaryExpr(op, right);
        }
        return ParsePower();
    }

    // power = primary ("^" unary) with potentially more powers
    // now primary in stead of unary cus order swap
    private Expression ParsePower()
    {
        Expression left = ParsePrimary();
        if (Current.Type == TokenType.Power)
        {
            Token op = Current;
            pos++;
            Expression right = ParseUnary();
            return new BinaryExpr(left, op, right);
        }
        return left;
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
