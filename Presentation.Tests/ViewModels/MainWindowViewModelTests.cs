using Presentation.Models;
using Presentation.ViewModels;
using Presentation.Tests.Fakes;
using Xunit;
using System.ComponentModel;

namespace Presentation.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void BallsCountToCreateShouldRaisePropertyChanged_WhenChanged()
    {
        var viewModel = new MainWindowViewModel(new SimulationModel(800, 400, new FakeLogicApi()));
        bool propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.BallsCountToCreate))
            {
                propertyChangedRaised = true;
            }
        };

        viewModel.BallsCountToCreate = 10;

        Assert.True(propertyChangedRaised, "ViewModel nie zgłosił zmiany właściwości BallsCountToCreate!");
        Assert.Equal(10, viewModel.BallsCountToCreate);
    }

    [Fact]
    public void StartSimulationCommandShouldPopulateBallsCollectionAndDisableStartButton()
    {
        var fakeLogicApi = new FakeLogicApi();
        var simulationModel = new SimulationModel(800, 400, fakeLogicApi);
        var viewModel = new MainWindowViewModel(simulationModel);

        viewModel.BallsCountToCreate = 5;

        viewModel.StartSimulationCommand.Execute(null);

        Assert.True(fakeLogicApi.IsSimulationStarted);
        Assert.Equal(5, fakeLogicApi.CreatedBallsCount);

        Assert.Equal(5, viewModel.Balls.Count);

        Assert.False(viewModel.StartSimulationCommand.CanExecute(null), "Przycisk Start powinien być zablokowany!");
        Assert.True(viewModel.StopSimulationCommand.CanExecute(null), "Przycisk Stop powinien być odblokowany!");
    }

    [Fact]
    public void StopSimulationCommandShouldClearCollectionAndEnableStartButton()
    {
        var fakeLogicApi = new FakeLogicApi();
        var simulationModel = new SimulationModel(800, 400, fakeLogicApi);
        var viewModel = new MainWindowViewModel(simulationModel);

        viewModel.StartSimulationCommand.Execute(null);

        viewModel.StopSimulationCommand.Execute(null);

        Assert.True(fakeLogicApi.IsSimulationStopped);
        Assert.True(fakeLogicApi.AreAllBallsRemoved);

        Assert.Empty(viewModel.Balls);

        Assert.True(viewModel.StartSimulationCommand.CanExecute(null), "Przycisk Start powinien być znowu aktywny!");
        Assert.False(viewModel.StopSimulationCommand.CanExecute(null), "Przycisk Stop powinien być zablokowany!");
    }
}