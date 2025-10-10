namespace Par5er.Tests;

using System.Reflection;
using Par5er.Core;
using Xunit;
using Xunit.Sdk;

public class EvaluatorTest
{
    [Theory]
    [InlineData("1+2", 3)]
    [InlineData("2*3+4", 10)]
    [InlineData("2*(3+4)", 14)]
    [InlineData("(1+2)^3", 27)]
    [InlineData("-5+3", -2)]
    [InlineData("-(2+3)*4", -20)]
    [InlineData("--5", 5)]
    [InlineData("2^-3", 0.125)]
    [InlineData("-2^--(1+2)", -8)]
    [InlineData("-5^2", -25)]
    [InlineData("- -- --(3  *(12 + 23 ^ 2 ^ 2 ^ -2)+6/(3*(1-5)^ 3*       2)-(  (  (1+3 )^   2)^2 )  *(5+6-(5+6)) ^2)    + 1)", -159.865754069)]
    [InlineData("3+4*2/(1-5)^2", 3 + 4 * 2 / 16.0)] // Math.Pow(1-5,2) = 16
    public void ParserEvaluator_CalculatesCorrectly(string expr, double expected)
    {
        var lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(expr);
        var parser = new Parser(tokens);
        var evaluator = new Evaluator();

        Expression AST = parser.Parse();
        double result = evaluator.Eval(AST);

        Assert.Equal(expected, result, 5); // what's a precision? how many numbers after , should be same
    }

    [Fact]
    public void ParserEvaluator_ThrowsOnMissingBracket()
    {
        var lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize("(1+2"); // no ")"
        var parser = new Parser(tokens);
        var evaluator = new Evaluator();

        var ex = Assert.Throws<Exception>(() =>
        {
            Expression ast = parser.Parse();
            evaluator.Eval(ast);
        });

        Assert.Contains("Expected RBracket", ex.Message);
    }

    [Fact]
    public void ParserEvaluator_CanFactorial()
    {
        var evaluator = new Evaluator();

        var result = evaluator.factorial(5);

        Assert.Equal(120, result);
    }

    [Fact]
    public void ParserEvaluator_ThrowsVarious()
    {
        var expression = new NumberExpr(5);
        var unary = new UnaryExpr(new Token(TokenType.Divide, "test"), expression);
        var binary = new BinaryExpr(expression, new Token(TokenType.END, "5"), expression);
        var invalid = new Invaled();
        var evaluator = new Evaluator();

        var ex = Assert.Throws<Exception>(() =>
        {
            evaluator.Eval(unary);
        });

        Assert.Contains("Unknown unary operator", ex.Message);

        var ex2 = Assert.Throws<Exception>(() =>
        {
            evaluator.Eval(binary);
        });

        Assert.Contains("Unknown binary operator", ex2.Message);

        var ex3 = Assert.Throws<Exception>(() =>
        {
            evaluator.Eval(invalid);
        });

        Assert.Contains("Unknown expression type", ex3.Message);
    }
}