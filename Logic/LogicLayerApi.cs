using Data;

namespace Logic
{
    internal class LogicLayerApi : LogicLayerAbstractApi
    {
        private static LogicLayerApi? _instance;
        
        private readonly DataLayerAbstractApi _dataApi;
        private CancellationTokenSource? _cancellationTokenSource;

        public override double BoardWidth => _dataApi.BoardWidth;
        public override double BoardHeight => _dataApi.BoardHeight;

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
            _dataApi.RemoveAllBalls();
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
                    ball.Move(_dataApi.BoardWidth, _dataApi.BoardHeight);
                }

                await Task.Delay(16, token);
            }
        }
    }
}