using BNA.Compile;
using System;

namespace BNA.Exceptions
{
	/// <summary>
	/// A BNA exception encountered at compile time.
	/// </summary>
	public class CompiletimeException : Exception
	{
		public readonly int Line;

		public readonly int Column;

		public readonly string LineString;

		public CompiletimeException( int line, int column , string lineString , Exception innerException )
			: base( $"t{innerException.Message}\nCompiletime Error - line {line}: {lineString}" , innerException )
		{
			this.Line = line;
			this.Column = column;
			this.LineString = lineString;
		}

		public CompiletimeException( int line_number , string line , string message )
			: base( message + "\nCompile error on line " + line_number + ":\n\t" + line )
		{
			this.Line = line;
			this.LineNumber = line_number;
			this.BNAMessage = message;
		}

		public CompiletimeException( CompiletimeException exception , int line_number , string line )
			: base( exception.BNAMessage + "\nCompile error on line " + line_number + ":\n\t" + line , exception )
		{
			this.Line = line;
			this.LineNumber = line_number;
			this.BNAMessage = exception.BNAMessage;
		}

		public CompiletimeException( string message )
			: base( message + "\nCompile error" )
		{
			this.Line = string.Empty;
			this.LineNumber = -1;
			this.BNAMessage = message;
		}
	}

	public class MissingWhitespaceException : Exception
	{ }

	public class InvalidTokenException : Exception
	{ }

	public class IllegalSymbolException : Exception
	{ }

	public class UnmatchedTerminatorException : Exception
	{ }

	public class UnexpectedTokenException : Exception
	{ }
}
