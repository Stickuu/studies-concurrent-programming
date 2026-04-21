using System.ComponentModel;
using Data;

namespace Logic.Tests.Fakes;

internal class FakeBall : IBall
{
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public double Diameter { get; set; } = 30;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int MoveCount { get; private set; } = 0;

    public void Move(double boardWidth, double boardHeight)
    {
        MoveCount++;
        Position += Velocity;
    }
}
