using System;

namespace BNA.Values
{
	/// <summary>
	/// NaN type value.
	/// </summary>
	public class NaNValue : Value
	{
		public override object Get
		{
			get => throw new Exception( "Cannot get value of a NaNValue." );
		}
		
		public override string TypeString( ) => "NaNValue";

		public override bool Equals( Value? other ) => false;

		public override int GetHashCode( ) => 0;

		public override string ToString( ) => "NaN";
	}
}
