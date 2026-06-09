using Logic.Tests.Fakes;
using Data.Interfaces;

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
    public async Task OnBallMovedShouldSwapVelocitiesWhenTwoBallsOfEqualMassCollide()
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

        await Task.Delay(50);

        _logicApi.StopSimulation();

        Assert.Equal(-10, ball1.Velocity.X, 3);
        Assert.Equal(10, ball2.Velocity.X, 3);
    }

    [Fact]
    public async Task OnBallMovedShouldNotChangeVelocitiesWhenOverlappingBallsMoveAwayFromEachOther()
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

        await Task.Delay(50);

        _logicApi.StopSimulation();

        Assert.Equal(-10, ball1.Velocity.X);
        Assert.Equal(10, ball2.Velocity.X);
    }

    [Fact]
    public async Task CheckBoundaryCollisionsShouldBounceOffWalls()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();

        var rand = new Random();
        double velX = rand.NextDouble() * 10 + 5;

        ball.Position = new Vector2(_logicApi.BoardWidth - ball.Diameter, 100);
        ball.Velocity = new Vector2(velX, 0);

        _logicApi.StartSimulation();

        await Task.Delay(50);

        _logicApi.StopSimulation();

        Assert.Equal(-velX, ball.Velocity.X);
    }

    [Fact]
    public async Task CheckBoundaryCollisionsShouldReverseBothVelocitiesWhenHittingCorner()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        ball.Position = new Vector2(0, 0);
        var velocity = -(rand.NextDouble() * 50 + 10);
        ball.Velocity = new Vector2(velocity, velocity);

        _logicApi.StartSimulation();

        await Task.Delay(50);

        _logicApi.StopSimulation();

        Assert.Equal(-velocity, ball.Velocity.X);
        Assert.Equal(-velocity, ball.Velocity.Y);
    }

    [Fact]
    public async Task CheckBoundaryCollisionsShouldReverseXVelocityWhenHittingRightWall()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double velX = rand.NextDouble() * 50 + 10;
        double velY = rand.NextDouble() * 10 - 5;

        ball.Position = new Vector2(_logicApi.BoardWidth - ball.Diameter, 100);
        ball.Velocity = new Vector2(velX, velY);

        _logicApi.StartSimulation();

        await Task.Delay(50);

        _logicApi.StopSimulation();

        Assert.Equal(-velX, ball.Velocity.X);
        Assert.Equal(velY, ball.Velocity.Y);
    }

    [Fact]
    public async Task CheckBoundaryCollisionsShouldReverseXVelocityWhenHittingLeftWall()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double velX = -(rand.NextDouble() * 50 + 10);
        double velY = rand.NextDouble() * 10 - 5;

        ball.Position = new Vector2(0, 100);
        ball.Velocity = new Vector2(velX, velY);

        _logicApi.StartSimulation();

        await Task.Delay(50);

        _logicApi.StopSimulation();

        Assert.Equal(-velX, ball.Velocity.X);
        Assert.Equal(velY, ball.Velocity.Y);
    }

    [Fact]
    public async Task CheckBoundaryCollisionsShouldReverseYVelocityWhenHittingBottomWall()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double velX = rand.NextDouble() * 10 - 5;
        double velY = rand.NextDouble() * 50 + 10;

        ball.Position = new Vector2(100, _logicApi.BoardHeight - ball.Diameter);
        ball.Velocity = new Vector2(velX, velY);

        _logicApi.StartSimulation();

        await Task.Delay(50);

        _logicApi.StopSimulation();

        Assert.Equal(velX, ball.Velocity.X);
        Assert.Equal(-velY, ball.Velocity.Y);
    }

    [Fact]
    public async Task CheckBoundaryCollisionsShouldClampPositionWhenPlacedFarOutOfBounds()
    {
        _logicApi.CreateBalls(1);
        var ball = (FakeBall)_logicApi.GetBalls().First();
        var rand = new Random();

        double farX = _logicApi.BoardWidth + rand.NextDouble() * 4000 + 1000;
        double farY = _logicApi.BoardHeight + rand.NextDouble() * 4000 + 1000;

        ball.Position = new Vector2(farX, farY);
        ball.Velocity = new Vector2(10, 10);

        _logicApi.StartSimulation();

        await Task.Delay(50);

        _logicApi.StopSimulation();

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

    [Fact]
    public async Task StartSimulationWith100BallsShouldSynchronizeMovements()
    {
        int ballCount = 100;
        _logicApi.CreateBalls(ballCount);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();

        _logicApi.StartSimulation();

        await Task.Delay(300);

        _logicApi.StopSimulation();

        foreach (var ball in balls)
        {
            ball.Dispose();
        }

        await Task.Delay(50);

        var baseMoveCount = balls.First().MoveCount;

        Assert.True(baseMoveCount > 0, "Kule w ogóle się nie poruszyły - bariera zatrzymała je na zawsze.");

        foreach (var ball in balls)
        {
            int diff = Math.Abs(ball.MoveCount - baseMoveCount);

            Assert.True(diff <= 1,
                $"Desynchronizacja! Kulka Id: {ball.Id} poruszyła się {ball.MoveCount} razy, podczas gdy baza to {baseMoveCount}.");
        }
    }

    [Fact]
    public async Task CheckBallCollisionsRandom2DCollisionShouldConserveMomentumAndKineticEnergy()
    {
        _logicApi.CreateBalls(2);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();
        var rand = new Random();

        balls[0].Mass = rand.NextDouble() * 15 + 5;
        balls[1].Mass = rand.NextDouble() * 15 + 5;
        balls[0].Diameter = 30;
        balls[1].Diameter = 30;

        balls[0].Position = new Vector2(100, 100);
        balls[1].Position = new Vector2(115, 120);

        balls[0].Velocity = new Vector2(rand.NextDouble() * 10 + 5, rand.NextDouble() * 10 + 5);
        balls[1].Velocity = new Vector2(-(rand.NextDouble() * 10 + 5), -(rand.NextDouble() * 10 + 5));

        var initialMomentumX = (balls[0].Mass * balls[0].Velocity.X) + (balls[1].Mass * balls[1].Velocity.X);
        var initialMomentumY = (balls[0].Mass * balls[0].Velocity.Y) + (balls[1].Mass * balls[1].Velocity.Y);

        var speed1Sq = (balls[0].Velocity.X * balls[0].Velocity.X) + (balls[0].Velocity.Y * balls[0].Velocity.Y);
        var speed2Sq = (balls[1].Velocity.X * balls[1].Velocity.X) + (balls[1].Velocity.Y * balls[1].Velocity.Y);
        var initialEnergy = 0.5 * balls[0].Mass * speed1Sq + 0.5 * balls[1].Mass * speed2Sq;

        _logicApi.StartSimulation();
        await Task.Delay(50);
        _logicApi.StopSimulation();

        var finalMomentumX = (balls[0].Mass * balls[0].Velocity.X) + (balls[1].Mass * balls[1].Velocity.X);
        var finalMomentumY = (balls[0].Mass * balls[0].Velocity.Y) + (balls[1].Mass * balls[1].Velocity.Y);

        var fSpeed1Sq = (balls[0].Velocity.X * balls[0].Velocity.X) + (balls[0].Velocity.Y * balls[0].Velocity.Y);
        var fSpeed2Sq = (balls[1].Velocity.X * balls[1].Velocity.X) + (balls[1].Velocity.Y * balls[1].Velocity.Y);
        var finalEnergy = 0.5 * balls[0].Mass * fSpeed1Sq + 0.5 * balls[1].Mass * fSpeed2Sq;

        Assert.Equal(initialMomentumX, finalMomentumX, 3);
        Assert.Equal(initialMomentumY, finalMomentumY, 3);
        Assert.Equal(initialEnergy, finalEnergy, 3);
    }

    [Fact]
    public async Task CheckBallCollisionsHeadOnEqualMassShouldSwapVelocities()
    {
        _logicApi.CreateBalls(2);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();
        var rand = new Random();

        double randomSpeed = rand.NextDouble() * 30 + 10;

        balls[0].Position = new Vector2(100, 100);
        balls[0].Velocity = new Vector2(randomSpeed, 0);
        balls[0].Mass = 10;

        balls[1].Position = new Vector2(125, 100);
        balls[1].Velocity = new Vector2(-randomSpeed, 0);
        balls[1].Mass = 10;

        _logicApi.StartSimulation();
        await Task.Delay(50);
        _logicApi.StopSimulation();

        Assert.Equal(-randomSpeed, balls[0].Velocity.X, 3);
        Assert.Equal(randomSpeed, balls[1].Velocity.X, 3);
    }

    [Fact]
    public async Task CheckBallCollisionsOverlappingButMovingAwayShouldNotChangeVelocities()
    {
        _logicApi.CreateBalls(2);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();
        var rand = new Random();

        double speedA = -(rand.NextDouble() * 20 + 10);
        double speedB = rand.NextDouble() * 20 + 10;

        balls[0].Position = new Vector2(100, 100);
        balls[0].Velocity = new Vector2(speedA, 0);

        balls[1].Position = new Vector2(110, 100);
        balls[1].Velocity = new Vector2(speedB, 0);

        _logicApi.StartSimulation();
        await Task.Delay(50);
        _logicApi.StopSimulation();

        Assert.Equal(speedA, balls[0].Velocity.X);
        Assert.Equal(speedB, balls[1].Velocity.X);
    }

    [Fact]
    public async Task CheckBallCollisionsThirdBallOutOfRangeShouldNotBeAffected()
    {
        _logicApi.CreateBalls(3);
        var balls = _logicApi.GetBalls().Cast<FakeBall>().ToList();
        var rand = new Random();

        balls[0].Position = new Vector2(100, 100);
        balls[0].Velocity = new Vector2(10, 0);
        balls[1].Position = new Vector2(115, 100);
        balls[1].Velocity = new Vector2(-10, 0);

        var originalVel2 = new Vector2(rand.NextDouble() * 10, -(rand.NextDouble() * 10));
        balls[2].Position = new Vector2(400, 400);
        balls[2].Velocity = originalVel2;

        _logicApi.StartSimulation();
        await Task.Delay(50);
        _logicApi.StopSimulation();

        Assert.NotEqual(10, balls[0].Velocity.X);
        Assert.NotEqual(-10, balls[1].Velocity.X);

        Assert.Equal(originalVel2.X, balls[2].Velocity.X);
        Assert.Equal(originalVel2.Y, balls[2].Velocity.Y);
    }
}
