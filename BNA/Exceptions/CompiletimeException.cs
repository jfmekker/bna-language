using BNA.Common;
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

		public CompiletimeException( int line , int column , string lineString , Exception innerException )
			: base( $"{innerException.Message}\n" +
					$"Compiletime Error - line {line}: {lineString}\n" +
					$"                         {" ".Repeat( line.ToString( ).Length )}  {" ".Repeat( column )}" ,
				  innerException )
		{
			this.Line = line;
			this.Column = column;
			this.LineString = lineString;
		}

		public CompiletimeException( CompiletimeException exception , int line_number , string line )
			: base( exception.Message + "\nCompile error" )
		{
			this.LineString = "";
		}

		public CompiletimeException( string message )
			: base( message + "\nCompile error" )
		{
			this.LineString = "";
		}
	}

	public class UnexpectedSymbolException : Exception
	{
		public UnexpectedSymbolException( char? symbol )
			: base( $"Unexpected symbol: '{symbol?.ToString( ) ?? "null" }'" )
		{
		}
	}

	public class MissingTerminatorException : Exception
	{
		public MissingTerminatorException( string thing, char terminator )
			: base( $"{thing} missing '{terminator}' terminator before end of line." )
		{
		}
	}

	public class IllegalTokenException : Exception
	{
		public IllegalTokenException( string message )
			: base( message )
		{
		}
	}

	public class InvalidTokenException : Exception
	{
		public InvalidTokenException( string message )
			: base( message )
		{
		}
	}
}
