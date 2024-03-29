﻿using BNA.Common;
using BNA.Values;

namespace BNA.Run
{
	/// <summary>
	/// The coupling of a <see cref="Value"/> and an identifying <see cref="Common.Token"/>.
	/// </summary>
	public class Variable
	{
		/// <summary>
		/// Token identifying a variable. Must be <see cref="TokenType.VARIABLE"/> type.
		/// </summary>
		public Token Token
		{
			get; private set;
		}

		/// <summary>
		/// The <see cref="Value"/> stored by the variable.
		/// </summary>
		public virtual Value Value
		{
			get; set;
		}

		/// <summary>
		/// Create a new <see cref="Variable"/> instance.
		/// </summary>
		/// <param name="token">Identifiying token</param>
		/// <param name="value">Initial value</param>
		public Variable( Token token , Value? value = null )
		{
			this.Token = token;
			this.Value = value ?? Value.NULL;
		}

		public void Deconstruct(out Token token, out Value value)
		{
			token = this.Token;
			value = this.Value;
		}
	}
}
