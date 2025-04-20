using System.Numerics;

namespace BNA.Compile.Tokens
{
    public record class Symbol :
        Token,
        IEqualityOperators<Symbol, string, bool>,
        IEqualityOperators<Symbol, char, bool>
    {
        public static Symbol Escape { get; } = new( '\\' );
        public static Symbol Comment { get; } = new( '#' );
        public static Symbol GreaterThan { get; } = new( '>' );
        public static Symbol LessThan { get; } = new( '<' );
        public static Symbol Equal { get; } = new( '=' );
        public static Symbol Not { get; } = new( '!' );
        public static Symbol LabelStart { get; } = new( '^' );
        public static Symbol LabelEnd { get; } = new( ':' );
        public static Symbol StringDelimiter { get; } = new( '"' );
        public static Symbol Accessor { get; } = new( '@' );
        public static Symbol ListStart { get; } = new( '(' );
        public static Symbol ListEnd { get; } = new( ')' );
        public static Symbol ListSeparator { get; } = new( ',' );

        public char AsChar { get; }

        private Symbol( char c ) : base( $"{c}" )
        {
            this.AsChar = c;
        }

        protected override string TypeString( ) => "Symbol";

        public static Symbol? From( char c )
        {
            return c switch {
                '\\' => Escape,
                '#' => Comment,
                '>' => GreaterThan,
                '<' => LessThan,
                '=' => Equal,
                '!' => Not,
                '^' => LabelStart,
                ':' => LabelEnd,
                '"' => StringDelimiter,
                '@' => Accessor,
                '(' => ListStart,
                ')' => ListEnd,
                ',' => ListSeparator,
                _ => null,
            };
        }

        public static bool operator ==( Symbol? left, string? right ) => right?.Length is 1 && left == right[0];
        public static bool operator !=( Symbol? left, string? right ) => !(left == right);
        public static bool operator ==( Symbol? left, char right ) => left?.AsChar == right;
        public static bool operator !=( Symbol? left, char right ) => !(left == right);
    }
}
