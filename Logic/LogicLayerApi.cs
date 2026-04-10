using Data;

namespace Logic
{
    internal class LogicLayerApi : LogicLayerAbstractApi
    {
        private static LogicLayerApi? _instance;
        
        private readonly DataLayerAbstractApi _dataApi;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly Random _random = new();

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
                    MoveBall(ball);
                }

                await Task.Delay(16, token);
            }
        }

        private void MoveBall(IBall ball)
        {
            var newX = ball.X + ball.VelocityX;
            var newY = ball.Y + ball.VelocityY;

            if (newX <= 0 || newX >= _dataApi.BoardWidth - ball.Diameter)
            {
                ball.VelocityX = -ball.VelocityX;
                newX = ball.X + ball.VelocityX;
            }

            if (newY <= 0 || newY >= _dataApi.BoardHeight - ball.Diameter)
            {
                ball.VelocityY = -ball.VelocityY;
                newY = ball.Y + ball.VelocityY;
            }

            ball.X = newX;
            ball.Y = newY;
        }
    }
}