using System.Collections.Immutable;

namespace Proxima.Parser.ParseResults;

public record Program(
    ImmutableArray<Line> Lines
);