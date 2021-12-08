using System;
using System.Collections.Generic;
using BNA.Common;
using BNA.Exceptions;

namespace BNA.Compile
{
	/// <summary>
	/// What a given Token is identified as.
	/// </summary>
	public enum TokenType
	{
		NULL = 0,
		LITERAL,
		VARIABLE,
		STRING,
		LIST,
		KEYWORD,
		SYMBOL,
		LABEL,
		COMMENT
	}

	/// <summary>
	/// Group of related characters that form keywords/variables/etc.
	/// </summary>
	public struct Token
	{
		/// <summary>
		/// The TokenType of this Token.
		/// </summary>
		public TokenType Type
		{
			get; set;
		}

		/// <summary>
		/// The actual string input for this Token.
		/// </summary>
		public string Value
		{
			get; set;
		}

		/// <summary>
		/// Create a new Token and try to identify it.
		/// </summary>
		/// <param name="value">The string input of the Token</param>
		/// <param name="type">The Token's type</param>
		public Token( string value , TokenType type )
		{
			this.Value = value;
			this.Type = type;
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given TokenTypes.
		/// </summary>
		/// <param name="token">Token to check type of (can be null)</param>
		/// <param name="types">The TokenTypes to check for</param>
		public static void ThrowIfNotTypes( Token token , ICollection<TokenType> types )
		{
			if ( !types.Contains( token.Type ) )
			{
				throw new CompiletimeException( "Unexpected token: '" + token.ToString( ) + "', expected {" + string.Join( ", " , types ) + "}." );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given Kymbols.
		/// </summary>
		/// <param name="symbols">List of valid keywords</param>
		public void ThrowIfNotKeywords( ICollection<Keyword> keywords )
		{
			ThrowIfNotTypes( this , new List<TokenType> { TokenType.KEYWORD } );
			if ( Enum.TryParse( this.Value , out Keyword result ) && !keywords.Contains( result ) )
			{
				throw new CompiletimeException( "Unexpected keyword '" + this.ToString( ) + "', expected {" + string.Join( ", " , keywords ) + "}." );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given Symbols.
		/// </summary>
		/// <param name="symbols">List of valid symbols</param>
		public void ThrowIfNotSymbols( ICollection<Symbol> symbols )
		{
			ThrowIfNotTypes( this , new List<TokenType> { TokenType.SYMBOL } );
			if ( Enum.TryParse( this.Value , out Symbol result ) && !symbols.Contains( result ) )
			{
				throw new CompiletimeException( "Unexpected symbol '" + this.ToString( ) + "', expected {" + string.Join( ", " , symbols ) + "}." );
			}
		}

		/// <summary>
		/// Check if a character is a defined Symbol
		/// </summary>
		/// <param name="c">The character to check</param>
		/// <param name="value">Output value</param>
		/// <returns>True if the character was a defined Symbol</returns>
		public static bool TryParseSymbol( char c , out Symbol value )
		{
			if ( Enum.IsDefined( typeof( Symbol ) , (int)c ) )
			{
				value = (Symbol)c;
				return true;
			}
			value = Symbol.NULL;
			return false;
		}

		/// <summary>
		/// Get a hash code for the Token object.
		/// </summary>
		/// <returns>Hash code.</returns>
		public override int GetHashCode( )
		{
			return HashCode.Combine( this.Type , this.Value );
		}

		/// <summary>
		/// Stringify the Token with type information.
		/// </summary>
		/// <returns>String description of the Token</returns>
		public override string ToString( )
		{
			string str = "<(" + this.Type;
			if ( this.Type == TokenType.KEYWORD )
			{
				str += ":" + ( (Keyword)Enum.Parse( typeof( Keyword ) , this.Value , true ) ).ToString( );
			}
			else if ( this.Type == TokenType.SYMBOL && Enum.IsDefined( typeof( Symbol ) , (int)this.Value[0] ) )
			{
				str += ":" + Enum.GetName( typeof( Symbol ) , (int)this.Value[0] );
			}
			str += ( this.Type == TokenType.NULL ) ? ")>" : ( ") " + this.Value + ">" );

			return str;
		}

		/// <summary>
		/// Compare equality of two Token with the equality operator.
		/// </summary>
		/// <param name="left">Left hand operand.</param>
		/// <param name="right">Right hand operand.</param>
		/// <returns>True if the Tokens have the same type and value.</returns>
		public static bool operator ==( Token left , Token right ) => ( left.Type == right.Type ) && ( left.Value.ToUpper( ) == right.Value.ToUpper( ) );

		/// <summary>
		/// Compare non-equality of two Token with the equality operator.
		/// </summary>
		/// <param name="left">Left hand operand.</param>
		/// <param name="right">Right hand operand.</param>
		/// <returns>True if the Tokens have different types or values.</returns>
		public static bool operator !=( Token left , Token right ) => !( left == right );

		/// <summary>
		/// Compare a Token to an object.
		/// </summary>
		/// <param name="obj">Object to compare against.</param>
		/// <returns>True if the object is an equal Token.</returns>
		public override bool Equals( object? obj ) =>
			(obj is Token other && this == other) ||
			(obj is Symbol symbol && ( this.Type == TokenType.SYMBOL ) && ( this.Value.ToUpper( ) == ( (char)symbol ).ToString( ).ToUpper( ) ));
	}
}
