using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        double X { get; }
        double Y { get; }
        double Diameter { get; }
    }

    public abstract class DataLayerAbstractApi
    {
        public abstract int BoardWidth { get; }
        public abstract int BoardHeight { get; }

        public abstract IBall CreateBall();
        public abstract IEnumerable<IBall> GetBalls();
        public abstract void RemoveAllBalls();

        public static DataLayerAbstractApi GetInstance(int boardWidth, int boardHeight)
        {
            return DataLayerApi.GetInstance(boardWidth, boardHeight);
        }
    }
}

