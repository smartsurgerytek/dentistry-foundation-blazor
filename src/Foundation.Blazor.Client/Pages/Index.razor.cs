using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundation.Blazor.Client.Pages;

public partial class Index
{
    protected override Task OnInitializedAsync()
    {
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
