using System;

namespace BNA.Exceptions
{
    /// <summary>
    /// Standard base class for all BNA exceptions.
    /// </summary>
    /// <remarks>
    /// This should only be used in the context of an issue with a BNA program, either at compiletime or runtime.<br/>
    /// It should not be used in the context of an issue with BNA itself (like an unexpected situation).
    /// </remarks>
    public abstract class LanguageException : Exception
    {
        /// <summary>
        /// The line number of the line that caused the exception.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The actual raw text of the line that caused the exception.
        /// </summary>
        public string Text { get; } = string.Empty;

        /// <summary>
        /// Create a new <see cref="LanguageException"/> instance.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="text"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        protected LanguageException( int line, string text, string message, Exception? innerException )
            : base( message, innerException )
        {
            this.Line = line;
            this.Text = text;
        }

        /// <summary>
        /// Create a new <see cref="LanguageException"/> instance.
        /// This constructor should only be used for the inner most exception.
        /// </summary>
        /// <param name="message"></param>
        protected LanguageException( string message ) : base( message ) { }

        /// <summary>
        /// Get the fully constructed message of the exception. This is separate from <see cref="Exception.Message"/>
        /// because this may need some extra processing.
        /// </summary>
        /// <remarks>
        /// This base implementation returns the string below, but derived classes should override this.
        /// <code>
        ///     $"line {this.Line}: {this.Text}\n{this.Message}"
        /// </code>
        /// </remarks>
        public virtual string GetFullMessage( ) => $"line {this.Line}: {this.Text}\n{this.Message}";
    }
}
