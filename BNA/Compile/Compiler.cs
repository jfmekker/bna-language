using BNA.Common;
using BNA.Exceptions;
using System;
using System.Collections.Generic;

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

        private readonly List<List<Token>> _tokenLines;

        private readonly List<Statement> _statements;

        public Compiler( IReadOnlyCollection<string> lines )
        {
            // Start with empty line to one-index lines and avoid 0-line programs
            this.Line = 0;
            this.Lines = [string.Empty, .. lines];

            this._tokenLines = [];
            this._statements = [];
        }

        public Statement[] Compile( )
        {
            this.ParseTokens( );

            this.DebugPrintTokens( );

            this.ParseStatements( );

            this.DebugPrintStatements( );

            return [.. this._statements];
        }

        private void ParseTokens( )
        {
            Debug.AddLine( "\nTokenizing..." );
            for ( int i = 0 ; i < this.Lines.Count ; i += 1 )
            {
                Lexer lexer = new( this.Lines[i] );
                try
                {
                    this._tokenLines.Add( [.. lexer.ReadTokens( )] );
                }
                catch ( CompiletimeException e )
                {
                    throw new CompiletimeException( i, lexer.Index, this.Lines[i], e );
                }
            }
        }

        private void ParseStatements( )
        {
            Debug.AddLine( "\nParsing..." );
            for ( int i = 0 ; i < this._tokenLines.Count ; i += 1 )
            {
                Parser parser = new( this.Lines[i], this._tokenLines[i] );
                try
                {
                    this._statements.Add( parser.ParseStatement( ) );
                    // this.statements.Add( Statement.ParseStatement( tokenLines[i] ) );
                }
                catch ( Exception e )
                {
                    if ( e is UnexpectedSymbolException
                          or MissingTerminatorException
                          or IllegalTokenException
                          or InvalidTokenException
                          or MissingTokenException )
                    {
                        throw new CompiletimeException( i, parser.RawIndex, parser.RawLine, e );
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private void DebugPrintTokens( )
        {
            Debug.AddLine( "\nTokens:" );
            int total = 0;
            for ( int i = 1 ; i < this._tokenLines.Count ; i += 1 )
            {
                Debug.Add( "  Line " + i + ": " );
                foreach ( Token t in this._tokenLines[i] )
                {
                    total += 1;
                    Debug.Add( t.ToString( ) + " " );
                }
                Debug.AddLine( );
            }
            Debug.AddLine( "" + total + " total from " + (this._tokenLines.Count - 1) + " lines" );
        }

        private void DebugPrintStatements( )
        {
            Debug.AddLine( "\nStatements:" );
            for ( int i = 1 ; i < this._statements.Count ; i += 1 )
            {
                Debug.AddLine( "  Line " + i + ": " + this._statements[i] );
            }
            Debug.AddLine( "" + (this._statements.Count - 1) + " lines" );
        }
    }
}
