using BNA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnaUnitTests
{
	[TestClass]
	public class TokenizerTests
	{
		[TestMethod]
		public void Parse_Keywords_Happy_Test( )
		{
			foreach ( Keyword keyword in Enum.GetValues( typeof( Keyword ) ) )
			{
				Token token = new( keyword.ToString( ) );
				Assert.AreEqual( TokenType.KEYWORD , token.Type , $"Failed value: {token.Value}" );
			}
		}

		[TestMethod]
		public void Parse_Keywords_Sad_Test( )
		{
			foreach ( Keyword keyword in Enum.GetValues( typeof( Keyword ) ) )
			{
				Token token = new( "_" + keyword.ToString( ) );
				Assert.AreNotEqual( TokenType.KEYWORD , token.Type , $"Failed value: {token.Value}" );

				token = new( keyword.ToString( ) + "_" );
				Assert.AreNotEqual( TokenType.KEYWORD , token.Type , $"Failed value: {token.Value}" );

				token = new( keyword.ToString( ) + keyword.ToString( ) );
				Assert.AreNotEqual( TokenType.KEYWORD , token.Type , $"Failed value: {token.Value}" );
			}
		}

		[TestMethod]
		public void Parse_Symbols_Happy_Test( )
		{
			foreach ( Symbol symbol in Enum.GetValues( typeof( Symbol ) ) )
			{
				Token token = new( "" + (char)symbol );
				Assert.AreEqual( TokenType.SYMBOL , token.Type , $"Failed value: {token.Value}" );
			}
		}

		[TestMethod]
		public void Parse_Symbols_Sad_Test( )
		{
			foreach ( Symbol symbol in Enum.GetValues( typeof( Symbol ) ) )
			{
				Token token = new( "_" + symbol.ToString( ) );
				Assert.AreNotEqual( TokenType.SYMBOL , token.Type , $"Failed value: {token.Value}" );

				token = new( symbol.ToString( ) + "_" );
				Assert.AreNotEqual( TokenType.KEYWORD , token.Type , $"Failed value: {token.Value}" );

				token = new( symbol.ToString( ) + symbol.ToString( ) );
				Assert.AreNotEqual( TokenType.KEYWORD , token.Type , $"Failed value: {token.Value}" );
			}
		}

		[TestMethod]
		public void Parse_NumericLiterals_Happy_Test( )
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
				Token token = new( str );
				Assert.AreEqual( TokenType.LITERAL , token.Type , $"Failed value: {str}" );
			}
		}

		[TestMethod]
		public void Parse_NumericLiterals_Sad_Test( )
		{
			foreach ( string str in new string[]
				{
					"zero", "_1", "1_2345", ",234,567", "0.0.0", "..333", "1 2", "1 2 3",
				}
			)
			{
				Token token = new( str );
				Assert.AreNotEqual( TokenType.LITERAL , token.Type , $"Failed value: {str}" );
			}
		}
	}
}
