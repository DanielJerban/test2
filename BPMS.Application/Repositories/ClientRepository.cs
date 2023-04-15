using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BPMS.Application.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(BpmsDbContext context) : base(context)
    {
    }
    public BpmsDbContext _dbContext => Context;

    public void CreateClient(ClientViewModel model)
    {
        var client = _dbContext.Clients.FirstOrDefault(d => d.Id == model.Id);
        if (client != null)
        {
            client.Dsr = model.Dsr;
            client.FName = model.FName;
            client.LName = model.LName;
            client.Address = model.Address;
            client.Avtive = model.Active;
            client.CellPhone = model.CellPhone;
            client.Email = model.Email;
            client.FromDsr = model.FromDsr;
            client.NationalNo = model.NationalNo;
            client.OrganizationPost = model.OrganizationPost;

            _dbContext.Entry(client).State = EntityState.Modified;
        }
        else
        {
            client = new Client()
            {
                Dsr = model.Dsr,
                FName = model.FName,
                LName = model.LName,
                Address = model.Address,
                Avtive = model.Active,
                CellPhone = model.CellPhone,
                Email = model.Email,
                FromDsr = model.FromDsr,
                NationalNo = model.NationalNo,
                OrganizationPost = model.OrganizationPost
            };
            _dbContext.Clients.Add(client);
        }
    }

    public void DeleteClient(Guid id)
    {
        var client = _dbContext.Clients.Find(id);
        if (client == null)
        {
            throw new ArgumentException(@"رکورد مورد نظر پیدا نشد.", "custom");
        }
        var encoding = new UnicodeEncoding();
        var requests = _dbContext.Requests.ToList();
        foreach (var request in requests)
        {
            if (request.Value == null) continue;
            var value = JObject.Parse(encoding.GetString(request.Value));
            foreach (var key in value)
            {
                if (key.Value.ToString() == id.ToString())
                {
                    throw new ArgumentException(@"امکان حذف رکورد به دلیل استفاده در گردش کار وجود ندارد.", "custom");
                }
            }
        }

        _dbContext.Clients.Remove(client);
    }

    public IEnumerable<ClientViewModel> GetAllClients()
    {
        var query = from client in _dbContext.Clients
                    select new ClientViewModel()
                    {
                        Id = client.Id,
                        Email = client.Email,
                        Active = client.Avtive,
                        OrganizationPost = client.OrganizationPost,
                        NationalNo = client.NationalNo,
                        FName = client.FName,
                        LName = client.LName,
                        CellPhone = client.CellPhone,
                        FromDsr = client.FromDsr,
                        Address = client.Address,
                        Dsr = client.Dsr
                    };
        return query;
    }

    public ClientViewModel GetClientDetail(Guid modelId)
    {
        var client = _dbContext.Clients.Find(modelId);
        if (client == null)
        {
            throw new ArgumentException("این شخص موجود نمی باشد");
        }

        return new ClientViewModel()
        {
            Email = client.Email,
            Id = client.Id,
            Dsr = client.Dsr,
            OrganizationPost = client.OrganizationPost,
            Active = client.Avtive,
            NationalNo = client.NationalNo,
            FromDsr = client.FromDsr,
            CellPhone = client.CellPhone,
            FName = client.FName,
            LName = client.LName,
            Address = client.Address
        };
    }

    public IEnumerable<ClientDto> GetClients()
    {
        return _dbContext.Clients.Where(c => c.Avtive)
            .Select(s => new ClientDto()
            {
                Id = s.Id,
                FullName = s.FName + " " + s.LName
            }).OrderBy(d => d.FullName).ToList();
    }

    public IEnumerable<ClientViewModel> GetClientsHaveEmail()
    {
        return _dbContext.Clients.Where(u => u.Email != null && u.Avtive).ToList().Select(c => new ClientViewModel
        {
            Id = c.Id,
            Email = c.Email,
            FName = c.FName,
            LName = c.LName
        });
    }

    public IEnumerable<ClientViewModel> GetClientsHavePhoneNumber()
    {
        return _dbContext.Clients.Where(u => u.CellPhone != "" && u.Avtive).ToList().Select(c => new ClientViewModel
        {
            Id = c.Id,
            CellPhone = c.CellPhone,
            FName = c.FName,
            LName = c.LName
        });
    }

    public bool PhoneNumberExist(string phoneNumber, Guid? id)
    {
        var result = false;
        if (id == null)
        {
            result = _dbContext.Clients.Any(i => i.CellPhone == phoneNumber);
        }
        else
        {
            var editingClient = _dbContext.Clients.FirstOrDefault(i => i.Id == id);
            var duplicatedPhoneClient = _dbContext.Clients.FirstOrDefault(u => u.CellPhone == phoneNumber);
            if (duplicatedPhoneClient != null)
            {
                result = editingClient != duplicatedPhoneClient;

            }
        }
        return result;
    }
}