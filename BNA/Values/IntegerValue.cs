using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Values
{
	public class IntegerValue : Value
	{
		public record IntegerObject( long Value )
		{
			public static implicit operator long( IntegerObject obj ) => obj.Value;

			public override string ToString( ) => this.Value.ToString( );
		}

		public IntegerValue( ) { this.Get = new( 0L ); }

		public IntegerValue(long val) { this.Get = new( val ); }

		public override IntegerObject Get { get; }

		public override Value DoOperation( StatementType operation , Value op2 ) => throw new NotImplementedException( );
		public override bool Equals( Value? other ) => other is IntegerValue intVal && intVal.Get == this.Get;
		public override int GetHashCode( ) => HashCode.Combine( this.Get.Value );
		public override string ToString( ) => this.Get.ToString( );
	}
}
