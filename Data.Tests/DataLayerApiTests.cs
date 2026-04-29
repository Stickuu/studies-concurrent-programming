using Data;
using Newtonsoft.Json.Bson;
using System.Linq;
using Xunit;

namespace Data.Tests;
public class DataLayerApiTests
{
    private readonly DataLayerAbstractApi _dataApi;

    public DataLayerApiTests()
    {
        _dataApi = DataLayerAbstractApi.GetInstance(800.0, 400.0);
        _dataApi.RemoveAllBalls();
    }

    [Fact]
    public void CreateBallShouldAddSingleBallToCollectionWhenCalled()
    {
        _dataApi.CreateBall();
        var balls = _dataApi.GetBalls().ToList();

        Assert.Single(balls);
    }

    [Fact]
    public void RemoveAllBallsShouldClearCollectionWhenCalled()
    {
        _dataApi.RemoveAllBalls();
        _dataApi.CreateBall();
        _dataApi.CreateBall();
        _dataApi.CreateBall();

        Assert.Equal(3, _dataApi.GetBalls().Count());

        _dataApi.RemoveAllBalls();
            
        Assert.Empty(_dataApi.GetBalls());
    }
}
        
