using Data;

namespace Logic
{
    internal class LogicLayerApi : LogicLayerAbstractApi
    {
        private static LogicLayerApi? _instance;
        
        private readonly DataLayerAbstractApi _dataApi;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly Random _random = new();

        private readonly Dictionary<IBall, (double dx, double dy)> _velocities = new();

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
                var ball = _dataApi.CreateBall();

                var dx = (_random.NextDouble() * 4) - 2;
                var dy = (_random.NextDouble() * 4) - 2;

                _velocities[ball] = (dx, dy);
            }
        }

        public override IEnumerable<IBall> GetBalls()
        {
            return _dataApi.GetBalls();
        }

        public override void StartSimulation()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => SimulationLoop(_cancellationTokenSource.Token));
        }

        public override void StopSimulation()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task SimulationLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                foreach (var ball in _dataApi.GetBalls())
                {
                    if (!_velocities.TryGetValue(ball, out var velocity)) continue;
                    
                    MoveBall(ball, ref velocity);
                    _velocities[ball] = velocity;
                }

                await Task.Delay(16, token);
            }
        }

        private void MoveBall(IBall ball, ref (double dx, double dy) velocity)
        {
            var newX = ball.X + velocity.dx;
            var newY = ball.Y + velocity.dy;

            if (newX <= 0 || newX >= _dataApi.BoardWidth - ball.Diameter)
            {
                velocity.dx = -velocity.dx;
                newX = ball.X + velocity.dx;
            }

            if (newY <= 0 || newY >= _dataApi.BoardHeight - ball.Diameter)
            {
                velocity.dy = -velocity.dy;
                newX = ball.Y + velocity.dy;
            }

            ball.X = newX;
            ball.Y = newY;
        }
    }
}