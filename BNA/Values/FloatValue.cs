using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Values
{
	public class FloatValue : Value
	{
		public record FloatObject( double Value )
		{
			public static implicit operator double( FloatObject obj ) => obj.Value;

			public override string ToString( ) => this.Value.ToString( );
		}

		public FloatValue( ) { this.Get = new( 0.0f ); }

		public FloatValue( double val ) { this.Get = new( val ); }

		public override FloatObject Get { get; }

		public override Value DoOperation( StatementType operation , Value op2 ) => throw new NotImplementedException( );
		public override bool Equals( Value? other ) => other is IntegerValue intVal && intVal.Get == this.Get;
		public override int GetHashCode( ) => HashCode.Combine( this.Get.Value );
		public override string ToString( ) => this.Get.ToString( );
	}
}
