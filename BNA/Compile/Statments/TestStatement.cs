using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;
using BNA.Exceptions;

namespace BNA.Compile.Statments
{
    public abstract record class TestStatement : ComplexStatement<AnyOperand>
    {
        //public enum TestType { EqualTo, NotEqualTo, GreaterThan, GreaterThanEqualTo, LessThan, LessThanEqualTo }

        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Test, Parse );

        protected TestStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        private static TestStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            _ = tokenHandler.Get( Keyword.Test )
                            .Get( out Identifier _ )
                            .Get( out Symbol symbol );

            if ( symbol == Symbol.Equal )
            {
                return TestEqualsStatement.Parse( tokens, raw );
            }
            else if ( symbol == Symbol.GreaterThan )
            {
                return TestGreaterThanStatement.Parse( tokens, raw );
            }
            else if ( symbol == Symbol.LessThan )
            {
                return TestLessThanStatement.Parse( tokens, raw );
            }
            else if ( symbol == Symbol.Not )
            {
                return TestNotEqualToStatement.Parse( tokens, raw );
            }
            else
            {
                throw new UnexpectedTokenException( symbol, tokenHandler.GetCurrentColumn( ), Symbol.Equal, Symbol.GreaterThan, Symbol.LessThan, Symbol.Not );
            }
        }
    }

    public record class TestEqualsStatement : TestStatement
    {
        public override string Operation => "TEST_EQU";

        private TestEqualsStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static TestEqualsStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Test )
                        .Get( out Identifier identifier )
                        .Get( Symbol.Equal )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
    public record class TestEqualToStatement : TestStatement
    {
        public override string Operation => "TEST_EQU";

        private TestEqualToStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static TestEqualToStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Test )
                        .Get( out Identifier identifier )
                        .Get( Symbol.Equal )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
    public record class TestNotEqualToStatement : TestStatement
    {
        public override string Operation => "TEST_NEQ";

        private TestNotEqualToStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static TestNotEqualToStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Test )
                        .Get( out Identifier identifier )
                        .Get( Symbol.Not )
                        .Get( Symbol.Equal, whitespaceBefore: false )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
    public record class TestGreaterThanStatement : TestStatement
    {
        public override string Operation => "TEST_GTR";

        private TestGreaterThanStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static TestGreaterThanStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Test )
                        .Get( out Identifier identifier )
                        .Get( Symbol.GreaterThan )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
    public record class TestGreaterThanEqualToStatement : TestStatement
    {
        public override string Operation => "TEST_GTE";

        private TestGreaterThanEqualToStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static TestGreaterThanEqualToStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Test )
                        .Get( out Identifier identifier )
                        .Get( Symbol.GreaterThan )
                        .Get( Symbol.Equal, whitespaceBefore: false )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
    public record class TestLessThanStatement : TestStatement
    {
        public override string Operation => "TEST_LSS";

        private TestLessThanStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static TestLessThanStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Test )
                        .Get( out Identifier identifier )
                        .Get( Symbol.LessThan )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
    public record class TestLessThanEqualToStatement : TestStatement
    {
        public override string Operation => "TEST_LTE";

        private TestLessThanEqualToStatement( Identifier identifier, AnyOperand operand, string raw )
            : base( identifier, operand, raw )
        {
        }

        internal static TestLessThanEqualToStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            tokenHandler.Get( Keyword.Test )
                        .Get( out Identifier identifier )
                        .Get( Symbol.LessThan )
                        .Get( Symbol.Equal, whitespaceBefore: false )
                        .Get( out AnyOperand operand )
                        .End( );

            return new( identifier, operand, raw );
        }
    }
}
