using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        double X { get; set; }
        double Y { get; set; }
        double Diameter { get; }
        double VelocityX { get; set; }
        double VelocityY { get; set; }

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

