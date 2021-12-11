using BNA.Common;
using BNA.Compile;
using System;
using System.Collections;
using System.Collections.Generic;

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
		public MissingTerminatorException( string thing , char terminator )
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

	public class MissingTokenException : Exception
	{
		public MissingTokenException( params TokenType[] types )
			: base( $"Missing token, expected {types.PrintElements( )}." )
		{
		}

		public MissingTokenException( string token )
			: base( $"Missing token, expected '{token}'." )
		{
		}

		public MissingTokenException( )
			: base( "Statement ended too early." )
		{
		}
	}
}
