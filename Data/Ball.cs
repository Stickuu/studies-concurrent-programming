using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    internal sealed class Ball(double x, double y, double diameter) : IBall
    {
        private double _x = x;
        private double _y = y;

        public double Diameter { get; } = diameter;
        public event PropertyChangedEventHandler? PropertyChanged;

        public double X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                
                _x = value;
                OnPropertyChanged();
            }
        }
        
        public double Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

