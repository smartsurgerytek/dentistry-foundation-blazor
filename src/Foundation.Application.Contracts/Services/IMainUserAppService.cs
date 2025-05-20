using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services
{
    public interface IMainUserAppService
    {
        Task<List<MainUserDto>> GetMainUserAsync();
        Task<MainUserDto> GetMainUserAsync(Guid mainUserId);
        Task CreateMainUserAsync(MainUserDto input);
        Task UpdateMainUserAsync(Guid mainUserId, MainUserDto input);
        Task DeleteMainUserAsync(Guid mainUserId);
    }
}
