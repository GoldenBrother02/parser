using Paarser;

namespace Tests;

public class DivisionTests
{
    [Fact]
    public void Division_LeftAssociative()
    {
        var parser = new Parser("1/2/3");
        var nodenized = parser.Nodenize(parser.Content);
        var ast = parser.TreeTime(nodenized);
        var node = Assert.IsType<Expression>(ast);
        //        (/)
        //       /   \
        //     (/)    3
        //    /   \
        //   1     2
        Assert.Equal("/", Assert.IsType<BinaryOperator>(node.Operator).Type);
        var left = Assert.IsType<Expression>(node.Left);
        Assert.Equal("/", Assert.IsType<BinaryOperator>(left.Operator).Type);
        Assert.Equal(1, Assert.IsType<Number>(left.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(left.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(node.Right).Value);
    }
}