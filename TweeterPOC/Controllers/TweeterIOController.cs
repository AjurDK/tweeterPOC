using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TweeterPOC.Models;

namespace TweeterPOC.Controllers
{
    [Produces("application/json")]
    [Route("api/TweeterIO")]
    public class TweeterIOController : Controller
    {
        private readonly ILogger<TweeterIOController> _logger;

        public TweeterIOController(ILogger<TweeterIOController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetTweetes")]
        public IEnumerable<List<Tweeter>> Get(string startDateStr, string endDateStr)
        {
            var result = new List<Tweeter>();

            if (string.IsNullOrWhiteSpace(startDateStr) || string.IsNullOrWhiteSpace(endDateStr))
            {
                yield return result;
            }

            var previousStartDate = Convert.ToDateTime(startDateStr);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var webApiURL = "https://badapi.iqvia.io/api/v1";

                //var startDateStr = "2016-01-01T00:00:00.00Z";
                var startDate = Convert.ToDateTime(startDateStr);

                //var endDateStr = "2018-01-01T00:00:00.00Z";
                var endDate = Convert.ToDateTime(endDateStr);

                var count = 0;
                while (startDate < endDate)
                {
                    var response = httpClient.GetAsync($@"{webApiURL}/Tweets?startDate={startDateStr}&endDate={endDateStr}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var tweets = (List<Tweeter>)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(List<Tweeter>));

                        if (tweets != null && tweets.Any() && tweets.Count > 0)
                        {
                            if (count >= 1000)
                            {
                                var resultData = result.Take(1000).Distinct().ToList();
                                result = result.Skip(1000).Take(result.Count - 1000).Distinct().ToList();
                                count = 0;
                                yield return resultData;
                            }
                            result.AddRange(tweets.OrderByDescending(d => d.stamp).Distinct().ToList());
                            startDate = TimeZoneInfo.ConvertTimeToUtc(result.OrderByDescending(d => d.stamp).Distinct().Select(d => d.stamp).FirstOrDefault());

                            if (previousStartDate < startDate)
                            {
                                previousStartDate = startDate;
                            }
                            else
                            {
                                startDate = endDate;
                                //Console.WriteLine($"result count {result.Count}");
                                yield return result;
                            }
                            startDateStr = startDate.ToString("yyyy-MM-ddThh:mm:ss.ffZ");
                            count += tweets.Count;
                        }
                        else
                        {
                            startDate = endDate;
                            yield return result;
                        }
                    }
                }
            }
        }
    }
}
