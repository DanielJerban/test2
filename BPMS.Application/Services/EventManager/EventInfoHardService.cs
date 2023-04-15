using System.Management;
using System.Text;

namespace BPMS.Application.Services.EventManager;

public sealed class EventInfoHardService
{
    public string GetUniqueInfo()
    {
        string os = getSoftData();
        string computerSystem = getHardData();
        string processor = getProcessData();
        string totalData = os + "@@" + computerSystem + "@@" + processor;
        string encodedString = base64Encode(totalData);
        return encodedString;
    }

    private string getSoftData()
    {
        string value = "";
        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get();
        foreach (var mo in searcher)
        {
            var searcherProperties = mo.Properties;
            foreach (var sp in searcherProperties)
            {
                if (sp.Name == "Caption" || sp.Name == "SerialNumber")
                {
                    value += sp.Name + ":" + sp.Value + "-";
                }
            }
        }

        return value;
    }

    private string getHardData()
    {
        string value = "";
        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem").Get();
        foreach (var mo in searcher)
        {
            var searcherProperties = mo.Properties;
            foreach (var sp in searcherProperties)
            {
                if (sp.Name == "Manufacturer" || sp.Name == "Model" || sp.Name == "SystemSKUNumber")
                {
                    value += sp.Name + ":" + sp.Value + "-";
                }
            }
        }

        return value;
    }

    private string getProcessData()
    {
        string value = "";
        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get();
        foreach (var mo in searcher)
        {
            var searcherProperties = mo.Properties;
            foreach (var sp in searcherProperties)
            {
                if (sp.Name == "Caption" || sp.Name == "DeviceID" || sp.Name == "Name" || sp.Name == "ProcessorId")
                {
                    value += sp.Name + ":" + sp.Value + "-";
                }
            }
        }

        return value;
    }

    private string base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }
}