using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BPMS.Application.Repositories;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(BpmsDbContext context) : base(context)
    {
    }
    public BpmsDbContext DbContext => Context;

    public void CreateCompany(CompanyViewModel model)
    {
        var registerDate = long.Parse(DateTime.Now.ToString("yyyyMMdd"));
        var registerTime = DateTime.Now.ToString("HHmm");
        var company = DbContext.Companies.FirstOrDefault(d => d.Id == model.Id);
        if (company != null)
        {
            company.Dsr = model.Dsr;
            company.FullAddress = model.FullAddress;
            company.Name = model.Name;
            company.PostalCode = model.PostalCode;
            company.RegistrationNo = model.RegistrationNo;
            company.ShortName = model.ShortName;
            company.Telephone = model.Telephone;
            company.WebSite = model.WebSite;
            company.EconomicCode = model.EconomicCode;
            company.Fax = model.Fax;
            company.Email = model.Email;
            company.NationalCode = model.NationalCode;
            company.RegisterDate = registerDate;
            company.RegisterTime = registerTime;
            DbContext.Entry(company).State = EntityState.Modified;
        }
        else
        {
            company = new Company()
            {
                Email = model.Email,
                Dsr = model.Dsr,
                EconomicCode = model.EconomicCode,
                Fax = model.Fax,
                FullAddress = model.FullAddress,
                Name = model.Name,
                NationalCode = model.NationalCode,
                PostalCode = model.PostalCode,
                RegistrationNo = model.RegistrationNo,
                RegisterDate = registerDate,
                ShortName = model.ShortName,
                Telephone = model.Telephone,
                WebSite = model.WebSite,
                RegisterTime = registerTime
            };
            DbContext.Companies.Add(company);
        }
    }

    public void DeleteCompany(Guid id)
    {

        var company = DbContext.Companies.Find(id);
        if (company == null)
        {
            throw new ArgumentException("رکورد مورد نظر پیدا نشد.");
        }
        UnicodeEncoding encoding = new UnicodeEncoding();
        var requests = DbContext.Requests.ToList();
        foreach (var request in requests)
        {
            if (request.Value == null) continue;
            var value = JObject.Parse(encoding.GetString(request.Value));
            foreach (var key in value)
            {
                if (key.Value.ToString() == id.ToString())
                {
                    throw new ArgumentException("امکان حذف رکورد به دلیل استفاده در گردش کار وجود ندارد.");
                }
            }
        }

        DbContext.Companies.Remove(company);
    }

    public IEnumerable<CompanyViewModel> GetAllCompanies()
    {
        var query = from company in DbContext.Companies
                    select new CompanyViewModel()
                    {
                        Id = company.Id,
                        Email = company.Email,
                        EconomicCode = company.EconomicCode,
                        Fax = company.Fax,
                        NationalCode = company.NationalCode,
                        RegistrationNo = company.RegistrationNo,
                        WebSite = company.WebSite,
                        Telephone = company.Telephone,
                        ShortName = company.ShortName,
                        PostalCode = company.PostalCode,
                        FullAddress = company.FullAddress,
                        Dsr = company.Dsr,
                        Name = company.Name,
                        RegisterDate = company.RegisterDate,
                        RegisterTime = company.RegisterTime,
                        AddDate = DateTime.MinValue
                    };

        var result = query.ToList();
        foreach (var item in result)
        {
            if (item.RegisterTime != null)
                item.AddDate = HelperBs.GetDateTime((int)item.RegisterDate, item.RegisterTime);
        }

        return result.OrderByDescending(a => a.AddDate);
    }

    public IEnumerable<CompanyDto> GetCompanies()
    {
        return DbContext.Companies
            .Select(s => new CompanyDto()
            {
                Id = s.Id,
                Name = s.Name
            }).OrderBy(d => d.Name);
    }

    public IEnumerable<CompanyViewModel> GetCompaniesHaveEmail()
    {
        return DbContext.Companies.Where(u => u.Email != null).ToList().Select(c => new CompanyViewModel
        {
            Id = c.Id,
            Email = c.Email,
            Name = c.Name
        });
    }

    public CompanyViewModel GetCompanyDetail(Guid modelId)
    {
        var comp = DbContext.Companies.Find(modelId);
        if (comp == null)
        {
            throw new ArgumentException("این شرکت موجود نمی باشد");
        }

        return new CompanyViewModel()
        {
            Email = comp.Email,
            Id = comp.Id,
            WebSite = comp.WebSite,
            EconomicCode = comp.EconomicCode,
            Fax = comp.Fax,
            Telephone = comp.Telephone,
            ShortName = comp.ShortName,
            NationalCode = comp.NationalCode,
            Name = comp.Name,
            RegistrationNo = comp.RegistrationNo,
            Dsr = comp.Dsr,
            PostalCode = comp.PostalCode,
            FullAddress = comp.FullAddress,
            RegisterDate = comp.RegisterDate
        };
    }
}