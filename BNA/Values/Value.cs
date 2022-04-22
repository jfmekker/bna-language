using System;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// A struct holding an abstact value of varying type.
	/// </summary>
	public abstract class Value : IEquatable<Value>
	{
		/// <summary>
		/// Special <see cref="Value"/> to represent null values.
		/// </summary>
		public static readonly Value NULL = new NullValue( );

		/// <summary>
		/// Special <see cref="Value"/> to represent results of bad operations.
		/// </summary>
		public static readonly Value NAN = new NaNValue( );

		/// <summary>
		/// Special <see cref="Value"/> to represent true.
		/// </summary>
		public static readonly Value TRUE = new IntegerValue( 1 );

		/// <summary>
		/// Special <see cref="Value"/> to represent false.
		/// </summary>
		public static readonly Value FALSE = new IntegerValue( 0 );

		/// <summary>
		/// Gets the actual <see cref="object"/> stored.
		/// </summary>
		public abstract object Get { get; }

		/// <summary>
		/// Get a string representing the name of the Value's type.
		/// </summary>
		/// <returns>String.</returns>
		public virtual string TypeString( ) => "Value";

		/// <summary>
		/// Compare equality to another <see cref="Value"/> instance.
		/// </summary>
		/// <param name="other">Value to compare against.</param>
		/// <returns>True if the values are equal.</returns>
		public abstract bool Equals( Value? other );

		/// <summary>
		/// Compare equality to an object.
		/// </summary>
		/// <param name="other">Object to compare against.</param>
		/// <returns>True if the object is an equal <see cref="Value"/> or equals this object's stored value.</returns>
		public override sealed bool Equals( object? obj ) => this.Equals( obj as Value ) || this.Get.Equals( obj );

		/// <summary>
		/// Get a hash code of this <see cref="Value"/>'s stored object.
		/// </summary>
		/// <returns>Integer hash code.</returns>
		public override int GetHashCode( ) => this.Get.GetHashCode( );

		/// <summary>
		/// Get a string representation of the value.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString( ) => this.Get.ToString( ) ?? string.Empty;

		/// <summary>
		/// Compare equality of two <see cref="Value"/> instances.
		/// </summary>
		/// <param name="first">First value.</param>
		/// <param name="second">Second value.</param>
		/// <returns>True if the two values are equal.</returns>
		public static bool operator ==( Value first , Value second ) => (first is not null && first.Equals( second )) || second is null;

		/// <summary>
		/// Compare non-equality of two <see cref="Value"/> instances.
		/// </summary>
		/// <param name="first">First value.</param>
		/// <param name="second">Second value.</param>
		/// <returns>True if the two values are not equal.</returns>
		public static bool operator !=( Value first , Value second ) => !(first == second);

		/// <summary>
		/// Compare inequality against another <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>True if this <see cref="Value"/> is greater than the argument <see cref="Value"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual bool GreaterThan( Value value ) => throw new UndefinedOperationException( this , ">" , value );

		/// <summary>
		/// Compare inequality against another <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>True if this <see cref="Value"/> is less than the argument <see cref="Value"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual bool LessThan( Value value ) => throw new UndefinedOperationException( this , "<" , value );

		/// <summary>
		/// Add a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Added <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Add( Value value ) => throw new UndefinedOperationException( this , "+" , value );

		/// <summary>
		/// Subtract a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Subtracted <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Subtract( Value value ) => throw new UndefinedOperationException( this , "-" , value );

		/// <summary>
		/// Multiply a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Multiplied <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Multiply( Value value ) => throw new UndefinedOperationException( this , "*" , value );

		/// <summary>
		/// Divide a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Divided <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Divide( Value value ) => throw new UndefinedOperationException( this , "/" , value );

		/// <summary>
		/// Raise to the power of a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Raised <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Exponentiate( Value value ) => throw new UndefinedOperationException( this , "POW" , value );

		/// <summary>
		/// Take the logarithm with a base of a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Resulting <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Log( Value value ) => throw new UndefinedOperationException( this , "LGO" , value );

		/// <summary>
		/// Take the modulus of a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Modulated <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Modulus( Value value ) => throw new UndefinedOperationException( this , "%" , value );

		/// <summary>
		/// Append a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Appended <see cref="Value"/>, needed for strings.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Append( Value value ) => throw new UndefinedOperationException( this , "APPEND" , value );

		/// <summary>
		/// Round this <see cref="Value"/> to the nearest integer.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Rounded <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Round( ) => throw new UndefinedOperationException( this , "ROUND" );

		/// <summary>
		/// Get the size of a <see cref="Value"/>.
		/// Throws <see cref="RuntimeException"/> if not overriden in derived class.
		/// </summary>
		/// <returns>Size stored in a <see cref="Value>"/>.</returns>
		/// <exception cref="RuntimeException"/>
		public virtual Value Size( ) => throw new UndefinedOperationException( this , "SIZE" );
	}
}
