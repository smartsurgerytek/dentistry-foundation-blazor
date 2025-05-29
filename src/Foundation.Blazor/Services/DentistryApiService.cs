using Foundation.Application.Contracts.Dtos;
using Foundation.Dtos;
using SkiaSharp;

namespace Foundation.Blazor.Services
{
    public class DentistryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiUrlInternal;
        private readonly ILogger<DentistryApiService> _logger;

        public DentistryApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DentistryApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("DentistryApiClient");
            ApiUrlInternal = configuration["ApiUrlInternal"];
            _logger = logger;

            // Set default headers for the HttpClient
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        public bool IsImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".gif", ".jpeg", ".dcm", ".tiff", ".tif" };
            var fileExtension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<bool> IsPeriapicalImage(string base64Img)
        {
            try
            {
                _logger.LogInformation("Checking if the image is periapical.");
                _logger.LogInformation("Expect:100 Continue Header value: {0}", _httpClient.DefaultRequestHeaders.ExpectContinue);
                _logger.LogInformation("HttpApi Host Url value: {0}", ApiUrlInternal);

                var imageRequest = new PaPanoClassificationRequestDto { Image = base64Img };
                var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, "dentistry-api/pa-pano-classification"), imageRequest);

                httpResponse.EnsureSuccessStatusCode();
                var paPanoClassification = await httpResponse.Content.ReadAsAsync<PaPanoClassificationResponseDto>();

                return paPanoClassification?.Predicted_Class?.Contains("periapical", StringComparison.OrdinalIgnoreCase) ?? false;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"An error occurred while checking if the image is periapical. {ex.Message}");
                throw;
                // return false;
            }
        }

        public async Task<SegmentationApiResponseDto> GetSegmentedImage(bool isPeriapicalImage, string base64Img)
        {
            try
            {
                _logger.LogInformation("Getting segmented image for isPeriapicalImage: {IsPeriapicalImage}", isPeriapicalImage);
                var imageRequest = new SegmentationApiRequestDto { Image = base64Img };

                var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, $"dentistry-api/segmented-image?isPeriapicalImage={IsPeriapicalImage}"), new SegmentationApiRequestDtoWrapper { IsPeriapicalImage = isPeriapicalImage, SegmentationApiRequest = imageRequest });

                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsAsync<SegmentationApiResponseDto>();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"An error occurred while getting the segmented image. {ex.Message}");
                throw;
            }
        }

        public async Task<FDISegmentationResponseDto> GetFDIData(bool isPeriapicalImage, string base64Img)
        {
            var imageRequest = new SegmentationApiRequestDto { Image = base64Img };

            var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, $"dentistry-api/pano-fdi-segmentation-cvat?isPeriapicalImage={IsPeriapicalImage}"), new SegmentationApiRequestDtoWrapper { IsPeriapicalImage = isPeriapicalImage, SegmentationApiRequest = imageRequest });

            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsAsync<FDISegmentationResponseDto>();
        }

        public async Task<SegmentationApiResponseDto> GetMeasurementImage(bool isPeriapicalImage, string base64Img)
        {
            try
            {
                _logger.LogInformation("Getting measurement image for isPeriapicalImage: {IsPeriapicalImage}", isPeriapicalImage);
                var imageRequest = new SegmentationApiRequestDto { Image = base64Img };

                var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, $"dentistry-api/measurement-image?isPeriapicalImage={IsPeriapicalImage}"), new SegmentationApiRequestDtoWrapper { IsPeriapicalImage = isPeriapicalImage, SegmentationApiRequest = imageRequest });

                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsAsync<SegmentationApiResponseDto>();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"An error occurred while getting the measurement image. {ex.Message}");
                throw;
            }
        }
    }
}