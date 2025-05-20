using Foundation.Dtos;
using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Foundation.Services
{
    public class MainUserAppService : ApplicationService, IMainUserAppService, ITransientDependency
    {
        private readonly IRepository<MainUser, Guid> _mainUserRepository;
        public async Task CreateMainUserAsync(MainUserDto input)
        {
            var organization = ObjectMapper.Map<MainUserDto, MainUser>(input);
            await _mainUserRepository.InsertAsync(organization);            
        }

        public async Task DeleteMainUserAsync(Guid mainUserId)
        {
            var organization = await _mainUserRepository.FirstOrDefaultAsync(x => x.Id == mainUserId, CancellationToken.None);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(MainUser), mainUserId);
            }

            await _mainUserRepository.DeleteAsync(organization);
        }

        public async Task<List<MainUserDto>> GetMainUserAsync()
        {
            var mainUsers = await _mainUserRepository.GetListAsync();
            return ObjectMapper.Map<List<MainUser>, List<MainUserDto>>(mainUsers);
        }

        public async Task<MainUserDto> GetMainUserAsync(Guid mainUserId)
        {
            var mainUser = await _mainUserRepository.FirstOrDefaultAsync(x => x.Id == mainUserId, CancellationToken.None);
            return ObjectMapper.Map<MainUser, MainUserDto>(mainUser);
        }

        public async Task UpdateMainUserAsync(Guid mainUserId, MainUserDto input)
        {
            var mainUser = await _mainUserRepository.FirstOrDefaultAsync(x => x.Id == mainUserId, CancellationToken.None);
            ObjectMapper.Map(input, mainUser);
            await _mainUserRepository.UpdateAsync(mainUser);
        }
    }
}
