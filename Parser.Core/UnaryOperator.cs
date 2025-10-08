namespace Paarser;

public class UnaryOperator : ASTnode
{
    public string Type { get; set; }

    public UnaryOperator(string type)
    {
        Type = type;
    }


}