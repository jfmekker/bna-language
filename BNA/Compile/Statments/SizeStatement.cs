using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class SizeStatement : ComplexStatement<AnyOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Size, Parse );

        public override string Operation => "SIZE";

        private SizeStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static SizeStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Size )
                        .Get( out Identifier identifier )
                        .Get( Keyword.Of )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }

}
