using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using Kendo.Mvc.UI;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IRequestRepository : IRepository<Request>
{
    List<Flow> GetNotificationBarData(string username);
    float GetDatasByRequestStatus(Guid requestStatusId, string username);
    ChartsViewModel GetChartsData(string username);
    IEnumerable<Flow> GetFlowsRequest(Guid? statusId, string username);
    IEnumerable<Flow> ExternalGetFlowsRequest(Guid? statusId, Guid staffId);
    IEnumerable<RequestsInCartbotViewModel> GetRequests(Guid id, string username);
    IEnumerable<RequestsInCartbotViewModel> ExternalGetRequests(Guid id, Guid staffId);
    IEnumerable<FlowsInCartbotViewModel> GetFlows(Guid id, string username);
    IEnumerable<ExternalFlowsInCartbotViewModel> ExternalGetFlows(Guid id, Guid staffId);
    Tuple<double, int> GetCountOfRequestsWithStatusCode1();
    Tuple<double, int> GetCountOfRequestsWithStatusCode2();
    IEnumerable<ThirdChartViewModel> GetThirdChartData();
    IEnumerable<ThirdChartViewModel> GetDataForSecondChart();
    IEnumerable<DashboardColumnChartViewModel> FindRequestsWithDelay();
    List<Flow> GetWorkFlowDiagramDatas(Guid requestId);
    void ChangeIsBalloonStatusWhanMessagesRead(Guid requestId);
    List<PieChartViewModel> GetRequestPieChartData(string username);
    void ChangeRequestsStaff(ChangeFlowStaffsViewModel vm);
    int GetCountOfAllNotification(Guid staffId);
    List<string> DeleteRequests(Guid requestid, string username);
    List<string> DeleteListRequests(List<Guid> requestsId, string username);
    void ChangeStatus(Guid requestId, bool isIgnored);
    void CreateEmployementCertificate(Guid requestId, RequestEmployementCertificationViewModel model);
    void UpdateEmployementCertificate(Guid currentFlowId, string dsr);
    List<PieChartViewModel> GetFlowPieChartData(string username);
    IEnumerable<DashboardColumnChartViewModel> SupportPersonelActivitiesCount();
    IEnumerable<DashboardColumnChartViewModel> ITPersonelActivitiesCount();
    List<OnlineUsersViewModel> GetOnlineUsersList();
    IEnumerable<RequestViewModel> GetRequestForManagment(string condition);
    IEnumerable<ThirdChartViewModel> GetSediment(int a, List<Holyday> holydays);
    DataSourceResult GetRequests(Guid userStaffId, DataSourceRequest request, int code);
    IEnumerable<RequestViewModel> GetRequestsByRequestType(Guid requestTypeId);
    object GetRequestsByUserId(Guid userStaffId);
    void SubmitChangeRequest(Guid currentStaffId, List<SubmitChangeRequestViewModel> model, string username);
    Request GetRequestById(Guid requestId);
    Request GetRequestsByRequestNo(long requestNo);
    Request FindById(Guid Id);
}