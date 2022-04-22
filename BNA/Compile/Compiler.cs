//using System;
using System.Collections.Generic;
using System.Linq;
using BNA.Common;
using BNA.Exceptions;

namespace BNA.Compile
{
	public class Compiler
	{
		public int Line
		{
			get; private set;
		}

		public IReadOnlyList<string> Lines
		{
			get; init;
		}

		private readonly List<List<Token>> tokenLines;

		private readonly List<Statement> statements;

		public Compiler( ICollection<string> lines )
		{
			// Start with empty line to one-index lines and avoid 0-line programs
			this.Line = 0;
			this.Lines = lines.Prepend( string.Empty ).ToList( );

			this.tokenLines = new( );
			this.statements = new( );
		}

		public Statement[] Compile( )
		{
			this.ParseTokens( );

			this.DebugPrintTokens( );

			this.ParseStatements( );

			this.DebugPrintStatements( );

			return this.statements.ToArray( );
		}

		private void ParseTokens()
		{
			Debug.AddLine( "\nTokenizing..." );
			for ( int i = 0 ; i < this.Lines.Count ; i += 1 )
			{
				Lexer lexer = new( this.Lines[i] );
				try
				{
					this.tokenLines.Add( lexer.ReadTokens( ) );
				}
				catch ( CompiletimeException e )
				{
					throw new CompiletimeException( i , lexer.Index , this.Lines[i] , e );
				}
			}
		}

		private void ParseStatements( )
		{
			Debug.AddLine( "\nParsing..." );
			for ( int i = 0 ; i < this.tokenLines.Count ; i += 1 )
			{
				Parser parser = new( this.Lines[i] , this.tokenLines[i] );
				try
				{
					this.statements.Add( parser.ParseStatement( ) );
					// this.statements.Add( Statement.ParseStatement( tokenLines[i] ) );
				}
				catch ( System.Exception e )
				when (e is UnexpectedSymbolException
						or MissingTerminatorException
						or IllegalTokenException
						or InvalidTokenException
						or MissingTokenException)
				{
					throw new CompiletimeException( i , parser.RawIndex , parser.RawLine , e );
				}
			}
		}

		private void DebugPrintTokens( )
		{
			Debug.AddLine( "\nTokens:" );
			int total = 0;
			for ( int i = 1 ; i < this.tokenLines.Count ; i += 1 )
			{
				Debug.Add( "  Line " + i + ": " );
				foreach ( Token t in this.tokenLines[i] )
				{
					total += 1;
					Debug.Add( t.ToString( ) + " " );
				}
				Debug.AddLine( );
			}
			Debug.AddLine( "" + total + " total from " + ( this.tokenLines.Count - 1 ) + " lines" );
		}

		private void DebugPrintStatements( )
		{
			Debug.AddLine( "\nStatements:" );
			for ( int i = 1 ; i < this.statements.Count ; i += 1 )
			{
				Debug.AddLine( "  Line " + i + ": " + this.statements[i] );
			}
			Debug.AddLine( "" + ( this.statements.Count - 1 ) + " lines" );
		}
	}
}
