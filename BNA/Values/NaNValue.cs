using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNA.Exceptions;

namespace BNA.Values
{
	public class NaNValue : Value
	{
		public override object Val
		{
			get => throw new RuntimeException( "Cannot get Val of a NaNValue." );
			set => throw new RuntimeException( "Cannot set Val of a NaNValue." );
		}

		public override Value DoOperation( StatementType operation , Value? op2 )
		{
			return NAN;
		}

		public override bool Equals( object? obj ) => false;

		public override int GetHashCode( ) => base.GetHashCode( );

		public override string ToString( ) => "NaN";
	}
}
