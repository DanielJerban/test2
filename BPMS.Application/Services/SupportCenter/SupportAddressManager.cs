using BPMS.Domain.Common.Helpers;

namespace BPMS.Application.Services.SupportCenter;

public static class SupportAddressManager
{
    public static string GetSupportCenterUrl()
    {
        if (RunningEnvironment.IsDevelopment())
            return "http://localhost:5174";

        if (RunningEnvironment.IsStaging())
            return "https://support-bpms-dev.tnc.ir";

        return "https://support.holoobpms.ir";
    }
}