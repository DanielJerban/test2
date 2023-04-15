using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IClientRepository : IRepository<Client>
{
    IEnumerable<ClientViewModel> GetAllClients();
    void CreateClient(ClientViewModel model);
    void DeleteClient(Guid id);
    IEnumerable<ClientDto> GetClients();
    ClientViewModel GetClientDetail(Guid modelId);
    IEnumerable<ClientViewModel> GetClientsHaveEmail();
    IEnumerable<ClientViewModel> GetClientsHavePhoneNumber();
    bool PhoneNumberExist(string phoneNumber, Guid? id);
}