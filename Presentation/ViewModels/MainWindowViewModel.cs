using System.Collections.ObjectModel;
using Presentation.Models;
using CommunityToolkit.Mvvm.Input;

namespace Presentation.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly SimulationModel _simulationModel;
    private int _ballsCountToCreate = 5;
    private bool _isSimulationRunning;

    public ObservableCollection<BallModel> Balls { get; } = [];
    public IRelayCommand StartSimulationCommand { get; }
    public IRelayCommand StopSimulationCommand { get; }

    public MainWindowViewModel()
    {
        _simulationModel = new SimulationModel(800.0, 400.0);
        StartSimulationCommand = new RelayCommand(StartSimulation, () => !_isSimulationRunning);
        StopSimulationCommand = new RelayCommand(StopSimulation, () => _isSimulationRunning);
    }

    public int BallsCountToCreate
    {
        get => _ballsCountToCreate;
        set
        {
            if (_ballsCountToCreate == value) return;
            
            _ballsCountToCreate = value;
            OnPropertyChanged(nameof(BallsCountToCreate));
        }
    }

    private void StartSimulation()
    {
        Balls.Clear();
        _simulationModel.Start(BallsCountToCreate);

        foreach (var ballModel in _simulationModel.GetBalls())
        {
            Balls.Add(ballModel);
        }

        ChangeSimulationRunningStatus(true);
    }

    private void StopSimulation()
    {
        _simulationModel.Stop();
        Balls.Clear();
        ChangeSimulationRunningStatus(false);
    }

    private void ChangeSimulationRunningStatus(bool status)
    {
        _isSimulationRunning = status;
        StartSimulationCommand.NotifyCanExecuteChanged();
        StopSimulationCommand.NotifyCanExecuteChanged();
    }
}