using Xunit;
using Par5er.Core;
using System;
using System.Collections.Generic;

namespace Par5er.Tests;

public class ParserTests
{
    [Fact]
    public void Parser_BuildsCorrectAst_1Plus2()
    {
        var lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize("1+2");
        var parser = new Parser(tokens);

        Expression ast = parser.Parse();

        // Root = "+"
        Assert.IsType<BinaryExpr>(ast);
        var root = (BinaryExpr)ast;
        Assert.Equal(TokenType.Plus, root.Op.Type);

        // Left = 1
        Assert.IsType<NumberExpr>(root.Left);
        Assert.Equal(1, ((NumberExpr)root.Left).Value);

        // Right = 2
        Assert.IsType<NumberExpr>(root.Right);
        Assert.Equal(2, ((NumberExpr)root.Right).Value);
    }

    [Fact]
    public void Parser_BuildsCorrectAst_NegativePowerUnaryParentheses()
    {
        var lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize("-2^--(1+2)");
        var parser = new Parser(tokens);

        Expression ast = parser.Parse();

        // Root = Unary(-, Power)
        Assert.IsType<UnaryExpr>(ast);
        var outerUnary = (UnaryExpr)ast;
        Assert.Equal(TokenType.Minus, outerUnary.Op.Type);

        // Inner = PowerExpr ("^")
        Assert.IsType<BinaryExpr>(outerUnary.Right);
        var power = (BinaryExpr)outerUnary.Right;
        Assert.Equal(TokenType.Power, power.Op.Type);

        // Left = 2
        Assert.IsType<NumberExpr>(power.Left);
        Assert.Equal(2, ((NumberExpr)power.Left).Value);

        // Right = Unary(-, Unary(-, (1+2)))
        Assert.IsType<UnaryExpr>(power.Right);
        var rightUnary1 = (UnaryExpr)power.Right;
        Assert.Equal(TokenType.Minus, rightUnary1.Op.Type);

        Assert.IsType<UnaryExpr>(rightUnary1.Right);
        var rightUnary2 = (UnaryExpr)rightUnary1.Right;
        Assert.Equal(TokenType.Minus, rightUnary2.Op.Type);

        Assert.IsType<BinaryExpr>(rightUnary2.Right);
        var innerAdd = (BinaryExpr)rightUnary2.Right;
        Assert.Equal(TokenType.Plus, innerAdd.Op.Type);

        Assert.Equal(1, ((NumberExpr)innerAdd.Left).Value);
        Assert.Equal(2, ((NumberExpr)innerAdd.Right).Value);
    }

    [Fact]
    public void Parser_BuildsCorrectAst_UnaryAndBinaryOperators()
    {
        var lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize("-(2 + 3) * 4");
        var parser = new Parser(tokens);

        Expression ast = parser.Parse();

        // Root = "*"
        Assert.IsType<BinaryExpr>(ast);
        var root = (BinaryExpr)ast;
        Assert.Equal(TokenType.Multiply, root.Op.Type);

        // LeftRoot = UnaryExpr
        Assert.IsType<UnaryExpr>(root.Left);
        var Unary = (UnaryExpr)root.Left;
        Assert.Equal(TokenType.Minus, Unary.Op.Type);

        // Unary = "+"
        Assert.IsType<BinaryExpr>(Unary.Right);
        var leftBinary = (BinaryExpr)Unary.Right;
        Assert.Equal(TokenType.Plus, leftBinary.Op.Type);

        // BinaryExpr = 2 and 3
        Assert.IsType<NumberExpr>(leftBinary.Left);
        Assert.IsType<NumberExpr>(leftBinary.Right);
        Assert.Equal(2, ((NumberExpr)leftBinary.Left).Value);
        Assert.Equal(3, ((NumberExpr)leftBinary.Right).Value);

        // RightRoot = 4
        Assert.IsType<NumberExpr>(root.Right);
        Assert.Equal(4, ((NumberExpr)root.Right).Value);
    }

    [Fact]
    public void Parser_BuildsCorrectAst_PowerAndUnary()
    {
        var lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize("--2^3");
        var parser = new Parser(tokens);

        Expression ast = parser.Parse();

        // Root = Unary(-)
        Assert.IsType<UnaryExpr>(ast);
        var outerUnary = (UnaryExpr)ast;
        Assert.Equal(TokenType.Minus, outerUnary.Op.Type);

        // Next = Unary(-)
        Assert.IsType<UnaryExpr>(outerUnary.Right);
        var innerUnary = (UnaryExpr)outerUnary.Right;
        Assert.Equal(TokenType.Minus, innerUnary.Op.Type);

        // Then = PowerExpr ("^")
        Assert.IsType<BinaryExpr>(innerUnary.Right);
        var power = (BinaryExpr)innerUnary.Right;
        Assert.Equal(TokenType.Power, power.Op.Type);

        // Left = 2
        Assert.IsType<NumberExpr>(power.Left);
        Assert.Equal(2, ((NumberExpr)power.Left).Value);

        // Right = 3
        Assert.IsType<NumberExpr>(power.Right);
        Assert.Equal(3, ((NumberExpr)power.Right).Value);
    }

    [Fact]
    public void Parser_ParsePrimary_Throws_OnUnexpectedToken()
    {
        var tokens = new List<Token>
    {
        new Token(TokenType.END, "") // now it's END that causes failure
    };
        var parser = new Parser(tokens);

        var ex = Assert.Throws<Exception>(() => parser.Parse());
        Assert.Contains("Unexpected token", ex.Message);
    }

    [Fact]
    public void Parser_Overkill() //GPT
    {
        var exprStr = "- -- --(3  *(12 + 23 ^ 2 ^ 2 ^ -2)+6/(3*(1-5)^ 3*       2)-(  (  (1+3 )^   2)^2 )  *(5+6-(5+6)) ^2)    + 1)";
        var lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(exprStr);
        var parser = new Parser(tokens);

        Expression ast = parser.Parse();

        // Root is top-level BinaryExpr '+' (from the final +1)
        Assert.IsType<BinaryExpr>(ast);
        var plusRoot = (BinaryExpr)ast;
        Assert.Equal(TokenType.Plus, plusRoot.Op.Type);

        // Right side of '+': NumberExpr 1
        Assert.IsType<NumberExpr>(plusRoot.Right);
        Assert.Equal(1, ((NumberExpr)plusRoot.Right).Value);

        // Left side of '+': the big expression wrapped by leading 5 minuses
        Assert.IsType<UnaryExpr>(plusRoot.Left);
        var u1 = (UnaryExpr)plusRoot.Left;
        Assert.Equal(TokenType.Minus, u1.Op.Type);

        Assert.IsType<UnaryExpr>(u1.Right);
        var u2 = (UnaryExpr)u1.Right;
        Assert.Equal(TokenType.Minus, u2.Op.Type);

        Assert.IsType<UnaryExpr>(u2.Right);
        var u3 = (UnaryExpr)u2.Right;
        Assert.Equal(TokenType.Minus, u3.Op.Type);

        Assert.IsType<UnaryExpr>(u3.Right);
        var u4 = (UnaryExpr)u3.Right;
        Assert.Equal(TokenType.Minus, u4.Op.Type);

        Assert.IsType<UnaryExpr>(u4.Right);
        var u5 = (UnaryExpr)u4.Right;
        Assert.Equal(TokenType.Minus, u5.Op.Type);

        // Inside the fifth minus: top-level BinaryExpr '-' (big subtraction)
        Assert.IsType<BinaryExpr>(u5.Right);
        var outerSub = (BinaryExpr)u5.Right;
        Assert.Equal(TokenType.Minus, outerSub.Op.Type);

        // Left side of subtraction: BinaryExpr '+' (3*(12+23^2^2^-2) + 6/(3*(1-5)^3*2))
        Assert.IsType<BinaryExpr>(outerSub.Left);
        var leftAdd = (BinaryExpr)outerSub.Left;
        Assert.Equal(TokenType.Plus, leftAdd.Op.Type);

        // Left side of leftAdd: 3 * (12 + 23^2^2^-2)
        Assert.IsType<BinaryExpr>(leftAdd.Left);
        var mult1 = (BinaryExpr)leftAdd.Left;
        Assert.Equal(TokenType.Multiply, mult1.Op.Type);
        Assert.IsType<NumberExpr>(mult1.Left);
        Assert.Equal(3, ((NumberExpr)mult1.Left).Value);

        Assert.IsType<BinaryExpr>(mult1.Right);
        var innerAdd = (BinaryExpr)mult1.Right;
        Assert.Equal(TokenType.Plus, innerAdd.Op.Type);
        Assert.IsType<NumberExpr>(innerAdd.Left);
        Assert.Equal(12, ((NumberExpr)innerAdd.Left).Value);

        Assert.IsType<BinaryExpr>(innerAdd.Right);
        var power1 = (BinaryExpr)innerAdd.Right;
        Assert.Equal(TokenType.Power, power1.Op.Type);
        Assert.IsType<NumberExpr>(power1.Left);
        Assert.Equal(23, ((NumberExpr)power1.Left).Value);

        Assert.IsType<BinaryExpr>(power1.Right);
        var power2 = (BinaryExpr)power1.Right;
        Assert.Equal(TokenType.Power, power2.Op.Type);
        Assert.IsType<NumberExpr>(power2.Left);
        Assert.Equal(2, ((NumberExpr)power2.Left).Value);

        Assert.IsType<BinaryExpr>(power2.Right);
        var power3 = (BinaryExpr)power2.Right;
        Assert.Equal(TokenType.Power, power3.Op.Type);
        Assert.IsType<NumberExpr>(power3.Left);
        Assert.Equal(2, ((NumberExpr)power3.Left).Value);

        Assert.IsType<UnaryExpr>(power3.Right);
        var unaryNeg2 = (UnaryExpr)power3.Right;
        Assert.Equal(TokenType.Minus, unaryNeg2.Op.Type);
        Assert.IsType<NumberExpr>(unaryNeg2.Right);
        Assert.Equal(2, ((NumberExpr)unaryNeg2.Right).Value);

        // Right side of leftAdd: 6 / (3*(1-5)^3*2)
        Assert.IsType<BinaryExpr>(leftAdd.Right);
        var div1 = (BinaryExpr)leftAdd.Right;
        Assert.Equal(TokenType.Divide, div1.Op.Type);
        Assert.IsType<NumberExpr>(div1.Left);
        Assert.Equal(6, ((NumberExpr)div1.Left).Value);

        Assert.IsType<BinaryExpr>(div1.Right);
        var mult2 = (BinaryExpr)div1.Right;
        Assert.Equal(TokenType.Multiply, mult2.Op.Type);

        // mult2.Left = 3*(1-5)^3
        Assert.IsType<BinaryExpr>(mult2.Left);
        var mult3 = (BinaryExpr)mult2.Left;
        Assert.Equal(TokenType.Multiply, mult3.Op.Type);
        Assert.IsType<NumberExpr>(mult3.Left);
        Assert.Equal(3, ((NumberExpr)mult3.Left).Value);

        Assert.IsType<BinaryExpr>(mult3.Right);
        var power4 = (BinaryExpr)mult3.Right;
        Assert.Equal(TokenType.Power, power4.Op.Type);

        Assert.IsType<BinaryExpr>(power4.Left);
        var sub1 = (BinaryExpr)power4.Left;
        Assert.Equal(TokenType.Minus, sub1.Op.Type);
        Assert.IsType<NumberExpr>(sub1.Left);
        Assert.Equal(1, ((NumberExpr)sub1.Left).Value);
        Assert.IsType<NumberExpr>(sub1.Right);
        Assert.Equal(5, ((NumberExpr)sub1.Right).Value);

        Assert.IsType<NumberExpr>(power4.Right);
        Assert.Equal(3, ((NumberExpr)power4.Right).Value);

        Assert.IsType<NumberExpr>(mult2.Right);
        Assert.Equal(2, ((NumberExpr)mult2.Right).Value);

        // Right side of outerSub: (((1+3)^2)^2)*(5+6-(5+6))^2
        Assert.IsType<BinaryExpr>(outerSub.Right);
        var mult4 = (BinaryExpr)outerSub.Right;
        Assert.Equal(TokenType.Multiply, mult4.Op.Type);

        // mult4.Left = ((1+3)^2)^2
        Assert.IsType<BinaryExpr>(mult4.Left);
        var power5 = (BinaryExpr)mult4.Left;
        Assert.Equal(TokenType.Power, power5.Op.Type);
        Assert.IsType<BinaryExpr>(power5.Left);
        var power6 = (BinaryExpr)power5.Left;
        Assert.Equal(TokenType.Power, power6.Op.Type);

        Assert.IsType<BinaryExpr>(power6.Left);
        var innerAdd2 = (BinaryExpr)power6.Left;
        Assert.Equal(TokenType.Plus, innerAdd2.Op.Type);
        Assert.IsType<NumberExpr>(innerAdd2.Left);
        Assert.Equal(1, ((NumberExpr)innerAdd2.Left).Value);
        Assert.IsType<NumberExpr>(innerAdd2.Right);
        Assert.Equal(3, ((NumberExpr)innerAdd2.Right).Value);

        Assert.IsType<NumberExpr>(power6.Right);
        Assert.Equal(2, ((NumberExpr)power6.Right).Value);
        Assert.IsType<NumberExpr>(power5.Right);
        Assert.Equal(2, ((NumberExpr)power5.Right).Value);

        // mult4.Right = (5+6-(5+6))^2
        Assert.IsType<BinaryExpr>(mult4.Right);
        var power7 = (BinaryExpr)mult4.Right;
        Assert.Equal(TokenType.Power, power7.Op.Type);

        Assert.IsType<BinaryExpr>(power7.Left);
        var sub2 = (BinaryExpr)power7.Left;
        Assert.Equal(TokenType.Minus, sub2.Op.Type);

        Assert.IsType<BinaryExpr>(sub2.Left);
        var add1 = (BinaryExpr)sub2.Left;
        Assert.Equal(TokenType.Plus, add1.Op.Type);
        Assert.IsType<NumberExpr>(add1.Left);
        Assert.Equal(5, ((NumberExpr)add1.Left).Value);
        Assert.IsType<NumberExpr>(add1.Right);
        Assert.Equal(6, ((NumberExpr)add1.Right).Value);

        Assert.IsType<BinaryExpr>(sub2.Right);
        var add2 = (BinaryExpr)sub2.Right;
        Assert.Equal(TokenType.Plus, add2.Op.Type);
        Assert.IsType<NumberExpr>(add2.Left);
        Assert.Equal(5, ((NumberExpr)add2.Left).Value);
        Assert.IsType<NumberExpr>(add2.Right);
        Assert.Equal(6, ((NumberExpr)add2.Right).Value);

        Assert.IsType<NumberExpr>(power7.Right);
        Assert.Equal(2, ((NumberExpr)power7.Right).Value);
    }
}
