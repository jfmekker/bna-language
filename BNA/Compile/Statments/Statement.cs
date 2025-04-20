using System.Text;
using BNA.Common;
using BNA.Compile.Tokens;

namespace BNA.Compile.Statments
{
    public abstract record class Statement
    {
        public abstract string Operation { get; }

        public string Raw { get; }

        protected Statement( string raw )
        {
            this.Raw = raw;
        }

        public override string ToString( )
        {
            StringBuilder builder = new( );

            _ = builder.Append( '[' )
                       .AppendLine( this.Operation )
                       .Append( "] " );

            return builder.ToString( );
        }
    }

    public abstract record class SimpleStatement<TOperand> : Statement
        where TOperand : IOperand<TOperand>
    {
        public TOperand Operand { get; }

        protected SimpleStatement( TOperand primary, string raw )
            : base( raw )
        {
            this.Operand = primary;
        }

        public override string ToString( )
        {
            StringBuilder builder = new( );

            _ = builder.Append( '[' )
                       .AppendLine( this.Operation )
                       .Append( "] " )
                       .Append( $"op={this.Operand,-24}" );

            return builder.ToString( );
        }
    }

    public abstract record class ComplexStatement<TOperand> : SimpleStatement<TOperand>
        where TOperand : IOperand<TOperand>
    {
        public Identifier Identifier { get; }

        protected ComplexStatement( Identifier identifier, TOperand operand , string raw )
            : base( operand, raw )
        {
            this.Identifier = identifier;
        }

        public override string ToString( )
        {
            StringBuilder builder = new( );

            _ = builder.Append( '[' )
                       .AppendLine( this.Operation )
                       .Append( "] " )
                       .Append( $"id={this.Identifier,-24} " )
                       .Append( $"op={this.Operand,-24}" );

            return builder.ToString( );
        }
    }
}
