using Logic.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Tests;

public class LogicLayerApiTests
{
    private readonly LogicLayerAbstractApi _logicApi;
    private readonly FakeDataApi _fakeDataApi;

    public LogicLayerApiTests()
    {
        _fakeDataApi = new FakeDataApi();
        _logicApi = LogicLayerAbstractApi.GetInstance(_fakeDataApi);
        _logicApi.RemoveAllBalls();
    }

    [Fact]
    public void BoardDimensionsShouldMatchInjectedDataLayerDimensions()
    {
        Assert.Equal(1000.0, _logicApi.BoardWidth);
        Assert.Equal(500.0, _logicApi.BoardHeight);
    }

    [Fact]
    public void CreateBallsShouldGenerateCorrectNumberOfBalls()
    {
        _logicApi.CreateBalls(5);

        Assert.Equal(5, _logicApi.GetBalls().Count());
    }

    [Fact]
    public void RemoveAllBallsShouldClearAllBalls()
    {
        _logicApi.CreateBalls(3);

        Assert.Equal(3, _logicApi.GetBalls().Count());

        _logicApi.RemoveAllBalls();

        Assert.Empty(_logicApi.GetBalls());
    }

    [Fact]
    public async Task SimulationLoopShouldCallMoveOnBallsConcurrently()
    {
        _logicApi.CreateBalls(2);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();

        Assert.Equal(0, balls[0].MoveCount);
        Assert.Equal(0, balls[1].MoveCount);

        _logicApi.StartSimulation();

        await Task.Delay(100);

        _logicApi.StopSimulation();

        await Task<int>.Delay(100);

        Assert.True(balls[0].MoveCount > 0);
        Assert.True(balls[1].MoveCount > 0);
    }
}
