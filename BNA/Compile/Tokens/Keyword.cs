using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace BNA.Compile.Tokens
{
    public record class Keyword : Token, IEqualityOperators<Keyword, string, bool>
    {
        public static Keyword Set { get; } = new( "SET" );
        public static Keyword Add { get; } = new( "ADD" );
        public static Keyword Subtract { get; } = new( "SUBTRACT" );
        public static Keyword Multiply { get; } = new( "MULTIPLY" );
        public static Keyword Divide { get; } = new( "DIVIDE" );
        public static Keyword Wait { get; } = new( "WAIT" );
        public static Keyword Random { get; } = new( "RANDOM" );
        public static Keyword Test { get; } = new( "TEST" );
        public static Keyword Goto { get; } = new( "GOTO" );
        public static Keyword Or { get; } = new( "OR" );
        public static Keyword And { get; } = new( "AND" );
        public static Keyword Xor { get; } = new( "XOR" );
        public static Keyword Negate { get; } = new( "NEGATE" );
        public static Keyword Raise { get; } = new( "RAISE" );
        public static Keyword Mod { get; } = new( "MOD" );
        public static Keyword Log { get; } = new( "LOG" );
        public static Keyword Round { get; } = new( "ROUND" );
        public static Keyword List { get; } = new( "LIST" );
        public static Keyword Append { get; } = new( "APPEND" );
        public static Keyword Size { get; } = new( "SIZE" );
        public static Keyword Open { get; } = new( "OPEN" );
        public static Keyword Close { get; } = new( "CLOSE" );
        public static Keyword Read { get; } = new( "READ" );
        public static Keyword Write { get; } = new( "WRITE" );
        public static Keyword Input { get; } = new( "INPUT" );
        public static Keyword Print { get; } = new( "PRINT" );
        public static Keyword Type { get; } = new( "TYPE" );
        public static Keyword Exit { get; } = new( "EXIT" );
        public static Keyword Error { get; } = new( "ERROR" );
        public static Keyword Scope { get; } = new( "SCOPE" );
        public static Keyword To { get; } = new( "TO" );
        public static Keyword By { get; } = new( "BY" );
        public static Keyword From { get; } = new( "FROM" );
        public static Keyword Max { get; } = new( "MAX" );
        public static Keyword If { get; } = new( "IF" );
        public static Keyword With { get; } = new( "WITH" );
        public static Keyword Of { get; } = new( "OF" );
        public static Keyword As { get; } = new( "AS" );

        private Keyword( string raw ) : base( raw ) { }

        public static bool TryParse( string str, [NotNullWhen( true )] out Keyword? keyword )
        {
            bool isKeyword =
                Set == str ||
                Add == str ||
                Subtract == str ||
                Multiply == str ||
                Divide == str ||
                Wait == str ||
                Random == str ||
                Test == str ||
                Goto == str ||
                Or == str ||
                And == str ||
                Xor == str ||
                Negate == str ||
                Raise == str ||
                Mod == str ||
                Log == str ||
                Round == str ||
                List == str ||
                Append == str ||
                Size == str ||
                Open == str ||
                Close == str ||
                Read == str ||
                Write == str ||
                Input == str ||
                Print == str ||
                Type == str ||
                Exit == str ||
                Error == str;

            keyword = isKeyword ? new( str ) : null;

            return keyword is not null;
        }

        protected override string TypeString( ) => "Keyword";

        public static bool operator ==( Keyword? left, string? right )
        {
            if ( left is null )
            {
                return right is null;
            }
            else
            {
                return left.Raw.Equals( right, StringComparison.OrdinalIgnoreCase );
            }
        }

        public static bool operator !=( Keyword? left, string? right ) => !(left == right);
    }
}
