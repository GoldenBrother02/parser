using Paarser;

namespace Tests;

public class ParserTests
{
    [Fact]
    public void Parser_returns_Correct_Expression()
    {
        var parser = new Parser("1+2");

        var expectedlist = new List<string> { "1", "+", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<Number>(nodenized[2]);

        var expr = (Expression)parser.TreeTime(nodenized);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(nodenized[0], expr.Left);
        Assert.Equivalent(nodenized[1], expr.Operator);
        Assert.Equivalent(nodenized[2], expr.Right);
    }

    [Fact]
    public void Parser_returns_Longer_Expressions_Correctly()
    {
        var parser = new Parser("1+2+3+4");

        var expectedlist = new List<string> { "1", "+", "2", "+", "3", "+", "4" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<Number>(nodenized[2]);
        Assert.IsType<BinaryOperator>(nodenized[3]);
        Assert.IsType<Number>(nodenized[4]);
        Assert.IsType<BinaryOperator>(nodenized[5]);
        Assert.IsType<Number>(nodenized[6]);

        var expr = (Expression)parser.TreeTime(nodenized);
        var leftExpression = new Expression(nodenized[0], nodenized[1], nodenized[2]);
        var rightExpression = new Expression(nodenized[4], nodenized[5], nodenized[6]);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(leftExpression, expr.Left);
        Assert.Equivalent(nodenized[3], expr.Operator);
        Assert.Equivalent(rightExpression, expr.Right);
    }

    [Fact]
    public void Wrongly_Built_Expression_Throws()
    {
        var parser = new Parser("1+2+3+4");

        var nodenized = parser.Nodenize(parser.Content);

        var exception = Assert.Throws<ArgumentException>(() => new Expression(nodenized[0], nodenized[0], nodenized[0]));
        Assert.Equal("Operator needs to be an operator", exception.Message);
    }

    [Fact]
    public void Ending_Invalid_Input_Causes_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Parser("1+2+"));

        Assert.Equal("Expression cannot end with operator or '('", ex.Message);
    }

    [Fact]
    public void Middle_Invalid_Input_Causes_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Parser("1++2"));

        Assert.Equal("The expression you have entered is invalid.", ex.Message);
    }
}