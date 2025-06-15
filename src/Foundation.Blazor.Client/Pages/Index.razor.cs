using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Foundation.Blazor.Client.Pages;

public partial class Index
{
    protected override async Task OnInitializedAsync()
    {
        // var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        // var user = authState.User;
        // Logger.LogInformation($"User identity is authenticated: {user.Identity?.IsAuthenticated ?? false}");
        // Logger.LogInformation($"User identity name: {user.Identity?.Name ?? "Anonymous"}");

        // if (!user.Identity?.IsAuthenticated ?? true)
        // {
        //     await Task.Delay(500); // Simulate some delay for better UX
        //     NavigationManager.NavigateTo($"/authentication/login", true);
        // }
        // else
        // {
        //     await Task.Delay(500); // Simulate some delay for better UX
        //     NavigationManager.NavigateTo("/Patient");
        // }

        NavigationManager.NavigateTo("/Patient", true);
    }
}
