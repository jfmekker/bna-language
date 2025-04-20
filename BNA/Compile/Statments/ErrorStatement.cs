using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class ErrorStatement : SimpleStatement<StringOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Wait, Parse );

        public override string Operation => "ERROR";

        private ErrorStatement( StringOperand operand, string raw )
            : base( operand, raw )
        {
        }

        private static ErrorStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Error )
                        .Get( out StringOperand operand )
                        .End( );

            return new( operand, raw );
        }
    }
}
