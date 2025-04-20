using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class InputStatement : ComplexStatement<StringOperand>
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Input, Parse );

        public override string Operation => "INPUT";

        private InputStatement( Identifier identifier, StringOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static InputStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            // TODO make "with" part optional?
            tokenHandler.Get( Keyword.Input )
                        .Get( out Identifier identifier )
                        .Get( Keyword.With )
                        .Get( out StringOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
