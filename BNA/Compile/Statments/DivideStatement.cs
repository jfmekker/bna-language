using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class DivideStatement : ComplexStatement<Identifier, NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Divide, Parse );

        public override string Operation => "DIVIDE";

        private DivideStatement( Identifier primary, NumberOperand secondary, string raw )
            : base( primary, secondary, raw )
        {
        }

        private static DivideStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Divide )
                        .Get( out Identifier primary )
                        .Get( Keyword.By )
                        .Get( out NumberOperand secondary )
                        .End( );

            return new( primary, secondary, raw );
        }
    }
}
