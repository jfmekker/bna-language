using BNA.Common;
using BNA.Exceptions;
using BNA.Utils;
using System;
using System.Collections.Generic;

namespace BNA.Compile
{
    public class Parser
    {
        public string RawLine { get; init; }

        public int RawIndex { get; private set; }

        public IReadOnlyList<Token> Tokens { get; init; }

        public int Index { get; private set; }

        public Token? Current => this.Index < this.Tokens.Count ? this.Tokens[this.Index] : null;

        private Operation? operation;

        private Token? operand1;

        private Token? operand2;

        public Parser( string line, ICollection<Token> tokens )
        {
            this.RawLine = line;
            this.Tokens = [.. tokens];
            this.Index = 0;
        }

        public Statement ParseStatement( )
        {
            if ( this.Current?.AsKeyword( ) is Keyword keyword )
            {
                this.ParseKeywordStatement( keyword );
            }
            else if ( this.Current?.AsSymbol( ) is Symbol symbol )
            {
                this.ParseSymbolStatement( symbol );
            }
            else
            {
                this.End( );
            }

            return new Statement( this.RawLine, this.operation ?? default, this.operand1, this.operand2 );
        }

        private void ParseKeywordStatement( Keyword start )
        {
            switch ( start )
            {
                case Keyword.SET:
                { this.SetOperation( Operation.Set ).Operand1( ).Next( Keyword.TO ).Operand2( AllOperandTypes( ) ).End( ); break; }

                case Keyword.ADD:
                { this.SetOperation( Operation.Add ).Operand2( NumericOperandTypes( ) ).Next( Keyword.TO ).Operand1( ).End( ); break; }

                case Keyword.SUBTRACT:
                { this.SetOperation( Operation.Subtract ).Operand2( NumericOperandTypes( ) ).Next( Keyword.FROM ).Operand1( ).End( ); break; }

                case Keyword.MULTIPLY:
                { this.SetOperation( Operation.Multiply ).Operand1( ).Next( Keyword.BY ).Operand2( NumericOperandTypes( ) ).End( ); break; }

                case Keyword.DIVIDE:
                { this.SetOperation( Operation.Divide ).Operand1( ).Next( Keyword.BY ).Operand2( NumericOperandTypes( ) ).End( ); break; }

                case Keyword.MOD:
                { this.SetOperation( Operation.Modulus ).Operand2( NumericOperandTypes( ) ).Next( Keyword.OF ).Operand1( ).End( ); break; }

                case Keyword.LOG:
                { this.SetOperation( Operation.Logarithm ).Operand2( NumericOperandTypes( ) ).Next( Keyword.OF ).Operand1( ).End( ); break; }

                case Keyword.RAISE:
                { this.SetOperation( Operation.Power ).Operand1( ).Next( Keyword.TO ).Operand2( NumericOperandTypes( ) ).End( ); break; }

                case Keyword.ROUND:
                { this.SetOperation( Operation.Round ).Operand1( ).End( ); break; }

                case Keyword.RANDOM:
                { this.SetOperation( Operation.Random ).Operand1( ).Next( Keyword.MAX ).Operand2( NumericOperandTypes( ) ).End( ); break; }

                case Keyword.WAIT:
                { this.SetOperation( Operation.Wait ).Operand2( NumericOperandTypes( ) ).End( ); break; }

                case Keyword.TEST:
                {
                    this.IncrementIndex( );
                    _ = this.Operand1( );

                    Operation operation;
                    if ( this.Current is Token token )
                    {
                        if ( token.AsSymbol( ) is Symbol symbol )
                        {
                            operation = symbol switch {
                                Symbol.GreaterThan => Operation.TestGreaterThan,
                                Symbol.LessThan => Operation.TestLessThan,
                                Symbol.Equal => Operation.TestEqualTo,
                                Symbol.Not => Operation.TestNotEqualTo,
                                _ => throw new IllegalTokenException( $"Expected a comparison operator symbol, got {token}." )
                            };
                        }
                        else
                        {
                            throw new IllegalTokenException( $"Expected a symbol token, got {token}." );
                        }
                    }
                    else
                    {
                        throw new MissingTokenException( $"Expected a comparison operator symbol." );
                    }

                    this.SetOperation( operation ).Operand2( AllOperandTypes( ) ).End( );
                    break;
                }

                case Keyword.GOTO:
                { this.SetOperation( Operation.Goto ).Operand1( ).Optional( Keyword.IF )?.Operand2( NumericOperandTypes( ) ).End( ); break; }

                case Keyword.LIST:
                { this.SetOperation( Operation.List ).Operand1( ).Optional( Keyword.SIZE )?.Operand2( NumericOperandTypes( ) ).End( ); break; }

                case Keyword.APPEND:
                { this.SetOperation( Operation.Append ).Operand2( AllOperandTypes( ) ).Next( Keyword.TO ).Operand1( ).End( ); break; }

                case Keyword.SIZE:
                { this.SetOperation( Operation.Size ).Operand1( ).Next( Keyword.OF ).Operand2( AllOperandTypes( ) ).End( ); break; }

                case Keyword.OPEN:
                {
                    this.IncrementIndex( );
                    this.Operand2( StringOperandTypes( ) ).Next( Keyword.AS ).SetOperation(
                        this.Optional( Keyword.READ, allow_illegal: true ) is not null ? Operation.OpenRead
                        : this.Next( Keyword.WRITE ) is not null ? Operation.OpenWrite
                        : throw new InvalidOperationException( "Parser.Next returned null." ),
                        false
                    ).Operand1( ).End( );
                    break;
                }

                case Keyword.CLOSE:
                { this.SetOperation( Operation.Close ).Operand1( ).End( ); break; }

                case Keyword.WRITE:
                { this.SetOperation( Operation.Write ).Operand2( AllOperandTypes( ) ).Next( Keyword.TO ).Operand1( ).End( ); break; }

                case Keyword.READ:
                { this.SetOperation( Operation.Read ).Operand2( TokenType.Variable ).Next( Keyword.FROM ).Operand1( ).End( ); break; }

                case Keyword.INPUT:
                { this.SetOperation( Operation.Input ).Operand1( ).Next( Keyword.WITH ).Operand2( StringOperandTypes( ) ).End( ); break; }

                case Keyword.PRINT:
                { this.SetOperation( Operation.Print ).Operand2( AllOperandTypes( ) ).End( ); break; }

                case Keyword.TYPE:
                { this.SetOperation( Operation.Type ).Operand1( ).Next( Keyword.OF ).Operand2( AllOperandTypes( ) ).End( ); break; }

                case Keyword.SCOPE:
                {
                    this.IncrementIndex( );
                    this.SetOperation(
                        this.Optional( Keyword.OPEN, allow_illegal: true ) is not null ? Operation.ScopeOpen
                        : this.Next( Keyword.CLOSE ) is not null ? Operation.ScopeClose
                        : throw new InvalidOperationException( "Parser.Next returned null." ),
                        false
                    ).End( );
                    break;
                }

                case Keyword.EXIT:
                { this.SetOperation( Operation.Exit ).End( ); break; }

                case Keyword.ERROR:
                { this.SetOperation( Operation.Error ).Operand2( StringOperandTypes( ) ).End( ); break; }

                case Keyword.AND:
                case Keyword.OR:
                case Keyword.XOR:
                case Keyword.NEGATE:
                {
                    throw new NotImplementedException( $"Parsing of {start} statement not implented." );
                }

                default:
                {
                    throw new IllegalTokenException( $"Illegal keyword to start statement: {this.Current}." );
                }
            }
        }

        private void ParseSymbolStatement( Symbol start )
        {
            switch ( start )
            {
                case Symbol.LabelStart:
                { this.SetOperation( Operation.Label ).Operand1( ).Next( Symbol.LabelEnd ).End( ); break; }

                default:
                    throw new IllegalTokenException( $"Illegal symbol to start statement: {this.Current}." );
            }
        }

        private Parser SetOperation( Operation operation, bool increment = true )
        {
            if ( this.operation.HasValue )
            {
                throw new InvalidOperationException( "Parser tried to set operation of statement more than once." );
            }

            if ( increment )
            {
                this.IncrementIndex( );
            }

            this.operation = operation;
            return this;
        }

        private Parser Next( Keyword keyword )
        {
            if ( this.Current is Token token && token.AsKeyword( ) == keyword )
            {
                if ( token.AsKeyword( ) == keyword )
                {
                    this.IncrementIndex( );
                    return this;
                }
                else
                {
                    throw new IllegalTokenException( $"Expected keyword '{keyword}', got token {token}." );
                }
            }
            else
            {
                throw new MissingTokenException( keyword.ToString( ) );
            }
        }

        private Parser Next( Symbol symbol )
        {
            if ( this.Current is Token token && token.AsSymbol( ) == symbol )
            {
                if ( token.AsSymbol( ) == symbol )
                {
                    this.IncrementIndex( );
                    return this;
                }
                else
                {
                    throw new IllegalTokenException( $"Expected keyword '{symbol}', got token {token}." );
                }
            }
            else
            {
                throw new MissingTokenException( symbol.ToString( ) );
            }
        }

        private Parser Next( params TokenType[] types )
        {
            if ( this.Current is Token token )
            {
                foreach ( TokenType type in types )
                {
                    if ( token.Type == type )
                    {
                        this.IncrementIndex( );
                        return this;
                    }
                }

                throw new IllegalTokenException( $"Expected operand of type {types.PrintElements( )}, got token {token}." );
            }
            else
            {
                throw new MissingTokenException( types );
            }
        }

        // TODO guard against setting operand twice
        private Parser Operand2( params TokenType[] types )
        {
            if ( this.Current is Token token )
            {
                foreach ( TokenType type in types )
                {
                    if ( token.Type == type )
                    {
                        this.operand2 = token;
                        this.IncrementIndex( );
                        return this;
                    }
                }

                throw new IllegalTokenException( $"Expected operand of type {types.PrintElements( )}, got token {token}." );
            }
            else
            {
                throw new MissingTokenException( types );
            }
        }

        private Parser Operand1( )
        {
            if ( this.Current is Token token )
            {
                if ( token.Type == TokenType.Variable )
                {
                    this.operand1 = token;
                    this.IncrementIndex( );
                    return this;
                }

                throw new IllegalTokenException( $"Expected operand of type {TokenType.Variable}, got token {token}." );
            }
            else
            {
                throw new MissingTokenException( TokenType.Variable );
            }
        }

        private Parser? Optional( Keyword keyword, bool allow_illegal = true )
        {
            try
            {
                return this.Next( keyword );
            }
            catch ( MissingTokenException )
            {
                return null;
            }
            catch ( IllegalTokenException )
            {
                if ( allow_illegal || this.Current?.Type is TokenType.Comment )
                {
                    return null;
                }
                throw;
            }
        }

        private Parser? Optional( Symbol symbol, bool allow_illegal = true )
        {
            try
            {
                return this.Next( symbol );
            }
            catch ( MissingTokenException )
            {
                return null;
            }
            catch ( IllegalTokenException )
            {
                if ( allow_illegal || this.Current?.Type is TokenType.Comment )
                {
                    return null;
                }
                throw;
            }
        }

        private Parser? Optional( params TokenType[] types )
        {
            try
            {
                return this.Next( types );
            }
            catch ( MissingTokenException )
            {
                return null;
            }
            catch ( IllegalTokenException )
            {
                if ( this.Current?.Type is TokenType.Comment )
                {
                    return null;
                }
                throw;
            }
        }

        private void End( bool allow_comment = true )
        {
            if ( this.Current is not null )
            {
                if ( allow_comment && this.Current.Value.Type == TokenType.Comment )
                {
                    this.IncrementIndex( );

                    if ( this.Current is not null )
                    {
                        throw new InvalidOperationException( "Parser found token after comment." );
                    }
                }
                else
                {
                    throw new IllegalTokenException( $"Statement ended with token still remaining: {this.Current}." );
                }
            }
        }

        private static TokenType[] AllOperandTypes( )
        {
            return [TokenType.Variable, TokenType.LiteralNumber, TokenType.LiteralString, TokenType.List];
        }

        private static TokenType[] StringOperandTypes( )
        {
            return [TokenType.Variable, TokenType.LiteralString];
        }

        private static TokenType[] NumericOperandTypes( )
        {
            return [TokenType.Variable, TokenType.LiteralNumber];
        }

        private void IncrementIndex( )
        {
            this.Index += 1;

            int i = -1;
            if ( this.Current is Token token )
            {
                i = this.RawLine.IndexOf( token.Value, this.RawIndex, StringComparison.Ordinal );
                i = i >= 0 ? i : throw new InvalidOperationException( "Could not get RawIndex of Token that should exist in Line." );
            }
            this.RawIndex = i >= 0 ? i : this.RawLine.Length - 1;
        }
    }
}
