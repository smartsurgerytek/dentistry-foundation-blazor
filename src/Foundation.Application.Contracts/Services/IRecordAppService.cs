using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services
{
    public interface IRecordAppService
    {
        Task<List<RecordDto>> GetRecordsAsync();
        Task<RecordDto> GetRecordAsync(Guid recordId);
        Task CreateRecordAsync(CreateUpdateRecordDto input);
        Task UpdateRecordAsync(Guid recordId, CreateUpdateRecordDto input);
        Task DeleteRecordAsync(Guid recordId);
    }
}
