using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class ModulusStatement : ComplexStatement<Identifier, NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Mod, Parse );

        public override string Operation => "MODULUS";

        private ModulusStatement( Identifier primary, NumberOperand secondary, string raw )
            : base( primary, secondary, raw )
        {
        }

        private static ModulusStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Mod )
                        .Get( out NumberOperand secondary )
                        .Get( Keyword.Of )
                        .Get( out Identifier primary )
                        .End( );

            return new( primary, secondary, raw );
        }
    }
}
