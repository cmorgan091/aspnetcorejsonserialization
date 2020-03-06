using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JsonTest.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        private IEnumerable<SampleDataPoint> GetSampleData()
        {
            var pointsToCreate = 20_000;
            var random = new Random();

            return Enumerable.Range(1, pointsToCreate).Select(x => new SampleDataPoint
                {
                    SampleDataPointId = x,
                    Amount = random.NextDouble(),
                    Count = random.Next(),
                    Temperature = random.Next()
                })
                .ToList();
        }

        /// <summary>
        /// Allow .net core to serialize the object to json once the action is complete
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SampleDataPoint> Normal()
        {
            var data = GetSampleData();

            return data;
        }

        /// <summary>
        /// Manually serialize the data using json.net
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<SampleDataPoint>> Manual()
        {
            var data = GetSampleData();

            var serialized = JsonConvert.SerializeObject(data);

            return Content(serialized, "application/json");
        }

        /// <summary>
        /// Manually serialize the data using json.net and a writer, which is more akin to what aspnetcore does
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<SampleDataPoint>> ManualWriter()
        {
            var data = GetSampleData();

            using (var stringWriter = new StringWriter())
            {
                var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
                jsonSerializer.Serialize(stringWriter, data);

                var serialized = stringWriter.ToString();
                
                return Content(serialized, "application/json");
            }
        }
    }
}
