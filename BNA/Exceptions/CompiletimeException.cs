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
		/// <summary>
		/// Line number the <see cref="Exception"/> occured on.
		/// </summary>
		public readonly int Line;

		/// <summary>
		/// Character column of the <see langword="char"/> or <see cref="Token"/>
		/// that caused the <see cref="Exception"/>.
		/// </summary>
		public readonly int Column;

		/// <summary>
		/// Raw <see langword="string"/> of the line that caused the
		/// <see cref="Exception"/>, as given to the compiler.
		/// </summary>
		public readonly string LineString;

		/// <summary>
		/// Create a new <see cref="CompiletimeException"/> instance by
		/// wrapping the thrown <see cref="Exception"/>.
		/// </summary>
		/// <param name="line">Line number.</param>
		/// <param name="column">Character column.</param>
		/// <param name="lineString">String value of the line.</param>
		/// <param name="innerException">Exception to wrap.</param>
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

	/// <summary>
	/// Exception thrown when a <see cref="Symbol"/> was found in an unexpected
	/// place, or did not match the expected symbol.
	/// </summary>
	public class UnexpectedSymbolException : Exception
	{
		public UnexpectedSymbolException( char? symbol )
			: base( $"Unexpected symbol: '{symbol?.ToString( ) ?? "null" }'" )
		{
		}
	}

	/// <summary>
	/// Exception thrown when a list or string <see cref="Token"/> is being
	/// parsed but the closing terminator (<see cref="Symbol.LIST_END"/> or
	/// <see cref="Symbol.STRING_MARKER"/>) was not found.
	/// </summary>
	public class MissingTerminatorException : Exception
	{
		public MissingTerminatorException( string thing , char terminator )
			: base( $"{thing} missing '{terminator}' terminator before end of line." )
		{
		}
	}

	/// <summary>
	/// Exception thrown when a <see cref="Token"/> of an incorrect/unexpected
	/// type was found when parsing a <see cref="Statement"/>.
	/// </summary>
	public class IllegalTokenException : Exception
	{
		public IllegalTokenException( string message )
			: base( message )
		{
		}
	}

	/// <summary>
	/// Exception thrown when a <see cref="Token"/> is parsed but is not valid,
	/// like an invalid number ("0.1.234") or variable with accessor ("x@").
	/// </summary>
	public class InvalidTokenException : Exception
	{
		public InvalidTokenException( string message )
			: base( message )
		{
		}
	}

	/// <summary>
	/// Exception thrown when a <see cref="Token"/> is expected but none found.
	/// </summary>
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
