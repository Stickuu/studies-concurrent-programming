using Data;
using Newtonsoft.Json.Bson;
using Xunit;

namespace Data.Tests;

public class BallTests
{
    private readonly DataLayerAbstractApi _dataApi;
    private readonly double _boardWidth = 800.0;
    private readonly double _boardHeight = 400.0;

    private readonly Random _random = new Random();

    public BallTests()
    {
        _dataApi = DataLayerAbstractApi.GetInstance(_boardWidth, _boardHeight);
    }

    [Fact]
    public void MoveShouldChangePositionWhenVelocityIsNonZero()
    {
        var ball = _dataApi.CreateBall();

        double startX = _random.NextDouble() * 600 + 100;
        double startY = _random.NextDouble() * 200 + 100;

        double velX = (_random.NextDouble() * 20) - 10;
        double velY = (_random.NextDouble() * 20) - 10;

        ball.Position = new Vector2(startX, startY);
        ball.Velocity = new Vector2(velX, velY);

        double expectedX = startX + velX;
        double expectedY = startY + velY;

        ball.Move(_boardWidth, _boardHeight);

        Assert.Equal(expectedX, ball.Position.X, 5);
        Assert.Equal(expectedY, ball.Position.Y, 5);
    }

    [Fact]
    public void MoveShouldClampToRightWallAndReverseVelocityWhenHitting()
    {
        var ball = _dataApi.CreateBall();

        double velX = _random.NextDouble() * 15 + 5;
        double velY = (_random.NextDouble() * 10) - 5;

        double boundaryX = _boardWidth - ball.Diameter;

        double startX = boundaryX - (velX * _random.NextDouble() * 0.8 + 0.1);
        double startY = _random.NextDouble() * 200 + 100;

        ball.Position = new Vector2(startX, startY);
        ball.Velocity = new Vector2(velX, velY);

        ball.Move(_boardWidth, _boardHeight);

        Assert.Equal(boundaryX, ball.Position.X, 5);
        Assert.True(ball.Velocity.X < 0);
    }

    [Fact]
    public void MoveShouldClampToLeftWallAndReverseVelocityWhenHitting()
    {
        var ball = _dataApi.CreateBall();

        double velX = -(_random.NextDouble() * 15 + 5);
        double velY = (_random.NextDouble() * 10) - 5;

        double startX = Math.Abs(velX) * _random.NextDouble() * 0.8;
        double startY = _random.NextDouble() * 200 + 100;

        ball.Position = new Vector2(startX, startY);
        ball.Velocity = new Vector2(velX, velY);

        ball.Move(_boardWidth, _boardHeight);

        Assert.Equal(0, ball.Position.X, 5);
        Assert.True(ball.Velocity.X > 0);
    }

    [Fact]
    public void MoveShouldClampToBottomWallAndReverseVelocityWhenHitting()
    {
        var ball = _dataApi.CreateBall();

        double velX = (_random.NextDouble() * 10) - 5;
        double velY = _random.NextDouble() * 15 + 5;

        double boundaryY = _boardHeight - ball.Diameter;

        double startX = _random.NextDouble() * 600 + 100;
        double startY = boundaryY - (velY * _random.NextDouble() * 0.8 + 0.1);

        ball.Position = new Vector2(startX, startY);
        ball.Velocity = new Vector2(velX, velY);

        ball.Move(_boardWidth, _boardHeight);
        
        Assert.Equal(boundaryY, ball.Position.Y, 5);
        Assert.True(ball.Velocity.Y < 0);
    }

    [Fact]
    public void MoveShouldClampToTopWallAndReverseVelocityWhenHitting()
    {
        var ball = _dataApi.CreateBall();

        double velX = (_random.NextDouble() * 10) - 5;
        double velY = -(_random.NextDouble() * 15 + 5);

        double startX = _random.NextDouble() * 600 + 100;
        double startY = Math.Abs(velY) * _random.NextDouble() * 0.8;

        ball.Position = new Vector2(startX, startY);
        ball.Velocity = new Vector2(velX, velY);

        ball.Move(_boardWidth, _boardHeight);

        Assert.Equal(0, ball.Position.Y, 5);
        Assert.True(ball.Velocity.Y > 0);
    }
}