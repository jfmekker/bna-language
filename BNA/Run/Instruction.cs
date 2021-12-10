using System;
using BNA.Common;
using BNA.Compile;
using BNA.Exceptions;
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
		public Operation Type { get; init; }

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
				case Operation.NULL:
				case Operation.LABEL:
					break;

				case Operation.SET:
				{
					this.Program.SetValue( this.PrimaryToken ,
						this.SecondaryValue is ListValue list ? list.DeepCopy( ) : this.SecondaryValue ,
						true );
					break;
				}

				case Operation.ADD:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Add( this.SecondaryValue ) );
					break;
				}

				case Operation.SUBTRACT:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Subtract( this.SecondaryValue ) );
					break;
				}

				case Operation.MULTIPLY:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Multiply( this.SecondaryValue ) );
					break;
				}

				case Operation.DIVIDE:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Divide( this.SecondaryValue ) );
					break;
				}

				case Operation.RANDOM:
				{
					Value randVal
						= this.SecondaryValue is IntegerValue intVal ? new IntegerValue( BNA.RNG.Next( intVal.Get ) )
						: this.SecondaryValue is FloatValue floatVal ? new FloatValue( BNA.RNG.NextDouble( ) * floatVal.Get )
						: throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Program.SetValue( this.PrimaryToken , randVal , true );
					break;
				}

				case Operation.BITWISE_OR:
				case Operation.BITWISE_AND:
				case Operation.BITWISE_XOR:
				case Operation.BITWISE_NEGATE:
					throw new NotImplementedException( $"Execution not implemented for {this.Type}." );

				case Operation.POWER:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.RaiseTo( this.SecondaryValue ) );
					break;
				}

				case Operation.MODULUS:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Modulus( this.SecondaryValue ) );
					break;
				}

				case Operation.LOGARITHM:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Log( this.SecondaryValue ) );
					break;
				}

				case Operation.ROUND:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Round( ) );
					break;
				}

				case Operation.APPEND:
				{
					this.Program.SetValue( this.PrimaryToken , this.PrimaryValue.Append( this.SecondaryValue ) );
					break;
				}

				case Operation.LIST:
				{
					int size = this.SecondaryValue is IntegerValue intVal ? intVal.Get : throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Program.SetValue( this.PrimaryToken , new ListValue( size ) , true );
					break;
				}

				case Operation.SIZE:
				{
					this.Program.SetValue( this.PrimaryToken , this.SecondaryValue.Size( ) , true );
					break;
				}

				case Operation.OPEN_WRITE:
				case Operation.OPEN_READ:
				{
					bool read = this.Type == Operation.READ;
					string filename = this.SecondaryValue is StringValue strVal ? strVal.Get : throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Program.SetValue( this.PrimaryToken , read ? new ReadFileValue( filename ) : new WriteFileValue( filename ) , true );
					break;
				}

				case Operation.CLOSE:
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

				case Operation.READ:
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

				case Operation.WRITE:
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

				case Operation.INPUT:
				{
					Console.Write(
						this.SecondaryValue is StringValue strVal ? strVal.Get
						: throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue )
					);

					Token token = Lexer.ReadSingleToken( Console.ReadLine( ) ?? string.Empty );
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

						// case TokenType.NULL:
						// 	this.Program.SetValue( this.PrimaryToken , Value.NULL , true );
						// 	break;

						default:
							throw new Exception( "Unexpected data from INPUT return: " + token.ToString( ) );
					}
					break;
				}

				case Operation.PRINT:
				{
					Console.WriteLine( this.PrimaryValue.ToString( ) );
					break;
				}

				case Operation.WAIT:
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

				case Operation.ERROR:
				{
					throw new ErrorStatementException( this.PrimaryValue );
				}

				case Operation.EXIT:
				{
					this.Program.Running = false;
					break;
				}

				case Operation.TEST_EQU:
				case Operation.TEST_NEQ:
				case Operation.TEST_GTR:
				case Operation.TEST_LSS:
				{
					Value result
						= this.Type == Operation.TEST_EQU ? this.PrimaryValue == this.SecondaryValue ? Value.TRUE : Value.FALSE
						: this.Type == Operation.TEST_NEQ ? this.PrimaryValue != this.SecondaryValue ? Value.TRUE : Value.FALSE
						: this.Type == Operation.TEST_GTR ? this.PrimaryValue.GreaterThan( this.SecondaryValue ) ? Value.TRUE : Value.FALSE
						: this.Type == Operation.TEST_LSS ? this.PrimaryValue.LessThan( this.SecondaryValue ) ? Value.TRUE : Value.FALSE
						: throw new Exception( $"Unexpted TEST statement type: {this.Type}" );
					this.Program.SetValue( SpecialVariables.TEST_RESULT , result );
					break;
				}

				case Operation.TYPE:
				{
					string type = this.SecondaryValue.TypeString( )[..^"Value".Length].ToUpper( );
					this.Program.SetValue( this.PrimaryToken , new StringValue( type ) , true );
					break;
				}

				case Operation.SCOPE_OPEN:
				{
					this.Program.OpenScope( );
					break;
				}

				case Operation.SCOPE_CLOSE:
				{
					this.Program.CloseScope( );
					break;
				}

				case Operation.GOTO:
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
