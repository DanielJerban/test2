using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace BPMS.Application.Repositories;

public class UserSettingRepository : Repository<UserSetting>, IUserSettingRepository
{
    public UserSettingRepository(BpmsDbContext context) : base(context)
    {

    }
    public BpmsDbContext _dbContext
    {
        get { return Context as BpmsDbContext; }
    }

    public void CreateUserSetting(WidgetViewModel[] data, string username)
    {
        var user = _dbContext.Users.Single(c => c.UserName == username);

        var widgetdata = JsonConvert.SerializeObject(data);
        byte[] bytes = null;
        if (widgetdata.GetType().Name.ToLower() == "string")
        {
            var content = HttpUtility.UrlDecode(widgetdata, Encoding.UTF8);
            UnicodeEncoding encoding = new UnicodeEncoding();
            bytes = encoding.GetBytes(content);
        }

        if (_dbContext.UserSettings.Any(u => u.UserId == user.Id))
        {
            var currentUserSetting =
                _dbContext.UserSettings.FirstOrDefault(u => u.UserId == user.Id);
            currentUserSetting.Content = bytes;
            _dbContext.UserSettings.Update(currentUserSetting);
        }
        else
        {
            var model = new UserSetting()
            {
                UserId = user.Id,
                SettingTypeId = Guid.Parse("3D742E1C-9DA9-423D-BF4A-1C5F9226BE8D"),
                Content = bytes
            };
            _dbContext.UserSettings.Add(model);
        }
    }


    public string FetchUserSettings(string username)
    {
        var user = _dbContext.Users.Single(c => c.UserName == username);
        var data = _dbContext.UserSettings.Where(u => u.UserId == user.Id).Select(s => s.Content).FirstOrDefault();
        UnicodeEncoding encoding = new UnicodeEncoding();
        if (data != null)
        {
            var json = encoding.GetString(data);
            return json;
        }

        return null;
    }

    public string FetchAllUsersSettings()
    {
        string allSettings = "";
        var data = _dbContext.UserSettings.Select(s => s.Content).ToList();
        UnicodeEncoding encoding = new UnicodeEncoding();
        if (data != null)
        {
            foreach (var item in data)
            {
                var json = encoding.GetString(item);
                if (json != "null")
                {
                    json = json.Remove(0, 1);
                    json = json.Remove(json.Length - 1, 1);
                    allSettings = allSettings + "," + json;
                }
            }

            allSettings = allSettings.Remove(0, 1).Insert(0, "[").Insert(allSettings.Length, "]");


            return allSettings;
        }

        return null;
    }
}