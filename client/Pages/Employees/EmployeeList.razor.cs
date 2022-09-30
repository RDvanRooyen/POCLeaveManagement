using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Shared.Model;

namespace client.Pages.Employees;

public partial class EmployeeList
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    private List<EmployeeModel> Employees { get; set; } = new List<EmployeeModel>();
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        await Refresh();
        await base.OnInitializedAsync();
    }

    async Task Refresh()
    {
        _loading = true;
        var employees = await HttpClient.GetFromJsonAsync<List<EmployeeModel>>("api/Employees/List");
        if (employees != null)
        {
            Employees = employees;
        }

        _loading = false;
        StateHasChanged();
    }
}