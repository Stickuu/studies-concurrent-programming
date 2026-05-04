using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    internal sealed class Ball : IBall
    {
        private Vector2 _position;
        private Vector2 _velocity;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _lockObject = new();
        private bool _isRunning;

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
                lock (_lockObject) return _position;
            }
            set
            {
                bool hasChanged = false;

                lock (_lockObject)
                {
                    if (_position != value)
                    {
                        _position = value;
                        hasChanged = true;
                    }
                }

                if (hasChanged)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
                }
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

        public void StartMovement()
        {
            if (_isRunning) return;
            _isRunning = true;

            Task.Run(() => MoveLoop(_cancellationTokenSource.Token));
        }

        private async Task MoveLoop(CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            double lastTime = stopwatch.Elapsed.TotalSeconds;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    double currentTime = stopwatch.Elapsed.TotalSeconds;
                    double deltaTime = currentTime - lastTime;
                    lastTime = currentTime;

                    Vector2 currentVelocity = Velocity;
                    Vector2 currentPosition = Position;

                    double newX = currentPosition.X + (currentVelocity.X * deltaTime);
                    double newY = currentPosition.Y + (currentVelocity.Y * deltaTime);

                    Position = new Vector2(newX, newY);

                    await Task.Delay(16, token);
                }
            }
            catch (TaskCanceledException) { }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

