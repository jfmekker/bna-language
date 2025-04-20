using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class AddStatement : ComplexStatement<NumberOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Add, Parse );

        public override string Operation => "ADD";

        private AddStatement( Identifier identifier, NumberOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static AddStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Add )
                        .Get( out NumberOperand operand )
                        .Get( Keyword.To )
                        .Get( out Identifier identifier )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
