using System.Collections.Generic;

namespace BNA
{
	public class Compiler
	{
		public int Line
		{
			get; private set;
		}

		public List<string> Lines
		{
			get; private set;
		}

		public Compiler( List<string> lines )
		{
			this.Line = 0;
			this.Lines = lines;
		}

		public Program Compile( )
		{
			// Convert lines to token stream
			Debug.AddLine( "\nTokenizing..." );
			var tokenLines = new List<List<Token>>( );
			tokenLines.Add( Token.TokenizeLine( "" ) ); // start with empty line to zero index lines
			for ( int i = 0 ; i < this.Lines.Count ; i += 1 ) {
				try {
					tokenLines.Add( Token.TokenizeLine( this.Lines[i] ) );
				}
				catch ( CompiletimeException e ) {
					throw new CompiletimeException( e , i , this.Lines[i] );
				}
			}

			// Print all Tokens for debugging
			Debug.AddLine( "\nTokens:" );
			int total = 0;
			for ( int i = 0 ; i < tokenLines.Count ; i += 1 ) {
				Debug.Add( "  Line " + i + ": " );
				foreach ( Token t in tokenLines[i] ) {
					total += 1;
					Debug.Add( t.ToString( ) + " " );
				}
				Debug.AddLine( );
			}
			Debug.AddLine( "" + total + " total from " + tokenLines.Count + " lines" );

			// Parse Statements from Token lines
			Debug.AddLine( "\nParsing..." );
			var statements = new List<Statement>( );
			for ( int i = 0 ; i < tokenLines.Count ; i += 1 ) {
				try {
					statements.Add( Statement.ParseStatement( tokenLines[i] ) );
				}
				catch ( CompiletimeException e ) {
					throw new CompiletimeException( e , i , this.Lines[i] );
				}
			}

			// Print all Statements for debugging
			Debug.AddLine( "\nStatements:" );
			for ( int i = 0 ; i < tokenLines.Count ; i += 1 ) {
				Debug.AddLine( "  Line " + i + ": " + statements[i] );
			}
			Debug.AddLine( "" + statements.Count + " lines" );

			// Create Program object
			return new Program( statements.ToArray( ) );
		}
	}
}
