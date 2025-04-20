using BNA.Compile.Tokens;
using BNA.Exceptions;
using BNA.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNA.Compile
{
    public class Lexer
    {
        public string Line { get; private init; }

        public int Index { get; private set; }

        public char? Current => this.Index < this.Line.Length ? this.Line[this.Index] : null;

        public char? Previous => this.Index + 1 < this.Line.Length ? this.Line[this.Index + 1] : null;

        public char? Next => this.Index - 1 > 0 ? this.Line[this.Index - 1] : null;

        private char ConsumeCurrent => this.Line[this.Index++];

        public static Token ReadSingleToken( string value )
        {
            Lexer lexer = new( value );
            IReadOnlyCollection<Token> tokens = lexer.ReadTokens( );

            return tokens.Count == 1 ? tokens.ElementAt( 0 )
                : throw new IllegalTokenException( $"One token expected but {tokens.Count} were parsed." );
        }

        public static Token ReadFirstToken( string line )
        {
            Lexer lexer = new( line );
            IReadOnlyCollection<Token> tokens = lexer.ReadTokens( );

            return tokens.FirstOrDefault( )
                ?? throw new IllegalTokenException( $"At least one token expected but none were parsed." );
        }

        public Lexer( string line )
        {
            this.Line = line;
            this.Index = 0;
        }

        public IReadOnlyCollection<Token> ReadTokens( )
        {
            List<Token> tokens = [];
            while ( this.Current is not null )
            {
                if ( this.NextToken( ) is Token token )
                {
                    tokens.Add( token );
                }
            }
            return tokens;
        }

        public Token? NextToken( )
        {
            if ( this.Current is null )
            {
                return null;
            }
            else if ( this.Current is ' ' or '\t' )
            {
                return this.NextWhiteSpace( );
            }
            else if ( this.Current.IsDigit( ) || this.Current is '-' or '.' )
            {
                return this.NextLiteral( );
            }
            else if ( this.Current.IsLetter( ) || this.Current is '_' )
            {
                return this.NextVariableOrKeyword( );
            }
            else if ( Symbol.StringDelimiter.Matches( this.Current ) )
            {
                return this.NextString( );
            }
            else if ( Symbol.ListStart.Matches( this.Current ) )
            {
                return this.NextList( );
            }
            else if ( Symbol.Comment.Matches( this.Current ) )
            {
                return this.NextComment( );
            }
            else
            {
                return this.NextSymbol( );
            }
        }

        private WhiteSpace NextWhiteSpace( )
        {
            StringBuilder space = new( this.ConsumeCurrent );
            while ( this.Current is not null )
            {
                if ( this.Current is ' ' or '\t' )
                {
                    _ = space.Append( this.ConsumeCurrent );
                }
                else
                {
                    break;
                }
            }

            return new WhiteSpace( space.ToString( ) );
        }

        private Literal NextLiteral( )
        {
            StringBuilder builder = new( $"{this.ConsumeCurrent}" );

            while ( this.Current is not null )
            {
                // TODO allow more number styles
                if ( this.Current.IsLetterOrDigit( ) || this.Current is '.' or '+' or '-' )
                {
                    _ = builder.Append( this.ConsumeCurrent );
                }
                else
                {
                    break;
                }
            }

            string str = builder.ToString( );
            if ( long.TryParse( str, out long l ) )
            {
                return new LiteralInteger( l, str );
            }
            else if ( double.TryParse( str, out double d ) )
            {
                return new LiteralReal( d, str );
            }
            else
            {
                throw new InvalidTokenException( $"Literal is not parsable as number: {str}" );
            }
        }

        private Token NextVariableOrKeyword( )
        {
            StringBuilder builder = new( $"{this.ConsumeCurrent}" );

            while ( this.Current is not null )
            {
                if ( !this.Current.IsLetterOrDigit( ) && this.Current is not '_' and not (char)Symbol.Accessor )
                {
                    break;
                }

                _ = builder.Append( this.ConsumeCurrent );
            }

            string str = builder.ToString( );
            for ( int i = 0 ; i < str.Length ; i += 1 )
            {
                if ( str[i] == (char)Symbol.Accessor &&
                    (i + 1 >= str.Length || i - 1 < 0
                    || !(char.IsLetterOrDigit( str[i + 1] ) || str[i + 1] == '_')
                    || !(char.IsLetterOrDigit( str[i - 1] ) || str[i - 1] == '_')) )
                {
                    throw new InvalidTokenException( $"Accessors must have a letter, digit, or underscore on either side: {str}" );
                }
            }

            if ( Keyword.TryParse( str, out Keyword? keyword ) )
            {
                return keyword;
            }
            else
            {
                return new Identifier( str );
            }
        }

        private LiteralString NextString( )
        {
            StringBuilder rawBuilder = new( $"{this.ConsumeCurrent}" );
            StringBuilder valBuilder = new( );

            bool escapeNext = false;
            bool stringEnded = false;
            while ( this.Current is not null && !stringEnded )
            {
                // TODO fix symbol handling
                if ( escapeNext )
                {
                    escapeNext = false;

                    char val = this.Current switch {
                        (char)Symbol.StringDelim => (char)Symbol.StringDelim,
                        (char)Symbol.Escape => (char)Symbol.Escape,
                        't' => '\t',
                        'n' => '\n',
                        _ => throw new InvalidTokenException( $"Invalid escape sequence: {(char)Symbol.Escape}{this.Current}" )
                    };

                    _ = valBuilder.Append( val );
                }
                else if ( this.Current == Symbol.Escape.Raw[0] )
                {
                    escapeNext = true;
                }
                else if ( this.Current == Symbol.StringDelimiter.Raw[0] )
                {
                    stringEnded = true;
                }
                else
                {
                    _ = valBuilder.Append( this.Current );
                }

                _ = rawBuilder.Append( this.ConsumeCurrent );
            }

            if ( !stringEnded )
            {
                throw new MissingTokenException( Symbol.StringDelimiter );
            }

            return new LiteralString( valBuilder.ToString( ), rawBuilder.ToString( ) );
        }

        private LiteralList NextList( )
        {
            int startIndex = this.Index;
            _ = this.ConsumeCurrent; // Consume the list start symbol
            List<Token> list = [Symbol.ListStart];

            bool listDone = false;
            while ( this.Current is not null && !listDone )
            {
                // TODO refactor this
                switch ( this.NextToken( ) )
                {
                    case Symbol symTok:
                        if ( symTok == Symbol.ListEnd || symTok == Symbol.ListSeparator )
                        {
                            listDone = symTok == Symbol.ListEnd;
                            if ( list[^1] is Symbol lastSymTok && lastSymTok == Symbol.ListSeparator )
                            {
                                list.Add( new Nothing( ) );
                            }
                            list.Add( symTok );
                        }
                        else
                        {
                            throw new UnexpectedTokenException( symTok, this.Index, Symbol.ListSeparator, Symbol.ListEnd );
                        }
                        break;
                    case Literal litTok:
                        list.Add( litTok );
                        break;
                    case Identifier varTok:
                        list.Add( varTok );
                        break;
                    case WhiteSpace wsTok:
                        list.Add( wsTok );
                        break;
                    case Token tok:
                        throw new UnexpectedTokenException( tok, this.Index, typeof( Symbol ), typeof( Literal ), typeof( Identifier ), typeof( WhiteSpace ) );
                    case null:
                        throw new MissingTokenException( Symbol.ListEnd );
                    default:
                        throw new InvalidOperationException( "Unhandled token type in Lexer" );
                }
            }

            return new LiteralList( list, this.Line[startIndex..this.Index] );
        }

        private Comment NextComment( )
        {
            StringBuilder builder = new( );

            while ( this.Current is not null )
            {
                _ = builder.Append( this.ConsumeCurrent );
            }

            return new Comment( builder.ToString( ) );
        }

        private Symbol NextSymbol( )
        {
            char s = this.Current ?? throw new InvalidOperationException( "Current should not have been null" );
            if ( Enum.IsDefined( (Symbol)s ) )
            {
                return new SymbolToken( (Symbol)this.ConsumeCurrent );
            }
            else
            {
                throw new UnexpectedSymbolException( s );
            }
        }
    }
}
