using BNA.Common;
using BNA.Compile.Statments;
using BNA.Compile.Tokens;
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

        private IStatementParser[] StatementParsers { get; }

        public Parser( string line, ICollection<Token> tokens )
        {
            this.RawLine = line;
            this.Tokens = [.. tokens];
            this.Index = 0;

            this.StatementParsers =
                [
                    // Generic
                    SetStatement.Parser,
                    EmptyStatement.Parser,
                    TypeStatement.Parser,
                    WaitStatement.Parser,
                    // Control
                    TestStatement.Parser,
                    JumpStatement.Parser,
                    ScopeStatement.Parser,
                    // List
                    ListStatement.Parser,
                    AppendStatement.Parser,
                    SizeStatement.Parser,
                    // Math
                    AddStatement.Parser,
                    SubtractStatement.Parser,
                    MultiplyStatement.Parser,
                    DivideStatement.Parser,
                    ExponentStatement.Parser,
                    LogarithmStatement.Parser,
                    ModulusStatement.Parser,
                    RoundStatement.Parser,
                    RandomStatement.Parser,
                    // IO
                    PrintStatement.Parser,
                    InputStatement.Parser,
                    OpenStatement.Parser,
                    CloseStatement.Parser,
                    ReadStatement.Parser,
                    WriteStatement.Parser,
                    // Exit
                    ErrorStatement.Parser,
                    ExitStatement.Parser,
                ];
        }

        public Statement? ParseStatement( )
        {
            foreach ( IStatementParser parser in this.StatementParsers )
            {
                if ( parser.IsStartToken( this.Tokens[0] ) )
                {
                    return parser.Parse( this.Tokens, this.RawLine );
                }
            }

            throw new UnexpectedTokenException( this.Tokens[0] );
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
