using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Compile;
using BNA.Common;

namespace CompiletimeTests
{
	[TestClass]
	public class HappyLexerTests
	{
		[TestMethod]
		[DataRow( 0 , DisplayName = "Zero" )]
		[DataRow( 1 , DisplayName = "One" )]
		[DataRow( -1 , DisplayName = "Negative one" )]
		[DataRow( long.MaxValue , DisplayName = "Max value" )]
		[DataRow( long.MinValue , DisplayName = "Min value" )]
		public void Lexer_ReadSingleToken_ReturnsLiteral_Long( long val )
		{
			Token token = Lexer.ReadSingleToken( val.ToString( ) );

			long token_val = long.Parse( token.Value );
			Assert.AreEqual( TokenType.NUMBER , token.Type );
			Assert.AreEqual( val , token_val );
		}

		[TestMethod]
		[DataRow( 0.0 , DisplayName = "Zero" )]
		[DataRow( 1.0 , DisplayName = "One" )]
		[DataRow( -1.0 , DisplayName = "Negative one" )]
		[DataRow( double.MaxValue , DisplayName = "Max value" )]
		[DataRow( double.MinValue , DisplayName = "Min value" )]
		public void Lexer_ReadSingleToken_ReturnsLiteral_Double( double val )
		{
			Token token = Lexer.ReadSingleToken( val.ToString( ) );

			double token_val = double.Parse( token.Value );
			Assert.AreEqual( TokenType.NUMBER , token.Type );
			Assert.AreEqual( val , token_val );
		}

		[TestMethod]
		[DataRow( ".01" , DisplayName = "Start with decimal point" )]
		[DataRow( "-.01" , DisplayName = "Start with negative sign then decimal point" )]
		public void Lexer_ReadSingleToken_ReturnsLiteral_StartWithoutDigit( string str )
		{
			Token token = Lexer.ReadSingleToken( str );

			double token_val = double.Parse( token.Value );
			double expected_val = double.Parse( str );
			Assert.AreEqual( TokenType.NUMBER , token.Type );
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
		[DataRow( "i" , DisplayName = "Single letter" )]
		[DataRow( "var" , DisplayName = "Multiple letters" )]
		[DataRow( "VAR" , DisplayName = "Capital letters" )]
		[DataRow( "i0" , DisplayName = "With number" )]
		[DataRow( "_i" , DisplayName = "Underscore before" )]
		[DataRow( "i_var" , DisplayName = "Underscore middle" )]
		[DataRow( "var_" , DisplayName = "Underscore after" )]
		[DataRow( "reeeeeeeeeeeeeeeeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaalllllllllllyyyy_long_variable_name" , DisplayName = "Long name" )]
		public void Lexer_ReadSingleToken_ReturnsVariable( string variable_string )
		{
			Token expected = new( variable_string , TokenType.VARIABLE );

			Token actual = Lexer.ReadSingleToken( variable_string );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( " " , DisplayName = "One space" )]
		[DataRow( "  " , DisplayName = "Two spaces" )]
		[DataRow( "\t" , DisplayName = "Tab character" )]
		[DataRow( " \t" , DisplayName = "Space and tab" )]
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
		[DataRow( Symbol.LIST_SEPARATOR )]
		[DataRow( Symbol.LIST_END )]
		public void Lexer_ReadSingleToken_ReturnsSymbol_StandAloneSymbols( Symbol symbol )
		{
			string symbol_string = $"{(char)symbol}";
			Token expected = new( symbol_string , TokenType.SYMBOL );

			Token actual = Lexer.ReadSingleToken( symbol_string );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "\"\"" , DisplayName = "Empty" )]
		[DataRow( "\" \"" , DisplayName = "With space" )]
		[DataRow( "\"string\"" , DisplayName = "Letters" )]
		[DataRow( "\"0()-+_%$^&*@#!=><?'\"" , DisplayName = "Symbols" )]
		// [DataRow( "\"\\t\\n\\\"\"" , DisplayName = "Escaped characters" )] // TODO uncomment when escaped characters in strings are fully implemented
		public void Lexer_ReadSingleToken_ReturnsString( string list )
		{
			Token expected = new( list , TokenType.STRING );

			Token actual = Lexer.ReadSingleToken( list );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "()" , DisplayName = "Empty" )]
		[DataRow( "(0)" , DisplayName = "Single element" )]
		[DataRow( "(0,0)" , DisplayName = "Two literals" )]
		[DataRow( "(\"a string\", \"(,)\")" , DisplayName = "Two strings" )]
		[DataRow( "((0,0),(0,0,0),())" , DisplayName = "Sublists" )]
		public void Lexer_ReadSingleToken_ReturnsList( string list )
		{
			Token expected = new( list , TokenType.LIST );

			Token actual = Lexer.ReadSingleToken( list );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "#" , DisplayName = "Empty" )]
		[DataRow( "##" , DisplayName = "Multiple comment symbols" )]
		[DataRow( "# words words" , DisplayName = "Words" )]
		[DataRow( "# \t\"(,)0" , DisplayName = "Symbols" )]
		public void Lexer_ReadSingleToken_ReturnsComment( string list )
		{
			Token expected = new( list , TokenType.COMMENT );

			Token actual = Lexer.ReadSingleToken( list );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( "0" , 1 , DisplayName = "One literal" )]
		[DataRow( "0 0" , 2 , DisplayName = "Two literals" )]
		[DataRow( "0 one \"2\" < (4,five,\"6\") #seven" , 6 , DisplayName = "All token types" )]
		public void Lexer_ReadTokens_ReturnsMultipleTokens( string str , int num )
		{
			Lexer lexer = new( str );

			List<Token> list = lexer.ReadTokens( );

			Assert.AreEqual( num , list.Count );
		}

		[TestMethod]
		[DataRow( "" , DisplayName = "Empty string" )]
		[DataRow( " " , DisplayName = "One space" )]
		[DataRow( "  " , DisplayName = "Two spaces" )]
		[DataRow( "\t" , DisplayName = "Tab character" )]
		public void Lexer_ReadTokens_ReturnsZeroTokens( string str )
		{
			Lexer lexer = new( str );

			List<Token> list = lexer.ReadTokens( );

			Assert.AreEqual( 0 , list.Count );
		}
	}
}
