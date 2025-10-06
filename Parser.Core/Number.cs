namespace Paarser;

public class Number : ASTnode
{
    public decimal Value { get; set; }


    public Number(decimal value)
    {
        Value = value;
    }
}