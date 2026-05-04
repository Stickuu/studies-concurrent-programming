using System.ComponentModel;
using Data;

namespace Presentation.Tests.Fakes;

internal class FakeBall : IBall
{
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public double Diameter { get; set; } = 30;
    public double Mass { get; set; } = 10;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void StartMovement() { }
    public void Dispose() { }

    public void RaisePositionChanged(double x, double y)
    {
        Position = new Vector2(x, y);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
    }
}