using System.Collections.Generic;
using System.Linq;
using Logic;

namespace Presentation.Models
{
    public class SimulationModel
    {
        private readonly LogicLayerAbstractApi _logicApi;

        public SimulationModel(LogicLayerAbstractApi? logicApi = null)
        {
            _logicApi = logicApi ?? LogicLayerAbstractApi.GetInstance();
        }

        public void Start(int ballsCount)
        {
            _logicApi.CreateBalls(ballsCount);
            _logicApi.StartSimulation();
        }

        public void Stop()
        {
            _logicApi.StopSimulation();
        }

        public IEnumerable<BallModel> GetBalls()
        {
            return _logicApi.GetBalls().Select(logicBall => new BallModel(logicBall)).ToList();
        }
    }
}