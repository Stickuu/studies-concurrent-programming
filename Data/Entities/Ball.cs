using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    internal sealed class Ball : IBall
    {
        private Vector2 _position;
        public double Diameter { get; }
        public Vector2 Velocity { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public Ball(Vector2 position, double diameter)
        {
            _position = position;
            Diameter = diameter;
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position == value) return;
                
                _position = value;
                OnPropertyChanged();
                
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

