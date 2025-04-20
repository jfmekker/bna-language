using System;

namespace BNA.Compile.Tokens
{
    public record class WhiteSpace : Token
    {
        public WhiteSpace( string raw )
            : base( raw )
        {
            if ( string.IsNullOrEmpty( raw ) || !string.IsNullOrWhiteSpace( raw ) )
            {
                throw new ArgumentException( "Whitespace cannot be empty or null." );
            }
        }

        protected override string TypeString( ) => "WhiteSpace";
    }
}
