using BNA.Common;
using BNA.Compile;
using BNA.Values;
using System;
using System.Text;

namespace BNA.Exceptions
{
    /// <summary>
    /// A BNA exception encountered at runtime.
    /// </summary>
    public class RuntimeException : LanguageException
    {
        /// <summary>
        /// The <see cref="Statement"/> that was compiled for the line.
        /// </summary>
        public Statement CompiledStatement { get; }

        /// <summary>
        /// Create a new <see cref="RuntimeException"/> instance by wrapping
        /// the thrown <see cref="Exception"/>.
        /// </summary>
        /// <remarks>
        /// This should be the outermost exception.
        /// </remarks>
        /// <param name="line">Line number.</param>
        /// <param name="statement">Compiled statement.</param>
        /// <param name="innerException">Exception that was thrown.</param>
        public RuntimeException( int line, Statement statement, string message, RuntimeException? innerException )
            : base( line, statement.Line, message, innerException )
        {
            this.CompiledStatement = statement;
        }

        protected RuntimeException( string message ) : base( message ) { }

        public override string GetFullMessage( )
        {
            // If this is the innermost exception, then we don't have the full info yet
            if ( this.InnerException == null )
            {
                return this.Message;
            }

            string linePre = "Runtime Error - line ";
            string lineNum = this.Line.ToString( );
            string lineSuf = ": ";

            StringBuilder sb = new( );

            // First line is the actual code line
            _ = sb.Append( linePre )
                  .Append( lineNum )
                  .Append( lineSuf )
                  .Append( this.Text )
                  .AppendLine( );

            // Second line is the compiled statement
            _ = sb.Append( "    ---> " )
                  .Append( this.CompiledStatement );

            // Third line is the message
            _ = sb.Append( "    Error: " )
                  .Append( this.Message );

            return sb.ToString( );
        }
    }

    /// <summary>
    /// Exception thrown when an undefined operation is attempted.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <c>APPEND 1 TO y</c>
    /// where y is a numeric type value.
    /// </remarks>
    public class UndefinedOperationException : RuntimeException
    {
        public string Operation { get; }

        public Value Operand1 { get; }

        public Value? Operand2 { get; }

        public UndefinedOperationException( Value operand1, string operation, Value? operand2 = null )
            : base( $"Undefined operation: {operand1?.TypeString( )} {operation} {operand2?.TypeString( ) ?? string.Empty}" )
        {
            ArgumentNullException.ThrowIfNull( operand1 );

            this.Operation = operation;
            this.Operand1 = operand1;
            this.Operand2 = operand2;
        }
    }

    /// <summary>
    /// Exception thrown when an operation is given an incorrect type operand.
    /// </summary>
    public class IncorrectOperandTypeException : RuntimeException
    {
        public Token Token { get; }

        public Value Value { get; }

        public Operation Statement { get; }

        public IncorrectOperandTypeException( Operation statement, Token token, Value value )
            : base( $"Incorrect operand type for {statement} statement: {token} = {value?.TypeString( )} '({value})'" )
        {
            ArgumentNullException.ThrowIfNull( value );

            this.Statement = statement;
            this.Token = token;
            this.Value = value;
        }
    }

    /// <summary>
    /// Exception thrown when the index to an accesed variable is not
    /// an integer value.
    /// </summary>
    public class InvalidIndexValueException : RuntimeException
    {
        public Token Token { get; }

        public Value Value { get; }

        public InvalidIndexValueException( Token token, Value value )
            : base( $"Invalid index ({token} = {value}), index must be integer." )
        {
            this.Token = token;
            this.Value = value;
        }
    }

    /// <summary>
    /// Exception thrown when a value is out of range for a certain context.
    /// </summary>
    public class ValueOutOfRangeException : RuntimeException
    {
        public Value Index { get; }

        public string Reason { get; }

        public ValueOutOfRangeException( Value index, string reason )
            : base( $"Value ({index}) out of range for {reason}." )
        {
            this.Index = index;
            this.Reason = reason;
        }
    }

    /// <summary>
    /// Exception thrown when a value is accesed that is not a string or list.
    /// </summary>
    public class NonIndexableValueException : RuntimeException
    {
        public Token Token { get; }

        public Value Value { get; }

        public NonIndexableValueException( Token token, Value value )
            : base( $"Cannot access index of non-indexable value ({token} = {value})" )
        {
            this.Token = token;
            this.Value = value;
        }
    }

    /// <summary>
    /// Exception thrown when a variables value is retrieved when it does not
    /// exist in the current scope.
    /// </summary>
    public class NonExistantVariableException : RuntimeException
    {
        public Token Token { get; }

        public NonExistantVariableException( Token token )
            : base( $"Variable '{token}' does not exist in current scope." )
        {
            this.Token = token;
        }
    }

    /// <summary>
    /// Exception thrown when the final / root scope is attempted to be closed.
    /// </summary>
    public class CannnotCloseFinalScopeException : RuntimeException
    {
        public CannnotCloseFinalScopeException( ) : base( "Cannot close the final scope." ) { }
    }

    /// <summary>
    /// Exception thrown by the BNA <see cref="Operation.Error"/> statement.
    /// </summary>
    public class ErrorStatementException : RuntimeException
    {
        public string StatementMessage { get; }

        public ErrorStatementException( Value value ) : base( $"ERROR {value}" )
        {
            ArgumentNullException.ThrowIfNull( value );
            this.StatementMessage = value.ToString( );
        }
    }
}
