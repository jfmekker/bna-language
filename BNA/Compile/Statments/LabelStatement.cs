using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class LabelStatement : SimpleStatement<IdentifierOperand>
    {
        public static StatementParser Parser { get; } = new( Symbol.LabelStart, Parse );

        public override string Operation => "LABEL";

        private LabelStatement( IdentifierOperand operand, string raw )
            : base( operand, raw )
        {
        }

        private static LabelStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            // TODO allow whitespace before and after label (will use this for testing first)
            tokenHandler.Get( Symbol.LabelStart )
                        .Get( out IdentifierOperand operand, whitespaceBefore: false )
                        .Get( Symbol.LabelEnd, whitespaceBefore: false )
                        .End( );

            return new( operand, raw );
        }
    }
}
