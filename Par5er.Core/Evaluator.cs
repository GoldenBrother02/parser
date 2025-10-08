namespace Par5er.Core;

public class Evaluator
{
    public double Eval(Expression expr)
    {
        return expr switch
        {
            NumberExpr n => n.Value,
            UnaryExpr u => u.Op.Type switch
            {
                TokenType.Minus => -Eval(u.Right),
                _ => throw new Exception("Unknown unary operator")
            },
            BinaryExpr b => b.Op.Type switch
            {
                TokenType.Plus => Eval(b.Left) + Eval(b.Right),
                TokenType.Minus => Eval(b.Left) - Eval(b.Right),
                TokenType.Multiply => Eval(b.Left) * Eval(b.Right),
                TokenType.Divide => Eval(b.Left) / Eval(b.Right),
                TokenType.Power => Math.Pow(Eval(b.Left), Eval(b.Right)),
                _ => throw new Exception("Unknown binary operator")
            },
            _ => throw new Exception("Unknown expression type")
        };
    }

    public int factorial(int n)  //add in future?
    {
        int ans = 1;
        for (int i = 2; i <= n; i++)
        {
            ans = ans * i;
        }
        return ans;
    }
}