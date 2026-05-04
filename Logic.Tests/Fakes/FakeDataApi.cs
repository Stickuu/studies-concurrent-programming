using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Tests.Fakes;

internal class FakeDataApi : DataLayerAbstractApi
{
    private readonly List<IBall> _balls = new();

    public override IBoard Board { get; } = new FakeBoard();

    public double BoardWidth => Board.Width;
    public double BoardHeight => Board.Height;

    public override IBall CreateBall()
    {
        var fakeBall = new FakeBall();
        _balls.Add(fakeBall);
        return fakeBall;
    }

    public override IEnumerable<IBall> GetBalls()
    {
        return _balls;
    }

    public override void RemoveAllBalls()
    {
        _balls.Clear();
    }
}
