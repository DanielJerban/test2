using BPMS.Domain.Common.Dtos.TncDataFix;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;
using Newtonsoft.Json;
using RestSharp;

namespace BPMS.Application.Services;

public class TncDataFixService : ITncDataFixService
{
    private readonly IUnitOfWork UnitOfWork;

    public TncDataFixService(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }
    /// <summary>
    /// Login to spad databse 
    /// </summary>
    /// <param name="model"></param>
    /// <returns>A Token to user other methods </returns>
    public LoginDeserializeDataFixDto Login(LoginInputDto model)
    {
        var client = new RestClient(model.Url);
        var request = new RestRequest(model.Url, Method.Post);
        request.AddParameter("Authorization", "");
        request.AddParameter("dbname", model.DatabaseName);
        request.AddParameter("username", model.AdminUserName);
        request.AddParameter("userpass", model.AdminPassword);
        var response = client.Execute(request);

        return JsonConvert.DeserializeObject<LoginDeserializeDataFixDto>(response.Content);
    }

    /// <summary>
    /// Get personels from spad database 
    /// </summary>
    public List<PartyDto> GetParties(PartyInputDto model, string username)
    {
        var client = new RestClient(model.Url);
        var request = new RestRequest(model.Url);
        request.AddHeader("Authorization", model.Token);
        var response = client.Execute(request);

        List<PartyDeserializeDto> deserializedData = JsonConvert.DeserializeObject<List<PartyDeserializeDto>>(response.Content);
        List<PartyDto> parties = new List<PartyDto>();

        foreach (var des in deserializedData)
        {
            var thisStaff = UnitOfWork.Staffs.SingleOrDefault(c => c.PersonalCode == des.Code);
            if (thisStaff != null)
            {
                parties.Add(new PartyDto()
                {
                    Id = thisStaff.Id.ToString(),
                    Title = des.PartyTitle
                });
            }
        }

        var user = UnitOfWork.Users.Single(c => c.UserName == username);
        if (parties.Any(c => c.Id == user.Staff.Id.ToString()))
        {
            var party = parties.Single(c => c.Id == user.StaffId.ToString());
            parties.Remove(party);
            List<PartyDto> filteredParties = parties;
            return filteredParties;
        }

        return parties;
    }

    /// <summary>
    /// Get databases Names and the company that they belong to
    /// </summary>
    public List<DatabaseDto> GetDatabases(DatabaseInputDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Token))
        {
            throw new ArgumentException("There is no token");
        }

        var client = new RestClient(model.Url);
        var request = new RestRequest(model.Url);
        request.AddHeader("Authorization", model.Token);
        var response = client.Execute(request);

        return JsonConvert.DeserializeObject<List<DatabaseDto>>(response.Content);
    }

    /// <summary>
    /// Get assets of a User or Employee or Personal 
    /// </summary>
    public List<AssetDto> GetAssets(AssetInputDto model)
    {
        List<AssetDto> assetList = new List<AssetDto>();

        var client = new RestClient(model.Url);
        //var client = new RestClient(model.Url + "?partycode="+ model.PersonalCode);
        var request = new RestRequest(model.Url);
        request.AddParameter("partycode", model.PersonalCode, ParameterType.QueryString);
        request.AddHeader("Authorization", model.Token);
        request.AlwaysMultipartFormData = true;
        var getAssetResponse = client.Execute(request);

        var assetContent = JsonConvert.DeserializeObject<List<AssetDeserializeDto>>(getAssetResponse.Content);
        if (assetContent.Count > 0)
        {
            foreach (var asset in assetContent)
            {
                if (assetList.FirstOrDefault(c => c.AssetNumber == asset.fixAssetNo) == null)
                {
                    assetList.Add(new AssetDto()
                    {
                        IsChecked = false,
                        AssetNumber = asset.fixAssetNo,
                        Location = asset.FixLocationTitle,
                        OwnerName = asset.PartyTitle,
                        Title = asset.FixTitle,
                        DatabaseName = model.DatabaseName,
                        CompanyName = model.CompanyName
                    });
                }
            }
        }

        return assetList;
    }

    /// <summary>
    /// Get the diffrent location of the company
    /// </summary>
    public List<LocationDto> GetLocation(LocationInputDto model)
    {
        var client = new RestClient(model.Url);
        var request = new RestRequest(model.Url);
        request.AddHeader("Authorization", model.Token);
        var response = client.Execute(request);

        return JsonConvert.DeserializeObject<List<LocationDto>>(response.Content);
    }

    /// <summary>
    /// Change the owner of stuff 
    /// </summary>
    public SetAssetDto SetAsset(SetAssetInputDto model)
    {
        var client = new RestClient(model.Url);
        var request = new RestRequest(model.Url, Method.Post);
        request.AddHeader("Authorization", model.Token);
        request.AddParameter("assetno", model.AssetNo);
        request.AddParameter("locationid", model.LocationId);
        request.AddParameter("topartycode", model.ToPersonalCode);
        request.AddParameter("assetdate", model.DateTime);
        var response = client.Execute(request);

        return JsonConvert.DeserializeObject<List<SetAssetDto>>(response.Content)[0];
    }

    public List<ValidateSetAssetDto> ValidateSetAsset(SetAssetInputDto model)
    {
        var client = new RestClient(model.Url);
        var request = new RestRequest(model.Url, Method.Post);
        request.AddHeader("Authorization", model.Token);
        request.AddParameter("assetno", model.AssetNo);
        request.AddParameter("locationid", model.LocationId);
        request.AddParameter("topartycode", model.ToPersonalCode);
        request.AddParameter("assetdate", model.DateTime);
        var response = client.Execute(request);

        return JsonConvert.DeserializeObject<List<ValidateSetAssetDto>>(response.Content);
    }
}