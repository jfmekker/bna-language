using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class AppendStatement : ComplexStatement<AnyOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Append, Parse );

        public override string Operation => "APPEND";

        private AppendStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static AppendStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Append )
                        .Get( out AnyOperand operand )
                        .Get( Keyword.To )
                        .Get( out Identifier identifier )
                        .End( );

            return new( identifier, operand, raw );
        }
    }

}
