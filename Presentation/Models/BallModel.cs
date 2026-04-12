using System.ComponentModel;
using Data;

namespace Presentation.Models
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;
        private readonly double _scale;

        public double X => _ball.X;
        public double Y => _ball.Y;
        public double Diameter => _ball.Diameter;
        public event PropertyChangedEventHandler? PropertyChanged;

        public BallModel(IBall ball, double scale)
        {
            _ball = ball;
            _scale = scale;

            _ball.PropertyChanged += (sender, args) =>
            {
                PropertyChanged?.Invoke(this, args);
            };
        }
    }
}