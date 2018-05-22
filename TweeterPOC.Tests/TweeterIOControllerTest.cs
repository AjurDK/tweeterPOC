using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TweeterPOC.Controllers;
using TweeterPOC.Models;
using Xunit;

namespace TweeterPOC.Tests
{
    public class TweeterIOControllerTest
    {
        [Fact]
        public void GetTweets()
        {
            var controller = new TweeterIOController();

            // Act
            var result = controller.Get("2016-01-01T00:00:00.00Z", "2017-12-31T00:00:00.00Z");

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tweets = okResult.Value.Should().BeAssignableTo<IEnumerable<Tweeter>>().Subject;

            tweets.Count().Should().Be(1000);
        }
    }
}
