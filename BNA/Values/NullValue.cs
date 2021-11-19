using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNA.Exceptions;

namespace BNA.Values
{
	public class NullValue : Value
	{
		public override object Val
		{
			get => throw new RuntimeException( "Cannot get Val of a NullValue." );
			set => throw new RuntimeException( "Cannot set Val of a NullValue." );
		}

		public override Value DoOperation( StatementType operation , Value? op2 )
		{
			return base.DoOperation( operation , op2 );
		}

		public override bool Equals( object? obj ) => obj is NullValue;

		public override int GetHashCode( ) => base.GetHashCode( );

		public override string ToString( ) => "NULL";
	}
}
