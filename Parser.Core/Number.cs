namespace Paarser;

public class Number : ASTnode
{
    public int Value { get; set; }


    public Number(int value)
    {
        Value = value;
    }
}