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
            ApiUrl = configuration["ApiUrl"];
            ApiKey = configuration["ApiKey"];
        }

        public async Task<PaPanoClassificationResponseDto> PostPaPanoClassificationAsync(PaPanoClassificationRequestDto imageRequest)
        {
            try
            {
                return await circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);

                    var response = await _httpClient.PostAsJsonAsync(ConfigurationPath.Combine(ApiUrl + $"/pa_pano_classification_dict?apikey={ApiKey}"), imageRequest);
                    return JsonConvert.DeserializeObject<PaPanoClassificationResponseDto>(await response.Content.ReadAsStringAsync());
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SegmentationApiResponseDto> PostSegmentedImageAsync(SegmentationApiRequestDtoWrapper imageRequest)
        {
            try
            {
                return await circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);

                    var apiEndpoint = imageRequest.IsPeriapicalImage ? "pa_measure_image" : "pano_fdi_segmentation_image";
                    var response = await _httpClient.PostAsJsonAsync(ConfigurationPath.Combine(ApiUrl + $"/{apiEndpoint}?apikey={ApiKey}"), imageRequest.SegmentationApiRequest);

                    return JsonConvert.DeserializeObject<SegmentationApiResponseDto>(await response.Content.ReadAsStringAsync());
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}