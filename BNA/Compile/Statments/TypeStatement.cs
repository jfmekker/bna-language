using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class TypeStatement : ComplexStatement<IdentifierOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Type, Parse );

        public override string Operation => "TYPE";

        private TypeStatement( Identifier identifier, IdentifierOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static TypeStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Type )
                        .Get( out Identifier identifier )
                        .Get( Keyword.Of )
                        .Get( out IdentifierOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
