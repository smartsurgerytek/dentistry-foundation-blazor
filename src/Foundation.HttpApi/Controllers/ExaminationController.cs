using Foundation.Dtos;
using Foundation.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Controllers
{
    [Route("api/app/examination")]
    public class ExaminationController : ControllerBase
    {
        private readonly IExaminationAppService _examinationAppService;

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
    }

}
