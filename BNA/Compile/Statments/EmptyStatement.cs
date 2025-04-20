using System.Collections.Generic;
using System.Text;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class EmptyStatement : Statement
    {
        private class EmptyStatementParser : IStatementParser
        {
            public bool IsStartToken( Token token ) => token is Comment;
            public Statement Parse( IReadOnlyCollection<Token> tokens, string raw )
                => EmptyStatement.Parse( tokens, raw );
        }

        public static IStatementParser Parser { get; } = new EmptyStatementParser( );

        public override string Operation => "EMPTY";

        private EmptyStatement( string raw )
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

        public static EmptyStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.End( );

            return new( raw );
        }
    }
}
