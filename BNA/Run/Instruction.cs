using System;
using BNA.Compile;
using BNA.Exceptions;
using BNA.Run;
using BNA.Values;

namespace BNA.Run
{
	/// <summary>
	/// Executable statement object.
	/// </summary>
	public class Instruction
	{
		/// <summary>
		/// Create a new <see cref="Instruction"/> instance.
		/// </summary>
		/// <param name="statement">Statement to generate from.</param>
		/// <param name="program">Program to modify and get values from.</param>
		public Instruction( Statement statement , Program program )
		{
			this.Program = program;
			(this.PrimaryToken, this.SecondaryToken) = statement.GetPrimaryAndSecondaryTokens( );
			this.PrimaryValue = this.Program.GetValue( this.PrimaryToken );
			this.SecondaryValue = this.Program.GetValue( this.SecondaryToken );
			this.Type = statement.Type;
		}

		/// <summary>
		/// Token representing the primary operand.
		/// </summary>
		public Token PrimaryToken { get; init; }

		/// <summary>
		/// Token representing the secondary operand.
		/// </summary>
		public Token SecondaryToken { get; init; }

		/// <summary>
		/// Value of the primary operand.
		/// </summary>
		public Value PrimaryValue { get; init; }

		/// <summary>
		/// Value of the secondary operand.
		/// </summary>
		public Value SecondaryValue { get; init; }

		/// <summary>
		/// The type of statement the instruction executes.
		/// </summary>
		public StatementType Type { get; init; }

		/// <summary>
		/// Program to modify and get values from.
		/// </summary>
		public Program Program { get; init; }

		/// <summary>
		/// Execute the instruction. Uses the operands and potentially modifies the <see cref="Program"/> instance.
		/// </summary>
		public void Execute( )
		{
			switch ( this.Type )
			{
				case StatementType.NULL:
				case StatementType.LABEL:
				case StatementType.COMMENT:
					break;

				case StatementType.SET:
				{
					this.Program.SetValue( this.PrimaryToken ,
						this.SecondaryValue is ListValue list ? list.DeepCopy( ) : this.SecondaryValue ,
						true );
					break;
				}

				case StatementType.ADD:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Add( this.SecondaryValue ) );
					break;
				}

				case StatementType.SUBTRACT:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Subtract( this.SecondaryValue ) );
					break;
				}

				case StatementType.MULTIPLY:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Multiply( this.SecondaryValue ) );
					break;
				}

				case StatementType.DIVIDE:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Divide( this.SecondaryValue ) );
					break;
				}

				case StatementType.RANDOM:
				{
					Value randVal
						= this.SecondaryValue is IntegerValue intVal ? new IntegerValue( BNA.RNG.Next( intVal.Get ) )
						: this.SecondaryValue is FloatValue floatVal ? new FloatValue( BNA.RNG.NextDouble( ) * floatVal.Get )
						: throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Program.SetValue( this.PrimaryToken , randVal , true );
					break;
				}

				case StatementType.BITWISE_OR:
				case StatementType.BITWISE_AND:
				case StatementType.BITWISE_XOR:
				case StatementType.BITWISE_NEGATE:
					throw new NotImplementedException( );

				case StatementType.POWER:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.RaiseTo( this.SecondaryValue ) );
					break;
				}

				case StatementType.MODULUS:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Modulus( this.SecondaryValue ) );
					break;
				}

				case StatementType.LOGARITHM:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Log( this.SecondaryValue ) );
					break;
				}

				case StatementType.ROUND:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Round( ) );
					break;
				}

				case StatementType.APPEND:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Append( this.SecondaryValue ) );
					break;
				}

				case StatementType.LIST:
				{
					int size = this.SecondaryValue is IntegerValue intVal ? intVal.Get : throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Program.SetValue( this.PrimaryToken , new ListValue( size ) , true );
					break;
				}

				case StatementType.SIZE:
				{
					this.Program.SetValue( this.PrimaryToken , this.SecondaryValue.Size( ) , true );
					break;
				}

				case StatementType.OPEN_WRITE:
				case StatementType.OPEN_READ:
				{
					bool read = this.Type == StatementType.READ;
					string filename = this.SecondaryValue is StringValue strVal ? strVal.Get : throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Program.SetValue( this.PrimaryToken , read ? new ReadFileValue( filename ) : new WriteFileValue( filename ) , true );
					break;
				}

				case StatementType.CLOSE:
				{
					if ( this.PrimaryValue is FileValue fileVal )
					{
						fileVal.Close( );
						this.Program.SetValue( this.PrimaryToken , Value.NULL );
						break;
					}
					else
					{
						throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue );
					}
				}

				case StatementType.READ:
				{
					if ( this.PrimaryValue is ReadFileValue readFileVal )
					{
						string? str = readFileVal.ReadLine( );

						if ( str is null )
						{
							readFileVal.Close( );
							this.Program.SetValue( this.SecondaryToken , Value.NULL );
						}

						Value val = str is null ? Value.NULL
								  : long.TryParse( str , out long lval ) ? new IntegerValue( lval )
								  : double.TryParse( str , out double dval ) ? new FloatValue( dval )
								  : new StringValue( str );

						this.Program.SetValue( this.PrimaryToken , val , true );
						break;
					}
					else
					{
						throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue );
					}
				}

				case StatementType.WRITE:
				{
					if ( this.PrimaryValue is WriteFileValue writeFileVal )
					{
						writeFileVal.WriteLine( this.SecondaryValue.ToString( ) );
						break;
					}
					else
					{
						throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue );
					}
				}

				case StatementType.INPUT:
				{
					Console.Write(
						this.SecondaryValue is StringValue strVal ? strVal.Get
						: throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue )
					);

					var token = new Token( Console.ReadLine( ) ?? string.Empty );
					switch ( token.Type )
					{
						case TokenType.LITERAL:
						case TokenType.LIST:
							this.Program.SetValue( this.PrimaryToken , this.Program.GetValue( token ) , true );
							break;

						case TokenType.STRING:
						case TokenType.SYMBOL:
						case TokenType.VARIABLE:
							this.Program.SetValue( this.PrimaryToken , new StringValue( token.Value ) , true );
							break;

						case TokenType.NULL:
							this.Program.SetValue( this.PrimaryToken , Value.NULL , true );
							break;

						default:
							throw new Exception( "Unexpected data from INPUT return: " + token.ToString( ) );
					}
					break;
				}

				case StatementType.PRINT:
				{
					Console.WriteLine( this.PrimaryValue.ToString( ) );
					break;
				}

				case StatementType.WAIT:
				{
					int ms = (int)( 1000 *
						( this.PrimaryValue is IntegerValue intVal ? intVal.Get
						: this.PrimaryValue is FloatValue floatVal ? floatVal.Get
						: throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue ) ) );

					try
					{
						System.Threading.Thread.Sleep( ms );
					}
					catch ( Exception e )
					{
						// TODO make this a new runtime exception class?
						throw new Exception( "Exception caught while waiting:\n" + e.Message );
					}

					break;
				}

				case StatementType.ERROR:
				{
					throw new ErrorStatementException( this.PrimaryValue );
				}

				case StatementType.EXIT:
				{
					this.Program.Running = false;
					break;
				}

				case StatementType.TEST_EQU:
				case StatementType.TEST_NEQ:
				case StatementType.TEST_GTR:
				case StatementType.TEST_LSS:
				{
					Value result
						= this.Type == StatementType.TEST_EQU ? this.PrimaryValue == this.SecondaryValue ? Value.TRUE : Value.FALSE
						: this.Type == StatementType.TEST_NEQ ? this.PrimaryValue != this.SecondaryValue ? Value.TRUE : Value.FALSE
						: this.Type == StatementType.TEST_GTR ? this.PrimaryValue.GreaterThan( this.SecondaryValue ) ? Value.TRUE : Value.FALSE
						: this.Type == StatementType.TEST_LSS ? this.PrimaryValue.LessThan( this.SecondaryValue ) ? Value.TRUE : Value.FALSE
						: throw new Exception( $"Unexpted TEST statement type: {this.Type}" );
					this.Program.SetValue( SpecialVariables.TEST_RESULT , result );
					break;
				}

				case StatementType.TYPE:
				{
					string type = this.SecondaryValue.TypeString( )[..^"Value".Length].ToUpper( );
					this.Program.SetValue( this.PrimaryToken , new StringValue( type ) , true );
					break;
				}

				case StatementType.SCOPE_OPEN:
				{
					this.Program.OpenScope( );
					break;
				}

				case StatementType.SCOPE_CLOSE:
				{
					this.Program.CloseScope( );
					break;
				}

				case StatementType.GOTO:
				{
					// Subtract one because the IP will be incremented after this
					int newIP = this.PrimaryValue is IntegerValue intVal ? intVal.Get - 1 : throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue );

					// No negative values or overflow (after increment)
					if ( newIP is < -1 or int.MaxValue )
					{
						throw new ValueOutOfRangeException( this.PrimaryValue , "GOTO address" );
					}

					// Check condition
					if ( this.SecondaryValue != Value.FALSE )
					{
						this.Program.IP = newIP;
					}

					break;
				}

				case StatementType.UNKNOWN:
				default:
					throw new Exception( $"Unexpected statement type: {this.Type}" );
			}
		}

		public override string ToString( ) => $"{this.Type} ({this.PrimaryOperandInfo( )}) ({this.SecondaryOperandInfo( )})";

		/// <summary>
		/// Display relevant info on the primary operand.
		/// </summary>
		/// <returns>String of the token, value, and value type.</returns>
		private string PrimaryOperandInfo( ) => $"{this.PrimaryToken} {this.PrimaryValue.TypeString( )} '{this.PrimaryValue}'";

		/// <summary>
		/// Display relevant info on the secondary operand.
		/// </summary>
		/// <returns>String of the token, value, and value type.</returns>
		private string SecondaryOperandInfo( ) => $"{this.SecondaryToken} {this.SecondaryValue.TypeString( )} '{this.SecondaryValue}'";
	}
}
