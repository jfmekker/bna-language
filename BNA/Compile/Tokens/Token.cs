namespace BNA.Compile.Tokens
{
    public abstract record class Token
    {
        public string Raw { get; }

        protected Token( string raw )
        {
            this.Raw = raw;
        }

        protected abstract string TypeString( );

        public override string ToString( )
        {
            return $"<({this.TypeString( )}):{this.Raw}>";
        }
    }
}
