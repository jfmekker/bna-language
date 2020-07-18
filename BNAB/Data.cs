using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAB
{
	public class DataSegment
	{
		private ArraySegment<ulong> _raw;

		public DataEntry[] Entries;

		public DataSegment( ArraySegment<ulong> raw )
		{
			_raw = raw;
			ReadEntries( );
		}

		private void ReadEntries( )
		{
			if ( Entries == null ) {
				var entry_list = new List<DataEntry>( );

				// TODO parse

				Entries = entry_list.ToArray( );
			}
		}
	}

	public class DataEntry
	{
		public enum DataEntryType
		{
			NONE, LITERAL_INT, LITERAL_FLOAT, STRING
		}

		public DataEntryType Identifier
		{
			get;
		}

		public int Length
		{
			get;
		}


	}
}
