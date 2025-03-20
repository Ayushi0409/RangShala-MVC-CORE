public class WeatherService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "15adc7deec1515dc26bca6f93c83749f"; // Replace with your key

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetWeatherAsync(string city)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={ApiKey}&units=metric";
        var response = await _httpClient.GetStringAsync(url);
        return response; // Returns JSON string (parse it as needed)
    }
}