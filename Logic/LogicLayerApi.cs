using Data;
using Data.Interfaces;

namespace Logic
{
    internal class LogicLayerApi : LogicLayerAbstractApi
    {
        private static LogicLayerApi? _instance;
        private readonly DataLayerAbstractApi _dataApi;
        
        private Barrier? _physicsBarrier;

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
            var balls = _dataApi.GetBalls().ToList();
            if (balls.Count == 0) return;

            _physicsBarrier = new Barrier(balls.Count, (barrier) =>
            {
                PerformPhysicsCalculations(balls);
                
                Thread.Sleep(16);
            });

            foreach (var ball in balls)
            {
                ball.StartMovement(() => 
                {
                    try 
                    { 
                        _physicsBarrier?.SignalAndWait(); 
                    }
                    catch (Exception) { /* Ignored when stopping simulation */ }
                });
            }
        }

        public override void StopSimulation()
        {
            if (_physicsBarrier == null) return;
            
            // Safely destroy the barrier so threads aren't trapped if we stop
            _physicsBarrier.Dispose();
            _physicsBarrier = null;
        }
        
        private void PerformPhysicsCalculations(List<IBall> balls)
        {
            // Update boundary collisions
            foreach (var ball in balls)
            {
                CheckBoundaryCollisions(ball);
            }

            // Check collisions between all unique pairs
            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = i + 1; j < balls.Count; j++)
                {
                    CheckBallCollisions(balls[i], balls[j]);
                }
            }
        }

        private void CheckBoundaryCollisions(IBall ball)
        {
            var position = ball.Position;
            var velocity = ball.Velocity;
            var board = _dataApi.Board;

            var newVelX = velocity.X;
            var newVelY = velocity.Y;
            var velocityChanged = false;

            // X-Axis bounds check
            if ((position.X <= 0 && velocity.X < 0) || (position.X >= board.Width - ball.Diameter && velocity.X > 0))
            {
                newVelX = -velocity.X;
                velocityChanged = true;
            }

            // Y-Axis bounds check
            if ((position.Y <= 0 && velocity.Y < 0) || (position.Y >= board.Height - ball.Diameter && velocity.Y > 0))
            {
                newVelY = -velocity.Y;
                velocityChanged = true;
            }

            if (velocityChanged)
            {
                ball.Velocity = new Vector2(newVelX, newVelY);
            }

            // Clamp position so they don't get stuck outside the bounds
            double clampedX = Math.Clamp(position.X, 0, board.Width - ball.Diameter);
            double clampedY = Math.Clamp(position.Y, 0, board.Height - ball.Diameter);
            ball.Position = new Vector2(clampedX, clampedY);
        }

        private void CheckBallCollisions(IBall currentBall, IBall otherBall)
        {
            if (!AreBallsOverlapping(currentBall, otherBall, out var distance, out var dx, out var dy))
                return;
                
            if (AreBallsMovingTowardsEachOther(currentBall, otherBall, dx, dy))
            {
                ApplyElasticCollision(currentBall, otherBall, distance, dx, dy);
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