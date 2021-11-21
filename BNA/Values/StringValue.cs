using System;
using System.Linq;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// String type value.
	/// </summary>
	public class StringValue : Value
	{
		/// <summary>
		/// Create a new default <see cref="StringValue"/> instance.
		/// </summary>
		/// <param name="val">String value to assign.</param>
		public StringValue( ) { this.Get = string.Empty; }

		/// <summary>
		/// Create a new <see cref="StringValue"/> instance.
		/// </summary>
		/// <param name="val">String value to assign.</param>
		public StringValue( string str ) { this.Get = str; }

		/// <summary>
		/// Get the actual <see cref="string"/> value stored.
		/// </summary>
		public override string Get { get; }

		public override string TypeString( ) => "StringValue";

		public override bool Equals( Value? other ) => other is StringValue strVal && strVal.Get == this.Get;

		public override Value Append( Value value ) => new StringValue( this.Get + value.ToString( ) );

		public override Value Size( ) => new IntegerValue( this.Get.Length );
	}
}
