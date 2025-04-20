using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BNA.Compile.Tokens;
using BNA.Exceptions;

namespace BNA.Common
{
    public interface IOperand<T>
        where T : IOperand<T>
    {
        public abstract static IReadOnlyCollection<Type> Types { get; }

        public abstract static T From( Token token );

        public bool IsValid( );
    }

    public abstract class Operand
    {
        public Token Token { get; }

        protected Operand( Token token ) { this.Token = token; }
    }

    public class AnyOperand : Operand, IOperand<AnyOperand>
    {
        public static IReadOnlyCollection<Type> Types =>
            [
                typeof( Identifier ),
                typeof( LiteralString ),
                typeof( LiteralInteger ),
                typeof( LiteralReal )
            ];

        public Identifier? AsIdentifier => this.Token as Identifier;
        public LiteralString? AsLiteralString => this.Token as LiteralString;
        public LiteralInteger? AsLiteralInteger => this.Token as LiteralInteger;
        public LiteralReal? AsLiteralReal => this.Token as LiteralReal;

        private AnyOperand( Token token ) : base( token ) { }

        public bool IsValid( )
            => this.AsIdentifier is not null ||
               this.AsLiteralString is not null ||
               this.AsLiteralInteger is not null ||
               this.AsLiteralReal is not null;

        public static AnyOperand From( Token token ) => new( token );
    }

    public class StringOperand : Operand, IOperand<StringOperand>
    {
        public static IReadOnlyCollection<Type> Types =>
            [
                typeof( Identifier ),
                typeof( LiteralString ),
            ];

        public Identifier? AsIdentifier => this.Token as Identifier;
        public LiteralString? AsLiteralString => this.Token as LiteralString;

        private StringOperand( Token token ) : base( token ) { }

        public bool IsValid( )
            => this.AsIdentifier is not null ||
               this.AsLiteralString is not null;

        public static StringOperand From( Token token ) => new( token );
    }

    public class NumberOperand : Operand, IOperand<NumberOperand>
    {
        public static IReadOnlyCollection<Type> Types =>
            [
                typeof( Identifier ),
                typeof( LiteralInteger ),
                typeof( LiteralReal )
            ];

        public Identifier? AsIdentifier => this.Token as Identifier;
        public LiteralInteger? AsLiteralInteger => this.Token as LiteralInteger;
        public LiteralReal? AsLiteralReal => this.Token as LiteralReal;

        private NumberOperand( Token token ) : base( token ) { }

        public bool IsValid( )
            => this.AsIdentifier is not null ||
               this.AsLiteralInteger is not null ||
               this.AsLiteralReal is not null;

        public static NumberOperand From( Token token ) => new( token );
    }

    public class IdentifierOperand : Operand, IOperand<IdentifierOperand>
    {
        public static IReadOnlyCollection<Type> Types =>
            [
                typeof( Identifier )
            ];

        public Identifier? AsIdentifier => this.Token as Identifier;

        private IdentifierOperand( Token token ) : base( token ) { }

        public bool IsValid( ) => this.AsIdentifier is not null;
        public static IdentifierOperand From( Token token ) => new( token );
    }

    public class TokenHandler
    {
        private readonly Token[] _tokens;
        private int _index;

        private Token? Current => this._index < this._tokens.Length ? this._tokens[this._index] : null;
        private Token? Next => this._index + 1 < this._tokens.Length ? this._tokens[this._index + 1] : null;

        public TokenHandler( IEnumerable<Token> tokens )
        {
            ArgumentNullException.ThrowIfNull( tokens );
            this._tokens = [.. tokens];
        }

        public int GetCurrentColumn( )
            => this._tokens.Take( this._index ).Sum( token => token.Raw.Length ) + 1;

        public int GetPreviousColumn( )
            => this._tokens.Take( this._index - 1 ).Sum( token => token.Raw.Length ) + 1;

        [MemberNotNullWhen( true, nameof( Current ) )]
        private bool MoveNext( )
        {
            if ( this._index < this._tokens.Length )
            {
                this._index += 1;
            }

            return this.Current is not null;
        }

        private void CheckWhitespace( bool? whitespace = null )
        {
            if ( whitespace is true )
            {
                if ( this.MoveNext( ) )
                {
                    if ( this.Current is not WhiteSpace )
                    {
                        throw new UnexpectedTokenException( this.Current, this.GetCurrentColumn( ), typeof( WhiteSpace ) );
                    }
                }
                else
                {
                    throw new MissingTokenException( typeof( WhiteSpace ) );
                }
            }
            else
            {
                if ( this.Next is WhiteSpace )
                {
                    _ = this.MoveNext( );
                    if ( whitespace is false )
                    {
                        throw new UnexpectedTokenException( this.Current!, this.GetCurrentColumn( ), typeof( WhiteSpace ) );
                    }
                }
            }
        }

        public void End( bool allowWhitespace = true, bool allowComment = true )
        {
            this.CheckWhitespace( allowWhitespace ? null : false );

            if ( this.MoveNext( ) )
            {
                if ( !allowComment || this.Current is not Comment )
                {
                    throw new UnexpectedTokenException( this.Current, this.GetCurrentColumn( ), typeof( Comment ) );
                }
            }
        }

        public TokenHandler Allow<T>( )
        {
            if ( this.Next is T )
            {
                _ = this.MoveNext( );
            }

            return this;
        }

        /// <summary>
        /// Get the next token if it is of the expected type.
        /// </summary>
        /// <remarks>
        /// The <paramref name="whitespaceBefore"/> parameter is used to check if a whitespace
        /// token is allowed before the token. For most things, this can be null because the
        /// Lexer will have parsed things differently if whitespace was missing.
        /// <br/>
        /// For example <c>ADD2</c> would be parsed as an identifier instead of a keyword then
        /// a number with no whitespace in between.
        /// </remarks>
        /// <typeparam name="T">Expected token type.</typeparam>
        /// <param name="token">Returned token.</param>
        /// <param name="whitespaceBefore">If a whitespace token is allowed before the token.</param>
        /// <returns></returns>
        /// <exception cref="UnexpectedTokenException"></exception>
        /// <exception cref="MissingTokenException"></exception>
        public TokenHandler Get<T>( out T token, bool? whitespaceBefore = null )
            where T : Token
        {
            this.CheckWhitespace( whitespaceBefore );

            if ( this.MoveNext( ) )
            {
                if ( this.Current is T t )
                {
                    token = t;
                    return this;
                }
                else
                {
                    throw new UnexpectedTokenException( this.Current, this.GetCurrentColumn( ), typeof( T ) );
                }
            }
            else
            {
                throw new MissingTokenException( typeof( T ) );
            }
        }

        public TokenHandler Get<T>( T token, bool? whitespaceBefore = null )
            where T : Token
        {
            _ = this.Get( out T t, whitespaceBefore );

            if ( t != token )
            {
                throw new UnexpectedTokenException( t, this.GetCurrentColumn( ), token );
            }

            return this;
        }

        public bool TryGet<T>( T token, bool? whitespaceBefore = null, bool allowComment = true )
            where T : Token
        {
            this.CheckWhitespace( whitespaceBefore );

            if ( this.Next is Token t )
            {
                if ( t == token )
                {
                    _ = this.MoveNext( );
                    return true;
                }
                else if ( allowComment && t is Comment )
                {
                    // This also means the statement is over
                    _ = this.MoveNext( );
                    return false;
                }
                else
                {
                    _ = this.MoveNext( );
                    throw new UnexpectedTokenException( t, this.GetCurrentColumn( ), token );
                }
            }

            _ = this.MoveNext( );
            return false;
        }

        /// <summary>
        /// Get the next token if it is of the expected type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operand"></param>
        /// <param name="defaultToken">
        /// Default token to try if there is no next token.
        /// This parameter mostly exists to allow reusing the Get name for this method.
        /// </param>
        /// <param name="whitespaceBefore"></param>
        /// <returns></returns>
        /// <exception cref="UnexpectedTokenException"></exception>
        /// <exception cref="MissingTokenException"></exception>
        public TokenHandler Get<T>( out T operand, Token? defaultToken = null, bool? whitespaceBefore = null )
            where T : Operand, IOperand<T>
        {
            _ = this.Get( out Token token, whitespaceBefore );
            if ( token is not null || defaultToken is not null )
            {
                Token tok = (token ?? defaultToken)!;
                if ( T.From( tok ) is T t && t.IsValid( ) )
                {
                    operand = t;
                    return this;
                }
                else
                {
                    throw new UnexpectedTokenException( tok, this.GetCurrentColumn( ), [.. T.Types] );
                }
            }
            else
            {
                throw new MissingTokenException( [.. T.Types] );
            }
        }

        /// <summary>
        /// Get a value based on a mapping of tokens to values.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TToken"></typeparam>
        /// <param name="value"></param>
        /// <param name="mapping"></param>
        /// <param name="whitespaceBefore"></param>
        /// <returns></returns>
        /// <exception cref="UnexpectedTokenException"></exception>
        /// <exception cref="MissingTokenException"></exception>
        public TokenHandler Get<TValue, TToken>( out TValue? value,
                                                 IReadOnlyDictionary<TToken, TValue> mapping,
                                                 bool? whitespaceBefore = null )
            where TToken : Token
        {
            ArgumentNullException.ThrowIfNull( mapping );

            this.CheckWhitespace( whitespaceBefore );

            if ( this.MoveNext( ) )
            {
                if ( this.Current is not TToken token || !mapping.TryGetValue( token, out value ) )
                {
                    throw new UnexpectedTokenException( this.Current, this.GetCurrentColumn( ), mapping.Keys.ToArray( ) );
                }
            }
            else
            {
                throw new MissingTokenException( mapping.Keys.ToArray( ) );
            }

            return this;
        }
    }
}
