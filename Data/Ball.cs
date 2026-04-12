using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    internal sealed class Ball(double x, double y, double diameter) : IBall
    {
        private double _x = x;
        private double _y = y;

        public double Diameter { get; } = diameter;
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
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

        public void Move(double boardWidth, double boardHeight)
        {
            var newX = X + VelocityX;
            var newY = Y + VelocityY;

            if (newX <= 0 || newX >= boardWidth - Diameter)
            {
                VelocityX = -VelocityX;
                newX = X + VelocityX;
            }
            
            if (newY <= 0 || newY >= boardHeight - Diameter)
            {
                VelocityY = -VelocityY;
                newY = Y + VelocityY;
            }

            X = newX;
            Y = newY;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

