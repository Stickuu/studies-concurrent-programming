using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged, IDisposable
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        double Diameter { get; }
        double Mass { get; }

        void StartMovement();
    }
}