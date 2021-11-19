using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Values
{
	public class StringValue : Value
	{
		public StringValue( )
		{
			this.Get = string.Empty;
		}

		public StringValue( string str )
		{
			this.Get = str;
		}

		public override string Get { get; }

		public override Value DoOperation( StatementType operation , Value op2 ) => throw new NotImplementedException( );
		public override bool Equals( object? obj ) => obj is StringValue strVal && strVal.Get == this.Get;
		public override int GetHashCode( ) => HashCode.Combine(this.Get);
		public override string ToString( ) => this.Get;
	}
}
