using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        double Diameter { get; }

        void Move(double boardWidth, double boardHeight);
    }

    public abstract class DataLayerAbstractApi
    {
        public abstract double BoardWidth { get; }
        public abstract double BoardHeight { get; }

        public abstract IBall CreateBall();
        public abstract IEnumerable<IBall> GetBalls();
        public abstract void RemoveAllBalls();

        public static DataLayerAbstractApi GetInstance(double boardWidth, double boardHeight)
        {
            return DataLayerApi.GetInstance(boardWidth, boardHeight);
        }
    }
}

