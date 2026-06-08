using System.Collections.Generic;
using Data.Interfaces;

namespace Data
{
    public abstract class DataLayerAbstractApi
    {
        public abstract IBoard Board { get; }
        public abstract IDiagnosticsLogger Logger { get; }

        public abstract IBall CreateBall();
        public abstract IEnumerable<IBall> GetBalls();
        public abstract void RemoveAllBalls();

        public static DataLayerAbstractApi GetInstance(double boardWidth, double boardHeight)
        {
            return new DataLayerApi(boardWidth, boardHeight);
        }
    }
}

