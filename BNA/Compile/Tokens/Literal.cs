using System.Collections.Generic;
using System;

namespace BNA.Compile.Tokens
{
    public abstract record class Literal : Token
    {
        protected Literal( string raw ) : base( raw ) { }
    }

    public record class LiteralString : Literal
    {
        public string Value { get; }

        public LiteralString( string value, string raw )
            : base( raw )
        {
            this.Value = value;
        }

        protected override string TypeString( ) => "String";
    }

    public abstract record class LiteralNumber : Literal
    {
        protected LiteralNumber( string raw ) : base( raw ) { }
    }

    public record class LiteralInteger : LiteralNumber
    {
        public long Value { get; }

        public LiteralInteger( long value, string raw )
            : base( raw )
        {
            this.Value = value;
        }

        protected override string TypeString( ) => "Integer";
    }

    public record class LiteralReal : LiteralNumber
    {
        public double Value { get; }

        public LiteralReal( double value, string raw )
            : base( raw )
        {
            this.Value = value;
        }

        protected override string TypeString( ) => "Real";
    }

    public record class LiteralList : Literal
    {
        public IReadOnlyCollection<Token> Tokens { get; }

        public LiteralList( IReadOnlyCollection<Token> tokens, string raw )
            : base( raw )
        {
            this.Tokens = tokens;
            Token[] tArr = [.. tokens];

            if ( tArr.Length < 2 ||
                (tArr[0] as Symbol)?.Symbol is not Symbol.ListStart ||
                (tArr[^1] as Symbol)?.Symbol is not Symbol.ListEnd )
            {
                throw new ArgumentException( "A list must contain start and end with correct symbols." );
            }
            else
            {
                for ( int i = 1 ; i < tArr.Length - 1 ; i++ )
                {
                    if ( i % 2 == 0 && (tArr[i] as Symbol)?.Symbol is not Symbol.ListSeparator )
                    {
                        throw new ArgumentException( "Tokens in list must be separated by separator." );
                    }
                    else if ( i % 2 == 1 && tArr[i] is not Literal or VariableToken )
                    {
                        throw new ArgumentException( "Tokens in list must be literals or variables." );
                    }
                }
            }
        }

        protected override string TypeString( ) => "List";
    }
}
