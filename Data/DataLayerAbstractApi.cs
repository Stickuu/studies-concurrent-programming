namespace Data
{
    public abstract class DataLayerAbstractApi
    {
        public abstract IBoard Board { get; }

        public abstract IBall CreateBall();
        public abstract IEnumerable<IBall> GetBalls();
        public abstract void RemoveAllBalls();

        public static DataLayerAbstractApi GetInstance(double boardWidth, double boardHeight)
        {
            return DataLayerApi.GetInstance(boardWidth, boardHeight);
        }
    }
}

