using Foundation.Dtos;
using SkiaSharp;

namespace Foundation.Blazor.Services
{
    public class ImageService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string ApiUrlInternal;

        public ImageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            ApiUrlInternal = configuration["ApiUrlInternal"];
        }

        public bool IsImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".gif", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<SegmentationApiResponseDto> GetEnhancedImage(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                // call the api
                var base64Img = Convert.ToBase64String(memoryStream.ToArray());
                var imageRequest = new SegmentationApiRequestDto { Image = base64Img };
                var httpResponse = await _httpClient.PostAsJsonAsync(Path.Combine(ApiUrlInternal, "image-enhancer/segmented-image"), imageRequest);

                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsAsync<SegmentationApiResponseDto>();
            }
        }

        public Stream CombineTwoImages(byte[] firstImageStream, byte[] secondImageStream)
        {
            if (firstImageStream == null)
            {
                throw new ArgumentNullException(nameof(firstImageStream));
            }

            if (secondImageStream == null)
            {
                throw new ArgumentNullException(nameof(secondImageStream));
            }

            // Decode the streams into SKBitmap objects
            using var firstImage = SKBitmap.Decode(firstImageStream);
            using var secondImage = SKBitmap.Decode(secondImageStream);

            if (firstImage == null || secondImage == null)
            {
                throw new ArgumentException("One or both streams could not be decoded into images.");
            }

            // Resize the second image to match the height of the first image
            int resizedSecondImageWidth = (int)((double)secondImage.Width / secondImage.Height * firstImage.Height);
            using var resizedSecondImage = secondImage.Resize(new SKImageInfo(resizedSecondImageWidth, firstImage.Height), SKFilterQuality.High);

            // Calculate the dimensions of the output image
            int outputImageWidth = firstImage.Width + resizedSecondImage.Width;
            int outputImageHeight = firstImage.Height;

            // Create the output image
            var outputImage = new SKBitmap(outputImageWidth, outputImageHeight);

            using (var canvas = new SKCanvas(outputImage))
            {
                // Draw the first image
                canvas.DrawBitmap(firstImage, new SKRect(0, 0, firstImage.Width, firstImage.Height));

                // Draw the resized second image next to the first image
                canvas.DrawBitmap(resizedSecondImage, new SKRect(firstImage.Width, 0, firstImage.Width + resizedSecondImage.Width, firstImage.Height));
            }

            // Encode the output image into a memory stream
            var memoryStream = new MemoryStream();
            using (var image = SKImage.FromBitmap(outputImage))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                data.SaveTo(memoryStream);
            }

            // Reset the stream position to the beginning
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}