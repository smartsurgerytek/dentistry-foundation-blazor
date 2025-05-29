using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Account;

namespace Foundation.HttpApi.Client.ConsoleTestApp;

public class ClientDemoService : ITransientDependency
{
    private readonly IProfileAppService _profileAppService;
    private readonly IIdentityUserAppService _identityUserAppService;

    public ClientDemoService(
        IProfileAppService profileAppService,
        IIdentityUserAppService identityUserAppService)
    {
        _profileAppService = profileAppService;
        _identityUserAppService = identityUserAppService;
    }

    public async Task RunAsync()
    {
        var profileDto = await _profileAppService.GetAsync();
        

        var resultDto = await _identityUserAppService.GetListAsync(new GetIdentityUsersInput());
        
        foreach (var identityUserDto in resultDto.Items)
        {
            Console.WriteLine($"- [{identityUserDto.Id}] {identityUserDto.Name}");
        }
    }
}
