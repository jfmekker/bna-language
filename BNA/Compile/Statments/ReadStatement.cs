using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class ReadStatement : ComplexStatement<IdentifierOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Read, Parse );

        public override string Operation => "READ";

        private ReadStatement( Identifier identifier, IdentifierOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static ReadStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Read )
                        .Get( out IdentifierOperand operand )
                        .Get( Keyword.From )
                        .Get( out Identifier identifier )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
