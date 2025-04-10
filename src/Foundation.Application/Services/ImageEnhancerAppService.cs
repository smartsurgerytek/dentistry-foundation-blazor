using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using System.Configuration;
using Foundation.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Polly.CircuitBreaker;
using System.IO;

namespace Foundation.Services
{
    public class ImageEnhancerAppService : ApplicationService, ITransientDependency, IImageEnhancerAppService
    {
        public readonly string ApiUrl;
        public readonly string ApiKey;
        private HttpClient _httpClient = new HttpClient();

        private AsyncPolicy circuitBreakerPolicy = Policy.Handle<HttpRequestException>()
                                                    .CircuitBreakerAsync(
                                                        exceptionsAllowedBeforeBreaking: 2,
                                                        durationOfBreak: TimeSpan.FromSeconds(3),
                                                        onBreak: (exception, timespan) =>
                                                        {
                                                            Console.WriteLine($"Circuit broken due to: {exception.Message}");
                                                        },
                                                        onReset: () => Console.WriteLine("Circuit closed."),
                                                        onHalfOpen: () => Console.WriteLine("Circuit in half-open state.")
                                                    );

        public ImageEnhancerAppService(IConfiguration configuration)
        {
            // ApiUrl = "https://dentistry-inference-core-2514-298229070754.asia-east1.run.app/v1";
            // ApiUrl = "https://api.smartsurgerytek.net/dentistry-stg/v1";
            // ApiKey = "Rul20gm69fj1TQ5TJIyULCd5iyQEPW8YHz2lS7cUD7A7jhBV";

            ApiUrl = configuration["ApiUrl"];
            ApiKey = configuration["ApiKey"];
        }

        public async Task<SegmentationApiResponseDto> PostSegmentedImageAsync(SegmentationApiRequestDto imageRequest)
        {
            try
            {
                return await circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);
                    var response = await _httpClient.PostAsJsonAsync(ConfigurationPath.Combine(ApiUrl + $"/pa_measure_image?apikey={ApiKey}"), imageRequest);
                    // response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<SegmentationApiResponseDto>(await response.Content.ReadAsStringAsync());
                    // return (await response.Content.ReadAsStringAsync()).As<SegmentationApiResponseDto>();
                });
            }
            catch (BrokenCircuitException ex)
            {
                var samleImgBytes = File.ReadAllBytes("~/Temp Images/sample-xray.png");
                return new SegmentationApiResponseDto {
                    Image = Convert.ToBase64String(samleImgBytes)
                };
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}