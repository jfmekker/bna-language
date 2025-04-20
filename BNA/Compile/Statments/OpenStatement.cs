using System.Collections.Generic;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public record class OpenStatement : ComplexStatement<StringOperand>
    {
        public enum FileType { Read, Write }

        public static IStatementParser Parser { get; } = new StatementParser( Keyword.Open, Parse );

        public override string Operation => "OPEN";

        public FileType OpenType { get; }

        private OpenStatement( Identifier identifier, StringOperand operand, FileType openType, string raw )
            : base( identifier, operand, raw )
        {
            this.OpenType = openType;
        }

        private static OpenStatement Parse( IReadOnlyCollection<Token> tokens, string raw )
        {
            TokenHandler tokenHandler = new( tokens );

            Dictionary<Token, FileType> mapping = new( ) {
                [Keyword.Read] = FileType.Read,
                [Keyword.Write] = FileType.Write,
            };

            tokenHandler.Get( Keyword.Open )
                        .Get( out StringOperand operand )
                        .Get( Keyword.As )
                        .Get( out FileType type, mapping )
                        .Get( out Identifier identifier )
                        .End( );

            return new( identifier, operand, type, raw );
        }
    }
}
