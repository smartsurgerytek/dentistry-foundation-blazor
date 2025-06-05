using Syncfusion.EJ2.FileManager.FileProvider;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Syncfusion.EJ2.FileManager.Base;
using Amazon;
using System.Text.Json;
using System.IO;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
using System.Threading.Tasks;
using Foundation.Dtos;
using Syncfusion.PdfExport;

namespace EJ2AmazonS3ASPCoreFileProvider.Controllers
{

    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigins")]
    public class FileProviderController : Controller
    {
        private readonly ILogger<FileProviderController> _logger;
        private readonly HttpClient _httpClient;
        public FileProvider operation;
        public string basePath;
        protected RegionEndpoint bucketRegion;
        // RootFolder for all S3 Operations (used by minio)
        // public string root = "Files/";

        public FileProviderController(IWebHostEnvironment hostingEnvironment, ILogger<FileProviderController> logger, FileProvider s3provider, IConfiguration configuration)
        {
            this.basePath = hostingEnvironment.ContentRootPath;
            this.basePath = basePath.Replace("../", "");
            this.operation = s3provider;
            _logger = logger;

            var fileProvider = configuration["FileProvider"];
            if (!string.IsNullOrEmpty(fileProvider))
            {
                var bucketName = configuration[$"{fileProvider}:BucketName"];
                var rootFolder = configuration[$"{fileProvider}:RootFolder"];

                if (fileProvider != null &&
                    (string.Compare(fileProvider, "amazons3", StringComparison.OrdinalIgnoreCase) == 0
                    || string.Compare(fileProvider, "minio", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    this.operation.RegisterAmazonS3OrMinioFileProvider(bucketName, rootFolder);
                    return;
                }
            }

            throw new Exception("FileProvider configuration is missing.");
        }

        [Route("AmazonS3FileOperations")]
        public object AmazonS3FileOperations([FromBody] FileManagerDirectoryContent args)
        {
            if (args.Action == "delete" || args.Action == "rename")
            {
                if ((args.TargetPath == null) && (args.Path == ""))
                {
                    FileManagerResponse response = new FileManagerResponse();
                    ErrorDetails er = new ErrorDetails
                    {
                        Code = "401",
                        Message = "Restricted to modify the root folder."
                    };
                    response.Error = er;
                    return this.operation.ToCamelCase(response);
                }
            }
            switch (args.Action)
            {
                case "read":
                    // reads the file(s) or folder(s) from the given path.
                    return this.operation.ToCamelCase(this.operation.GetFiles(args.Path, false, args.Data));
                case "delete":
                    // deletes the selected file(s) or folder(s) from the given path.
                    return this.operation.ToCamelCase(this.operation.Delete(args.Path, args.Names, args.Data));
                case "copy":
                    // copies the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.operation.ToCamelCase(this.operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData, args.Data));
                case "move":
                    // cuts the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.operation.ToCamelCase(this.operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData, args.Data));
                case "details":
                    // gets the details of the selected file(s) or folder(s).
                    return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names, args.Data));
                case "create":
                    // creates a new folder in a given path.
                    return this.operation.ToCamelCase(this.operation.Create(args.Path, args.Name, args.Data));
                case "search":
                    // gets the list of file(s) or folder(s) from a given path based on the searched key string.
                    return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive, args.Data));
                case "rename":
                    // renames a file or folder.
                    return this.operation.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName, false, args.ShowFileExtension, args.Data));
            }
            return null;
        }

        // uploads the file(s) into a specified path
        // when upload happens, call the dentistry ai api
        // the response returned will be an image
        // upload this image into aws bucket
        [Route("AmazonS3Upload")]
        public IActionResult AmazonS3Upload(string path, IList<IFormFile> uploadFiles, string action, string data)
        {
            FileManagerResponse uploadResponse;
            FileManagerDirectoryContent[] dataObject = new FileManagerDirectoryContent[1];
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            dataObject[0] = JsonSerializer.Deserialize<FileManagerDirectoryContent>(data, options);
            foreach (var file in uploadFiles)
            {
                var folders = (file.FileName).Split('/');
                // checking the folder upload
                if (folders.Length > 1)
                {
                    for (var i = 0; i < folders.Length - 1; i++)
                    {
                        if (!this.operation.checkFileExist(path, folders[i]))
                        {
                            this.operation.ToCamelCase(this.operation.Create(path, folders[i], dataObject));
                        }
                        path += folders[i] + "/";
                    }
                }
            }
            int chunkIndex = int.TryParse(HttpContext.Request.Form["chunk-index"], out int parsedChunkIndex) ? parsedChunkIndex : 0;
            int totalChunk = int.TryParse(HttpContext.Request.Form["total-chunk"], out int parsedTotalChunk) ? parsedTotalChunk : 0;
            uploadResponse = operation.Upload(path, uploadFiles, action, dataObject, chunkIndex, totalChunk);
            if (uploadResponse.Error != null)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = uploadResponse.Error.Message;
            }
            return Content("");
        }

        // downloads the selected file(s) and folder(s)
        [Route("AmazonS3Download")]
        public IActionResult AmazonS3Download(string downloadInput)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            FileManagerDirectoryContent args = JsonSerializer.Deserialize<FileManagerDirectoryContent>(downloadInput, options);
            return operation.Download(args.Path, args.Names);
        }

        // downloads the selected file(s) and folder(s)
        // returns a byte[]
        [Route("OriginalImageDownload")]
        public async Task<FileStreamResult> OriginalImageDownload(string path, string name)
        {
            return await operation.DownloadAsByteArrayAsync(path, name);
        }

        // gets the image(s) from the given path
        [Route("AmazonS3GetImage")]
        public IActionResult AmazonS3GetImage(FileManagerDirectoryContent args)
        {
            return operation.GetImage(args.Path, args.Id, false, null, args.Data);
        }

        // uploads a single image to the bucket
        // this is used by the image editor
        [HttpPost]
        [Route("UploadImageAsStream")]
        public async Task UploadImageAsStream([FromBody] ImageEditorUploadAndReplaceDto replaceDto)
        {
            // replace the old image with the edited image
            // check if the file exists
            // if it does, delete it
            var oldImagePath = replaceDto.FilePath;
            var oldImageName = replaceDto.OldFileName;
            if (oldImagePath != null && oldImageName != "")
            {
                var fileExists = this.operation.checkFileExist(oldImagePath, oldImageName);
                if (fileExists)
                {
                    // delete the old image
                    var fileManagerDirectoryContent = new FileManagerDirectoryContent
                    {
                        Path = oldImagePath,
                        Id = oldImageName,
                        Names = [oldImageName],
                        IsFile = true,
                        Data = null
                    };
                    this.operation.Delete(oldImagePath, [oldImageName], [fileManagerDirectoryContent]);
                }
            }

            // upload the image
            // set the root name
            this.operation.GetBucketList();
            using Stream stream = new MemoryStream(Convert.FromBase64String(replaceDto.FileStream.Split(",")[1]));
            await this.operation.UploadFileToS3(stream, replaceDto.FileName, replaceDto.FilePath);
        }

        // accepts the image path and
        // 1. gets the image bytes
        // 2. checks if the image is periapical
        // 3. calls the segmentation API and uploads the segmented image to the bucket
        [HttpGet("GetSegmentedImage")]
        public async Task<string> GetSegmentedImage(string filterPath, string fileName, string combinedImageFileName = "")
        {
            try
            {
                var originalImageStream = this.operation.GetImage(Path.Combine(filterPath, fileName), null, false, null, null);
                var originalImageBytes = await originalImageStream.FileStream.GetAllBytesAsync();
                var originalImageBase64 = Convert.ToBase64String(originalImageBytes);

                // check if the image is periapical
                var checkPath = filterPath.TrimEnd('/');
                var isPeriapicalImage = checkPath.Split("/").Last() == "pa";

                _logger.LogInformation("Processing segmented image for isPeriapicalImage: {IsPeriapicalImage}", isPeriapicalImage);

                return await this.operation.GetSegmentedImage(isPeriapicalImage, originalImageBase64, filterPath, fileName, combinedImageFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing segmented image: {ex.Message}");
                throw;
            }
        }
        // accepts the image path and
        // 1. gets the image bytes
        // 2. checks if the image is periapical
        // 3. calls the measurement API and uploads the measurement image to the bucket
        [HttpGet("GetMeasurementImage")]
        public async Task<string> GetMeasurementImage(string filterPath, string fileName, string combinedImageFileName = "")
        {
            try
            {
                var originalImageStream = this.operation.GetImage(Path.Combine(filterPath, fileName), null, false, null, null);
                var originalImageBytes = await originalImageStream.FileStream.GetAllBytesAsync();
                var originalImageBase64 = Convert.ToBase64String(originalImageBytes);

                // check if the image is periapical
                var checkPath = filterPath.TrimEnd('/');
                var isPeriapicalImage = checkPath.Split("/").Last() == "pa";

                _logger.LogInformation("Processing measurement image for isPeriapicalImage: {IsPeriapicalImage}", isPeriapicalImage);

                return await this.operation.GetMeasurementImage(isPeriapicalImage, originalImageBase64, filterPath, fileName, combinedImageFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing measurement image: {ex.Message}");
                throw;
            }
        }

        [HttpPost("SaveDocument")]
        public async Task<IActionResult> SaveDocument([FromBody] SaveDocumentRequest request)
        {
            try
            {
                byte[] documentBytes = Convert.FromBase64String(request.Content);

                using (var docxStream = new MemoryStream(documentBytes))
                {
                    using (WordDocument wordDocument = new WordDocument(docxStream, Syncfusion.DocIO.FormatType.Docx))
                    {
                        DocIORenderer renderer = new DocIORenderer();
                        //PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument);

                        using (var pdfMemoryStream = new MemoryStream())
                        {
                            //pdfDocument.Save(pdfMemoryStream);
                            wordDocument.Save(pdfMemoryStream, Syncfusion.DocIO.FormatType.Docx);
                            pdfMemoryStream.Position = 0;
                            string pdfS3Path = "foundation/documents/" + request.FileName;
                            bool uploadSuccess = await operation.UploadAsync(pdfS3Path, pdfMemoryStream);
                            renderer.Dispose();
                            //pdfDocument.Close(true);
                            wordDocument.Close();
                            return Ok(uploadSuccess);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SaveDocument: {ex.Message}");
                return StatusCode(500, new { message = "Error saving the document", error = ex.Message });
            }
        }

        public class SaveDocumentRequest
        {
            public string Content { get; set; }
            public string FileName { get; set; }
        }
    }
}