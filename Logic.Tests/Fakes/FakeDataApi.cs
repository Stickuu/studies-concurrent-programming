using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Tests.Fakes;

internal class FakeDataApi : DataLayerAbstractApi
{
    private readonly List<IBall> _balls = new();

    public override double BoardWidth => 1000.0;
    public override double BoardHeight => 500.0;

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
