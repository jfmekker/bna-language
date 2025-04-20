using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class PrintStatement : SimpleStatement<AnyOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Print, Parse );

        public override string Operation => "PRINT";

        private PrintStatement( AnyOperand operand, string raw )
            : base( operand, raw )
        {
        }

        private static PrintStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Print )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( operand, raw );
        }
    }
}
