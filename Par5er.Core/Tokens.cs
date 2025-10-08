namespace Par5er.Core;

public enum TokenType
{
    Number,
    Plus,
    Minus,
    Multiply,
    Divide,
    Power,
    LBracket,
    RBracket,
    Factor,
    END  //out of range BS
}

public record Token(TokenType Type, string Value);

