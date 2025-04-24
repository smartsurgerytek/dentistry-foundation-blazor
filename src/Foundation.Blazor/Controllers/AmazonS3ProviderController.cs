using Syncfusion.EJ2.FileManager.AmazonS3FileProvider;
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

namespace EJ2AmazonS3ASPCoreFileProvider.Controllers
{

    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigins")]
    public class AmazonS3ProviderController : Controller
    {
        private readonly ILogger<AmazonS3ProviderController> _logger;
        private readonly HttpClient _httpClient;
        public AmazonS3FileProvider operation;
        public string basePath;
        protected RegionEndpoint bucketRegion;

        public AmazonS3ProviderController(IWebHostEnvironment hostingEnvironment, ILogger<AmazonS3ProviderController> logger, AmazonS3FileProvider s3provider)
        {
            this.basePath = hostingEnvironment.ContentRootPath;
            this.basePath = basePath.Replace("../", "");
            this.operation = s3provider;
            this.operation.RegisterAmazonS3("smartsurgerytek.foundation", "AKIAZI2LGNNVDTFYF57P", "tR/1EYOayK8i5R5DCZTJCyqAXCkDVMJWYhYEfDRp", "us-west-2");
            _logger = logger;
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

        //save the document
        //[HttpPost("SaveDocument")]
        //public async Task<IActionResult> SaveDocument([FromBody] SaveDocumentRequest request)
        //{
        //    bool uploadSuccess = false;
        //    try
        //    {
        //        byte[] documentBytes = Convert.FromBase64String(request.Content);
        //        using (var memoryStream = new MemoryStream(documentBytes))
        //        {
        //            string filepath = "foundation/documents/" + request.FileName;
        //            uploadSuccess = await operation.UploadAsync(filepath, memoryStream);
        //        }
        //        return Ok(uploadSuccess);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error in SaveDocument: {ex.Message}");
        //        return StatusCode(500, new { message = "Error saving the document", error = ex.Message });
        //    }
        //}

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
                        PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument);

                        using (var pdfMemoryStream = new MemoryStream())
                        {
                            pdfDocument.Save(pdfMemoryStream);
                            pdfMemoryStream.Position = 0;
                            string pdfS3Path = "foundation/documents/" + request.FileName;
                            bool uploadSuccess = await operation.UploadAsync(pdfS3Path, pdfMemoryStream);
                            renderer.Dispose();
                            pdfDocument.Close(true);
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