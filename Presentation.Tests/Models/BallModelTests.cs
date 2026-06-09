using System.ComponentModel;
using Data.Interfaces;
using Presentation.Models;
using Presentation.Tests.Fakes;
using Xunit;

namespace Presentation.Tests.Models;

public class BallModelTests
{
    [Fact]
    public void BallModelShouldScaleCoordinatesAndDiameterCorrectly()
    {
        var rand = new Random();

        double randomX = rand.NextDouble() * 1000;
        double randomY = rand.NextDouble() * 500;
        double randomDiameter = rand.NextDouble() * 50 + 10;

        double randomScale = rand.NextDouble() * 5 + 0.1;

        var fakeBall = new FakeBall
        {
            Position = new Vector2(randomX, randomY),
            Diameter = randomDiameter
        };

        var ballModel = new BallModel(fakeBall, randomScale);

        Assert.Equal(randomX * randomScale, ballModel.X, 4);
        Assert.Equal(randomY * randomScale, ballModel.Y, 4);
        Assert.Equal(randomDiameter * randomScale, ballModel.Diameter, 4);
    }
}