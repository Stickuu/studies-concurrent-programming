using Data;
using Newtonsoft.Json.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    public void PositionProperty_ShouldRaisePropertyChangedEvent_WhenChanged()
    {
        var ball = _dataApi.CreateBall();
        bool eventRaised = false;

        ball.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ball.Position))
            {
                eventRaised = true;
            }
        };

        ball.Position = new Vector2(100, 100);

        Assert.True(eventRaised, "Zdarzenie PropertyChanged nie zostało wywołane dla zmiany pozycji!");
    }

    [Fact]
    public void PositionProperty_ShouldNotRaiseEvent_WhenValueIsTheSame()
    {
        var ball = _dataApi.CreateBall();
        ball.Position = new Vector2(50, 50);

        bool eventRaised = false;
        ball.PropertyChanged += (sender, e) => eventRaised = true;

        ball.Position = new Vector2(50, 50);

        Assert.False(eventRaised, "Zdarzenie wywołało się niepotrzebnie dla tej samej wartości!");
    }
}