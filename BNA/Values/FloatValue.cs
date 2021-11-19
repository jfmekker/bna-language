using System;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// Floating point type value.
	/// </summary>
	public class FloatValue : Value
	{
		/// <summary>
		/// Object wrapper for a 'double' type value.
		/// </summary>
		public record FloatObject( double Value )
		{
			public static implicit operator double( FloatObject obj ) => obj.Value;

			public override string ToString( ) => this.Value.ToString( );
		}

		/// <summary>
		/// Create a new default <see cref="FloatValue"/> instance.
		/// </summary>
		public FloatValue( ) { this.Get = new( 0.0f ); }

		/// <summary>
		/// Create a new <see cref="FloatValue"/> instance.
		/// </summary>
		/// <param name="val">Double floating point value to assign.</param>
		public FloatValue( double val ) { this.Get = new( val ); }

		/// <summary>
		/// Gets the actual <see cref="FloatObject"/> stored.
		/// </summary>
		public override FloatObject Get { get; }

		public override string TypeString( ) => "FloatValue";

		public override bool Equals( Value? other )
			=> ( other is FloatValue floatVal && floatVal.Get == this.Get )
			|| ( other is IntegerValue intVal && intVal.Get == this.Get );

		public static Value operator +( FloatValue value , Value other )
			=> other is FloatValue floatVal ? new FloatValue( value.Get + floatVal.Get )
			 : other is IntegerValue intVal ? new FloatValue( value.Get + intVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} + {other.TypeString( )}" );

		public static Value operator -( FloatValue value , Value other )
			=> other is FloatValue floatVal ? new FloatValue( value.Get - floatVal.Get )
			 : other is IntegerValue intVal ? new FloatValue( value.Get - intVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} - {other.TypeString( )}" );

		public static Value operator *( FloatValue value , Value other )
			=> other is FloatValue floatVal ? new FloatValue( value.Get * floatVal.Get )
			 : other is IntegerValue intVal ? new FloatValue( value.Get * intVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} * {other.TypeString( )}" );

		public static Value operator /( FloatValue value , Value other )
			=> other is FloatValue floatVal ? new FloatValue( value.Get / floatVal.Get )
			 : other is IntegerValue intVal ? new FloatValue( value.Get / intVal.Get )
			 : throw new RuntimeException( $"Invalid operation: {value.TypeString( )} / {other.TypeString( )}" );
	}
}
