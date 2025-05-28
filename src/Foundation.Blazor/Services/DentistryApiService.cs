using Foundation.Application.Contracts.Dtos;
using Foundation.Dtos;
using SkiaSharp;

namespace Foundation.Blazor.Services
{
    public class DentistryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiUrlInternal;

        public DentistryApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            ApiUrlInternal = configuration["ApiUrlInternal"];
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
            var imageRequest = new PaPanoClassificationRequestDto { Image = base64Img };
            var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, "dentistry-api/pa-pano-classification"), imageRequest);

            httpResponse.EnsureSuccessStatusCode();
            var paPanoClassification = await httpResponse.Content.ReadAsAsync<PaPanoClassificationResponseDto>();

            return paPanoClassification?.Predicted_Class?.Contains("periapical", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public async Task<SegmentationApiResponseDto> GetEnhancedImage(bool isPeriapicalImage, string base64Img)
        {
            var imageRequest = new SegmentationApiRequestDto { Image = base64Img };

            var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, $"dentistry-api/segmented-image?isPeriapicalImage={IsPeriapicalImage}"), new SegmentationApiRequestDtoWrapper { IsPeriapicalImage = isPeriapicalImage, SegmentationApiRequest = imageRequest });

            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsAsync<SegmentationApiResponseDto>();
        }

        public async Task<FDISegmentationResponseDto> GetFDIData(bool isPeriapicalImage, string base64Img)
        {
            var imageRequest = new SegmentationApiRequestDto { Image = base64Img };

            var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, $"dentistry-api/pano-fdi-segmentation-cvat?isPeriapicalImage={IsPeriapicalImage}"), new SegmentationApiRequestDtoWrapper { IsPeriapicalImage = isPeriapicalImage, SegmentationApiRequest = imageRequest });

            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsAsync<FDISegmentationResponseDto>();
        }
    }
}