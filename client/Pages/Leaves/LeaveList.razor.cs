using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Shared.Model;

namespace client.Pages.Leaves;

public partial class LeaveList
{ 
    [Inject]
    public HttpClient HttpClient { get; set; }

    private List<LeaveModel> Leaves { get; set; } = new List<LeaveModel>();
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        await Refresh();
        await base.OnInitializedAsync();
    }

    async Task Refresh()
    {
        _loading = true;
        var leaves = await HttpClient.GetFromJsonAsync<List<LeaveModel>>("api/Leaves/List");
        if (leaves != null)
        {
            Leaves = leaves;
        }

        _loading = false;
        StateHasChanged();
    }
}