using BNA.Common;
using BNA.Compile;
using BNA.Compile.Statments;
using BNA.Compile.Tokens;
using BNA.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnitTestProject.CompiletimeTests
{
    [TestClass]
    public class HappyParserTests
    {
        [SuppressMessage( "Naming", "CA1720:Identifier contains type name",
            Justification = "TODO: Use operand from BNA lib" )]
        public enum OperandType { ANY, NUMERIC, STRING, VARIABLE }

        public IReadOnlyDictionary<OperandType, IReadOnlyList<Token>> OperandsByType { get; }

        public Token Operand1 { get; }

        public HappyParserTests( )
        {
            Token tokenVariable = new( "var", TokenType.Variable );
            Token tokenLiteral = new( "1.0", TokenType.LiteralNumber );
            Token tokenString = new( "\"my string\"", TokenType.LiteralString );
            Token tokenList = new( "(0, 0)", TokenType.List );

            var anyTypeOperands = new List<Token>( ) { tokenVariable, tokenLiteral, tokenString, tokenList };
            var numericTypeOperands = new List<Token>( ) { tokenVariable, tokenLiteral };
            var stringTypeOperands = new List<Token>( ) { tokenVariable, tokenString };
            var variableTypeOperands = new List<Token>( ) { tokenVariable };

            this.OperandsByType = new Dictionary<OperandType, IReadOnlyList<Token>>
            {
                { OperandType.ANY , anyTypeOperands },
                { OperandType.NUMERIC , numericTypeOperands },
                { OperandType.STRING , stringTypeOperands },
                { OperandType.VARIABLE , variableTypeOperands },
            };

            this.Operand1 = tokenVariable;
        }

        [TestMethod]
        [DataRow( false, DisplayName = "Empty line" )]
        [DataRow( true, DisplayName = "Comment" )]
        public void Parser_ParseStatement_NullStatements( bool comment )
        {
            Token token = new( "# a comment", TokenType.Comment );
            string line = comment ? token.Value : string.Empty;
            Statement expected = new( line, Operation.None );
            List<Token> tokens = [];
            tokens.AddIf( comment, token );
            Parser parser = new( line, tokens );

            Statement actual = parser.ParseStatement( );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [DataRow( Operation.Set, Keyword.SET, Keyword.TO, OperandType.ANY )]
        [DataRow( Operation.Multiply, Keyword.MULTIPLY, Keyword.BY, OperandType.NUMERIC )]
        [DataRow( Operation.Divide, Keyword.DIVIDE, Keyword.BY, OperandType.NUMERIC )]
        [DataRow( Operation.Power, Keyword.RAISE, Keyword.TO, OperandType.NUMERIC )]
        [DataRow( Operation.Random, Keyword.RANDOM, Keyword.MAX, OperandType.NUMERIC )]
        [DataRow( Operation.Goto, Keyword.GOTO, Keyword.IF, OperandType.NUMERIC )]
        [DataRow( Operation.List, Keyword.LIST, Keyword.SIZE, OperandType.NUMERIC )]
        [DataRow( Operation.Size, Keyword.SIZE, Keyword.OF, OperandType.ANY )]
        [DataRow( Operation.Input, Keyword.INPUT, Keyword.WITH, OperandType.STRING )]
        [DataRow( Operation.Type, Keyword.TYPE, Keyword.OF, OperandType.ANY )]
        public void Parser_ParseStatement_Operand1First( Operation operation, Keyword first, Keyword mid, OperandType operandType )
        {
            foreach ( Token operand2 in this.OperandsByType[operandType] )
            {
                string line = $"{first} {this.Operand1.Value} {mid} {operand2.Value}";
                Statement expected = new( line, operation, this.Operand1, operand2 );
                List<Token> tokens = [new( first ), this.Operand1, new( mid ), operand2];
                Parser parser = new( line, tokens );

                Statement actual = parser.ParseStatement( );

                Assert.AreEqual( expected, actual );
            }
        }

        [TestMethod]
        [DataRow( Operation.Add, Keyword.ADD, OperandType.NUMERIC, Keyword.TO )]
        [DataRow( Operation.Subtract, Keyword.SUBTRACT, OperandType.NUMERIC, Keyword.FROM )]
        [DataRow( Operation.Modulus, Keyword.MOD, OperandType.NUMERIC, Keyword.OF )]
        [DataRow( Operation.Logarithm, Keyword.LOG, OperandType.NUMERIC, Keyword.OF )]
        [DataRow( Operation.Append, Keyword.APPEND, OperandType.ANY, Keyword.TO )]
        [DataRow( Operation.Write, Keyword.WRITE, OperandType.ANY, Keyword.TO )]
        [DataRow( Operation.Read, Keyword.READ, OperandType.VARIABLE, Keyword.FROM )]
        public void Parser_ParseStatement_Operand2First( Operation operation, Keyword first, OperandType operandType, Keyword mid )
        {
            foreach ( Token operand2 in this.OperandsByType[operandType] )
            {
                string line = $"{first} {operand2.Value} {mid} {this.Operand1.Value}";
                Statement expected = new( line, operation, this.Operand1, operand2 );
                List<Token> tokens = [new( first ), operand2, new( mid ), this.Operand1];
                Parser parser = new( line, tokens );

                Statement actual = parser.ParseStatement( );

                Assert.AreEqual( expected, actual );
            }
        }

        [TestMethod]
        [DataRow( Operation.Round, Keyword.ROUND, OperandType.VARIABLE )]
        [DataRow( Operation.Close, Keyword.CLOSE, OperandType.VARIABLE )]
        public void Parser_ParseStatement_Operand1Only( Operation operation, Keyword first, OperandType operandType )
        {
            foreach ( Token operand in this.OperandsByType[operandType] )
            {
                string line = $"{first} {operand.Value}";
                Statement expected = new( line, operation, operand );
                List<Token> tokens = [new( first ), operand];
                Parser parser = new( line, tokens );

                Statement actual = parser.ParseStatement( );

                Assert.AreEqual( expected, actual );
            }
        }

        [TestMethod]
        [DataRow( Operation.Print, Keyword.PRINT, OperandType.ANY )]
        [DataRow( Operation.Wait, Keyword.WAIT, OperandType.NUMERIC )]
        [DataRow( Operation.Error, Keyword.ERROR, OperandType.STRING )]
        public void Parser_ParseStatement_Operand2Only( Operation operation, Keyword first, OperandType operandType )
        {
            foreach ( Token operand2 in this.OperandsByType[operandType] )
            {
                string line = $"{first} {operand2.Value}";
                Statement expected = new( line, operation, operand2: operand2 );
                List<Token> tokens = [new( first ), operand2];
                Parser parser = new( line, tokens );

                Statement actual = parser.ParseStatement( );

                Assert.AreEqual( expected, actual );
            }
        }

        [TestMethod]
        [DataRow( Operation.OpenRead, OperandType.STRING )]
        [DataRow( Operation.OpenWrite, OperandType.STRING )]
        public void Parser_ParseStatement_OpenReadWrite( Operation operation, OperandType operandType )
        {
            Keyword first = Keyword.OPEN;
            Keyword mid1 = Keyword.AS;
            Keyword mid2 = operation == Operation.OpenRead ? Keyword.READ
                         : operation == Operation.OpenWrite ? Keyword.WRITE
                         : throw new ArgumentException( "Test given incompatible or unexpected input." );

            foreach ( Token operand2 in this.OperandsByType[operandType] )
            {
                string line = $"{first} {operand2.Value} {mid1} {mid2} {this.Operand1.Value}";
                Statement expected = new( line, operation, this.Operand1, operand2 );
                List<Token> tokens = [new( first ), operand2, new( mid1 ), new( mid2 ), this.Operand1];
                Parser parser = new( line, tokens );

                Statement actual = parser.ParseStatement( );

                Assert.AreEqual( expected, actual );
            }
        }

        [TestMethod]
        [DataRow( Operation.TestGreaterThan, Symbol.GreaterThan )]
        [DataRow( Operation.TestLessThan, Symbol.LessThan )]
        [DataRow( Operation.TestEqualTo, Symbol.Equal )]
        [DataRow( Operation.TestNotEqualTo, Symbol.Not )]
        public void Parser_ParseStatement_TestStatement( Operation operation, Symbol symbol )
        {
            Keyword first = Keyword.TEST;
            Token symbol_token = new( symbol );

            foreach ( Token operand2 in this.OperandsByType[OperandType.ANY] )
            {
                string line = $"{first} {this.Operand1.Value} {symbol_token.Value} {operand2.Value}";
                Statement expected = new( line, operation, this.Operand1, operand2 );
                List<Token> tokens = [new( first ), this.Operand1, symbol_token, operand2];
                Parser parser = new( line, tokens );

                Statement actual = parser.ParseStatement( );

                Assert.AreEqual( expected, actual );
            }
        }

        [TestMethod]
        [DataRow( Operation.Exit, Keyword.EXIT, null )]
        [DataRow( Operation.ScopeOpen, Keyword.SCOPE, Keyword.OPEN )]
        [DataRow( Operation.ScopeClose, Keyword.SCOPE, Keyword.CLOSE )]
        public void Parser_ParseStatement_KeywordOnly( Operation operation, Keyword first, Keyword? second )
        {
            string line = $"{first} {second}";
            Statement expected = new( line, operation );
            List<Token> tokens = [new( first )];
            Token second_token = second is Keyword second_keyword ? new( second_keyword ) : default;
            tokens.AddIf( second is not null, second_token );
            Parser parser = new( line, tokens );

            Statement actual = parser.ParseStatement( );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        public void Parser_ParseStatement_Label( )
        {
            Token start = new( Symbol.LabelStart );
            Token middle = new( "label", TokenType.Variable );
            Token end = new( Symbol.LabelEnd );

            string line = $"{start.Value} {middle.Value} {end.Value}";
            Statement expected = new( line, Operation.Label, middle );
            List<Token> tokens = [start, middle, end];
            Parser parser = new( line, tokens );

            Statement actual = parser.ParseStatement( );

            Assert.AreEqual( expected, actual );
        }
    }
}
