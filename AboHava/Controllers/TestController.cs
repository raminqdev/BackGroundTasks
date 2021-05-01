using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AboHava.Models;
using AboHava.Services;
using Microsoft.AspNetCore.Mvc;

namespace AboHava.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TestController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        public TestController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IList<WeatherModel>> List()
        {
            return await _weatherService.ListCitiesWeather();
        }
    }
}