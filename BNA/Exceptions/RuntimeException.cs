using System;
using BNA.Common;
using BNA.Compile;
using BNA.Run;
using BNA.Values;

namespace BNA.Exceptions
{
	/// <summary>
	/// A BNA exception encountered at runtime.
	/// </summary>
	public class RuntimeException : Exception
	{
		public readonly int Line;

		public readonly Statement Statement;

		public RuntimeException( int line , Statement statement , Exception innerException )
			: base( $"{innerException.Message}\nRuntime Error - line {line}: {statement.RawString( )}\n\t-> {statement}" , innerException )
		{
			this.Line = line;
			this.Statement = statement;
		}
	}

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

	public class NonExistantVariableException : Exception
	{
		public readonly Token Token;

		public NonExistantVariableException( Token token )
			: base( $"Variable '{token}' does not exist in current scope." )
		{
			this.Token = token;
		}
	}

	public class CloseFinalScopeException : Exception
	{
		public CloseFinalScopeException( ) : base( "Cannot close the final scope." ) { }
	}

	public class ErrorStatementException : Exception
	{
		public string StatementMessage;

		public ErrorStatementException( Value value ) : base( $"ERROR {value}" )
		{
			this.StatementMessage = value.ToString( );
		}
	}
}
