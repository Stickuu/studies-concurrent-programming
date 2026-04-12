using Data;

namespace Logic
{
    public abstract class LogicLayerAbstractApi
    {
        public abstract double BoardWidth { get; }
        public abstract double BoardHeight { get; }
        
        public abstract void CreateBalls(int count);
        public abstract void RemoveAllBalls();
        public abstract void StartSimulation();
        public abstract void StopSimulation();

        public abstract IEnumerable<IBall> GetBalls();

        public static LogicLayerAbstractApi GetInstance(DataLayerAbstractApi? dataApi = null)
        {
            var dataLayerAbstractApi = dataApi ?? DataLayerAbstractApi.GetInstance(800.0, 400.0);

            return LogicLayerApi.GetInstance(dataLayerAbstractApi);
        }
    }
}