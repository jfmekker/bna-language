using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Compile;
using BNA.Common;

namespace BnaUnitTests
{
	[TestClass]
	public class ParserTests
	{
		[TestMethod]
		public void Parser_001_Set_Statement_Test( )
		{
			List<Token> tokens = new( )
			{
				new Token( "SET" , TokenType.KEYWORD ) ,
				new Token( "x" , TokenType.VARIABLE ) ,
				new Token( "TO" , TokenType.KEYWORD ) ,
				new Token( "5.0" , TokenType.LITERAL ) ,
				new Token( "# cool beans" , TokenType.COMMENT ) ,
			};
			Parser parser = new( tokens.PrintElements( "" , " " , "" ) , tokens );
			Statement statement = parser.ParseStatement( );
			Assert.AreEqual( Operation.SET , statement.Type );
			Assert.AreEqual( tokens[1] , statement.Operand1 );
			Assert.AreEqual( tokens[3] , statement.Operand2 );
		}

		[TestMethod]
		public void Parser_002_Subtract_Statement_Test( )
		{
			List<Token> tokens = new( )
			{
				new Token( "SUBTRACT" , TokenType.KEYWORD ) ,
				new Token( ".6" , TokenType.LITERAL ) ,
				new Token( "FROM" , TokenType.KEYWORD ) ,
				new Token( "_y" , TokenType.VARIABLE ) ,
			};
			Parser parser = new( tokens.PrintElements( "" , " " , "" ) , tokens );
			Statement statement = parser.ParseStatement( );
			Assert.AreEqual( Operation.SUBTRACT , statement.Type );
			Assert.AreEqual( tokens[3] , statement.Operand1 );
			Assert.AreEqual( tokens[1] , statement.Operand2 );
		}

		[TestMethod]
		public void Parser_003_Wait_Statement_Test( )
		{
			List<Token> tokens = new( )
			{
				new Token( "WAIT" , TokenType.KEYWORD ) ,
				new Token( "7.5" , TokenType.VARIABLE ) ,
				new Token( "# cool beans" , TokenType.COMMENT ) ,
			};
			Parser parser = new( tokens.PrintElements( "" , " " , "" ) , tokens );
			Statement statement = parser.ParseStatement( );
			Assert.AreEqual( Operation.WAIT , statement.Type );
			Assert.AreEqual( tokens[1] , statement.Operand2 );
		}

		[TestMethod]
		public void Parser_004_Comment_Statement_Test( )
		{
			List<Token> tokens = new( )
			{
				new Token( "# cool beans" , TokenType.COMMENT ) ,
			};
			Parser parser = new( tokens.PrintElements( "" , " " , "" ) , tokens );
			Statement statement = parser.ParseStatement( );
			Assert.AreEqual( Operation.NULL , statement.Type );
		}

		[TestMethod]
		public void Parser_005_Null_Statement_Test( )
		{
			List<Token> tokens = new( )
			{
			};
			Parser parser = new( tokens.PrintElements( "" , " " , "" ) , tokens );
			Statement statement = parser.ParseStatement( );
			Assert.AreEqual( Operation.NULL , statement.Type );
		}
	}
}
