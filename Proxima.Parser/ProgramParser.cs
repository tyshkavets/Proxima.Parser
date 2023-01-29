using System.Collections.Immutable;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Proxima.Parser;

public class ProgramParser
{   
    private static Parser<char, T> Tok<T>(Parser<char, T> parser)
        => parser.Before(SkipWhitespacesNoLineBreaks);
    private static Parser<char, char> Tok(char value)
        => Tok(Char(value));
    private static Parser<char, string> Tok(string value)
        => Tok(String(value));
    
    private static Parser<char, T> Keyword<T>(Parser<char, T> parser)
        => Tok(parser);
    private static Parser<char, char> Keyword(char value)
        => Keyword(Char(value));
    private static Parser<char, string> Keyword(string value)
        => Keyword(String(value));
    
    public static Parser<char, Unit> SkipWhitespacesNoLineBreaks { get; }
        = new PredicatedWhitespaceParser(c => c != '\n' && c != '\r');
    
    private static Parser<char, char> _comma = Tok(',');
    private static Parser<char, char> dot = Tok('.');
    private static Parser<char, char> hash = Tok('#');
    private static Parser<char, char> colon = Tok(':');
    private static Parser<char, string> endOfString = String("\r\n").Or(String("\n\r")).Or(String("\n"));// Tok("__ENDOFLINE__");
    
    private static Parser<char, ImmutableArray<T>> CommaSeparated<T>(Parser<char, T> p)
        => p.Separated(_comma).Select(x => x.ToImmutableArray());

    private static Parser<char, string> operation = Keyword("mov").Or(Keyword("add"));
    private static Parser<char, string> parameter = Keyword("ax").Or(Keyword("bx"));
    private static Parser<char, ImmutableArray<string>> ParameterList = CommaSeparated(parameter);

    private static Parser<char, string> identifier =
        Token(c => char.IsLetterOrDigit(c)).Many().Select(cs => new string(cs.ToArray())).Before(SkipWhitespacesNoLineBreaks);

    private static Parser<char, string> commentString =
        Token(c => c != '#' && !Environment.NewLine.Contains(c)).Many().Select(cs => new string(cs.ToArray()));

    private static Parser<char, string> fullComment = Map((h, x) => x, hash, new CommentParser());

    private static Parser<char, string> label = Map((h, i, t) => i, dot, identifier, colon).Before(SkipWhitespacesNoLineBreaks);

    private static Parser<char, Line> line = Map(
        (label, head, body, comment) => new Line(
            label.HasValue ? label.Value : default,
            head.HasValue ? head.Value : default,
            body.HasValue ? body.Value : default,
            comment.HasValue ? comment.Value : default),
        label.Optional(),
        operation.Optional(),
        ParameterList.Optional(),
        fullComment.Optional()
    );

    private static Parser<char, Program> program = line.Separated(endOfString).Select(x => new Program(x.ToImmutableArray()));

    public static Line ParseLine(string input) => line.ParseOrThrow(input);
    
    public static Program ParseProgram(string input) => program.ParseOrThrow(input
    //    .Replace(Environment.NewLine, Environment.NewLine + "__ENDOFLINE__")
    );
}

public record Program(
    ImmutableArray<Line> Lines
);

public record Line(string Label, string Operation, ImmutableArray<string> Parameters, string Comment);