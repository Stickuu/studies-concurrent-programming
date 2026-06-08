using System;
using System.ComponentModel;

namespace Data.Interfaces
{
    public interface IBall : INotifyPropertyChanged, IDisposable
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        double Diameter { get; }
        double Mass { get; }

        void StartMovement(Action syncAction);
    }
}