using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class ExponentStatement : ComplexStatement<Identifier, NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Mod, Parse );

        public override string Operation => "EXPONENT";

        private ExponentStatement( Identifier primary, NumberOperand secondary, string raw )
            : base( primary, secondary, raw )
        {
        }

        private static ExponentStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Raise )
                        .Get( out Identifier primary )
                        .Get( Keyword.To )
                        .Get( out NumberOperand secondary )
                        .End( );

            return new( primary, secondary, raw );
        }
    }
}
