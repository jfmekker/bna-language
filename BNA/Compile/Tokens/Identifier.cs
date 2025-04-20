namespace BNA.Compile.Tokens
{
    public record class Identifier : Token
    {
        public Identifier( string raw )
            : base( raw )
        {
        }

        protected override string TypeString( ) => "Identifier";
    }
}
