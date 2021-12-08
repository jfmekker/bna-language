using System;
using BNA.Common;

namespace BNA.Compile
{
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

	public struct Token
	{
		public TokenType Type
		{
			get; set;
		}

		public string Value
		{
			get; set;
		}

		public Token( string value , TokenType type )
		{
			this.Value = value;
			this.Type = type;
		}

		public override int GetHashCode( )
		{
			return HashCode.Combine( this.Type , this.Value );
		}

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

		public override bool Equals( object? obj ) =>
			( obj is Token other && ( this.Type == other.Type ) && ( this.Value.ToUpper( ) == other.Value.ToUpper( ) ) ) ||
			( obj is Symbol symbol && ( this.Type == TokenType.SYMBOL ) && ( this.Value.ToUpper( ) == ( (char)symbol ).ToString( ).ToUpper( ) ) ) ||
			( obj is Keyword keyword && ( this.Type == TokenType.KEYWORD ) && Enum.TryParse( this.Value , out Keyword word ) && word == keyword );

		public static bool operator ==( Token left , Token right ) => left.Equals( right );

		public static bool operator !=( Token left , Token right ) => !( left == right );
	}
}
