using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        public static int DIAMETER = 30;
        
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        double Diameter { get; }
    }
}