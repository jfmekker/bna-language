using System;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// Integer type value.
	/// </summary>
	public class IntegerValue : Value
	{
		/// <summary>
		/// Object wrapper for a 'long' type value.
		/// </summary>
		public record IntegerObject( long Value )
		{
			public static implicit operator long( IntegerObject obj ) => obj.Value;

			public static explicit operator ulong( IntegerObject obj ) => (ulong)obj.Value;

			public static implicit operator int( IntegerObject obj ) => (int)obj.Value;

			public override string ToString( ) => this.Value.ToString( );
		}

		/// <summary>
		/// Create a new default <see cref="IntegerValue"/> instance.
		/// </summary>
		public IntegerValue( ) { this.Get = new( 0L ); }

		/// <summary>
		/// Create a new <see cref="IntegerValue"/> instance.
		/// </summary>
		/// <param name="val">Long integer value to assign.</param>
		public IntegerValue( long val ) { this.Get = new( val ); }

		/// <summary>
		/// Gets the actual <see cref="IntegerObject"/> value stored.
		/// </summary>
		public override IntegerObject Get { get; }

		public override string TypeString( ) => "IntegerValue";

		public override bool Equals( Value? other )
			=> ( other is IntegerValue intVal && intVal.Get == this.Get )
			|| ( other is FloatValue floatVal && floatVal.Get == this.Get );

		public static Value operator +( IntegerValue value , Value other )
			=> other is IntegerValue intVal ? new IntegerValue( value.Get + intVal.Get )
			 : other is FloatValue floatVal ? new FloatValue( value.Get + floatVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} + {other.TypeString( )}" );

		public static Value operator -( IntegerValue value , Value other )
			=> other is IntegerValue intVal ? new IntegerValue( value.Get - intVal.Get )
			 : other is FloatValue floatVal ? new FloatValue( value.Get - floatVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} - {other.TypeString( )}" );

		public static Value operator *( IntegerValue value , Value other )
			=> other is IntegerValue intVal ? new IntegerValue( value.Get * intVal.Get )
			 : other is FloatValue floatVal ? new FloatValue( value.Get * floatVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} * {other.TypeString( )}" );

		public static Value operator /( IntegerValue value , Value other )
			=> other is IntegerValue intVal ? new IntegerValue( value.Get / intVal.Get )
			 : other is FloatValue floatVal ? new FloatValue( value.Get / floatVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} / {other.TypeString( )}" );
	}
}
