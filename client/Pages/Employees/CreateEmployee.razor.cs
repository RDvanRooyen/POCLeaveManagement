using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared;
using Shared.Model;

namespace client.Pages.Employees;

public partial class CreateEmployee : ComponentBase
{
    [Inject] public HttpClient HttpClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    private CreateEmployeeRequest Request = new CreateEmployeeRequest();

    async Task Save()
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync("/api/Employees/Create", Request);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
            if (result == null)
            {
                Snackbar.SomethingUnexpected();
            }

            Snackbar.HandleApiResult(result);
            if (result.Succeeded)
            {
                Navigation.NavigateTo("/app/employees");
            }
        }
        catch
        {
            Snackbar.SomethingUnexpected();
        }
    }
}