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
            return new DataLayerApi(boardWidth, boardHeight);
        }

        internal DataLayerApi(double boardWidth, double boardHeight)
        {
            Board = new Board(boardWidth, boardHeight);
        }

        public override IBall CreateBall()
        {
            var diameter = _random.Next(20, 50);
            var radius = diameter / 2.0;
            var mass = Math.PI * radius * radius;
            
            var x = _random.NextDouble() * (Board.Width - diameter);
            var y = _random.NextDouble() * (Board.Height - diameter);
            
            var ball = new Ball(new Vector2(x, y), diameter, mass)
            {
                Velocity = new Vector2(
                    (_random.NextDouble() * 100) - 50,
                    (_random.NextDouble() * 100) - 50
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
            foreach (var ball in _balls)
            {
                ball.Dispose();
            }
            _balls.Clear();
        }
    }
}