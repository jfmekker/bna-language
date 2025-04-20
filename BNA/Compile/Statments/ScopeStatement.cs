using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class ScopeStatement : Statement
    {
        public enum ScopeOperation { Open, Close }

        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Scope, Parse );

        public override string Operation => "SCOPE";

        public ScopeOperation OperationType { get; }

        private ScopeStatement( ScopeOperation operation, string raw )
            : base( raw )
        {
            this.OperationType = operation;
        }

        public static ScopeStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            Dictionary<Token, ScopeOperation> mapping = new( ) {
                [Keyword.Open] = ScopeOperation.Open,
                [Keyword.Close] = ScopeOperation.Close,
            };

            tokenHandler.Get( Keyword.Scope )
                        .Get( out ScopeOperation operation, mapping )
                        .End();

            return new( operation, raw );
        }
    }
}
