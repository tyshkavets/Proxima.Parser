using System.Text;
using Pidgin;

namespace Proxima.Parser;

public class CommentParser : Parser<char, string>
{
    public override bool TryParse(ref ParseState<char> state, ref PooledList<Expected<char>> expecteds, out string result)
    {
        var cumulativeResult = new StringBuilder();

        var nextSymbol = state.LookAhead(1);

        while (nextSymbol.Length == 1 && nextSymbol[0] != '\r' && nextSymbol[0] != '\n')
        {
            cumulativeResult.Append(nextSymbol[0]);
            state.Advance();
            nextSymbol = state.LookAhead(1);
        }

        result = cumulativeResult.ToString();

        return true;
    }
}

public class PredicatedWhitespaceParser : Parser<char, Unit>
{
    private readonly Func<char, bool> _predicate;

    public PredicatedWhitespaceParser(Func<char, bool> predicate) => _predicate = predicate;

    public override bool TryParse(ref ParseState<char> state, ref PooledList<Expected<char>> expecteds, out Unit result)
    {
        result = Unit.Value;
        var nextSymbol = state.LookAhead(1);
        
        while (nextSymbol.Length == 1 && char.IsWhiteSpace(nextSymbol[0]) && _predicate(nextSymbol[0]))
        {
            state.Advance();
            nextSymbol = state.LookAhead(1);
        }

        return true;
    }
}