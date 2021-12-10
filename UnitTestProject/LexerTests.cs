using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Compile;
using BNA.Common;
using BNA.Exceptions;
using System.Collections.Generic;

namespace CompiletimeTests
{
	[TestClass]
	public class LexerTests
	{
		[TestMethod]
		[DataRow( 0 )]
		[DataRow( 1 )]
		[DataRow( -1 )]
		[DataRow( long.MaxValue )]
		[DataRow( long.MinValue )]
		public void Lexer_ReadSingleToken_ReturnsLiteral_Long( long val )
		{
			Token token = Lexer.ReadSingleToken( val.ToString( ) );

			long token_val = long.Parse( token.Value );
			Assert.AreEqual( TokenType.LITERAL , token.Type );
			Assert.AreEqual( val , token_val );
		}

		[TestMethod]
		[DataRow( 0.0 )]
		[DataRow( 1.0 )]
		[DataRow( -1.0 )]
		[DataRow( double.MaxValue )]
		[DataRow( double.MinValue )]
		public void Lexer_ReadSingleToken_ReturnsLiteral_Double( double val )
		{
			Token token = Lexer.ReadSingleToken( val.ToString( ) );

			double token_val = double.Parse( token.Value );
			Assert.AreEqual( TokenType.LITERAL , token.Type );
			Assert.AreEqual( val , token_val );
		}

		[TestMethod]
		[DataRow( ".01" )]
		[DataRow( "-.01" )]
		public void Lexer_ReadSingleToken_ReturnsLiteral_StartWithoutDigit( string str )
		{
			Token token = Lexer.ReadSingleToken( str );

			double token_val = double.Parse( token.Value );
			double expected_val = double.Parse( str );
			Assert.AreEqual( TokenType.LITERAL , token.Type );
			Assert.AreEqual( expected_val , token_val );
		}

		[TestMethod]
		public void Lexer_ReadSingleToken_ReturnsKeyword( )
		{
			foreach ( Keyword keyword in Enum.GetValues( typeof( Keyword ) ) )
			{
				Token expected = new( keyword.ToString( ) , TokenType.KEYWORD );

				Token actual = Lexer.ReadSingleToken( keyword.ToString( ) );

				Assert.AreEqual( expected , actual );
			}
		}

		[TestMethod]
		[DataRow( "i" )]
		[DataRow( "var" )]
		[DataRow( "_i" )]
		[DataRow( "VAR" )]
		[DataRow( "i_0" )]
		[DataRow( "var_" )]
		[DataRow( "reeeeeeeeeeeeeeeeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaalllllllllllyyyy_long_variable_name" )]
		public void Lexer_ReadSingleToken_ReturnsVariable( string variable_string )
		{
			Token expected = new( variable_string , TokenType.VARIABLE );

			Token actual = Lexer.ReadSingleToken( variable_string );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( " " )]
		[DataRow( "  " )]
		[DataRow( "\t" )]
		[DataRow( " \t" )]
		public void Lexer_ReadSingleToken_ReturnsVariable_IgnoreWhitespace( string whitespace )
		{
			string variable_string = "variable";
			Token expected = new( variable_string , TokenType.VARIABLE );

			Token actual = Lexer.ReadSingleToken( whitespace + variable_string + whitespace );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( Symbol.EQUAL )]
		[DataRow( Symbol.NOT )]
		[DataRow( Symbol.GREATER_THAN )]
		[DataRow( Symbol.LESS_THAN )]
		[DataRow( Symbol.LABEL_START )]
		[DataRow( Symbol.LABEL_END )]
		[DataRow( Symbol.LIST_SEPERATOR )]
		[DataRow( Symbol.LIST_END )]
		public void Lexer_ReadSingleToken_ReturnsSymbol_StandAloneSymbols( Symbol symbol )
		{
			string symbol_string = $"{(char)symbol}";
			Token expected = new( symbol_string , TokenType.SYMBOL );

			Token actual = Lexer.ReadSingleToken( symbol_string );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "\"\"" )]
		[DataRow( "\" \"" )]
		[DataRow( "\"string\"" )]
		[DataRow( "\"0()-+_%$^&*@#!=><?'\"" )]
		// [DataRow( "\"\\t\\n\\\"\"" )] // TODO uncomment when escaped characters in strings are fully implemented
		public void Lexer_ReadSingleToken_ReturnsString( string list )
		{
			Token expected = new( list , TokenType.STRING );

			Token actual = Lexer.ReadSingleToken( list );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "()" )]
		[DataRow( "(0)" )]
		[DataRow( "(0,0)" )]
		[DataRow( "(\"a string\", \"(,)\")" )]
		[DataRow( "((0,0),(0,0,0),())" )]
		public void Lexer_ReadSingleToken_ReturnsList( string list )
		{
			Token expected = new( list , TokenType.LIST );

			Token actual = Lexer.ReadSingleToken( list );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "#" )]
		[DataRow( "##" )]
		[DataRow( "# words" )]
		[DataRow( "# \t\"(,)0" )]
		public void Lexer_ReadSingleToken_ReturnsComment( string list )
		{
			Token expected = new( list , TokenType.COMMENT );

			Token actual = Lexer.ReadSingleToken( list );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "0 0" )]
		[DataRow( "( 0 0 )" )]
		[DataRow( "( > )" )]
		public void Lexer_ReadSingleToken_ThrowsIllegalTokenException( string str )
		{
			_ = Assert.ThrowsException<IllegalTokenException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}

		[TestMethod]
		[DataRow( "(" )]
		[DataRow( "(0," )]
		[DataRow( "\"" )]
		[DataRow( "\"string" )]
		[DataRow( "\"string\\\"" )]
		public void Lexer_ReadSingleToken_ThrowsMissingTerminatorException( string str )
		{
			_ = Assert.ThrowsException<MissingTerminatorException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}

		[TestMethod]
		[DataRow( "*" )]
		[DataRow( "/" )]
		[DataRow( "\\" )]
		[DataRow( "@" )]
		[DataRow( "$" )]
		[DataRow( "%" )]
		[DataRow( "&" )]
		public void Lexer_ReadSingleToken_ThrowsUnexpectedSymbolException( string str )
		{
			_ = Assert.ThrowsException<UnexpectedSymbolException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}

		[TestMethod]
		[DataRow( "x@" )]
		[DataRow( "x@@1" )]
		[DataRow( "0.." )]
		[DataRow( "0-" )]
		[DataRow( "0+" )]
		public void Lexer_ReadSingleToken_ThrowsInvalidTokenException( string str )
		{
			_ = Assert.ThrowsException<InvalidTokenException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}

		[TestMethod]
		[DataRow( "0" , 1 )]
		[DataRow( "0 0" , 2 )]
		[DataRow( "0 one \"2\" < (4,five,\"6\") #seven" , 6 )]
		public void Lexer_ReadTokens_ReturnsMultipleTokens( string str , int num )
		{
			Lexer lexer = new( str );

			List<Token> list = lexer.ReadTokens( );

			Assert.AreEqual( num , list.Count );
		}

		[TestMethod]
		[DataRow( "" )]
		[DataRow( " " )]
		[DataRow( "  " )]
		[DataRow( "\t" )]
		public void Lexer_ReadTokens_ReturnsZeroTokens( string str )
		{
			Lexer lexer = new( str );

			List<Token> list = lexer.ReadTokens( );

			Assert.AreEqual( 0 , list.Count );
		}
	}
}
