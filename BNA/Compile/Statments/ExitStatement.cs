using System.Collections.Generic;
using System.Text;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class ExitStatement : Statement
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Exit, Parse );

        public override string Operation => "EXIT";

        private ExitStatement( string raw )
            : base( raw )
        {
        }

        public override string ToString( )
        {
            StringBuilder builder = new( );

            _ = builder.Append( '[' )
                       .AppendLine( this.Operation )
                       .Append( ']' );

            return builder.ToString( );
        }

        public static ExitStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Exit )
                        .End( );

            return new( raw );
        }
    }
}
