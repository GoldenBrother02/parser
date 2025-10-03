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
}