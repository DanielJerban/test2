using BPMS.Domain.Common.ViewModels;
using System.Collections.Concurrent;

namespace BPMS.Application.Hubs;

//هاب اصلی که لیست افراد آنلاین را نگه میدارد
public class MainHub /*: Hub*/
{
    public static ConcurrentDictionary<string, string> OnlineUsersList = new();

    public static List<Guid> SendNotificationToSpecificClients(SendNotificationViewModel model)
    {
        return new List<Guid>();
    }

    public static void UpdateNotificationCountOnMenuBar(string? username)
    {

    }

    public static void ConfirmNotification(Guid requestId, Guid workflowDetailId, Guid flowId)
    {

    }

    public static void RejectNotification(Guid requestId, Guid workflowDetailId, Guid flowId)
    {

    }

    public static void UpdateNotificationCount(List<string> personalCodes)
    {

    }

    public static bool CheckStaffIsOnline(string personalCode)
    {
        return true;
    }

    public static void LogoutUser(string username)
    {

    }

    public static void UpdateDashboardCharts()
    {

    }

    public static void RefreshSubProcessGrid(Guid id)
    {

    }

    public static void RefreshScheduleRecords()
    {

    }

    public static void RefreshCartbotGridOnChangingFlow(List<string> usernames)
    {

    }

    //public override Task OnConnectedAsync()
    //{
    //    return base.OnConnectedAsync();
    //}

    //public override Task OnDisconnectedAsync(Exception exception)
    //{
    //    return base.OnDisconnectedAsync(exception);
    //}

    private void GetOnlineUser()
    {

    }

    public void TakeUserName(string userName, string connectionId)
    {

    }




    //private static IUnitOfWork _unitOfWork;
    //public static IUnitOfWork UnitOfWork
    //{
    //    get
    //    {
    //        _unitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>();
    //        return _unitOfWork;
    //    }
    //}


    ////لیستی شامل اطلاعات کاربران آنلاین
    ////نام کاربری و همچنین کد اتصال کاربران در قالب این دیکشنری به صورت موقتی در داخل برنامه ذخیره می شوند
    //// Value => UserName 
    //// Key => signalR connectionId 
    //public static ConcurrentDictionary<string, string> _onlineUsersList = new ConcurrentDictionary<string, string>();
    ////تعداد کاربران آنلاین سایت
    //public static int OnlineUserCount;

    //public static List<string> PersonelCodesToNotify;
    //public static string mobileconnectionId;

    //public static List<Guid> SendNotificationToSpecificClientss(SendNotificationViewModel model)
    //{
    //    var mylist = new List<string>();
    //    var flowList = new List<Guid>();
    //    PersonelCodesToNotify = model.FlowStaffs.Select(s => s.UserName).ToList();
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    if (model.RequestNo != null)
    //    {
    //        foreach (var item in model.FlowStaffs)
    //        {
    //            var flow = UnitOfWork.Flows.Get(item.FlowId);

    //            if (flow == null) continue;

    //            if (flow.IsActive == false) continue;
    //            if (_onlineUsersList.Values.Contains(item.UserName))
    //            {
    //                var connectionIds = _onlineUsersList.Where(d => d.Value == item.UserName);
    //                foreach (var connectionId in connectionIds)
    //                {
    //                    mylist.Add(connectionId.Key);
    //                    if (item.FlowId != Guid.Empty && item.FlowId != null)
    //                    {
    //                        var currentFlow = UnitOfWork.Flows.Get(item.FlowId);
    //                        currentFlow.IsBalloon = true;
    //                        UnitOfWork.Flows.AddOrUpdate(currentFlow);
    //                        UnitOfWork.Complete();
    //                        flowList.Add(item.FlowId);
    //                    }


    //                }
    //            }
    //        }
    //        var message = "شما یک درخواست   " + model.RequestTitle + " از  " + model.RequesterStaff + " " + "دریافت کرده اید.";
    //        _context.Clients.Clients(mylist).SendNotification(message);
    //        // Send Notification to mobile devices
    //        List<string> mobileClients = new List<string>();
    //        model.FlowStaffs.ForEach(item => { mobileClients.Add(item.UserName); });
    //        //MobileService mobileService = new MobileService();
    //        //mobileService.SendNotification("Name1", message, "BPMS", Operations.iOS_Android, mobileClients);
    //    }
    //    foreach (var item in model.FlowStaffs.DistinctBy(d => d.UserName))
    //    {
    //        mylist = new List<string>();
    //        var datacountlist = UnitOfWork.Workflows.CountOfEachType(item.UserName);
    //        if (_onlineUsersList.Values.Contains(item.UserName))
    //        {
    //            var connectionIds = _onlineUsersList.Where(d => d.Value == item.UserName);
    //            foreach (var connectionId in connectionIds)
    //            {
    //                mylist.Add(connectionId.Key);
    //            }
    //        }
    //        _context.Clients.Clients(mylist.Distinct().ToList()).SendDataCount(datacountlist);
    //    }
    //    return flowList;
    //}
    //public static void UpdateNotificationCountOnMenuBar(string username = null)
    //{
    //    if (PersonelCodesToNotify == null)
    //    {
    //        PersonelCodesToNotify = new List<string>();
    //    }
    //    var userName = username == SystemConstant.SystemUser ? SystemConstant.SystemUser : username;
    //    PersonelCodesToNotify.Add(userName);
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    var connectionids = new List<string>();

    //    foreach (var item in PersonelCodesToNotify)
    //    {
    //        if (_onlineUsersList.Values.Contains(item))
    //        {
    //            var connectionId = _onlineUsersList.Where(d => d.Value == item).Select(s => s.Key).ToList();
    //            connectionids.AddRange(connectionId);
    //        }
    //    }
    //    _context.Clients.Clients(connectionids).UpdateNotificationCount();
    //    PersonelCodesToNotify = new List<string>();


    //}

    ////todo-daniel commented at 1401-07-08 if nothing happened remove this method
    ////public static void NotificationOnLogin()
    ////{
    ////    var messageList = new List<string>();
    ////    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    ////    var connectionId = _onlineUsersList.Where(d => d.Value == GlobalVariables.User.UserName).Select(s => s.Key).ToList();
    ////    var StaffId = UnitOfWork.Users.Find(u => u.UserName == GlobalVariables.User.UserName).Select(s => s.StaffId).FirstOrDefault();
    ////    var flows = from flowss in UnitOfWork.Flows.GetAll()
    ////                join request in UnitOfWork.Request.GetAll() on flowss.RequestId equals request.Id
    ////                where flowss.StaffId == StaffId && flowss.IsBalloon == false && flowss.IsActive
    ////                select new SendNotificationViewModel()
    ////                {
    ////                    RequestNo = request.RequestNo.ToString(),
    ////                    RequestTitle = request.Workflow.RequestType.Title,
    ////                    RequesterPersonelCode = request.Staff.PersonalCode,
    ////                    RequesterStaff = request.Staff.FName + " " + request.Staff.LName,
    ////                };
    ////    foreach (var flow in flows)
    ////    {
    ////        var message = "شما یک درخواست   " + flow.RequestTitle + " از  " + flow.RequesterStaff + " " + "دریافت کرده اید.";
    ////        messageList.Add(message);
    ////    }
    ////    _context.Clients.Clients(connectionId).sendNotificationOnLogin(messageList);
    ////    //return messageList;

    ////}
    //public static void ConfirmNotification(Guid requestId, Guid worklowDetailId, Guid flowId)
    //{


    //    var request = UnitOfWork.Request.Get(requestId);
    //    var workflowdetail = UnitOfWork.WorkflowDetails.Get(worklowDetailId);



    //    var person = UnitOfWork.Users.Find(d => d.StaffId == request.StaffId).FirstOrDefault().UserName;
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    var connectionids = new List<string>();

    //    var message = "درخواست" + "  " + request.Workflow.RequestType.Title + " " + " با " + " " + request.RequestNo + "در مرحله ی " + " " + workflowdetail.Title + " تایید شد.";
    //    //foreach (var item in person)
    //    //{
    //    if (_onlineUsersList.Values.Contains(person))
    //    {
    //        var connectionId = _onlineUsersList.Where(d => d.Value == person).Select(s => s.Key).ToList();
    //        connectionids.AddRange(connectionId);
    //        UnitOfWork.Flows.ChangeIsBalloonStatusInFlow(new List<Guid>()
    //                {
    //                    flowId
    //                });
    //        UnitOfWork.Complete();

    //        var datacountlist = UnitOfWork.Workflows.CountOfEachType(person);
    //        _context.Clients.Clients(connectionids).SendDataCount(datacountlist);
    //        //  UnitOfWork.Complete();
    //    }
    //    // }

    //    // var flow = UnitOfWork.Flows.Get(flowId);
    //    // if (flow.IsActive)
    //    _context.Clients.Clients(connectionids).ConfirmNotif(message);


    //}

    //public static void RejectNotification(Guid requestId, Guid worklowDetailId, Guid FlowId)
    //{
    //    var request = UnitOfWork.Request.Get(requestId);
    //    var workflowdetail = UnitOfWork.WorkflowDetails.Get(worklowDetailId);
    //    var person = from staff in UnitOfWork.Staffs.GetAll()
    //                 join users in UnitOfWork.Users.GetAll() on staff.Id equals users.StaffId
    //                 where staff.Id == request.StaffId
    //                 select users.UserName;
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    var connectionids = new List<string>();
    //    if (person != null)
    //    {
    //        var message = "درخواست" + "  " + request.Workflow.RequestType.Title + " " + " با شماره رهگیری" + " " + request.RequestNo + "در مرحله ی " + " " + workflowdetail.Title + " رد شد.";
    //        foreach (var item in person)
    //        {
    //            if (_onlineUsersList.Values.Contains(item))
    //            {
    //                var connectionId = _onlineUsersList.Where(d => d.Value == item).Select(s => s.Key).ToList();
    //                connectionids.AddRange(connectionId);
    //                UnitOfWork.Flows.ChangeIsBalloonStatusInFlow(new List<Guid>()
    //                {
    //                    FlowId
    //                });
    //                UnitOfWork.Complete();
    //                var datacountlist = UnitOfWork.Workflows.CountOfEachType(item);
    //                _context.Clients.Clients(connectionids).SendDataCount(datacountlist);
    //            }
    //        }
    //        _context.Clients.Clients(connectionids).RejecrNotif(message);
    //    }
    //}

    //public static void UpdateNotificationCount(List<string> personelCodes)
    //{
    //    var context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    var connectionids = new List<string>();
    //    foreach (var item in personelCodes)
    //    {
    //        if (_onlineUsersList.Values.Contains(item))
    //        {
    //            var connectionId = _onlineUsersList.Where(d => d.Value == item).Select(s => s.Key).ToList();
    //            connectionids.AddRange(connectionId);
    //        }
    //    }
    //    context.Clients.Clients(connectionids).UpdateNotificationCount();
    //}

    ///// <summary>
    ///// در این بخش می توان آنلاین بودن یا نبودن افراد را بررسی کرد
    ///// </summary>
    //public static bool CheckStaffIsOnline(string personelCode)
    //{
    //    if (_onlineUsersList.Values.Contains(personelCode))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    ///// <summary>
    ///// این متد زمانی فراخوانی می شود که یک فرد به Hub متصل شود
    ///// </summary>
    //public override Task OnConnected()
    //{
    //    GetOnlineUser();
    //    return base.OnConnected();
    //}

    //private void GetOnlineUser()
    //{
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    var connectedUserName = HostingApplication.Context.User.Identity.Name;
    //    //  var connectedUserName = Context.QueryString.FirstOrDefault(d => d.Key == "PersonalCode").Value;
    //    //برای زمانی که کاربری با موبایل متصل می شود
    //    if (connectedUserName == String.Empty)
    //    {
    //        mobileconnectionId = HostingApplication.Context.ConnectionId;
    //    }
    //    //برای زمانی که کاربری با مرورگر متصل می شود
    //    else
    //    {
    //        if (!_onlineUsersList.Values.Contains(connectedUserName))
    //        {
    //            OnlineUserCount++;
    //        }

    //        _onlineUsersList.TryAdd(HostingApplication.Context.ConnectionId, connectedUserName);
    //        //   CartbotHub.ShowOnlineStatusForUsers(connectedUserName);
    //        _context.Clients.All.UpdateOnlineUsersGrid(connectedUserName);
    //    }
    //}

    //public override Task OnReconnected()
    //{
    //    GetOnlineUser();
    //    return base.OnReconnected();
    //}

    //public void TakeUserName(string userName, string connectionId)
    //{
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    if (!_onlineUsersList.Values.Contains(userName))
    //    {
    //        OnlineUserCount++;
    //    }
    //    _onlineUsersList.TryAdd(connectionId, userName);
    //    _context.Clients.All.UpdateOnlineUsersGrid(userName);
    //    //_context.Groups.Add("a", userName);

    //    mobileconnectionId = string.Empty;

    //}

    //public static void LogoutUser(string username)
    //{
    //    var context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();

    //    var offlineUsers = _onlineUsersList.Where(c => c.Value == username).ToList();
    //    foreach (var offlineUser in offlineUsers)
    //    {
    //        string key = offlineUser.Key;
    //        _onlineUsersList.TryRemove(key, out _);
    //    }

    //    OnlineUserCount--;

    //    context.Clients.All.OflineUsersGrid(username);
    //}

    ///// <summary>
    /////  این متد زمانی فراخوانی می شود که یک فرد از Hub قطع شود
    ///// </summary>
    //public override Task OnDisconnected(bool stopCalled)
    //{
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    string username;
    //    _onlineUsersList.TryGetValue(HostingApplication.Context.ConnectionId, out username);
    //    if (username != null)
    //    {
    //        var count = _onlineUsersList.Values.Count(e => e.Contains(username));
    //        if (count == 1)
    //        {
    //            _onlineUsersList.TryRemove(HostingApplication.Context.ConnectionId, out username);
    //            if (OnlineUserCount != 0)
    //            {
    //                OnlineUserCount = OnlineUserCount - 1;

    //            }
    //            _context.Clients.All.OflineUsersGrid(username);
    //        }
    //        else if (count > 1)
    //        {
    //            _onlineUsersList.TryRemove(HostingApplication.Context.ConnectionId, out username);
    //        }

    //        // CartbotHub.ShowOfflineStatusForUsers(username);
    //    }
    //    return base.OnDisconnected(stopCalled);
    //}

    //public static void UpdateDashboardCharts()
    //{
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    _context.Clients.All.UpdateDashboards();
    //}

    //public static void RefreshSubProcessGrid(Guid id)
    //{
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    _context.Clients.All.refreshSubProcessGrid(id);
    //}

    //public static void RefreshScheduleRecords()
    //{
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    _context.Clients.All.refreshScheduleRecords();
    //}



    ///// <summary>
    ///// Refresh all grids in CartbotIndex when a flow diverts to an other user ( تغییر انجام دهنده کار )
    ///// </summary>
    //public static void RefreshCartbotGridOnChangingFlow(List<string> usernames)
    //{
    //    IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
    //    List<string> connectionIds = new List<string>();

    //    foreach (var username in usernames)
    //    {
    //        var connectionIdsByUser = _onlineUsersList.Where(c => c.Value == username).Select(s => s.Key).ToList();
    //        foreach (var item in connectionIdsByUser)
    //        {
    //            connectionIds.Add(item);
    //        }
    //    }

    //    GlobalHost.ConnectionManager.GetHubContext<MainHub>().Clients.Clients(connectionIds).refreshCartbotGrid();
    //}


}