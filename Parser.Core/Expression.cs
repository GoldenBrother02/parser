namespace Paarser;

public class Expression : ASTnode
{
    public ASTnode Left { get; set; }

    public ASTnode Right { get; set; }

    public ASTnode Operator { get; set; }

    public Expression(ASTnode left, ASTnode op, ASTnode right)
    {
        if (op.GetType() != typeof(BinaryOperator)) { throw new Exception("Operator needs to be an operator"); }
        Left = left;
        Right = right;
        Operator = op;
    }
}