using System;
using BNA.Exceptions;

namespace BNA.Values
{
	public class NullValue : Value
	{
		public override object Get
		{
			get => throw new RuntimeException( "Cannot get value of a NullValue." );
		}

		public override Value DoOperation( StatementType operation , Value? op2 ) => throw new NotImplementedException( );

		public override bool Equals( Value? other ) => other is NullValue;

		public override int GetHashCode( ) => 0;

		public override string ToString( ) => "NULL";
	}
}
