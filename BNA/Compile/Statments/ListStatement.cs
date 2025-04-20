using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class ListStatement : ComplexStatement<NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.List, Parse );

        public override string Operation => "LIST";

        private ListStatement( Identifier identifier, NumberOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static ListStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.List )
                        .Get( out Identifier identifier )
                        .Get( Keyword.Size )
                        .Get( out NumberOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }

}
