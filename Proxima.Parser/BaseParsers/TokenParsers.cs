using System.Collections.Immutable;
using Pidgin;
using static Pidgin.Parser;

namespace Proxima.Parser.BaseParsers;

public class TokenParsers
{
    public static Parser<char, Unit> SkipWhitespacesNoLineBreaks { get; }
        = new PredicatedWhitespaceParser(c => c != '\n' && c != '\r');
    
    public static Parser<char, T> Tok<T>(Parser<char, T> parser)
        => parser.Before(SkipWhitespacesNoLineBreaks);
    public static Parser<char, char> Tok(char value)
        => Tok(Char(value));
    public static Parser<char, string> Tok(string value)
        => Tok(String(value));
    
    public static Parser<char, char> Comma = Tok(',');
    public static Parser<char, char> Dot = Tok('.');
    public static Parser<char, char> Hash = Tok('#');
    public static Parser<char, char> Colon = Tok(':');
    public static Parser<char, string> AllEndOfStringVariants = String("\r\n").Or(String("\n\r")).Or(String("\n"));
    
    public static Parser<char, ImmutableArray<T>> CommaSeparated<T>(Parser<char, T> p)
        => p.Separated(Comma).Select(x => x.ToImmutableArray());
}