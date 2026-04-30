using System.Collections.Generic;
using System.Linq;
using Logic;

namespace Presentation.Models
{
    public class SimulationModel
    {
        private readonly double _uiCanvasWidth;
        private readonly double _uiCanvasHeight;
        private readonly LogicLayerAbstractApi _logicApi;

        public SimulationModel(double uiCanvasWidth, double uiCanvasHeight, LogicLayerAbstractApi? logicApi = null)
        {
            _uiCanvasWidth = uiCanvasWidth;
            _uiCanvasHeight = uiCanvasHeight;
            _logicApi = logicApi ?? LogicLayerAbstractApi.GetInstance();
        }

        public void Start(int ballsCount)
        {
            _logicApi.RemoveAllBalls();
            _logicApi.CreateBalls(ballsCount);
            _logicApi.StartSimulation();
        }

        public void Stop()
        {
            _logicApi.RemoveAllBalls();
        }

        public IEnumerable<BallModel> GetBalls()
        {
            var scaleX = _uiCanvasWidth / _logicApi.BoardWidth;
            var scaleY = _uiCanvasHeight / _logicApi.BoardHeight;
            var scale = System.Math.Min(scaleX, scaleY);
            
            return _logicApi.GetBalls()
                .Select(logicBall => new BallModel(logicBall, scale))
                .ToList();
        }
    }
}