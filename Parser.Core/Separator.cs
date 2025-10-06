namespace Paarser;

public class Separator : ASTnode
{
    public string Type { get; set; }

    public Separator(string type)
    {
        Type = type;
    }
}