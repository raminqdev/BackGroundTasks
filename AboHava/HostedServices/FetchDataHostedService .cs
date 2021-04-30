using AboHava.Models;
using AboHava.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HostedServices
{
    public class FetchDataHostedService : IHostedService
    {
        private Timer _timer;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;

        public FetchDataHostedService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IFileService fileService)
        {
            _configuration = configuration;
            _fileService = fileService;
            _httpClient = httpClientFactory.CreateClient();

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async (state) => await GetWeatherData(state), null, 0, 10000); //every 10 seconds
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }


        private async Task GetWeatherData(object state)
        {
            var weatherTasks = new ConcurrentBag<Task<WeatherModel>>();
            GetIranCities().ForEach((city) =>
            {
                weatherTasks.Add(SendRequestByCityName(city));
            });
            await Task.WhenAll(weatherTasks);

            var weatherModels = weatherTasks.Select(t => t.Result).ToList();

            await _fileService.WriteToFile(JsonConvert.SerializeObject(weatherModels));

            Console.WriteLine("Request Success");
        }

        private async Task<WeatherModel> SendRequestByCityName(string city)
        {
            var url = _configuration["Openweathermap:Api"].ToString() +
                        $"?q={city},IR&units=metric&appid={_configuration["Openweathermap:AppId"]}&lang=fa";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseJObject = JsonConvert.DeserializeObject<JObject>(responseContent);

            return new WeatherModel
            {
                Name = Convert.ToString(responseJObject["name"]),
                Description = Convert.ToString(responseJObject["weather"][0]["description"]),
                Temp = Convert.ToDouble(responseJObject["main"]["temp"]),
                TempMax = Convert.ToDouble(responseJObject["main"]["temp_max"]),
                TempMin = Convert.ToDouble(responseJObject["main"]["temp_min"]) + DateTime.Now.Millisecond
            };
        }

        private List<string> GetIranCities()
        {
            return new List<string>{
                "Tehran",
                "Qom",
                "Tabriz",
                "Isfahan",
                "Shiraz",
                "Ahvaz"
            };
        }
    }
}

