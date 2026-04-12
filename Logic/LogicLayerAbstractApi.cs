using Data;

namespace Logic
{
    public abstract class LogicLayerAbstractApi
    {
        public abstract int BoardWidth { get; }
        public abstract int BoardHeight { get; }
        
        public abstract void CreateBalls(int count);
        public abstract void RemoveAllBalls();
        public abstract void StartSimulation();
        public abstract void StopSimulation();

        public abstract IEnumerable<IBall> GetBalls();

        public static LogicLayerAbstractApi GetInstance(DataLayerAbstractApi? dataApi = null)
        {
            DataLayerAbstractApi dataLayerAbstractApi = dataApi ?? DataLayerAbstractApi.GetInstance(800, 400);

            return LogicLayerApi.GetInstance(dataLayerAbstractApi);
        }
    }
}