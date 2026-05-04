using Data;

namespace Logic.Tests.Fakes;

internal class FakeBoard : IBoard
{
    public double Width { get; } = 1000.0;
    public double Height { get; } = 500.0;
}