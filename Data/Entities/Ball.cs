using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Data.Interfaces;

namespace Data.Entities
{
    internal sealed class Ball : IBall
    {
        private static readonly Random _random = new();
        
        private Vector2 _position;
        private Vector2 _velocity;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _lockObject = new();
        private bool _isRunning;

        public int Id { get; }
        public double Diameter { get; }
        public double Mass { get; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public Ball(Vector2 position, double diameter, double mass)
        {
            Id = _random.Next(1000, 10000);
            _position = position;
            Diameter = diameter;
            Mass = mass;

            _cancellationTokenSource = new CancellationTokenSource();
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
                    OnPropertyChanged();
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

        public void StartMovement(Action syncAction)
        {
            if (_isRunning) return;
            _isRunning = true;

            Task.Factory.StartNew(
                () => MoveLoop(syncAction, _cancellationTokenSource.Token),
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
        }

        private void MoveLoop( Action syncAction, CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            var lastTime = stopwatch.Elapsed.TotalSeconds;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var currentTime = stopwatch.Elapsed.TotalSeconds;
                    var deltaTime = currentTime - lastTime;
                    lastTime = currentTime;

                    var currentVelocity = Velocity;
                    var currentPosition = Position;

                    var newX = currentPosition.X + (currentVelocity.X * deltaTime);
                    var newY = currentPosition.Y + (currentVelocity.Y * deltaTime);

                    Position = new Vector2(newX, newY);
                    
                    // Rendezvous point
                    syncAction?.Invoke();
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

