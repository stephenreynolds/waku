using System.Collections.Generic;
using Waku.Controllers;
using Xunit;
using static Waku.Controllers.SampleDataController;

namespace Waku.Test
{
    public class SampleDataControllerTest
    {
        [Fact]
        public void WeatherForecastTest()
        {
            var controller = new SampleDataController();

            var result = controller.WeatherForecasts();

            Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(result);
        }
    }
}
