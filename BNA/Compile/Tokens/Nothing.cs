using System.Diagnostics.CodeAnalysis;

namespace BNA.Compile.Tokens
{
    [SuppressMessage( "Naming", "CA1716:Identifiers should not match keywords", Justification = "Nothing is not a keyword in C#" )]
    public record class Nothing : Token
    {
        //public static Nothing Token {get; } = new( );

        public Nothing( ) : base( string.Empty ) { }

        protected override string TypeString( ) => "Nothing";
    }
}
