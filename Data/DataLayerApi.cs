namespace Data
{
    internal class DataLayerApi : DataLayerAbstractApi
    {
        private static DataLayerApi? _instance;
        
        private readonly List<IBall> _balls = [];
        private readonly Random _random = new();

        public override double BoardWidth { get; }
        public override double BoardHeight { get; }

        public new static DataLayerApi GetInstance(double boardWidth, double boardHeight)
        {
            _instance ??= new DataLayerApi(boardWidth, boardHeight);

            return _instance;
        }
        
        private DataLayerApi(double boardWidth, double boardHeight)
        {
            BoardHeight = boardHeight;
            BoardWidth = boardWidth;
        }

        public override IBall CreateBall()
        {
            var diameter = 30;
            var x = _random.NextDouble() * (BoardWidth - diameter);
            var y = _random.NextDouble() * (BoardHeight - diameter);
            var ball = new Ball(new Vector2(x, y), diameter)
            {
                Velocity = new Vector2(
                    (_random.NextDouble() * 4) - 2,
                    (_random.NextDouble() * 4) - 2
                )
            };
            
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