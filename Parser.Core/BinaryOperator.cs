namespace Paarser;

public class BinaryOperator : ASTnode
{
    public string Type { get; set; }

    public BinaryOperator(string type)
    {
        Type = type;
    }
}