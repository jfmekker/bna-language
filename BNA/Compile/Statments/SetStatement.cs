using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class SetStatement : ComplexStatement<AnyOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Set, Parse );

        public override string Operation => "SET";

        private SetStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static SetStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Set )
                        .Get( out Identifier identifier )
                        .Get( Keyword.To )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
