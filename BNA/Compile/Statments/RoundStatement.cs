using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class RoundStatement : SimpleStatement<IdentifierOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Round, Parse );

        public override string Operation => "ROUND";

        private RoundStatement( IdentifierOperand operand, string raw )
            : base( operand, raw )
        {
        }

        private static RoundStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Round )
                        .Get( out IdentifierOperand operand )
                        .End( );

            return new( operand, raw );
        }
    }
}
