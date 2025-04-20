using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class CloseStatement : SimpleStatement<IdentifierOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Close, Parse );

        public override string Operation => "CLOSE";

        private CloseStatement( IdentifierOperand operand, string raw )
            : base( operand, raw )
        {
        }

        private static CloseStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Close )
                        .Get( out IdentifierOperand operand )
                        .End( );

            return new( operand, raw );
        }
    }
}
