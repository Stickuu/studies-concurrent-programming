namespace Data
{
    internal class DataLayerApi : DataLayerAbstractApi
    {
        private static DataLayerApi? _instance;
        
        private readonly List<IBall> _balls = [];
        private readonly Random _random = new();

        public override int BoardWidth { get; }
        public override int BoardHeight { get; }

        public new static DataLayerApi GetInstance(int boardWidth, int boardHeight)
        {
            _instance ??= new DataLayerApi(boardWidth, boardHeight);

            return _instance;
        }
        
        private DataLayerApi(int boardWidth, int boardHeight)
        {
            BoardHeight = boardHeight;
            BoardWidth = boardWidth;
        }

        public override IBall CreateBall()
        {
            var diameter = 30;
            var x = _random.NextDouble() * (BoardWidth - diameter);
            var y = _random.NextDouble() * (BoardHeight - diameter);
            var ball = new Ball(x, y, diameter);
            
            _balls.Add(ball);

            return ball;
        }

        public override IEnumerable<IBall> GetBalls()
        {
            return _balls;
        }

        public override void RemoveAllBalls()
        {
            _balls.Clear();
        }
    }
}