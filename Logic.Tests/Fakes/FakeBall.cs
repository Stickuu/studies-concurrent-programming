using System.ComponentModel;
using Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Logic.Tests.Fakes;

internal class FakeBall : IBall
{
    public int Id { get; } = new Random().Next(1, 10000);
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public double Diameter { get; set; } = 30;
    public double Mass { get; set; } = 10;
    public bool IsMovementStarted { get; private set; } = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int MoveCount { get; private set; } = 0;

    private readonly CancellationTokenSource _cts = new();

    public void StartMovement(Action syncAction) {
        IsMovementStarted = true;

        Task.Run(() =>
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    MoveCount++;
                    syncAction?.Invoke();
                }
            }
            catch (Exception){}
        }, _cts.Token);
    }

    public void Dispose() { _cts.Cancel(); _cts.Dispose(); }

    public void RaisePositionChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
    }
}
