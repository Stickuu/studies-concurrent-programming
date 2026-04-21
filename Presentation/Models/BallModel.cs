using System.ComponentModel;
using Data;

namespace Presentation.Models
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;
        private readonly double _scale;

        public double X => _ball.Position.X * _scale;
        public double Y => _ball.Position.Y * _scale;
        public double Diameter => _ball.Diameter * _scale;
        public event PropertyChangedEventHandler? PropertyChanged;

        public BallModel(IBall ball, double scale)
        {
            _ball = ball;
            _scale = scale;

            _ball.PropertyChanged += OnBallPropertyChanged;
        }

        private void OnBallPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IBall.Position)) return;
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
        }
            
        
    }
}