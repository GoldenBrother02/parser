namespace Paarser;

public class OpenBracket : ASTnode
{
    private string token;

    public OpenBracket(string token)
    {
        this.token = token;
    }
}

public class ClosedBracket : ASTnode
{
    private string token;

    public ClosedBracket(string token)
    {
        this.token = token;
    }
}