using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;
using BNA.Exceptions;

namespace BNA.Compile.Statments
{
    public abstract record class JumpStatement : Statement
    {
        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Goto, Parse );

        protected JumpStatement( string raw ) : base( raw ) { }

        private static Statement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            _ = tokenHandler.Get( Keyword.Goto )
                            .Get( out Identifier _ );
            int column = tokenHandler.GetCurrentColumn( );
            _ = tokenHandler.Get( out Token token );

            if ( token == Keyword.If )
            {
                return ConditionalJumpStatement.Parse( tokens, raw );
            }
            else if ( token is Identifier )
            {
                return SimpleJumpStatement.Parse( tokens, raw );
            }
            else
            {
                throw new UnexpectedTokenException( token, column, typeof( Keyword ), typeof( Identifier ) );
            }
        }
    }

    public record class SimpleJumpStatement : SimpleStatement<IdentifierOperand>
    {
        public override string Operation => "GOTO";

        private SimpleJumpStatement( IdentifierOperand operand, string raw )
            : base( operand, raw )
        {
        }

        internal static SimpleJumpStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            // TODO allow other operand types?
            tokenHandler.Get( Keyword.Goto )
                        .Get( out IdentifierOperand operand )
                        .End( );

            return new SimpleJumpStatement( operand, raw );
        }
    }

    public record class ConditionalJumpStatement : ComplexStatement<NumberOperand>
    {
        public override string Operation => "GOTO_IF";

        private ConditionalJumpStatement( Identifier identifier, NumberOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static ConditionalJumpStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Goto )
                        .Get( out Identifier identifier )
                        .Get( Keyword.If )
                        .Get( out NumberOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
