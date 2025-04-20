using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class SubtractStatement : Statement
    {
        public static StatementParser Parser { get; } = new( Keyword.Subtract, Parse );

        public override string Operation => "SUBTRACT";
        public override Identifier PrimaryOperand { get; }
        public override Token? SecondaryOperand { get; }

        private SubtractStatement( Identifier primary, Token secondary, string raw )
            : base( raw )
        {
            this.PrimaryOperand = primary;
            this.SecondaryOperand = secondary;
        }

        private static SubtractStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Subtract )
                        .Get( out NumberOperand secondary )
                        .Get( Keyword.From )
                        .Get( out Identifier primary )
                        .End( );

            return new( primary, secondary.Token, raw );
        }
    }
}
