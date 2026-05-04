using Logic.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Text;
using Data;

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
    public void StartSimulationShouldTriggerStartMovementOnAllBalls()
    {
        _logicApi.CreateBalls(3);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();

        Assert.All(balls, ball => Assert.False(ball.IsMovementStarted));

        _logicApi.StartSimulation();

        Assert.All(balls, ball => Assert.True(ball.IsMovementStarted));
    }

    [Fact]
    public void OnBallMovedShouldSwapVelocitiesWhenTwoBallsOfEqualMassCollide()
    {
        _logicApi.CreateBalls(2);

        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();
        var ball1 = balls[0];
        var ball2 = balls[1];

        ball1.Position = new Vector2(100, 100);
        ball2.Position = new Vector2(120, 100);

        ball1.Velocity = new Vector2(10, 0);
        ball2.Velocity = new Vector2(-10, 0);

        _logicApi.StartSimulation();

        ball1.RaisePositionChanged();

        Assert.Equal(-10, ball1.Velocity.X, 3);
        Assert.Equal(10, ball2.Velocity.X, 3);
    }

    [Fact]
    public void OnBallMovedShouldNotChangeVelocitiesWhenOverlappingBallsMoveAwayFromEachOther()
    {
        _logicApi.CreateBalls(2);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();
        var ball1 = balls[0];
        var ball2 = balls[1];

        ball1.Position = new Vector2(100, 100);
        ball2.Position = new Vector2(120, 100);

        ball1.Velocity = new Vector2(-10, 0);
        ball2.Velocity = new Vector2(10, 0);

        _logicApi.StartSimulation();

        ball1.RaisePositionChanged();

        Assert.Equal(-10, ball1.Velocity.X);
        Assert.Equal(10, ball2.Velocity.X);
    }

    [Fact]
    public void CheckBoundaryCollisionsShouldBounceOffWalls()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();

        var rand = new Random();
        double velX = rand.NextDouble() * 10 + 5;

        ball.Position = new Vector2(_logicApi.BoardWidth - ball.Diameter, 100);
        ball.Velocity = new Vector2(velX, 0);

        _logicApi.StartSimulation();

        ball.RaisePositionChanged();

        Assert.Equal(-velX, ball.Velocity.X);
    }

    [Fact]
    public void CheckBoundaryCollisionsShouldReverseBothVelocitiesWhenHittingCorner()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        ball.Position = new Vector2(0, 0);

        double velX = -(rand.NextDouble() * 50 + 10);
        double velY = -(rand.NextDouble() * 50 + 10);
        ball.Velocity = new Vector2(velX, velY);

        _logicApi.StartSimulation();

        ball.RaisePositionChanged();

        Assert.Equal(-velX, ball.Velocity.X);
        Assert.Equal(-velY, ball.Velocity.Y);
    }

    [Fact]
    public void CheckBoundaryCollisionsShouldReverseXVelocityWhenHittingRightWall()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double velX = rand.NextDouble() * 50 + 10;
        double velY = rand.NextDouble() * 10 - 5;

        ball.Position = new Vector2(_logicApi.BoardWidth - ball.Diameter, 100);
        ball.Velocity = new Vector2(velX, velY);

        _logicApi.StartSimulation();

        ball.RaisePositionChanged();

        Assert.Equal(-velX, ball.Velocity.X);
        Assert.Equal(velY, ball.Velocity.Y);
    }

    [Fact]
    public void CheckBoundaryCollisionsShouldReverseXVelocityWhenHittingLeftWall()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double velX = -(rand.NextDouble() * 50 + 10);
        double velY = rand.NextDouble() * 10 - 5;

        ball.Position = new Vector2(0, 100);
        ball.Velocity = new Vector2(velX, velY);

        _logicApi.StartSimulation();
        ball.RaisePositionChanged();

        Assert.Equal(-velX, ball.Velocity.X);
        Assert.Equal(velY, ball.Velocity.Y);
    }

    [Fact]
    public void CheckBoundaryCollisionsShouldReverseYVelocityWhenHittingBottomWall()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double velX = rand.NextDouble() * 10 - 5;
        double velY = rand.NextDouble() * 50 + 10;

        ball.Position = new Vector2(100, _logicApi.BoardHeight - ball.Diameter);
        ball.Velocity = new Vector2(velX, velY);

        _logicApi.StartSimulation();
        ball.RaisePositionChanged();

        Assert.Equal(velX, ball.Velocity.X);
        Assert.Equal(-velY, ball.Velocity.Y);
    }

    [Fact]
    public void CheckBoundaryCollisionsShouldClampPositionWhenPlacedFarOutOfBounds()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double farX = _logicApi.BoardWidth + rand.NextDouble() * 4000 + 1000;
        double farY = _logicApi.BoardHeight + rand.NextDouble() * 4000 + 1000;

        ball.Position = new Vector2(farX, farY);
        ball.Velocity = new Vector2(10, 10);

        _logicApi.StartSimulation();
        ball.RaisePositionChanged();

        double expectedMaxX = _logicApi.BoardWidth - ball.Diameter;
        double expectedMaxY = _logicApi.BoardHeight - ball.Diameter;

        Assert.Equal(expectedMaxX, ball.Position.X, 5);
        Assert.Equal(expectedMaxY, ball.Position.Y, 5);

        Assert.True(ball.Velocity.X < 0);
        Assert.True(ball.Velocity.Y < 0);
    }

    [Fact]
    public void StopSimulationShouldUnhookEventsSoCollisionsAreNotChecked()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();

        ball.Position = new Vector2(0, 0);
        ball.Velocity = new Vector2(-10, -10);

        _logicApi.StartSimulation();
        _logicApi.StopSimulation();

        ball.RaisePositionChanged();

        Assert.Equal(-10, ball.Velocity.X);
        Assert.Equal(-10, ball.Velocity.Y);
    }
}
