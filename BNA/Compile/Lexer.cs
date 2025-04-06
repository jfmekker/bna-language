using BNA.Common;
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
            while ( this.Current is not null )
            {
                if ( this.Current is ' ' or '\t' )
                {
                    // Ignore whitespace
                    this.Index += 1;
                }
                else
                {
                    return this.Current.IsDigit( ) || this.Current is '-' or '.' ? this.NextLiteral( )
                        : this.Current.IsLetter( ) || this.Current is '_' ? this.NextVariableOrKeyword( )
                        : this.Current is (char)Symbol.StringDelim ? this.NextString( )
                        : this.Current is (char)Symbol.ListStart ? this.NextList( )
                        : this.Current is (char)Symbol.Comment ? this.NextComment( )
                        : this.NextSymbol( );
                }
            }

            return null;
        }

        private Token NextLiteral( )
        {
            StringBuilder builder = new( $"{this.ConsumeCurrent}" );

            while ( this.Current is not null )
            {
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
            return long.TryParse( str, out long _ ) || double.TryParse( str, out double _ )
                ? new Token( str, TokenType.LiteralNumber )
                : throw new InvalidTokenException( "Literal is not parsable as number." );
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

            return new Token( str, Enum.TryParse( str, out Keyword _ ) ? TokenType.Keyword : TokenType.Variable );
        }

        private Token NextString( )
        {
            StringBuilder builder = new( $"{this.ConsumeCurrent}" );

            bool string_ended = false;
            while ( this.Current is not null )
            {
                _ = builder.Append( this.ConsumeCurrent );

                if ( builder.ToString( )[^1] == (char)Symbol.StringDelim &&
                    builder.ToString( )[^2] != (char)Symbol.Escape )
                {
                    string_ended = true;
                    break;
                }
            }

            return string_ended ? new Token( builder.ToString( ), TokenType.LiteralString )
                : throw new MissingTerminatorException( "String", (char)Symbol.StringDelim );
        }

        private Token NextList( )
        {
            int start_index = this.Index;
            List<Token> list = [new Token( $"{this.ConsumeCurrent}", TokenType.Symbol )];

            while ( this.Current is not null )
            {
                if ( this.NextToken( ) is Token token )
                {
                    if ( token.AsSymbol( ) is Symbol.ListEnd )
                    {
                        list.Add( token );
                        break;
                    }
                    else if ( token.AsSymbol( ) is Symbol.ListSeparator )
                    {
                        list.Add( token );
                    }
                    else if ( token.Type is TokenType.LiteralNumber or TokenType.Variable or TokenType.LiteralString or TokenType.List )
                    {
                        if ( list[^1].AsSymbol( ) is Symbol.ListSeparator or Symbol.ListStart )
                        {
                            list.Add( token );
                        }
                        else
                        {
                            throw new IllegalTokenException( $"Tokens in lists must be separated by '{Symbol.ListSeparator}'." );
                        }
                    }
                    else
                    {
                        throw new IllegalTokenException( $"Tokens of type '{token.Type}' not allowed in lists." );
                    }
                }
            }

            return this.Line[this.Index - 1] == (char)Symbol.ListEnd
                ? new Token( this.Line[start_index..this.Index], TokenType.List )
                : throw new MissingTerminatorException( "List", (char)Symbol.ListEnd );
        }

        private Token NextComment( )
        {
            StringBuilder builder = new( );

            while ( this.Current is not null )
            {
                _ = builder.Append( this.ConsumeCurrent );
            }

            return new Token( builder.ToString( ), TokenType.Comment );
        }

        private Token NextSymbol( )
        {
            return this.Current is (char)Symbol.LessThan
                                or (char)Symbol.GreaterThan
                                or (char)Symbol.Equal
                                or (char)Symbol.Not
                                or (char)Symbol.ListSeparator
                                or (char)Symbol.ListEnd
                                or (char)Symbol.LabelStart
                                or (char)Symbol.LabelEnd
                ? new Token( $"{this.ConsumeCurrent}", TokenType.Symbol )
                : throw new UnexpectedSymbolException( this.Current );
        }
    }
}
