using Data;

namespace Logic
{
    public abstract class LogicLayerAbstractApi
    {
        public abstract void CreateBalls(int count);
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