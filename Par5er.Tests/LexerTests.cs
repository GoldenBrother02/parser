using Xunit;
using Par5er.Core;
using System;
using System.Collections.Generic;

namespace Par5er.Tests;

public class LexerTests
{
    [Fact]
    public void Lexer_Throws_OnInvalid()
    {
        var lexer = new Lexer();

        var ex = Assert.Throws<Exception>(() => lexer.Tokenize("1+2@3"));
        Assert.Equal("Unexpected character: @", ex.Message);
    }

    [Fact]
    public void Lexer_Throws_OnMultipleNumbers()
    {
        var lexer = new Lexer();

        var ex = Assert.Throws<Exception>(() => lexer.Tokenize("1 2"));
        Assert.Equal("Unexpected token sequence: 1 2", ex.Message);
    }

    [Fact]
    public void Lexer_Throws_OnInvalidNumberWithMultipleDots()
    {
        var lexer = new Lexer();

        var ex = Assert.Throws<Exception>(() => lexer.Tokenize("3.3.3"));
        Assert.Equal("Invalid number: 3.3.3", ex.Message);
    }
}