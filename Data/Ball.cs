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

        public void Move(double boardWidth, double boardHeight)
        {
            var newPos = Position + Velocity;
            bool hitWall = false;

            if (newPos.X <= 0)
            {
                newPos = new Vector2(0, newPos.Y);
                hitWall = true;
            } else if (newPos.X >= boardWidth - Diameter)
            {
                newPos = new Vector2(boardWidth - Diameter, newPos.Y);
                hitWall = true;
            }

            if (newPos.Y <= 0)
            {
                newPos = new Vector2(newPos.X, 0);
                hitWall = true;
            } else if (newPos.Y >= boardHeight - Diameter)
            {
                newPos = new Vector2(newPos.X, boardHeight - Diameter);
                hitWall = true;
            }

            if (hitWall)
            {
                Velocity = GenerateRandomVelocity(newPos, boardWidth, boardHeight);
            }

            Position = newPos;

            // var newPos = Position + Velocity;
            // var newVel = Velocity;
            //
            // if (newPos.X <= 0 || newPos.X >= boardWidth - Diameter)
            // {
            //     newVel = new Vector2(-Velocity.X, Velocity.Y);
            //     newPos = new Vector2(Position.X + newVel.X, Position.Y);
            // }
            //
            // if (newPos.Y <= 0 || newPos.Y >= boardHeight - Diameter)
            // {
            //     newVel = new Vector2(newVel.X, -Velocity.Y);
            //     newPos = new Vector2(newPos.X, Position.Y + newVel.Y);
            // }
            //
            // Velocity = newVel;
            // Position = newPos;
        }

        private Vector2 GenerateRandomVelocity(Vector2 currentPosition, double boardWidth, double boardHeight)
        {
            var speedX = (Random.Shared.NextDouble() * 2.0) + 0.5;
            var speedY = (Random.Shared.NextDouble() * 2.0) + 0.5;

            // force right
            if (currentPosition.X <= 0) speedX = Math.Abs(speedX);
            else if (currentPosition.X >= boardWidth - Diameter)
            {
                // force left
                speedX = -Math.Abs(speedX);
            }
            else
            {
                speedX = Random.Shared.NextDouble() > 0.5 ? speedX : -speedX;
            }
            
            // force down
            if (currentPosition.Y <= 0) speedY = Math.Abs(speedX);
            else if (currentPosition.Y >= boardHeight - Diameter)
            {
                // force up
                speedY = -Math.Abs(speedY);
            }
            else
            {
                speedY = Random.Shared.NextDouble() > 0.5 ? speedY : -speedY;
            }

            return new Vector2(speedX, speedY);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

