using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class RandomStatement : ComplexStatement<NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Random, Parse );

        public override string Operation => "RANDOM";

        private RandomStatement( Identifier identifier, NumberOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static RandomStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Set )
                        .Get( out Identifier identifier )
                        .Get( Keyword.To )
                        .Get( out NumberOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
