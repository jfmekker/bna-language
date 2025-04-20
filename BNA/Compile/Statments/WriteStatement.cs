using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class WriteStatement : ComplexStatement<AnyOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Write, Parse );

        public override string Operation => "WRITE";

        private WriteStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static WriteStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Write )
                        .Get( out AnyOperand operand )
                        .Get( Keyword.To )
                        .Get( out Identifier identifier )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
