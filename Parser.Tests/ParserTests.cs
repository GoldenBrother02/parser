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
    public void Parser_returns_Correct_Expression_Even_With_Spaces()
    {
        var parser = new Parser(" 1+ 2");

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
        var firstExpression = new Expression(nodenized[0], nodenized[1], nodenized[2]);
        var secondExpression = new Expression(firstExpression, nodenized[3], nodenized[4]);
        var thirdExpression = new Expression(secondExpression, nodenized[5], nodenized[6]);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(thirdExpression, expr);
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

    [Fact]
    public void Wrongly_Built_Expression_Throws()
    {
        var parser = new Parser("1+2+3+4");

        var nodenized = parser.Nodenize(parser.Content);

        var exception = Assert.Throws<ArgumentException>(() => new Expression(nodenized[0], nodenized[0], nodenized[0]));
        Assert.Equal("Operator needs to be an operator", exception.Message);
    }


    [Fact]
    public void Parser_returns_Longer_Expressions_Correctly_Even_With_Brackets()
    {
        var parser = new Parser("1+(2+(3+4))");

        var expectedlist = new List<string> { "1", "+", "(", "2", "+", "(", "3", "+", "4", ")", ")" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<OpenBracket>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);
        Assert.IsType<BinaryOperator>(nodenized[4]);
        Assert.IsType<OpenBracket>(nodenized[5]);
        Assert.IsType<Number>(nodenized[6]);
        Assert.IsType<BinaryOperator>(nodenized[7]);
        Assert.IsType<Number>(nodenized[8]);
        Assert.IsType<ClosedBracket>(nodenized[9]);
        Assert.IsType<ClosedBracket>(nodenized[10]);

        var expr = (Expression)parser.TreeTime(nodenized);
        var firstExpression = new Expression(nodenized[6], nodenized[7], nodenized[8]);
        var secondExpression = new Expression(nodenized[3], nodenized[4], firstExpression);
        var thirdExpression = new Expression(nodenized[0], nodenized[1], secondExpression);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(thirdExpression, expr);
    }

    [Fact]
    public void Parser_returns_Longer_Expressions_Correctly_Even_With_More_Brackets()
    {
        var parser = new Parser("1+(2+(3+4))+(7+8)");

        var expectedlist = new List<string> { "1", "+", "(", "2", "+", "(", "3", "+", "4", ")", ")", "+", "(", "7", "+", "8", ")" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<OpenBracket>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);
        Assert.IsType<BinaryOperator>(nodenized[4]);
        Assert.IsType<OpenBracket>(nodenized[5]);
        Assert.IsType<Number>(nodenized[6]);
        Assert.IsType<BinaryOperator>(nodenized[7]);
        Assert.IsType<Number>(nodenized[8]);
        Assert.IsType<ClosedBracket>(nodenized[9]);
        Assert.IsType<ClosedBracket>(nodenized[10]);
        Assert.IsType<BinaryOperator>(nodenized[11]);
        Assert.IsType<OpenBracket>(nodenized[12]);
        Assert.IsType<Number>(nodenized[13]);
        Assert.IsType<BinaryOperator>(nodenized[14]);
        Assert.IsType<Number>(nodenized[15]);
        Assert.IsType<ClosedBracket>(nodenized[16]);

        var expr = (Expression)parser.TreeTime(nodenized);
        var firstExpression = new Expression(nodenized[6], nodenized[7], nodenized[8]);
        var secondExpression = new Expression(nodenized[3], nodenized[4], firstExpression);
        var thirdExpression = new Expression(nodenized[0], nodenized[1], secondExpression);
        var fourthExpression = new Expression(nodenized[13], nodenized[14], nodenized[15]);
        var fifthExpression = new Expression(thirdExpression, nodenized[11], fourthExpression);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(fifthExpression, expr);
    }
}