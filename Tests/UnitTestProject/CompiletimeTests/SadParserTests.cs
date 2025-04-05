using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Compile;
using BNA.Common;
using BNA.Exceptions;

namespace CompiletimeTests
{
	[TestClass]
	public class SadParserTests
	{
		public enum OperandType { NON_NUMERIC, NON_STRING, NON_VARIABLE }

		public readonly IReadOnlyDictionary<OperandType , IReadOnlyList<Token>> OperandsByType;

		public readonly Token VariableOperand;

		public SadParserTests( )
		{
			Token tokenVariable = new( "var" , TokenType.VARIABLE );
			Token tokenLiteral = new( "1.0" , TokenType.NUMBER );
			Token tokenString = new( "\"my string\"" , TokenType.STRING );
			Token tokenList = new( "(0, 0)" , TokenType.LIST );

			var nonNumericTypeOperands = new List<Token>( ) { tokenString , tokenList };
			var nonStringTypeOperands = new List<Token>( ) { tokenLiteral , tokenList };
			var nonVariableTypeOperands = new List<Token>( ) { tokenLiteral , tokenString , tokenList };

			this.OperandsByType = new Dictionary<OperandType , IReadOnlyList<Token>>
			{
				{ OperandType.NON_NUMERIC , nonNumericTypeOperands },
				{ OperandType.NON_STRING , nonStringTypeOperands },
				{ OperandType.NON_VARIABLE , nonVariableTypeOperands },
			};

			this.VariableOperand = tokenVariable;
		}

		[TestMethod]
		[DataRow( Keyword.SET , Keyword.TO , DisplayName = "Set statement" )]
		[DataRow( Keyword.MULTIPLY , Keyword.BY , DisplayName = "Multiply statement" )]
		[DataRow( Keyword.DIVIDE , Keyword.BY , DisplayName = "Divide statement" )]
		[DataRow( Keyword.RAISE , Keyword.TO , DisplayName = "Raise statement" )]
		[DataRow( Keyword.RANDOM , Keyword.MAX , DisplayName = "Random statement" )]
		[DataRow( Keyword.GOTO , Keyword.IF , DisplayName = "Goto statement" )]
		[DataRow( Keyword.LIST , Keyword.SIZE , DisplayName = "List statement" )]
		[DataRow( Keyword.SIZE , Keyword.OF , DisplayName = "Size statement" )]
		[DataRow( Keyword.INPUT , Keyword.WITH , DisplayName = "Input statement" )]
		[DataRow( Keyword.TYPE , Keyword.OF , DisplayName = "Type statement" )]
		public void Parser_ParseStatement_Operand1First_Operand1IsNonVariable_ThrowsIllegalTokenException( Keyword first , Keyword mid )
		{
			foreach ( Token operand1 in this.OperandsByType[OperandType.NON_VARIABLE] )
			{
				string line = $"{first} {operand1.Value} {mid} {this.VariableOperand.Value}";
				List<Token> tokens = new( ) { new( first ) , operand1 , new( mid ) , this.VariableOperand };
				Parser parser = new( line , tokens );

				_ = Assert.ThrowsException<IllegalTokenException>( ( ) => _ = parser.ParseStatement( ) );
			}
		}

		[TestMethod]
		[DataRow( Keyword.ADD , Keyword.TO , DisplayName = "Add statement" )]
		[DataRow( Keyword.SUBTRACT , Keyword.FROM , DisplayName = "Subtract statement" )]
		[DataRow( Keyword.MOD , Keyword.OF , DisplayName = "Modulus statement" )]
		[DataRow( Keyword.LOG , Keyword.OF , DisplayName = "Logartithm statement" )]
		[DataRow( Keyword.APPEND , Keyword.TO , DisplayName = "Append statement" )]
		[DataRow( Keyword.WRITE , Keyword.TO , DisplayName = "Write statement" )]
		[DataRow( Keyword.READ , Keyword.FROM , DisplayName = "Read statement" )]
		public void Parser_ParseStatement_Operand2First_Operand1IsNonVariable_ThrowsIllegalTokenException( Keyword first , Keyword mid )
		{
			foreach ( Token operand1 in this.OperandsByType[OperandType.NON_VARIABLE] )
			{
				string line = $"{first} {this.VariableOperand.Value} {mid} {operand1.Value}";
				List<Token> tokens = new( ) { new( first ) , this.VariableOperand , new( mid ) , operand1 };
				Parser parser = new( line , tokens );

				_ = Assert.ThrowsException<IllegalTokenException>( ( ) => _ = parser.ParseStatement( ) );
			}
		}

		[TestMethod]
		[DataRow( Keyword.TO , DisplayName = "TO" )]
		[DataRow( Keyword.BY , DisplayName = "BY" )]
		[DataRow( Keyword.FROM , DisplayName = "FROM" )]
		[DataRow( Keyword.MAX , DisplayName = "MAX" )]
		[DataRow( Keyword.IF , DisplayName = "IF" )]
		[DataRow( Keyword.WITH , DisplayName = "WITH" )]
		[DataRow( Keyword.OF , DisplayName = "OF" )]
		[DataRow( Keyword.AS , DisplayName = "AS" )]
		public void Parser_ParseStatement_IllegalKeywordAtStatementStart_ThrowsIllegalTokenException( Keyword word )
		{
			List<Token> tokens = new( ) { new( word ) };
			Parser parser = new( tokens[0].Value , tokens );

			_ = Assert.ThrowsException<IllegalTokenException>( ( ) => _ = parser.ParseStatement( ) );
		}

		[TestMethod]
		[DataRow( Symbol.NULL , DisplayName = "NULL" )]
		[DataRow( Symbol.ESCAPE , DisplayName = "ESCAPE" )]
		[DataRow( Symbol.GREATER_THAN , DisplayName = "GREATER_THAN" )]
		[DataRow( Symbol.LESS_THAN , DisplayName = "LESS_THAN" )]
		[DataRow( Symbol.EQUAL , DisplayName = "EQUAL" )]
		[DataRow( Symbol.NOT , DisplayName = "NOT" )]
		[DataRow( Symbol.LABEL_END , DisplayName = "LABEL_END" )]
		[DataRow( Symbol.STRING_MARKER , DisplayName = "STRING_MARKER" )]
		[DataRow( Symbol.ACCESSOR , DisplayName = "ACCESSOR" )]
		[DataRow( Symbol.LIST_START , DisplayName = "LIST_START" )]
		[DataRow( Symbol.LIST_END , DisplayName = "LIST_END" )]
		[DataRow( Symbol.LIST_SEPARATOR , DisplayName = "LIST_SEPATOR" )]
		public void Parser_ParseStatement_IllegalSymbolAtStatementStart_ThrowsIllegalTokenException( Symbol symbol )
		{
			List<Token> tokens = new( ) { new( symbol ) };
			Parser parser = new( tokens[0].Value , tokens );

			_ = Assert.ThrowsException<IllegalTokenException>( ( ) => _ = parser.ParseStatement( ) );
		}
	}
}
