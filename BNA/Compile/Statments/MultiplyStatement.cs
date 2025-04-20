using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class MultiplyStatement : ComplexStatement<Identifier, NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Multiply, Parse );

        public override string Operation => "MULTIPLY";

        private MultiplyStatement( Identifier primary, NumberOperand secondary, string raw )
            : base( primary, secondary, raw )
        {
        }

        private static MultiplyStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Multiply )
                        .Get( out Identifier primary )
                        .Get( Keyword.By )
                        .Get( out NumberOperand secondary )
                        .End( );

            return new( primary, secondary, raw );
        }
    }
}
