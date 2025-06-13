using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundation.Blazor.Client.Pages;

public partial class Index
{
    protected override Task OnInitializedAsync()
    {
        Console.WriteLine($"Is current user authenticated: {CurrentUser.IsAuthenticated}");
        Console.WriteLine($"Current user roles: {string.Join(", ", CurrentUser.Roles)}");
        Console.WriteLine($"Current user name: {CurrentUser.Name}");
        if (CurrentUser.IsAuthenticated && CurrentUser.Roles.Contains("Doctor"))
        {
            NavigationManager.NavigateTo("/Patient");
        }
        else
        {
            NavigationManager.NavigateTo($"/authentication/login", true);
        }

        return Task.CompletedTask;
    }
}
