using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Values
{
	public class ListValue : Value
	{
		public ListValue( ) { this.Get = new( ); }

		public ListValue( List<Value> vals ) { this.Get = vals; }

		public override List<Value> Get { get; }

		public ListValue DeepCopy(ListValue listVal)
		{
			List<Value> list = listVal.Get;
			List<Value> newList = new( );

			foreach ( Value val in list )
			{
				if ( val is ListValue sublist )
				{
					newList.Add( this.DeepCopy( sublist ) );
				}
				else
				{
					newList.Add( val );
				}
			}

			return new( newList );
		}

		public override Value DoOperation( StatementType operation , Value op2 ) => throw new NotImplementedException( );
		public override bool Equals( Value? other ) => other is ListValue listVal && Enumerable.SequenceEqual( this.Get , listVal.Get );
		public override int GetHashCode( ) => HashCode.Combine( this.Get );
		public override string ToString( ) => $"{Symbol.LIST_START} {string.Join( $"{Symbol.LIST_SEPERATOR} " , this.Get )} {Symbol.LIST_END}";
	}
}
