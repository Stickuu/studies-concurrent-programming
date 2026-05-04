using System.Linq;
using Presentation.Models;
using Presentation.Tests.Fakes;
using Xunit;

namespace Presentation.Tests.Models;

public class SimulationModelTests
{
    [Fact]
    public void StartShouldClearCreateAndStartLogicSimulation()
    {
        var fakeApi = new FakeLogicApi();
        var simulationModel = new SimulationModel(500, 250, fakeApi);

        simulationModel.Start(5);

        Assert.True(fakeApi.AreAllBallsRemoved);
        Assert.Equal(5, fakeApi.CreatedBallsCount);
        Assert.True(fakeApi.IsSimulationStarted);
    }

    [Fact]
    public void GetBallsShouldReturnScaledBallModels()
    {
        var fakeApi = new FakeLogicApi();
        var simulationModel = new SimulationModel(500, 250, fakeApi);

        fakeApi.CreateBalls(1);
        var fakeBall = (FakeBall)fakeApi.GetBalls().First();
        fakeBall.Position = new Data.Vector2(200, 100);
        fakeBall.Diameter = 40;

        var ballModels = simulationModel.GetBalls().ToList();

        Assert.Single(ballModels);
        var model = ballModels[0];

        Assert.Equal(100, model.X);
        Assert.Equal(50, model.Y);
        Assert.Equal(20, model.Diameter);
    }
}