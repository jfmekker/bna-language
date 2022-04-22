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

		public override bool GreaterThan( Value value )
			=> value is IntegerValue intVal ? this.Get > intVal.Get
			 : value is FloatValue floatVal ? this.Get > floatVal.Get
			 : base.GreaterThan( value );

		public override bool LessThan( Value value )
			=> value is IntegerValue intVal ? this.Get < intVal.Get
			 : value is FloatValue floatVal ? this.Get < floatVal.Get
			 : base.LessThan( value );

		public override Value Add( Value value )
			=> value is FloatValue floatVal ? new FloatValue( this.Get + floatVal.Get )
			 : value is IntegerValue intVal ? new FloatValue( this.Get + intVal.Get )
			 : base.Add( value );

		public override Value Subtract( Value value )
			=> value is FloatValue floatVal ? new FloatValue( this.Get - floatVal.Get )
			 : value is IntegerValue intVal ? new FloatValue( this.Get - intVal.Get )
			 : base.Subtract( value );

		public override Value Multiply( Value value )
			=> value is FloatValue floatVal ? new FloatValue( this.Get * floatVal.Get )
			 : value is IntegerValue intVal ? new FloatValue( this.Get * intVal.Get )
			 : base.Multiply( value );

		public override Value Divide( Value value )
			=> value is FloatValue floatVal ? new FloatValue( this.Get / floatVal.Get )
			 : value is IntegerValue intVal ? new FloatValue( this.Get / intVal.Get )
			 : base.Divide( value );

		public override Value Exponentiate( Value value )
			=> value is IntegerValue intVal ? new FloatValue( Math.Pow( this.Get , intVal.Get ) )
			 : value is FloatValue floatVal ? new FloatValue( Math.Pow( this.Get , floatVal.Get ) )
			 : base.Exponentiate( value );

		public override Value Log( Value value )
			=> value is IntegerValue intVal ? new FloatValue( Math.Log( this.Get , intVal.Get ) )
			 : value is FloatValue floatVal ? new FloatValue( Math.Log( this.Get , floatVal.Get ) )
			 : base.Log( value );

		public override Value Round( ) => new IntegerValue( (long)Math.Round( this.Get ) );
	}
}
