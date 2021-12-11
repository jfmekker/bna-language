using System;
using BNA.Common;
using BNA.Compile;
using BNA.Values;

namespace BNA.Exceptions
{
	/// <summary>
	/// A BNA exception encountered at runtime.
	/// </summary>
	public class RuntimeException : Exception
	{
		/// <summary>
		/// Line number of the line that caused the <see cref="Exception"/>.
		/// </summary>
		public readonly int Line;

		/// <summary>
		/// The <see cref="Statement"/> that was compiled for the line.
		/// </summary>
		public readonly Statement Statement;

		/// <summary>
		/// Create a new <see cref="RuntimeException"/> instance by wrapping
		/// the thrown <see cref="Exception"/>.
		/// </summary>
		/// <param name="line">Line number.</param>
		/// <param name="statement">Compiled statement.</param>
		/// <param name="innerException">Exception that was thrown.</param>
		public RuntimeException( int line , Statement statement , Exception innerException )
			: base( $"{innerException.Message}\nRuntime Error - line {line}: {statement.Line}\n\t-> {statement}" , innerException )
		{
			this.Line = line;
			this.Statement = statement;
		}
	}

	/// <summary>
	/// Exception thrown when an undefined operation is attempted.
	/// </summary>
	/// <remarks>
	/// Example:
	/// <c>APPEND 1 TO y</c>
	/// where y is a numeric type value.
	/// </remarks>
	public class UndefinedOperationException : Exception
	{
		public readonly string Operation;

		public readonly Value Operand1;

		public readonly Value? Operand2;

		public UndefinedOperationException( Value operand1 , string operation , Value? operand2 = null )
			: base( $"Undefined operation: {operand1.TypeString( )} {operation} {operand2?.TypeString( ) ?? string.Empty}" )
		{
			this.Operation = operation;
			this.Operand1 = operand1;
			this.Operand2 = operand2;
		}
	}

	/// <summary>
	/// Exception thrown when an operation is given an incorrect type operand.
	/// </summary>
	public class IncorrectOperandTypeException : Exception
	{
		public readonly Token Token;

		public readonly Value Value;

		public readonly Operation Statement;

		public IncorrectOperandTypeException( Operation statement , Token token , Value value )
			: base( $"Incorrect operand type for {statement} statement: {token} = {value.TypeString( )} '({value})'" )
		{
			this.Statement = statement;
			this.Token = token;
			this.Value = value;
		}
	}

	/// <summary>
	/// Exception thrown when the index to an accesed variable is not
	/// an integer value.
	/// </summary>
	public class InvalidIndexValueException : Exception
	{
		public readonly Token Token;

		public readonly Value Value;

		public InvalidIndexValueException( Token token , Value value )
			: base( $"Invalid index ({token} = {value}), index must be integer." )
		{
			this.Token = token;
			this.Value = value;
		}
	}

	/// <summary>
	/// Exception thrown when a value is out of range for a certain context.
	/// </summary>
	public class ValueOutOfRangeException : Exception
	{
		public readonly Value Index;

		public readonly string Reason;

		public ValueOutOfRangeException( Value index , string reason )
			: base( $"Value ({index}) out of range for {reason}." )
		{
			this.Index = index;
			this.Reason = reason;
		}
	}

	/// <summary>
	/// Exception thrown when a value is accesed that is not a string or list.
	/// </summary>
	public class NonIndexableValueException : Exception
	{
		public readonly Token Token;

		public readonly Value Value;

		public NonIndexableValueException( Token token , Value value )
			: base( $"Cannot access index of non-indexable value ({token} = {value})" )
		{
			this.Token = token;
			this.Value = value;
		}
	}

	/// <summary>
	/// Exception thrown when a variables value is retrieved when it does not
	/// exist in the current scope.
	/// </summary>
	public class NonExistantVariableException : Exception
	{
		public readonly Token Token;

		public NonExistantVariableException( Token token )
			: base( $"Variable '{token}' does not exist in current scope." )
		{
			this.Token = token;
		}
	}

	/// <summary>
	/// Exception thrown when the final / root scope is attempted to be closed.
	/// </summary>
	public class CannnotCloseFinalScopeException : Exception
	{
		public CannnotCloseFinalScopeException( ) : base( "Cannot close the final scope." ) { }
	}

	/// <summary>
	/// Exception thrown by the BNA <see cref="Operation.ERROR"/> statement.
	/// </summary>
	public class ErrorStatementException : Exception
	{
		public string StatementMessage;

		public ErrorStatementException( Value value ) : base( $"ERROR {value}" )
		{
			this.StatementMessage = value.ToString( );
		}
	}
}
