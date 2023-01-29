using System.Collections.Immutable;

namespace Proxima.Parser.ParseResults;

public record Line(string Label, string Operation, ImmutableArray<string> Parameters, string Comment);