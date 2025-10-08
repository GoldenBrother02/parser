namespace Par5er.Core;

public abstract class Expression { }

public class NumberExpr : Expression
{
    public double Value;
    public NumberExpr(double value)
    {
        Value = value;
    }
}

public class UnaryExpr : Expression
{
    public Token Op;
    public Expression Right;
    public UnaryExpr(Token op, Expression right)
    {
        Op = op;
        Right = right;
    }
}

public class BinaryExpr : Expression
{
    public Expression Left;
    public Token Op;
    public Expression Right;
    public BinaryExpr(Expression left, Token op, Expression right)
    {
        Left = left;
        Op = op;
        Right = right;
    }
}

public class Invaled : Expression
{

}
