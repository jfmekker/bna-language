using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Compile;
using BNA.Common;

namespace CompiletimeTests
{
	[TestClass]
	public class HappyParserTests
	{
		public enum OperandType { ANY, NUMERIC, STRING, VARIABLE }

		public readonly IReadOnlyDictionary<OperandType , IReadOnlyList<Token>> OperandsByType;

		public readonly Token Operand1;

		public HappyParserTests( )
		{
			Token tokenVariable = new( "var" , TokenType.VARIABLE );
			Token tokenLiteral = new( "1.0" , TokenType.LITERAL );
			Token tokenString = new( "\"my string\"" , TokenType.STRING );
			Token tokenList = new( "(0, 0)" , TokenType.LIST );

			var anyTypeOperands = new List<Token>( ) { tokenVariable , tokenLiteral , tokenString , tokenList };
			var numericTypeOperands = new List<Token>( ) { tokenVariable , tokenLiteral };
			var stringTypeOperands = new List<Token>( ) { tokenVariable , tokenString };
			var variableTypeOperands = new List<Token>( ) { tokenVariable };

			this.OperandsByType = new Dictionary<OperandType , IReadOnlyList<Token>>
			{
				{ OperandType.ANY , anyTypeOperands },
				{ OperandType.NUMERIC , numericTypeOperands },
				{ OperandType.STRING , stringTypeOperands },
				{ OperandType.VARIABLE , variableTypeOperands },
			};

			this.Operand1 = tokenVariable;
		}

		[TestMethod]
		[DataRow( false , DisplayName = "Empty line" )]
		[DataRow( true , DisplayName = "Comment" )]
		public void Parser_ParseStatement_NullStatements( bool comment )
		{
			Token token = new( "# a comment" , TokenType.COMMENT );
			string line = comment ? token.Value : string.Empty;
			Statement expected = new( line , Operation.NULL );
			List<Token> tokens = new( );
			tokens.AddIf( comment , token );
			Parser parser = new( line , tokens );

			Statement actual = parser.ParseStatement( );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		[DataRow( Operation.SET , Keyword.SET , Keyword.TO , OperandType.ANY )]
		[DataRow( Operation.MULTIPLY , Keyword.MULTIPLY , Keyword.BY , OperandType.NUMERIC )]
		[DataRow( Operation.DIVIDE , Keyword.DIVIDE , Keyword.BY , OperandType.NUMERIC )]
		[DataRow( Operation.POWER , Keyword.RAISE , Keyword.TO , OperandType.NUMERIC )]
		[DataRow( Operation.RANDOM , Keyword.RANDOM , Keyword.MAX , OperandType.NUMERIC )]
		[DataRow( Operation.GOTO , Keyword.GOTO , Keyword.IF , OperandType.NUMERIC )]
		[DataRow( Operation.LIST , Keyword.LIST , Keyword.SIZE , OperandType.NUMERIC )]
		[DataRow( Operation.SIZE , Keyword.SIZE , Keyword.OF , OperandType.ANY )]
		[DataRow( Operation.INPUT , Keyword.INPUT , Keyword.WITH , OperandType.STRING )]
		[DataRow( Operation.TYPE , Keyword.TYPE , Keyword.OF , OperandType.ANY )]
		public void Parser_ParseStatement_Operand1First( Operation operation , Keyword first , Keyword mid , OperandType operandType )
		{
			foreach ( Token operand2 in this.OperandsByType[operandType] )
			{
				string line = $"{first} {this.Operand1.Value} {mid} {operand2.Value}";
				Statement expected = new( line , operation , this.Operand1 , operand2 );
				List<Token> tokens = new( ) { new( first ) , this.Operand1 , new( mid ) , operand2 };
				Parser parser = new( line , tokens );

				Statement actual = parser.ParseStatement( );

				Assert.AreEqual( expected , actual );
			}
		}

		[TestMethod]
		[DataRow( Operation.ADD , Keyword.ADD , OperandType.NUMERIC , Keyword.TO )]
		[DataRow( Operation.SUBTRACT , Keyword.SUBTRACT , OperandType.NUMERIC , Keyword.FROM )]
		[DataRow( Operation.MODULUS , Keyword.MOD , OperandType.NUMERIC , Keyword.OF )]
		[DataRow( Operation.LOGARITHM , Keyword.LOG , OperandType.NUMERIC , Keyword.OF )]
		[DataRow( Operation.APPEND , Keyword.APPEND , OperandType.ANY , Keyword.TO )]
		[DataRow( Operation.WRITE , Keyword.WRITE , OperandType.ANY , Keyword.TO )]
		[DataRow( Operation.READ , Keyword.READ , OperandType.VARIABLE , Keyword.FROM )]
		public void Parser_ParseStatement_Operand2First( Operation operation , Keyword first , OperandType operandType , Keyword mid )
		{
			foreach ( Token operand2 in this.OperandsByType[operandType] )
			{
				string line = $"{first} {operand2.Value} {mid} {this.Operand1.Value}";
				Statement expected = new( line , operation , this.Operand1 , operand2 );
				List<Token> tokens = new( ) { new( first ) , operand2 , new( mid ) , this.Operand1 };
				Parser parser = new( line , tokens );

				Statement actual = parser.ParseStatement( );

				Assert.AreEqual( expected , actual );
			}
		}

		[TestMethod]
		[DataRow( Operation.ROUND , Keyword.ROUND , OperandType.VARIABLE )]
		[DataRow( Operation.CLOSE , Keyword.CLOSE , OperandType.VARIABLE )]
		public void Parser_ParseStatement_Operand1Only( Operation operation , Keyword first , OperandType operandType )
		{
			foreach ( Token operand in this.OperandsByType[operandType] )
			{
				string line = $"{first} {operand.Value}";
				Statement expected = new( line , operation , operand );
				List<Token> tokens = new( ) { new( first ) , operand };
				Parser parser = new( line , tokens );

				Statement actual = parser.ParseStatement( );

				Assert.AreEqual( expected , actual );
			}
		}

		[TestMethod]
		[DataRow( Operation.PRINT , Keyword.PRINT , OperandType.ANY )]
		[DataRow( Operation.WAIT , Keyword.WAIT , OperandType.NUMERIC )]
		[DataRow( Operation.ERROR , Keyword.ERROR , OperandType.STRING )]
		public void Parser_ParseStatement_Operand2Only( Operation operation , Keyword first , OperandType operandType )
		{
			foreach ( Token operand2 in this.OperandsByType[operandType] )
			{
				string line = $"{first} {operand2.Value}";
				Statement expected = new( line , operation , operand2: operand2 );
				List<Token> tokens = new( ) { new( first ) , operand2 };
				Parser parser = new( line , tokens );

				Statement actual = parser.ParseStatement( );

				Assert.AreEqual( expected , actual );
			}
		}

		[TestMethod]
		[DataRow( Operation.OPEN_READ , OperandType.STRING )]
		[DataRow( Operation.OPEN_WRITE , OperandType.STRING )]
		public void Parser_ParseStatement_OpenReadWrite( Operation operation , OperandType operandType )
		{
			Keyword first = Keyword.OPEN;
			Keyword mid1 = Keyword.AS;
			Keyword mid2 = operation == Operation.OPEN_READ ? Keyword.READ
						 : operation == Operation.OPEN_WRITE ? Keyword.WRITE
						 : throw new System.Exception( "Test given incompatible or unexpected input." );

			foreach ( Token operand2 in this.OperandsByType[operandType] )
			{
				string line = $"{first} {operand2.Value} {mid1} {mid2} {this.Operand1.Value}";
				Statement expected = new( line , operation , this.Operand1 , operand2 );
				List<Token> tokens = new( ) { new( first ) , operand2 , new( mid1 ) , new( mid2 ) , this.Operand1 };
				Parser parser = new( line , tokens );

				Statement actual = parser.ParseStatement( );

				Assert.AreEqual( expected , actual );
			}
		}

		[TestMethod]
		[DataRow( Operation.TEST_GREATER_THAN , Symbol.GREATER_THAN )]
		[DataRow( Operation.TEST_LESS_THAN , Symbol.LESS_THAN )]
		[DataRow( Operation.TEST_EQUAL , Symbol.EQUAL )]
		[DataRow( Operation.TEST_NOT_EQUAL , Symbol.NOT )]
		public void Parser_ParseStatement_TestStatement( Operation operation , Symbol symbol )
		{
			Keyword first = Keyword.TEST;
			Token symbol_token = new( symbol );

			foreach ( Token operand2 in this.OperandsByType[OperandType.ANY] )
			{
				string line = $"{first} {this.Operand1.Value} {symbol_token.Value} {operand2.Value}";
				Statement expected = new( line , operation , this.Operand1 , operand2 );
				List<Token> tokens = new( ) { new( first ) , this.Operand1 , symbol_token , operand2 };
				Parser parser = new( line , tokens );

				Statement actual = parser.ParseStatement( );

				Assert.AreEqual( expected , actual );
			}
		}

		[TestMethod]
		[DataRow( Operation.EXIT , Keyword.EXIT , null )]
		[DataRow( Operation.SCOPE_OPEN , Keyword.SCOPE , Keyword.OPEN )]
		[DataRow( Operation.SCOPE_CLOSE , Keyword.SCOPE , Keyword.CLOSE )]
		public void Parser_ParseStatement_KeywordOnly( Operation operation , Keyword first , Keyword? second )
		{
			string line = $"{first} {second}";
			Statement expected = new( line , operation );
			List<Token> tokens = new( ) { new( first ) };
			Token second_token = second is Keyword second_keyword ? new( second_keyword ) : default;
			tokens.AddIf( second is not null , second_token );
			Parser parser = new( line , tokens );

			Statement actual = parser.ParseStatement( );

			Assert.AreEqual( expected , actual );
		}

		[TestMethod]
		public void Parser_ParseStatement_Label( )
		{
			Token start = new( Symbol.LABEL_START );
			Token middle = new( "label" , TokenType.VARIABLE );
			Token end = new( Symbol.LABEL_END );

			string line = $"{start.Value} {middle.Value} {end.Value}";
			Statement expected = new( line , Operation.LABEL , middle );
			List<Token> tokens = new( ) { start , middle , end };
			Parser parser = new( line , tokens );

			Statement actual = parser.ParseStatement( );

			Assert.AreEqual( expected , actual );
		}
	}
}
