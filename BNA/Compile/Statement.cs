using System;
using System.Collections.Generic;
using System.Linq;
using BNA.Common;
using BNA.Exceptions;

namespace BNA.Compile
{
	/// <summary>
	/// A collection of tokens that make up a whole valid statement or instruction.
	/// Each statement maps to a "line of code".
	/// </summary>
	public struct Statement
	{
		/// <summary>
		/// The StatementType of this Statement
		/// </summary>
		public Operation Type { get; init; }

		/// <summary>
		/// First operand of this Statement, the storage variable for operations that store a value
		/// </summary>
		public Token Operand1 { get; init; }

		/// <summary>
		/// Second operand of this Statement
		/// </summary>
		public Token Operand2 { get; init; }

		/// <summary>
		/// The raw string of the line that generated this statement.
		/// </summary>
		public string Line { get; init; }

		public Statement( string line, Operation type , Token? operand1 = null , Token? operand2 = null )
		{
			this.Line = line;
			this.Type = type;
			this.Operand1 = operand1 ?? default;
			this.Operand2 = operand2 ?? default;
		}

		/// <summary>
		/// Stringify the Statement with type information.
		/// </summary>
		/// <returns>String description of this Statement</returns>
		public override string ToString( )
		{
			string str = "[" + this.Type + "] \t";

			str += $"op1={this.Operand1,-24} op2={this.Operand2,-24}";

			return str;
		}

		/// <summary>
		/// Get the primary and secondary tokens of a statement.
		/// This is determined by the statement sematics, not syntactic order.
		/// </summary>
		/// <returns>A tuple with the primary and secondary tokens in order.</returns>
		public (Token, Token) GetPrimaryAndSecondaryTokens( )
		{
			// TODO remove this function
			switch ( this.Type )
			{
				case Operation.NULL:
				case Operation.LABEL:
				case Operation.SCOPE_OPEN:
				case Operation.SCOPE_CLOSE:
				case Operation.EXIT:
					return (default, default);

				case Operation.ADD:
				case Operation.SUBTRACT:
				case Operation.MODULUS:
				case Operation.LOGARITHM:
				case Operation.APPEND:
				case Operation.OPEN_READ:
				case Operation.OPEN_WRITE:
				case Operation.READ:
				case Operation.WRITE:
				case Operation.SET:
				case Operation.MULTIPLY:
				case Operation.DIVIDE:
				case Operation.RANDOM:
				case Operation.POWER:
				case Operation.LIST:
				case Operation.SIZE:
				case Operation.TEST_GTR:
				case Operation.TEST_LSS:
				case Operation.TEST_EQU:
				case Operation.TEST_NEQ:
				case Operation.GOTO:
				case Operation.TYPE:
					return (this.Operand1, this.Operand2);

				case Operation.ROUND:
				case Operation.CLOSE:
				case Operation.INPUT:
					return (this.Operand1, default);

				case Operation.PRINT:
				case Operation.WAIT:
				case Operation.ERROR:
					return (this.Operand2, default);

				case Operation.BITWISE_OR:
				case Operation.BITWISE_AND:
				case Operation.BITWISE_XOR:
				case Operation.BITWISE_NEGATE:
					throw new NotImplementedException( $"Token sorting not implemented for {this.Type}." );

				default:
					throw new Exception( $"Unexpected statement type in token sorting: {this.Type}." );
			}
		}
	}
}
