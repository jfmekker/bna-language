namespace BNA.Compile.Tokens
{
    public record class Comment : Token
    {
        public Comment( string raw )
            : base( raw )
        {
        }

        protected override string TypeString( ) => "Comment";
    }
}
