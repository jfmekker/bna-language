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

			public static explicit operator double( IntegerObject obj ) => (double)obj.Value;

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

		public override bool GreaterThan( Value value )
			=> value is IntegerValue intVal ? this.Get > intVal.Get
			 : value is FloatValue floatVal ? this.Get > floatVal.Get
			 : base.GreaterThan( value );

		public override bool LessThan( Value value )
			=> value is IntegerValue intVal ? this.Get < intVal.Get
			 : value is FloatValue floatVal ? this.Get < floatVal.Get
			 : base.LessThan( value );

		public override Value Add( Value value )
			=> value is IntegerValue intVal ? new IntegerValue( this.Get + intVal.Get )
			 : value is FloatValue floatVal ? new FloatValue( this.Get + floatVal.Get )
			 : base.Add( value );

		public override Value Subtract( Value value )
			=> value is IntegerValue intVal ? new IntegerValue( this.Get - intVal.Get )
			 : value is FloatValue floatVal ? new FloatValue( this.Get - floatVal.Get )
			 : base.Subtract( value );

		public override Value Multiply( Value value )
			=> value is IntegerValue intVal ? new IntegerValue( this.Get * intVal.Get )
			 : value is FloatValue floatVal ? new FloatValue( this.Get * floatVal.Get )
			 : base.Multiply( value );

		public override Value Divide( Value value )
			=> value is IntegerValue intVal ? new IntegerValue( this.Get / intVal.Get )
			 : value is FloatValue floatVal ? new FloatValue( this.Get / floatVal.Get )
			 : base.Divide( value );

		public override Value RaiseTo( Value value )
			=> value is IntegerValue intVal ? new IntegerValue( (long)Math.Pow( this.Get , intVal.Get ) )
			 : value is FloatValue floatVal ? new FloatValue( Math.Pow( this.Get , floatVal.Get ) )
			 : base.RaiseTo( value );

		public override Value Log( Value value )
			=> value is IntegerValue intVal ? new IntegerValue( (long)Math.Log( this.Get , intVal.Get ) )
			 : value is FloatValue floatVal ? new FloatValue( Math.Log( this.Get , floatVal.Get ) )
			 : base.Log( value );

		public override Value Modulus( Value value )
			=> value is IntegerValue intVal ? new IntegerValue( this.Get % intVal.Get )
			 : base.Modulus( value );

		public override Value Round( ) => this;
	}
}
