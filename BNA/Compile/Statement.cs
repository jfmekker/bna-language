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
	public record struct Statement
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

		public Statement( string line , Operation type , Token? operand1 = null , Token? operand2 = null )
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
	}
}
