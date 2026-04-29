using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    internal sealed class Ball : IBall
    {
        private Vector2 _position;
        private Vector2 _velocity;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _lockObject = new();
        
        public double Diameter { get; }
        public double Mass { get; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public Ball(Vector2 position, double diameter, double mass)
        {
            _position = position;
            Diameter = diameter;
            Mass = mass;

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => MoveLoop(_cancellationTokenSource.Token));
        }

        public Vector2 Position
        {
            get
            {
                lock (_lockObject) { return _position; }
            }
            set
            {
                lock (_lockObject)
                {
                    if (_position == value) return;
                    _position = value;
                }

                OnPropertyChanged();
            }
        }

        public Vector2 Velocity
        {
            get
            {
                lock (_lockObject) { return _velocity; }
            }
            set
            {
                lock (_lockObject) { _velocity = value; }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async Task MoveLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Position = new Vector2(Position.X + Velocity.X, Position.Y + Velocity.Y);
                    await Task.Delay(16, token);
                }
            }
            catch (TaskCanceledException)
            {
                
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

