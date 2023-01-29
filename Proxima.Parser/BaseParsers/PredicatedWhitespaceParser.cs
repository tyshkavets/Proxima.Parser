using Pidgin;

namespace Proxima.Parser.BaseParsers;

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