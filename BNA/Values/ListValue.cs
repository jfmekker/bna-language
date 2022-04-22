using System;
using System.Collections.Generic;
using System.Linq;
using BNA.Common;

namespace BNA.Values
{
	/// <summary>
	/// List type value.
	/// </summary>
	public class ListValue : Value
	{
		/// <summary>
		/// Create a new default <see cref="ListValue"/> instance.
		/// </summary>
		public ListValue( ) { this.Get = new( ); }

		/// <summary>
		/// Create a new <see cref="ListValue"/> instance filled with <see cref="Value.NULL"/> values.
		/// </summary>
		/// <param name="length">Length of list to start with.</param>
		public ListValue( int length ) { this.Get = Enumerable.Repeat( NULL , length ).ToList( ); }

		/// <summary>
		/// Create a new <see cref="ListValue"/> instance.
		/// </summary>
		/// <param name="vals">List of values to store.</param>
		public ListValue( List<Value> vals ) { this.Get = vals; }

		/// <summary>
		/// Gets the actual <see cref="List{T}"/> of <see cref="Value"/> objects stored.
		/// </summary>
		public override List<Value> Get { get; }

		public override string TypeString( ) => "ListValue";

		public override bool Equals( Value? other ) => other is ListValue listVal && Enumerable.SequenceEqual( this.Get , listVal.Get );

		/// <summary>
		/// Generate a print out of all list elements. Uses the proper symbols from the <see cref="Symbol"/> enum.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString( ) => $"{Symbol.LIST_START} {string.Join( $"{Symbol.LIST_SEPARATOR} " , this.Get )} {Symbol.LIST_END}";

		/// <summary>
		/// Create a deep copy of this <see cref="ListValue"/>. Recusively copies all elements in the list.
		/// </summary>
		/// <returns>A deep copy of the list.</returns>
		public ListValue DeepCopy( )
		{
			List<Value> list = this.Get;
			List<Value> newList = new( );

			foreach ( Value val in list )
			{
				if ( val is ListValue sublist )
				{
					newList.Add( sublist.DeepCopy( ) );
				}
				else
				{
					newList.Add( val );
				}
			}

			return new( newList );
		}

		public override Value Append( Value value )
		{
			_ = this.Get.Append( value );
			return this;
		}

		public override Value Size( ) => new IntegerValue(this.Get.Count);
	}
}
