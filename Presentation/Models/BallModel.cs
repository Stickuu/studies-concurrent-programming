using System.ComponentModel;
using Data;

namespace Presentation.Models
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;

        public double X => _ball.X;
        public double Y => _ball.Y;
        public double Diameter => _ball.Diameter;
        public event PropertyChangedEventHandler? PropertyChanged;

        public BallModel(IBall ball)
        {
            _ball = ball;

            _ball.PropertyChanged += (sender, args) =>
            {
                PropertyChanged?.Invoke(this, args);
            };
        }
    }
}