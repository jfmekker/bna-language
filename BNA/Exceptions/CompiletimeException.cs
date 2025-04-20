using BNA.Compile.Statments;
using BNA.Compile.Tokens;
using BNA.Utils;
using System;
using System.Linq;
using System.Text;

namespace BNA.Exceptions
{
    /// <summary>
    /// A BNA exception encountered at compile time.
    /// </summary>
    public class CompiletimeException : LanguageException
    {
        /// <summary>
        /// Character column of the <see langword="char"/> or <see cref="Token"/>
        /// that caused the <see cref="Exception"/>.
        /// </summary>
        public int Column { get; protected init; }

        /// <summary>
        /// Create a new <see cref="CompiletimeException"/> instance by
        /// wrapping the thrown <see cref="CompiletimeException"/>.
        /// </summary>
        /// <remarks>
        /// This should be the outermost exception.
        /// </remarks>
        /// <param name="line">Line number.</param>
        /// <param name="column">Character column.</param>
        /// <param name="text">String value of the line.</param>
        /// <param name="innerException">CompiletimeException to wrap.</param>
        public CompiletimeException( int line, int column, string text, string message, CompiletimeException? innerException )
            : base( line, text, message, innerException )
        {
            this.Column = column;
        }

        protected CompiletimeException( string message ) : base( message ) { }

        public override string GetFullMessage( )
        {
            // If this is the innermost exception, then we don't have the full info yet
            if ( this.InnerException == null )
            {
                return this.Message;
            }

            string linePre = "Compiletime Error - line ";
            string lineNum = this.Line.ToString( );
            string lineSuf = ": ";

            StringBuilder sb = new( );

            // First line is the actual code line
            _ = sb.Append( linePre )
                  .Append( lineNum )
                  .Append( lineSuf )
                  .Append( this.Text )
                  .AppendLine( );

            // TODO check if we need the -1 below
            // Second line is just the caret pointing to the character
            int numSpaces = linePre.Length + lineNum.Length + lineSuf.Length + (this.Column - 1);
            _ = sb.AppendRepeated( ' ', numSpaces )
                  .Append( '^' )
                  .AppendLine( );

            // Third line is the message
            _ = sb.Append( "    Error: " )
                  .Append( this.Message );

            return sb.ToString( );
        }
    }

    /// <summary>
    /// Exception thrown when a <see cref="Token"/> is expected but none found.
    /// </summary>
    public class MissingTokenException : CompiletimeException
    {
        public MissingTokenException( params Token[] expected )
            : base( $"Missing token, expected '{string.Join( "' or '", expected.AsEnumerable( ) )}'." )
        {
        }

        public MissingTokenException( params Type[] expectedTypes )
            : base( $"Missing token, expected {string.Join( " or ", expectedTypes.Select( t => t.Name ) )}." )
        {
        }
    }

    public class UnexpectedTokenException : CompiletimeException
    {
        public Token Token { get; }

        public UnexpectedTokenException( Token token )
            : base( $"Token '{token}' is not a valid start to any statement." )
        {
            this.Token = token;
            this.Column = 1;
        }

        public UnexpectedTokenException( Token token, int column, params Token[] expected )
            : base( $"Unexpected token '{token}', expected '{string.Join( "' or '", expected.AsEnumerable( ) )}'." )
        {
            this.Token = token;
            this.Column = column;
        }

        public UnexpectedTokenException( Token token, int column, params Type[] expectedTypes )
            : base( $"Unexpected token '{token}', expected {string.Join( " or ", expectedTypes.Select( t => t.Name ) )}." )
        {
            this.Token = token;
            this.Column = column;
        }
    }

    /// <summary>
    /// Exception thrown when a <see cref="Symbol"/> was found in an unexpected
    /// place, or did not match the expected symbol.
    /// </summary>
    public class UnexpectedSymbolException : CompiletimeException
    {
        public UnexpectedSymbolException( char? symbol )
            : base( $"Unexpected symbol: '{symbol.NullableString( )}'" )
        {
        }
    }

    /// <summary>
    /// Exception thrown when a list or string <see cref="Token"/> is being
    /// parsed but the closing terminator (<see cref="Symbol.ListEnd"/> or
    /// <see cref="Symbol.StringDelim"/>) was not found.
    /// </summary>
    public class MissingTerminatorException : CompiletimeException
    {
        public MissingTerminatorException( string thing, char terminator )
            : base( $"{thing} missing '{terminator}' terminator before end of line." )
        {
        }
    }

    /// <summary>
    /// Exception thrown when a <see cref="Token"/> of an incorrect/unexpected
    /// type was found when parsing a <see cref="Statement"/>.
    /// </summary>
    public class IllegalTokenException : CompiletimeException
    {
        public IllegalTokenException( string message )
            : base( message )
        {
        }
    }

    /// <summary>
    /// Exception thrown when a <see cref="Token"/> is parsed but is not valid,
    /// like an invalid number ("0.1.234") or variable with accessor ("x@").
    /// </summary>
    public class InvalidTokenException : CompiletimeException
    {
        public InvalidTokenException( string message )
            : base( message )
        {
        }
    }

}
