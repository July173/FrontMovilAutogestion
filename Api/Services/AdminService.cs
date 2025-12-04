using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSena.MAUI.Api.Services
{
    public class AdminService
    {
        // Currently we use a placeholder because the backend endpoint for aggregated admin counts
        // might not exist. This can be replaced with an ApiService call.
        public AdminService()
        {
        }

        public Task<AdminCountsDto> GetAdminCountsAsync()
        {
            // Return sample data for now. Replace with HTTP call to the backend when endpoint exists.
            var dto = new AdminCountsDto
            {
                TotalUsers = 128,
                TotalRoles = 8,
                TotalModules = 12,
                TotalForms = 20
            };
            return Task.FromResult(dto);
        }
    }
}
