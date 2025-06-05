using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Foundation.Dtos;
using Foundation.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Controllers
{
    [Route("api/app/examination")]
    public class ExaminationController : ControllerBase
    {
        
        private readonly IExaminationAppService _examinationAppService;

        //static IAmazonS3 client;

        public ExaminationController(IExaminationAppService examinationAppService)
        {
            _examinationAppService = examinationAppService;
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PatientExaminationRecordDto input)
        {
            var fNmae = await _examinationAppService.CreateExaminationAsync(input);            
            return Content(fNmae, "text/plain");
        }

        //[HttpGet("downloadpdf")]
        //public async Task<IActionResult> DownloadPdf(string fileName)
        //{            
        //    try
        //    {
        //        var response = await client.GetObjectAsync("smartsurgerytek.foundation", $"foundation/documents/{fileName}");
        //        return File(response.ResponseStream, response.Headers.ContentType, fileName);
        //    }
        //    catch (Exception ex)
        //    {                
        //        throw;
        //    }                       
        //}

        [HttpGet("presignedurl")]
        public string PreSignedUrl(string fileName)
        {
            string preSignedUrl =  _examinationAppService.GetPreSignedUrl(fileName);

            return preSignedUrl;
        }
    }

}
