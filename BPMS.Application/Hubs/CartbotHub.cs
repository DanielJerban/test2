namespace BPMS.Application.Hubs;

//todo-daniel => this hub seems useless remove if nothing happened commented at 1401-07-08 
//public class CartbotHub : Hub
//{
//    public static void ShowOnlineStatusForUsers(string connectedPersonel)
//    {
//        IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<CartbotHub>();
//        //کاربر را در تمام صفحات مورد نیاز، آنلاین می کند
//        _context.Clients.All.MakeUserOnline(connectedPersonel);
//    }

//    public static void ShowOfflineStatusForUsers(string disconnectedPersonel)
//    {
//        IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<CartbotHub>();
//        //کاربر را در تمام صفحات مربوط ، آفلاین می کند
//        _context.Clients.All.MakeUserOffline(disconnectedPersonel);
//    }

//}