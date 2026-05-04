using System.ComponentModel;
using Data;

namespace Logic
{
    internal class LogicLayerApi : LogicLayerAbstractApi
    {
        private static LogicLayerApi? _instance;
        
        private readonly DataLayerAbstractApi _dataApi;
        private readonly object _physicsLock = new();

        public override double BoardWidth => _dataApi.Board.Width;
        public override double BoardHeight => _dataApi.Board.Height;

        public new static LogicLayerApi GetInstance(DataLayerAbstractApi dataApi)
        {
            _instance ??= new LogicLayerApi(dataApi);

            return _instance;
        }
        
        private LogicLayerApi(DataLayerAbstractApi dataApi)
        {
            _dataApi = dataApi;
        }

        public override void CreateBalls(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _dataApi.CreateBall();
            }
        }

        public override void RemoveAllBalls()
        {
            StopSimulation();
            _dataApi.RemoveAllBalls();
        }

        public override IEnumerable<IBall> GetBalls()
        {
            return _dataApi.GetBalls();
        }

        public override void StartSimulation()
        {
            foreach (var ball in _dataApi.GetBalls())
            {
                ball.PropertyChanged += OnBallMoved;
                ball.StartMovement();
            }
        }

        public override void StopSimulation()
        {
            foreach (var ball in _dataApi.GetBalls())
            {
                ball.PropertyChanged -= OnBallMoved;    
            }
        }

        private void OnBallMoved(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IBall.Position) || sender == null) return;

            var ball = (IBall)sender;

            CheckBoundaryCollisions(ball);
            CheckBallCollisions(ball);
        }

        private void CheckBoundaryCollisions(IBall ball)
        {
            var position = ball.Position;
            var velocity = ball.Velocity;
            var board = _dataApi.Board;

            double newVelX = velocity.X;
            double newVelY = velocity.Y;
            bool stateChanged = false;

            if ((position.X <= 0 && velocity.X < 0) || (position.X >= board.Width - ball.Diameter && velocity.X > 0))
            {
                newVelX = -velocity.X;
                stateChanged = true;
            }

            if ((position.Y <= 0 && velocity.Y < 0) || (position.Y >= board.Height - ball.Diameter && velocity.Y > 0))
            {
                newVelY = -velocity.Y;
                stateChanged = true;
            }

            double clampedX = Math.Clamp(position.X, 0, board.Width - ball.Diameter);
            double clampedY = Math.Clamp(position.Y, 0, board.Height - ball.Diameter);

            if (Math.Abs(clampedX - position.X) > 0.001 || Math.Abs(clampedY - position.Y) > 0.001)
            {
                stateChanged = true;
            }

            if (stateChanged)
            {
                ball.Velocity = new Vector2(newVelX, newVelY);
                ball.Position = new Vector2(clampedX, clampedY);
            }
        }

        private void CheckBallCollisions(IBall currentBall)
        {
            lock (_physicsLock)
            {
                foreach (var ball in _dataApi.GetBalls())
                {
                    if (currentBall == ball) continue;

                    if (!AreBallsOverlapping(currentBall, ball, out var distance, out var dx, out var dy))
                        continue;
                    
                    if (AreBallsMovingTowardsEachOther(currentBall, ball, dx, dy))
                    {
                        ApplyElasticCollision(currentBall, ball, distance, dx, dy);
                    }
                }
            }
        }

        private static bool AreBallsOverlapping(IBall ball1, IBall ball2, out double distance, out double dx, out double dy)
        {
            var radius1 = ball1.Diameter / 2;
            var radius2 = ball2.Diameter / 2;

            var center1X = ball1.Position.X + radius1;
            var center1Y = ball1.Position.Y + radius1;
            var center2X = ball2.Position.X + radius2;
            var center2Y = ball2.Position.Y + radius2;

            dx = center1X - center2X;
            dy = center1Y - center2Y;
            distance = Math.Sqrt(dx * dx + dy * dy);

            return distance <= radius1 + radius2;
        }

        private static bool AreBallsMovingTowardsEachOther(IBall ball1, IBall ball2, double dx, double dy)
        {
            var relativeVelocityX = ball1.Velocity.X - ball2.Velocity.X;
            var relativeVelocityY = ball1.Velocity.Y - ball2.Velocity.Y;
            var dotProduct = relativeVelocityX * dx + relativeVelocityY * dy;

            return dotProduct < 0;
        }

        private static void ApplyElasticCollision(IBall ball1, IBall ball2, double distance, double dx, double dy)
        {
            var velocity1 = ball1.Velocity;
            var velocity2 = ball2.Velocity;
            var mass1 = ball1.Mass;
            var mass2 = ball2.Mass;

            var collisionVectorX = dx / distance;
            var collisionVectorY = dy / distance;
            
            // Compute velocity components along the normal and tangent vectors
            var v1n = collisionVectorX * velocity1.X + collisionVectorY * velocity1.Y;
            var v1t = -collisionVectorY * velocity1.X + collisionVectorX * velocity1.Y;
            
            var v2n = collisionVectorX * velocity2.X + collisionVectorY * velocity2.Y;
            var v2t = -collisionVectorY * velocity2.X + collisionVectorX * velocity2.Y;
            
            // 1D Elastic collision equation along the normal
            var v1nPrime = (v1n * (mass1 - mass2) + 2 * mass2 * v2n) / (mass1 + mass2);
            var v2nPrime = (v2n * (mass2 - mass1) + 2 * mass1 * v1n) / (mass1 + mass2);
            
            // Recombine the normal and tangential components into the new velocity vectors
            var newVelocity1X = v1nPrime * collisionVectorX - v1t * collisionVectorY;
            var newVelocity1Y = v1nPrime * collisionVectorY + v1t * collisionVectorX;
            ball1.Velocity = new Vector2(newVelocity1X, newVelocity1Y);
            
            var newVelocity2X = v2nPrime * collisionVectorX - v2t * collisionVectorY;
            var newVelocity2Y = v2nPrime * collisionVectorY + v2t * collisionVectorX;
            ball2.Velocity = new Vector2(newVelocity2X, newVelocity2Y);
        }
    }
}