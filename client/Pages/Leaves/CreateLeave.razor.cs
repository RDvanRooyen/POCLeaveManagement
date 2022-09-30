using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;

namespace client.Pages.Leaves;

public partial class CreateLeave : ComponentBase
{
    [Inject] public HttpClient HttpClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    private CreateLeaveRequest Request = new CreateLeaveRequest();

    async Task Save()
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync("/api/Leaves/Create", Request);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
            if (result == null)
            {
                Snackbar.SomethingUnexpected();
            }

            Snackbar.HandleApiResult(result);
            if (result.Succeeded)
            {
                Navigation.NavigateTo("/app/leaves");
            }
        }
        catch
        {
            Snackbar.SomethingUnexpected();
        }
    }

    string AutocompleteDisplayFunction(EmployeeModel model)
    {
        return $"{model.Name} {model.LastName}";
    }

    async Task<List<EmployeeModel>> AutocompleteEmployees(string input)
    {
        try
        {
            return await HttpClient.GetFromJsonAsync<List<EmployeeModel>>(
                $"api/Employees/AutocompleteEmployees?input={input}");
        }
        catch
        {
            return new();
        }
    }

    void AutocompleteValueChanged(EmployeeModel model)
    {
        if (model == null)
        {
            StateHasChanged();
            return;
        }

        if (model.Id == null)
        {
            StateHasChanged();
            return;
        }
        Request.EmployeeId = model.Id;
        StateHasChanged();
    }
}