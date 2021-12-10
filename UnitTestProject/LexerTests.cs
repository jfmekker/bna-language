using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Compile;
using BNA.Common;
using BNA.Exceptions;

namespace BnaUnitTests
{
	[TestClass]
	public class LexerTests
	{
		[TestMethod]
		public void Lexer_001_Keyword_Token_Test( )
		{
			foreach ( Keyword keyword in Enum.GetValues( typeof( Keyword ) ) )
			{
				Token expected_token = new( keyword.ToString( ) , TokenType.KEYWORD );
				Lexer lexer = new( keyword.ToString( ) );
				Assert.AreEqual( expected_token , lexer.NextToken( ) );
			}
		}

		[TestMethod]
		public void Lexer_002_Symbol_Token_Test( )
		{
			foreach ( Symbol symbol in new Symbol[]
				{
					Symbol.GREATER_THAN,
					Symbol.LESS_THAN,
					Symbol.EQUAL,
					Symbol.NOT,
					Symbol.LABEL_START,
					Symbol.LABEL_END,
					Symbol.LIST_SEPERATOR,
					Symbol.LIST_END
				} )
			{
				Lexer lexer = new( $"{(char)symbol}" );
				Assert.AreEqual( symbol , lexer.NextToken( )?.AsSymbol( ) , $"Failed value: {symbol}" );
			}
		}

		[TestMethod]
		public void Lexer_003_Literal_Token_Test( )
		{
			foreach ( string str in new string[]
				{
					"0", "1", "12345", "0.0", ".333", $"{Math.PI}",
					"-0", "-1", "-12345", "-0.0", "-.333", $"-{Math.PI}",
					$"{long.MaxValue - 1}", $"{long.MaxValue}",
					$"{double.MaxValue - 1}", $"{double.MaxValue}",
					$"{long.MinValue + 1}", $"{long.MinValue}",
					$"{double.MinValue + 1}", $"{double.MinValue}",
				}
			)
			{
				Token expected_token = new( str , TokenType.LITERAL );
				Lexer lexer = new( str );
				Assert.AreEqual( expected_token , lexer.NextToken( ) , $"Failed value: {str}" );
			}
		}

		[TestMethod]
		public void Lexer_004_Variable_Token_Test( )
		{
			foreach ( string str in new string[]
				{
					"variable", "_variable", "variable_", "___var_iable",
					"_set", "SET_", "_1024", "some_var_1", "i",
					"reeeeeeeeeeeeeeeeeeeeeeaaaaaaaaaaaaaaaaaaaaaaaalllllllllllyyyy_long_variable_name"
				}
			)
			{
				Token expected_token = new( str , TokenType.VARIABLE );
				Lexer lexer = new( str );
				Assert.AreEqual( expected_token , lexer.NextToken( ) , $"Failed value: {str}" );
			}
		}

		[TestMethod]
		public void Lexer_005_List_Token_Test( )
		{
			Lexer lexer = new( "( x, y, z ,w , ( 1, 2, 3.0, \"string\" ))" );
			Assert.AreEqual( TokenType.LIST , lexer.NextToken( )?.Type );
		}

		[TestMethod]
		public void Lexer_006_Comment_Token_Test( )
		{
			Lexer lexer = new( "( x, y, z ,w , ( 1, 2, 3.0, \"string\" )) # a list comment )()\\,,stuff 123" );
			Assert.AreEqual( TokenType.COMMENT , lexer.ReadTokens( )[^1].Type );
		}

		[TestMethod]
		public void Lexer_007_ReadSingleToken_Test( )
		{
			Assert.AreEqual( TokenType.LIST , Lexer.ReadSingleToken( "(1,2,3, \"4\")" ).Type );

			_ = Assert.ThrowsException<IllegalTokenException>( ( ) => Lexer.ReadSingleToken( "token1 token2" ) );
		}
	}
}
