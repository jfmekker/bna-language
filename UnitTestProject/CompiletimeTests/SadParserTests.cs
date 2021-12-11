using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			Token tokenLiteral = new( "1.0" , TokenType.LITERAL );
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
		[DataRow( Operation.SET , Keyword.SET , Keyword.TO )]
		[DataRow( Operation.MULTIPLY , Keyword.MULTIPLY , Keyword.BY )]
		[DataRow( Operation.DIVIDE , Keyword.DIVIDE , Keyword.BY )]
		[DataRow( Operation.POWER , Keyword.RAISE , Keyword.TO )]
		[DataRow( Operation.RANDOM , Keyword.RANDOM , Keyword.MAX )]
		[DataRow( Operation.GOTO , Keyword.GOTO , Keyword.IF )]
		[DataRow( Operation.LIST , Keyword.LIST , Keyword.SIZE )]
		[DataRow( Operation.SIZE , Keyword.SIZE , Keyword.OF )]
		[DataRow( Operation.INPUT , Keyword.INPUT , Keyword.WITH )]
		[DataRow( Operation.TYPE , Keyword.TYPE , Keyword.OF )]
		public void Parser_ParseStatement_Operand1First_Operand1IsNonVariable_ThrowsIllegalTokenException( Operation operation , Keyword first , Keyword mid )
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
		[DataRow( Operation.ADD , Keyword.ADD , Keyword.TO )]
		[DataRow( Operation.SUBTRACT , Keyword.SUBTRACT , Keyword.FROM )]
		[DataRow( Operation.MODULUS , Keyword.MOD , Keyword.OF )]
		[DataRow( Operation.LOGARITHM , Keyword.LOG , Keyword.OF )]
		[DataRow( Operation.APPEND , Keyword.APPEND , Keyword.TO )]
		[DataRow( Operation.WRITE , Keyword.WRITE , Keyword.TO )]
		[DataRow( Operation.READ , Keyword.READ , Keyword.FROM )]
		public void Parser_ParseStatement_Operand2First_Operand1IsNonVariable_ThrowsIllegalTokenException( Operation operation , Keyword first , Keyword mid )
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
		[DataRow( Keyword.TO )]
		[DataRow( Keyword.BY )]
		[DataRow( Keyword.FROM )]
		[DataRow( Keyword.MAX )]
		[DataRow( Keyword.IF )]
		[DataRow( Keyword.WITH )]
		[DataRow( Keyword.OF )]
		[DataRow( Keyword.AS )]
		public void Parser_ParseStatement_IllegalKeywordAtStatementStart_ThrowsIllegalTokenException( Keyword word )
		{
			List<Token> tokens = new( ) { new( word ) };
			Parser parser = new( tokens[0].Value , tokens );

			_ = Assert.ThrowsException<IllegalTokenException>( ( ) => _ = parser.ParseStatement( ) );
		}

		[TestMethod]
		[DataRow( Symbol.NULL )]
		[DataRow( Symbol.ESCAPE )]
		[DataRow( Symbol.GREATER_THAN )]
		[DataRow( Symbol.LESS_THAN )]
		[DataRow( Symbol.EQUAL )]
		[DataRow( Symbol.NOT )]
		[DataRow( Symbol.LABEL_END )]
		[DataRow( Symbol.STRING_MARKER )]
		[DataRow( Symbol.ACCESSOR )]
		[DataRow( Symbol.LIST_START )]
		[DataRow( Symbol.LIST_END )]
		[DataRow( Symbol.LIST_SEPERATOR )]
		public void Parser_ParseStatement_IllegalSymbolAtStatementStart_ThrowsIllegalTokenException( Symbol symbol )
		{
			List<Token> tokens = new( ) { new( symbol ) };
			Parser parser = new( tokens[0].Value , tokens );

			_ = Assert.ThrowsException<IllegalTokenException>( ( ) => _ = parser.ParseStatement( ) );
		}
	}
}
