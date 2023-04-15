namespace BPMS.Domain.Common.Constants.PermissionStructure;

public class PermissionPolicyType
{
    public const string RoutePermission = "86105BD1-10EE-4CCF-8484-CDEA6D8B52FS"; // For Page Access

    // Connected to RequestTypeId column in Workflow table 
    public const string WorkFlowPermission = "80E0B41C-86E6-40B7-9198-8CFC39D7A759"; // For workflow dropdown in cartbotIndex 

    // Connected to RequestTypeId column in Workflow table 
    public const string WorkFlowPreviewPermission = "6583AF2F-262F-44DA-AF9F-66A92F878544"; // For /Cartbot/PreviewDiagram grid

    // Connected to Id column in WorkFlowForms table
    public const string WorkFlowFormPreviewPermission = "1554BFFE-F261-4B04-A6E9-DA9CB0CC5566"; // For /Cartbot/PreviewForms grid

    public const string WorkFlowIndexPermission = "284241B3-0149-4702-8197-8CA67B245BFE"; // For /Cartbot/CreateWorkFlowIndicator workflow dropdown

    // Connected to Id column in DynamicCharts table
    public const string DynamicChartReportPermission = "7C17262B-EF60-4E4F-A6CA-4721622FC0AB"; // For /Report/DynamicChartPrevious 

    // Connected to Id column in WorkFlowFormLists table
    public const string WorkFlowFormListPermission = "112D4219-BF01-404E-BBF2-78842B73508C"; // For /Report/DynamicChartPrevious 

    // Connected to Id column in Report table
    public const string ReportPermission = "B13257FA-514C-44C6-9D65-FC8D945DB691"; // For /Report/DynamicReport 

    // Connect to Id Column in lookup where type = RequestType
    public const string WorkFlowStatusPermission = "E170A86E-DD86-41E4-BF03-935D03175713"; // For /Report/ProcessStatus

    // Connect to Id Column in lookup where type = Widget
    public const string WidgetPermission = "43589716-7254-41C5-8D71-8C58E06B5B22"; // For /Dashboard/EditDashboard
}