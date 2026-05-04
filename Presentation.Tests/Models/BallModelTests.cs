using System.ComponentModel;
using Presentation.Models;
using Presentation.Tests.Fakes;
using Xunit;

namespace Presentation.Tests.Models;

public class BallModelTests
{
    [Fact]
    public void BallModelShouldScaleCoordinatesAndDiameterCorrectly()
    {
        var fakeBall = new FakeBall { Position = new Data.Vector2(100, 50), Diameter = 30 };
        double scale = 0.5;

        var ballModel = new BallModel(fakeBall, scale);

        Assert.Equal(50, ballModel.X);
        Assert.Equal(25, ballModel.Y);
        Assert.Equal(15, ballModel.Diameter);
    }

    [Fact]
    public void BallModelShouldRaisePropertyChangedForXAndYWhenLogicBallMoves()
    {
        var fakeBall = new FakeBall();
        var ballModel = new BallModel(fakeBall, 1.0);

        bool xChanged = false;
        bool yChanged = false;

        ballModel.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ballModel.X)) xChanged = true;
            if (e.PropertyName == nameof(ballModel.Y)) yChanged = true;
        };

        fakeBall.RaisePositionChanged(10, 10);

        Assert.True(xChanged, "Nie wywołano PropertyChanged dla X!");
        Assert.True(yChanged, "Nie wywołano PropertyChanged dla Y!");
    }
}