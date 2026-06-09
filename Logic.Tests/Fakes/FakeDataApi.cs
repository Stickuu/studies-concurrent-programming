using Data;
using System;
using System.Collections.Generic;
using System.Text;
using Data.Interfaces;

namespace Logic.Tests.Fakes;

internal class FakeDataApi : DataLayerAbstractApi
{
    private readonly List<IBall> _balls = new();

    public override IBoard Board { get; } = new FakeBoard();

    public override IDiagnosticsLogger Logger { get; } = new FakeLogger();

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
        foreach (var ball in _balls)
        {
            ball.Dispose();
        }

        _balls.Clear();
    }
}

internal class FakeLogger : IDiagnosticsLogger
{
    public List<string> LoggedMessages { get; } = new();

    public void LogMessage(string message)
    {
        LoggedMessages.Add(message);
    }
    public void Dispose() {}
}