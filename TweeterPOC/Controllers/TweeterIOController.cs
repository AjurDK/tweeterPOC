using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TweeterPOC.Models;

namespace TweeterPOC.Controllers
{
    [Produces("application/json")]
    [Route("api/TweeterIO")]
    public class TweeterIOController : Controller
    {
        // GET: api/TweeterIO/5
        [HttpGet]
        [Route("GetTweetes")]
        public IEnumerable<List<Tweeter>> Get(string startDateStr, string endDateStr)
        {
            var result = new List<Tweeter>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var webApiURL = "https://badapi.iqvia.io/api/v1";
                //var startDateStr = "2016-01-01T00:00:00.00Z";
                var startDate = Convert.ToDateTime(startDateStr);

                //var endDateStr = "2017-12-31T00:00:00.00Z";
                var endDate = Convert.ToDateTime(endDateStr);
                var skipCount = 0;
                var count = 0;
                while (startDate < endDate)
                {
                    var response = httpClient.GetAsync($@"{webApiURL}/Tweets?startDate={startDateStr}&endDate={endDateStr}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var tweets = (List<Tweeter>)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(List<Tweeter>));
                        if (tweets.Any())
                        {
                            if (count > 1000)
                            {
                                yield return result.Skip(skipCount).Take(1000).Distinct().ToList();
                                skipCount = result.Count;
                                count = 0;
                            }

                            result.AddRange(tweets.OrderByDescending(d => d.stamp).Distinct().ToList());
                            startDate = TimeZoneInfo.ConvertTimeToUtc(result.OrderByDescending(d => d.stamp).Distinct().Select(d => d.stamp).FirstOrDefault());
                            startDateStr = startDate.ToString("yyyy-MM-ddThh:mm:ss.ffZ");
                            count += tweets.Count;
                        }
                        else
                        {
                            yield break;
                        }
                    }
                }
            }

            yield return result.Distinct().ToList();
        }
    }
}
