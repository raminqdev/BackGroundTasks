using AboHava.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AboHava.Services
{

    public class WeatherService : IWeatherService
    {
        private readonly IFileService _fileService;

        public WeatherService(IFileService fileService)
        {
            _fileService = fileService;
        }
        public async Task<IList<WeatherModel>> ListCitiesWeather()
        {
            var stringResult = await _fileService.ReadFromFile();

            return JsonConvert.DeserializeObject<IList<WeatherModel>>(stringResult);
        }
    }
    public interface IWeatherService
    {
        Task<IList<WeatherModel>> ListCitiesWeather();
    }

}
