using System;
using System.Diagnostics.CodeAnalysis;

namespace BNA.Common
{
    [SuppressMessage( "Naming", "CA1707:Identifiers should not contain underscores", Justification = "TODO" )]
    public enum TokenType
    {
        NULL = 0,
        NUMBER,
        VARIABLE,
        [SuppressMessage( "Naming", "CA1720:Identifier contains type name", Justification = "TODO" )]
        STRING,
        LIST,
        KEYWORD,
        SYMBOL,
        COMMENT
    }

    // TODO make readonly record struct
    public struct Token : IEquatable<Token>
    {
        public TokenType Type
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        public Token( string value, TokenType type )
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

        public override readonly int GetHashCode( ) => HashCode.Combine( this.Type, this.Value );

        public override readonly string ToString( )
        {
            string str = "<(" + this.Type;
            if ( this.Type == TokenType.KEYWORD )
            {
                str += ":" + ((Keyword)Enum.Parse( typeof( Keyword ), this.Value, true )).ToString( );
            }
            else if ( this.Type == TokenType.SYMBOL && Enum.IsDefined( typeof( Symbol ), (int)this.Value[0] ) )
            {
                str += ":" + Enum.GetName( typeof( Symbol ), (int)this.Value[0] );
            }
            str += (this.Type == TokenType.NULL) ? ")>" : (") " + this.Value + ">");

            return str;
        }

        public readonly Keyword? AsKeyword( ) => (this.Type == TokenType.KEYWORD) && Enum.TryParse( this.Value, out Keyword word ) ? word : null;

        public readonly Symbol? AsSymbol( ) => (this.Type == TokenType.SYMBOL) ? (Symbol)this.Value[0] : null;

        public readonly override bool Equals( object? obj ) => obj is Token other && this.Equals( other );

        public readonly bool Equals( Token other ) => (this.Type == other.Type)
                                          && ((this.Type == TokenType.NULL)
                                            || this.Value.Equals( other.Value, StringComparison.OrdinalIgnoreCase ));

        public static bool operator ==( Token left, Token right ) => left.Equals( right );

        public static bool operator !=( Token left, Token right ) => !(left == right);
    }
}
