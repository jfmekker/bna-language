using System;
using BNA.Exceptions;

namespace BNA.Values
{
	public class NaNValue : Value
	{
		public override object Get
		{
			get => throw new RuntimeException( "Cannot get value of a NaNValue." );
		}

		public override Value DoOperation( StatementType operation , Value? op2 ) => NAN;

		public override bool Equals( Value? other ) => false;

		public override int GetHashCode( ) => 0;

		public override string ToString( ) => "NaN";
	}
}
