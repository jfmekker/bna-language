using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Common
{
	public static class Extensions
	{
		public static bool IsLetter( this char? character ) => character is char c && char.IsLetter( c );

		public static bool IsDigit( this char? character ) => character is char c && char.IsDigit( c );

		public static bool IsLetterOrDigit( this char? character ) => character is char c && char.IsLetterOrDigit( c );
	}
}
