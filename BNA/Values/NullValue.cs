using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// Null type value.
	/// </summary>
	public class NullValue : Value
	{
		public override object Get
		{
			get => throw new RuntimeException( "Cannot get value of a NullValue." );
		}

		public override string TypeString( ) => "NullValue";

		public override bool Equals( Value? other ) => other is NullValue;

		public override int GetHashCode( ) => 0;

		public override string ToString( ) => "null";
	}
}
