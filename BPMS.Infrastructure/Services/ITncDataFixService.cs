using BPMS.Domain.Common.Dtos.TncDataFix;

namespace BPMS.Infrastructure.Services;

public interface ITncDataFixService
{
    LoginDeserializeDataFixDto Login(LoginInputDto model);
    List<PartyDto> GetParties(PartyInputDto model, string username);
    List<DatabaseDto> GetDatabases(DatabaseInputDto model);
    List<AssetDto> GetAssets(AssetInputDto model);
    List<LocationDto> GetLocation(LocationInputDto model);
    SetAssetDto SetAsset(SetAssetInputDto model);
    List<ValidateSetAssetDto> ValidateSetAsset(SetAssetInputDto model);
}