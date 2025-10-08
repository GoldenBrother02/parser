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

    [Fact]
    public void Parser_returns_Correct_Expression_With_Power()
    {
        var parser = new Parser("3^2");

        var expectedlist = new List<string> { "3", "^", "2" };
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
    public void Parser_returns_Correct_Expression_With_More_Power()
    {
        var parser = new Parser("1+3^2");

        var expectedlist = new List<string> { "1", "+", "3", "^", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<Number>(nodenized[2]);
        Assert.IsType<BinaryOperator>(nodenized[3]);
        Assert.IsType<Number>(nodenized[4]);

        var expr = (Expression)parser.TreeTime(nodenized);
        var firstExpression = new Expression(nodenized[2], nodenized[3], nodenized[4]);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(nodenized[0], expr.Left);
        Assert.Equivalent(nodenized[1], expr.Operator);
        Assert.Equivalent(firstExpression, expr.Right);
    }

    [Fact]
    public void Parser_returns_Correct_Expression_With_Even_More_Power()
    {
        var parser = new Parser("1+3^2^2");

        var expectedlist = new List<string> { "1", "+", "3", "^", "2", "^", "2" };
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
        var firstExpression = new Expression(nodenized[4], nodenized[5], nodenized[6]);
        var secondExpression = new Expression(nodenized[2], nodenized[3], firstExpression);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(nodenized[0], expr.Left);
        Assert.Equivalent(nodenized[1], expr.Operator);
        Assert.Equivalent(secondExpression, expr.Right);
    }

    [Fact]
    public void Parser_returns_Correct_Expression_With_Multiplicative_Power()
    {
        var parser = new Parser("7*3^2*8");

        var expectedlist = new List<string> { "7", "*", "3", "^", "2", "*", "8" };
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
        var firstExpression = new Expression(nodenized[2], nodenized[3], nodenized[4]);
        var secondExpression = new Expression(nodenized[0], nodenized[1], firstExpression);
        var thirdExpression = new Expression(secondExpression, nodenized[5], nodenized[6]);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(thirdExpression, expr);
    }

    [Fact]
    public void Parser_returns_Correct_Expression_With_Multiplication()
    {
        var parser = new Parser("1+3*2");

        var expectedlist = new List<string> { "1", "+", "3", "*", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<Number>(nodenized[2]);
        Assert.IsType<BinaryOperator>(nodenized[3]);
        Assert.IsType<Number>(nodenized[4]);

        var expr = (Expression)parser.TreeTime(nodenized);
        var firstExpression = new Expression(nodenized[2], nodenized[3], nodenized[4]);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(nodenized[0], expr.Left);
        Assert.Equivalent(nodenized[1], expr.Operator);
        Assert.Equivalent(firstExpression, expr.Right);
    }

    [Fact]
    public void Parser_returns_Correct_Expression_With_Multiplication_And_Power()
    {
        var parser = new Parser("1+3*2^2");

        var expectedlist = new List<string> { "1", "+", "3", "*", "2", "^", "2" };
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
        var firstExpression = new Expression(nodenized[4], nodenized[5], nodenized[6]);
        var secondExpression = new Expression(nodenized[2], nodenized[3], firstExpression);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(nodenized[0], expr.Left);
        Assert.Equivalent(nodenized[1], expr.Operator);
        Assert.Equivalent(secondExpression, expr.Right);
    }

    [Fact]
    public void Ultimate_Test()
    {
        var parser = new Parser("1 +(2*   (3+4))+(7  ^8)+2 ^3");

        var expectedlist = new List<string> { "1", "+", "(", "2", "*", "(", "3", "+", "4", ")", ")", "+", "(", "7", "^", "8", ")", "+", "2", "^", "3" };
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
        Assert.IsType<BinaryOperator>(nodenized[17]);
        Assert.IsType<Number>(nodenized[18]);
        Assert.IsType<BinaryOperator>(nodenized[19]);
        Assert.IsType<Number>(nodenized[20]);

        var expr = (Expression)parser.TreeTime(nodenized);
        var firstExpression = new Expression(nodenized[6], nodenized[7], nodenized[8]);
        var secondExpression = new Expression(nodenized[3], nodenized[4], firstExpression);
        var thirdExpression = new Expression(nodenized[13], nodenized[14], nodenized[15]);
        var fourthExpression = new Expression(nodenized[18], nodenized[19], nodenized[20]);
        var fifthExpression = new Expression(nodenized[0], nodenized[1], secondExpression);
        var sixthExpression = new Expression(fifthExpression, nodenized[11], thirdExpression);
        var seventhExpression = new Expression(sixthExpression, nodenized[17], fourthExpression);
        Assert.IsType<Expression>(expr);
        Assert.Equivalent(seventhExpression, expr);
    }

    [Fact]
    public void Decimal_Work_Now()
    {
        var parser = new Parser("1.5+2.3");

        var expectedlist = new List<string> { "1.5", "+", "2.3" };
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
    public void Minus_Works_Properly()
    {
        var parser = new Parser("(1+2)-3");
        var expectedTokens = new List<string> { "(", "1", "+", "2", ")", "-", "3" };
        Assert.Equal(expectedTokens, parser.Content);
        var nodenize = parser.Nodenize(parser.Content);
        Assert.IsType<OpenBracket>(nodenize[0]);
        Assert.IsType<Number>(nodenize[1]);
        Assert.IsType<BinaryOperator>(nodenize[2]);
        Assert.IsType<Number>(nodenize[3]);
        Assert.IsType<ClosedBracket>(nodenize[4]);
        Assert.IsType<BinaryOperator>(nodenize[5]);
        Assert.IsType<Number>(nodenize[6]);
    }

    [Fact]
    public void UnaryOperator_Nodenizes_V1()
    {
        var parser = new Parser("-1+2");

        var expectedlist = new List<string> { "-", "1", "+", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<UnaryOperator>(nodenized[0]);
        Assert.IsType<Number>(nodenized[1]);
        Assert.IsType<BinaryOperator>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);
    }

    [Fact]
    public void UnaryOperator_Nodenizes_V2()
    {
        var parser = new Parser("1+-2");

        var expectedlist = new List<string> { "1", "+", "-", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<UnaryOperator>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);
    }

    [Fact]
    public void UnaryOperator_Nodenizes_V3()
    {
        var parser = new Parser("1--2");

        var expectedlist = new List<string> { "1", "-", "-", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<UnaryOperator>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);
    }

    [Fact]
    public void UnaryOperator_Nodenizes_V4()
    {
        var parser = new Parser("1^-2");

        var expectedlist = new List<string> { "1", "^", "-", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<UnaryOperator>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);
    }

    [Fact]
    public void UnaryOperator_Nodenizes_V5()
    {
        var parser = new Parser("-(1+2)");

        var expectedlist = new List<string> { "-", "(", "1", "+", "2", ")" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<UnaryOperator>(nodenized[0]);
        Assert.IsType<OpenBracket>(nodenized[1]);
        Assert.IsType<Number>(nodenized[2]);
        Assert.IsType<BinaryOperator>(nodenized[3]);
        Assert.IsType<Number>(nodenized[4]);
        Assert.IsType<ClosedBracket>(nodenized[5]);
    }

    [Fact]
    public void UnaryOperator_Nodenizes_V6()
    {
        var parser = new Parser("1*-2");

        var expectedlist = new List<string> { "1", "*", "-", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<UnaryOperator>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);
    }

    [Fact]
    public void UnaryOperator_Parsing_V1()
    {
        var parser = new Parser("-(1+2)");

        var expectedlist = new List<string> { "-", "(", "1", "+", "2", ")" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<UnaryOperator>(nodenized[0]);
        Assert.IsType<OpenBracket>(nodenized[1]);
        Assert.IsType<Number>(nodenized[2]);
        Assert.IsType<BinaryOperator>(nodenized[3]);
        Assert.IsType<Number>(nodenized[4]);
        Assert.IsType<ClosedBracket>(nodenized[5]);

        var expr = (Expression)parser.TreeTime(nodenized);
        Assert.IsType<Expression>(expr);
        ASTnode minusone = new Number(-1);
        ASTnode multiply = new BinaryOperator("*");
        var firstExpression = new Expression(nodenized[2], nodenized[3], nodenized[4]);
        var secondExpression = new Expression(minusone, multiply, firstExpression);
        Assert.Equivalent(secondExpression, expr);
    }

    [Fact]
    public void UnaryOperator_Parsing_V2()
    {
        var parser = new Parser("2^-(1+2)");

        var expectedlist = new List<string> { "2", "^", "-", "(", "1", "+", "2", ")" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<Number>(nodenized[0]);
        Assert.IsType<BinaryOperator>(nodenized[1]);
        Assert.IsType<UnaryOperator>(nodenized[2]);
        Assert.IsType<OpenBracket>(nodenized[3]);
        Assert.IsType<Number>(nodenized[4]);
        Assert.IsType<BinaryOperator>(nodenized[5]);
        Assert.IsType<Number>(nodenized[6]);
        Assert.IsType<ClosedBracket>(nodenized[7]);

        var expr = (Expression)parser.TreeTime(nodenized);
        Assert.IsType<Expression>(expr);
        ASTnode minusone = new Number(-1);
        ASTnode multiply = new BinaryOperator("*");
        var firstExpression = new Expression(nodenized[4], nodenized[5], nodenized[6]);
        var secondExpression = new Expression(minusone, multiply, firstExpression);
        var thirdExpression = new Expression(nodenized[0], nodenized[1], secondExpression);
        Assert.Equivalent(thirdExpression, expr);
    }

    [Fact]
    public void UnaryOperator_Parsing_V3()
    {
        var parser = new Parser("-2^2");

        var expectedlist = new List<string> { "-", "2", "^", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<UnaryOperator>(nodenized[0]);
        Assert.IsType<Number>(nodenized[1]);
        Assert.IsType<BinaryOperator>(nodenized[2]);
        Assert.IsType<Number>(nodenized[3]);


        var expr = (Expression)parser.TreeTime(nodenized);
        Assert.IsType<Expression>(expr);
        ASTnode minusone = new Number(-1);
        ASTnode multiply = new BinaryOperator("*");
        var firstExpression = new Expression(nodenized[1], nodenized[2], nodenized[3]);
        var secondExpression = new Expression(minusone, multiply, firstExpression);
        Assert.Equivalent(secondExpression, expr);
    }

    [Fact]
    public void UnaryOperator_Parsing_V4()
    {
        var parser = new Parser("-2^-2");

        var expectedlist = new List<string> { "-", "2", "^", "-", "2" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<UnaryOperator>(nodenized[0]);
        Assert.IsType<Number>(nodenized[1]);
        Assert.IsType<BinaryOperator>(nodenized[2]);
        Assert.IsType<UnaryOperator>(nodenized[3]);
        Assert.IsType<Number>(nodenized[4]);


        var expr = (Expression)parser.TreeTime(nodenized);
        Assert.IsType<Expression>(expr);
        ASTnode minusone = new Number(-1);
        ASTnode multiply = new BinaryOperator("*");
        var innerunary = new Expression(minusone, multiply, nodenized[4]);
        var firstExpression = new Expression(nodenized[1], nodenized[2], innerunary);
        var secondExpression = new Expression(minusone, multiply, firstExpression);
        Assert.Equivalent(secondExpression, expr);
    }

    [Fact]
    public void UnaryOperator_Parsing_V5()
    {
        var parser = new Parser("-2^-(1+2)");

        var expectedlist = new List<string> { "-", "2", "^", "-", "(", "1", "+", "2", ")" };
        Assert.Equal(parser.Content, expectedlist);

        var nodenized = parser.Nodenize(parser.Content);
        Assert.IsType<List<ASTnode>>(nodenized);
        Assert.IsType<UnaryOperator>(nodenized[0]);
        Assert.IsType<Number>(nodenized[1]);
        Assert.IsType<BinaryOperator>(nodenized[2]);
        Assert.IsType<UnaryOperator>(nodenized[3]);
        Assert.IsType<OpenBracket>(nodenized[4]);
        Assert.IsType<Number>(nodenized[5]);
        Assert.IsType<BinaryOperator>(nodenized[6]);
        Assert.IsType<Number>(nodenized[7]);
        Assert.IsType<ClosedBracket>(nodenized[8]);

        var expr = (Expression)parser.TreeTime(nodenized);
        Assert.IsType<Expression>(expr);
        ASTnode minusone = new Number(-1);
        ASTnode multiply = new BinaryOperator("*");
        var firstExpression = new Expression(nodenized[5], nodenized[6], nodenized[7]);
        var innerunary = new Expression(minusone, multiply, firstExpression);
        var secondExpression = new Expression(nodenized[1], nodenized[2], innerunary);
        var thirdExpression = new Expression(minusone, multiply, secondExpression);
        Assert.Equivalent(thirdExpression, expr);
    }
}
