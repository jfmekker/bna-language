using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public enum ValueType
	{
		INVALID = -1,
		NULL = 0,
		INTEGER = 1,
		FLOAT,
		STRING,
		LIST
	}

	public struct Value
	{
		public static Value DoNumericOperation( Value op1 , Value op2 , StatementType operation )
		{
			throw new NotImplementedException( );
		}

		public static Value DoComparisonOperation( Value op1 , Value op2 , StatementType operation )
		{
			throw new NotImplementedException( );
		}

		public ValueType Type
		{
			get; set;
		}

		public object Val
		{
			get; set;
		}

		public Value( ValueType type , object val ) : this( )
		{
			this.Type = type;
			this.Val = val;
		}

		public override string ToString( )
		{
			if ( Val == null )
				return "";
			return Val.ToString( );
		}
	}
}
