using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services
{
    public interface IExaminationAppService
    {
        Task<string> CreateExaminationAsync(PatientExaminationRecordDto input);
        string GetPreSignedUrl(string fileName);
    }
}
