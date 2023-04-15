namespace BPMS.Domain.Common.Helpers;

public static class RunningEnvironment
{
    public static bool IsDevelopment() => GetEnvironmentName() == "development";

    public static bool IsStaging() => GetEnvironmentName() == "staging";

    public static bool IsProduction()
    {
        string environmentName = GetEnvironmentName();

        return (string.IsNullOrEmpty(environmentName) || environmentName == "production");
    }

    private static string GetEnvironmentName() => "development" /*ConfigurationManager.AppSettings["RunningEnvironment"]?.ToLower()*/;
}