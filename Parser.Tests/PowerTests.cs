using Paarser;

namespace Tests;

public class PowerTests
{
    [Fact]
    public void RightAssociative()
    {
        var parser = new Parser("1^2^3");

        var expectedlist = new List<string> { "1", "^", "2", "^", "3" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        var op1 = Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.Equal("^", op1.Type);
        Assert.IsType<Number>(nodenized[2]);
        var op2 = Assert.IsType<BinaryOperator>(nodenized[3]);
        Assert.Equal("^", op2.Type);
        Assert.IsType<Number>(nodenized[4]);

        var ast = parser.TreeTime(nodenized);
        var node = Assert.IsType<Expression>(ast);
        //       (^)
        //      /   \
        //     1    (^)
        //         /   \
        //        2     3
        Assert.Equal("^", Assert.IsType<BinaryOperator>(node.Operator).Type);
        Assert.Equal(1, Assert.IsType<Number>(node.Left).Value);
        var right = Assert.IsType<Expression>(node.Right);
        Assert.Equal("^", Assert.IsType<BinaryOperator>(right.Operator).Type);
        Assert.Equal(2, Assert.IsType<Number>(right.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(right.Right).Value);
    }
}