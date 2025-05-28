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
using Foundation.Application.Contracts.Dtos;
using JetBrains.Annotations;

namespace Foundation.Services
{
    public class DentistryApiAppService : ApplicationService, ITransientDependency, IDentistryApiAppService
    {
        private readonly string ApiUrl;
        private readonly string ApiKey;
        private readonly HttpClient _httpClient;

        public DentistryApiAppService(IConfiguration configuration, HttpClient httpClient)
        {
            ApiUrl = configuration["ApiUrl"];
            ApiKey = configuration["ApiKey"];
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        public async Task<FDISegmentationResponseDto> PostPanoFdiSegmentationCvatAsync(SegmentationApiRequestDtoWrapper imageRequest)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);

                var apiEndpoint = imageRequest.IsPeriapicalImage ? "pa_measure_image" : "pano_fdi_segmentation_cvat";
                var response = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrl, $"{apiEndpoint}?apikey={ApiKey}"), new  PaPanoClassificationRequestDto { Image = imageRequest.SegmentationApiRequest.Image });
                return JsonConvert.DeserializeObject<FDISegmentationResponseDto>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaPanoClassificationResponseDto> PostPaPanoClassificationAsync(PaPanoClassificationRequestDto imageRequest)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);

                var response = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrl, $"pa_pano_classification_dict?apikey={ApiKey}"), imageRequest);
                return JsonConvert.DeserializeObject<PaPanoClassificationResponseDto>(await response.Content.ReadAsStringAsync());
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
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);

                var apiEndpoint = imageRequest.IsPeriapicalImage ? "pa_measure_image" : "pano_fdi_segmentation_image";
                var response = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrl, $"{apiEndpoint}?apikey={ApiKey}"), imageRequest.SegmentationApiRequest);

                return JsonConvert.DeserializeObject<SegmentationApiResponseDto>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}