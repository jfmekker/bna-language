using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class WaitStatement : SimpleStatement<NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Wait, Parse );

        public override string Operation => "WAIT";

        private WaitStatement( NumberOperand operand, string raw )
            : base( operand, raw )
        {
        }

        private static WaitStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Wait )
                        .Get( out NumberOperand operand )
                        .End( );

            return new( operand, raw );
        }
    }
}
