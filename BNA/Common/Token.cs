using System;

namespace BNA.Common
{
	public enum TokenType
	{
		NULL = 0,
		NUMBER,
		VARIABLE,
		STRING,
		LIST,
		KEYWORD,
		SYMBOL,
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

		public Token( Keyword keyword )
		{
			this.Value = keyword.ToString( );
			this.Type = TokenType.KEYWORD;
		}

		public Token( Symbol symbol )
		{
			this.Value = $"{(char)symbol}";
			this.Type = TokenType.SYMBOL;
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

		public Keyword? AsKeyword( ) => ( this.Type == TokenType.KEYWORD ) && Enum.TryParse( this.Value , out Keyword word ) ? word : null;

		public Symbol? AsSymbol( ) => ( this.Type == TokenType.SYMBOL ) ? (Symbol)this.Value[0] : null;

		public override bool Equals( object? obj ) => obj is Token other
													&& ( this.Type == other.Type )
													&& ( ( this.Type == TokenType.NULL )
														|| ( this.Value.ToUpper( ) == other.Value.ToUpper( ) ) );

		public static bool operator ==( Token left , Token right ) => left.Equals( right );

		public static bool operator !=( Token left , Token right ) => !( left == right );
	}
}
