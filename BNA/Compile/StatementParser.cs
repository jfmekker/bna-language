using System.Collections.Generic;
using BNA.Compile.Statments;
using BNA.Compile.Tokens;

namespace BNA.Compile
{
    public interface IStatementParser
    {
        public bool IsStartToken( Token token );

        public Statement Parse( IReadOnlyCollection<Token> tokens, string raw );
    }

    public delegate Statement StatementParserFunc( IReadOnlyCollection<Token> tokens, string raw );

    public class StatementParser : IStatementParser
    {
        private readonly Token _start;
        private readonly StatementParserFunc _parse;

        public StatementParser( Token start, StatementParserFunc parse )
        {
            this._start = start;
            this._parse = parse;
        }

        public bool IsStartToken( Token token ) => token == this._start;

        public Statement Parse( IReadOnlyCollection<Token> tokens, string raw ) => this._parse( tokens, raw );
    }
}
