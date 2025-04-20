using BNA.Compile;
using BNA.Compile.Tokens;
using BNA.Exceptions;
using BNA.Values;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BNA.Run
{
    public class Evaluator
    {
        public IMemory Memory { get; init; }

        public Evaluator( IMemory memory )
        {
            this.Memory = memory;
        }

        public Value Evaluate( Token token )
        {
            switch ( token.Type )
            {
                case TokenType.LiteralNumber:
                    return this.EvaluateNumber( token.Value );

                case TokenType.LiteralString:
                    return this.EvaluateString( token.Value );

                case TokenType.Variable:
                    return this.EvaluateVariable( token.Value );

                case TokenType.List:
                    return this.EvaluateList( token.Value );

                case TokenType.Null:
                    return Value.NULL;

                default:
                    throw new InvalidOperationException( "Unexpected token type in given to Evaluator: " + token.ToString( ) );
            }
        }

        [SuppressMessage( "Performance", "CA1822:Mark members as static",
            Justification = "Keep the Evaluate* methods all non-static." )]
        private Value EvaluateNumber( string str )
        {
            if ( long.TryParse( str, out long lval ) )
            {
                return new IntegerValue( lval );
            }
            else if ( double.TryParse( str, out double dval ) )
            {
                return new FloatValue( dval );
            }

            throw new FormatException( $"Could not parse value from number literal: '{str}'" );
        }

        [SuppressMessage( "Performance", "CA1822:Mark members as static",
            Justification = "Keep the Evaluate* methods all non-static." )]
        private StringValue EvaluateString( string str )
        {
            if ( str.Length < 2 )
            {
                throw new ArgumentException( $"String token too short to be valid: '{str}'" );
            }

            StringBuilder sb = new( );
            for ( int i = 1 ; i < str.Length - 1 ; i += 1 )
            {
                if ( str[i] is (char)Symbol.Escape )
                {
                    if ( i + 1 == str.Length )
                    {
                        throw new InvalidOperationException( "Escape character at end of string, should have been caught by compiler." );
                    }

                    char escaped_char = str[i + 1] switch {
                        'a' => '\a', // Alert/beep/bell - TODO does this work?
                        'b' => '\b', // Backspace - TODO does this work?
                        'n' => '\n', // New line
                        't' => '\t', // Tab
                        (char)Symbol.StringDelim => (char)Symbol.StringDelim,
                        (char)Symbol.Escape => (char)Symbol.Escape,
                        _ => str[i + 1], // By default add just the escaped character
                    };

                    _ = sb.Append( escaped_char );
                }
                else
                {
                    _ = sb.Append( str[i] );
                }
            }

            return new StringValue( sb.ToString( ) );
        }

        private ListValue EvaluateList( string str )
        {
            Lexer lexer = new( str[1..^1] );
            IReadOnlyCollection<Token> listTokens = lexer.ReadTokens( );
            List<Value> listValues = [];

            foreach ( Token t in listTokens )
            {
                if ( t.AsSymbol( ) is not Symbol.ListSeparator )
                {
                    listValues.Add( this.Evaluate( t ) );
                }
            }

            return new ListValue( listValues );
        }

        private Value EvaluateVariable( string str )
        {
            // Get list element
            if ( str.Contains( (char)Symbol.Accessor, StringComparison.Ordinal ) )
            {
                // Get last accessor first (so multi-lists are properly chained)
                int accessor = str.LastIndexOf( (char)Symbol.Accessor );
                if ( accessor == 0 || accessor == str.Length )
                {
                    // Should have been detected at compiletime
                    throw new InvalidOperationException( $"Accessor at start or end of token: '{str}'" );
                }

                // Get value of index part
                Token indexTok = Lexer.ReadSingleToken( str[(accessor + 1)..] );
                Value indexVal = this.Memory.GetValue( indexTok );
                if ( indexVal is not IntegerValue index )
                {
                    throw new InvalidIndexValueException( indexTok, indexVal );
                }

                // Get the value
                Token accessedTok = Lexer.ReadSingleToken( str[..accessor] );
                Value accessedVal = this.Memory.GetValue( accessedTok );
                return accessedVal is ListValue listVal
                        ? index.Get >= 0 && index.Get < listVal.Get.Count ? listVal.Get[index.Get]
                        : throw new ValueOutOfRangeException( indexVal, $"List {listVal}" )
                    : accessedVal is StringValue strVal
                        ? index.Get >= 0 && index.Get < strVal.Get.Length ? new StringValue( "" + strVal.Get[index.Get] )
                        : throw new ValueOutOfRangeException( indexVal, $"String \"{strVal}\"" )
                    : throw new NonIndexableValueException( accessedTok, accessedVal );
            }

            return this.Memory.GetValue( new Token( str, TokenType.Variable ) );
        }
    }
}
