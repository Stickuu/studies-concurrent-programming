using System.Collections.Generic;
using Logic;
using Data;

namespace Presentation.Tests.Fakes;

internal class FakeLogicApi : LogicLayerAbstractApi
{
    private readonly List<IBall> _balls = new();
    public bool IsSimulationStarted { get; private set; }
    public bool IsSimulationStopped { get; private set; }
    public bool AreAllBallsRemoved { get; private set; }
    public int CreatedBallsCount { get; private set; }

    public override double BoardWidth => 1000;
    public override double BoardHeight => 500;

    public override void CreateBalls(int count)
    {
        CreatedBallsCount += count;
        for (int i = 0; i < count; i++) _balls.Add(new FakeBall());
    }

    public override IEnumerable<IBall> GetBalls() => _balls;

    public override void RemoveAllBalls()
    {
        _balls.Clear();
        AreAllBallsRemoved = true;
    }

    public override void StartSimulation() => IsSimulationStarted = true;

    public override void StopSimulation() => IsSimulationStopped = true;
}