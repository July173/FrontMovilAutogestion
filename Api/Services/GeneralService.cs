using System.Collections.Generic;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api.Dtos.General;

namespace AutogestionSena.MAUI.Api.Services
{
    public class GeneralService
    {
        private readonly ApiService _api;
        
        public GeneralService()
        {
            _api = new ApiService();
        }
        
        public GeneralService(ApiService api)
        {
            _api = api;
        }

        public async Task<List<TypeOfQueryDto>?> GetTypeOfQueriesAsync()
        {
            return await _api.GetAsync<List<TypeOfQueryDto>>(Endpoints.General.TypeOfQueries);
        }

        public async Task<List<SupportContactDto>?> GetSupportContactsAsync()
        {
            return await _api.GetAsync<List<SupportContactDto>>(Endpoints.General.SupportContacts);
        }

        public async Task<List<SupportScheduleDto>?> GetSupportSchedulesAsync()
        {
            return await _api.GetAsync<List<SupportScheduleDto>>(Endpoints.General.SupportSchedules);
        }

        public async Task<List<Dtos.LegalDocumentDto>?> GetLegalDocumentsAsync()
        {
            return await _api.GetAsync<List<Dtos.LegalDocumentDto>>(Endpoints.General.LegalDocuments);
        }

        public async Task<List<Dtos.LegalSectionDto>?> GetLegalSectionsAsync()
        {
            return await _api.GetAsync<List<Dtos.LegalSectionDto>>(Endpoints.General.LegalSections);
        }
    }
}
