namespace Data
{
    internal class DataLayerApi : DataLayerAbstractApi
    {
        private static DataLayerApi? _instance;
        
        private readonly List<IBall> _balls = [];
        private readonly Random _random = new();

        public override IBoard Board { get; }

        public new static DataLayerApi GetInstance(double boardWidth, double boardHeight)
        {
            _instance ??= new DataLayerApi(boardWidth, boardHeight);

            return _instance;
        }
        
        private DataLayerApi(double boardWidth, double boardHeight)
        {
            Board = new Board(boardWidth, boardHeight);
        }

        public override IBall CreateBall()
        {
            var x = _random.NextDouble() * (Board.Width - IBall.DIAMETER);
            var y = _random.NextDouble() * (Board.Height - IBall.DIAMETER);
            var ball = new Ball(new Vector2(x, y), IBall.DIAMETER)
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