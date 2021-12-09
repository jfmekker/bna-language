﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA;
using BNA.Compile;
using BNA.Common;

namespace BnaUnitTests
{
	[TestClass]
	public class TokenizerTests
	{
		[TestMethod]
		public void Parse_Keyword_Token_Test( )
		{
			foreach ( Keyword keyword in Enum.GetValues( typeof( Keyword ) ) )
			{
				Token expected_token = new( keyword.ToString( ) , TokenType.KEYWORD );

				Lexer parser = new( keyword.ToString( ) );

				Assert.AreEqual( expected_token , parser.NextToken( ) );
			}
		}

		[TestMethod]
		public void Parse_Symbol_Token_Test( )
		{
			foreach ( string symbol in new string[] { ">", "<", "=", "!" } )
			{
				Lexer parser = new( symbol );
				Assert.AreEqual( TokenType.SYMBOL , parser.NextToken()?.Type , $"Failed value: {parser.Line}" );
			}
		}

		[TestMethod]
		public void Parse_Literal_Token_Test( )
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

				Lexer parser = new( str );
				
				Assert.AreEqual( expected_token , parser.NextToken( ) , $"Failed value: {str}" );
			}
		}

		[TestMethod]
		public void Parse_Variable_Token_Test( )
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

				Lexer parser = new( str );

				Assert.AreEqual( expected_token , parser.NextToken( ) , $"Failed value: {str}" );
			}
		}

		[TestMethod]
		public void Parse_List_Token_Test( )
		{
			Lexer parser = new( "( x, y, z ,w , ( 1, 2, 3.0, \"string\" ))" );
			Assert.AreEqual( TokenType.LIST , parser.NextToken( )?.Type );
		}

		[TestMethod]
		public void Parse_Comment_Token_Test( )
		{
			Lexer parser = new( "( x, y, z ,w , ( 1, 2, 3.0, \"string\" )) # a list comment )()\\,,stuff 123" );
			Assert.AreEqual( TokenType.COMMENT , parser.ReadTokens()[^1].Type );
		}
	}
}
