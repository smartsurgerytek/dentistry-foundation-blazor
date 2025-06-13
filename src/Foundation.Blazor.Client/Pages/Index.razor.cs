using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Foundation.Blazor.Client.Pages;

public partial class Index
{
    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation($"Is current user authenticated: {CurrentUser.IsAuthenticated}");
        Logger.LogInformation($"Current user roles: {string.Join(", ", CurrentUser.Roles)}");
        Logger.LogInformation($"Current user name: {CurrentUser.Name}");
        if (CurrentUser.IsAuthenticated && CurrentUser.Roles.Contains("Doctor"))
        {
            await Task.Delay(1000); // Simulate some delay for better UX
            NavigationManager.NavigateTo("/Patient");
        }
        else
        {
            await Task.Delay(5000); // Simulate some delay for better UX
            NavigationManager.NavigateTo($"/authentication/login", true);
        }
    }
}
