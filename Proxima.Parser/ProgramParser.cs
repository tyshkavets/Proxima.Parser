using System.Collections.Immutable;
using Pidgin;
using Proxima.Parser.ParseResults;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Proxima.Parser.BaseParsers.TokenParsers;

namespace Proxima.Parser;

public class ProgramParser
{
    private static Parser<char, T> Keyword<T>(Parser<char, T> parser)
        => Tok(parser);
    private static Parser<char, char> Keyword(char value)
        => Keyword(Char(value));
    private static Parser<char, string> Keyword(string value)
        => Keyword(String(value));

    private static Parser<char, string> operation = Keyword("mov").Or(Keyword("add"));
    private static Parser<char, string> parameter = Keyword("ax").Or(Keyword("bx"));
    private static Parser<char, ImmutableArray<string>> ParameterList = CommaSeparated(parameter);

    private static Parser<char, string> identifier =
        Token(c => char.IsLetterOrDigit(c)).Many().Select(cs => new string(cs.ToArray())).Before(SkipWhitespacesNoLineBreaks);

    private static Parser<char, string> fullComment = Map((h, x) => x, Hash, new CommentParser());

    private static Parser<char, string> label = Map((h, i, t) => i, Dot, identifier, Colon).Before(SkipWhitespacesNoLineBreaks);

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

    private static Parser<char, Program> program = line.Separated(AllEndOfStringVariants).Select(x => new Program(x.ToImmutableArray()));

    public static Program ParseProgram(string input) => program.ParseOrThrow(input);
}