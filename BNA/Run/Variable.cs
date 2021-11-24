using BNA.Values;

namespace BNA
{
	/// <summary>
	/// The coupling of a <see cref="Value"/> and an identifying <see cref="Token"/>.
	/// </summary>
	public class Variable
	{
		/// <summary>
		/// Token identifying a variable. Must be <see cref="TokenType.VARIABLE"/> type.
		/// </summary>
		public Token Identifier
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
			this.Identifier = token;
			this.Value = value ?? Value.NULL;
		}
	}
}
