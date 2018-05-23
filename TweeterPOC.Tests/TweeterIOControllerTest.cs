using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TweeterPOC.Controllers;
using Xunit;

namespace TweeterPOC.Tests
{
    public class TweeterIOControllerTest
    {
        [Fact]
        public void TestGetTweets()
        {
            // call controller action 
            var controllerLogger = new Mock<ILogger<TweeterIOController>>();
            var tweeterIOController = new TweeterIOController(controllerLogger.Object);

            //Act
            var result = tweeterIOController.Get("2016-01-01T00:00:00.00Z", "2018-01-01T00:00:00.00Z");

            var count = 0;
            foreach (var item in result)
            {
                count += item.Count;
            }

            // Check with the actual tweet count for two years
            count.Should().Be(12192);
        }
    }
}
