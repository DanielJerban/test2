namespace BPMS.Application.Hubs;

//todo-daniel => this hub seems useless remove if nothing happened commented at 1401-07-08 
//public class DashboardHub : Hub
//{
//    public static List<OnlineUsersViewModel> SendOnlineList()
//    {
//        var list = new List<OnlineUsersViewModel>();
//        var onlineUsers = MainHub._onlineUsersList.DistinctBy(s => s.Value);
//        foreach (KeyValuePair<String, String> entry in onlineUsers)
//        {
//            list.Add(new OnlineUsersViewModel()
//            {
//                UserName = entry.Value,
//            });
//        }
//        return list;
//    }
//}