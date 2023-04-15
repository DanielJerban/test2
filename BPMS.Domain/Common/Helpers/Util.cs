using System.Globalization;

namespace BPMS.Domain.Common.Helpers;

public static class Util
{
    public static string MakeTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        if (value.Length != 4)
            return string.Empty;
        if (String.IsNullOrWhiteSpace(value))
            return string.Empty;
        return value.Substring(0, 2) + ":" + value.Substring(2, 2);
    }

    public static string MakeDate(string value)
    {
        if (String.IsNullOrWhiteSpace(value))
            return string.Empty;
        if (value.Length < 8)
            return string.Empty;
        return value.Substring(0, 4) + "/" + value.Substring(4, 2) + "/" + value.Substring(6, 2);
    }

    public static int ConvertMiladyToShamsi(string date)
    {
        var datetime = Convert.ToDateTime(date);
        var p = new PersianCalendar();
        var year = p.GetYear(datetime).ToString("D4");
        var month = p.GetMonth(datetime).ToString("D2");
        var day = p.GetDayOfMonth(datetime).ToString("D2");
        return int.Parse(year + month + day);


    }
    public static DateTime ConvertMiladyToShamsi(DateTime datetime)
    {
        var p = new PersianCalendar();
        var year = p.GetYear(datetime);
        var month = p.GetMonth(datetime);
        var day = p.GetDayOfMonth(datetime);
        var hours = p.GetHour(datetime);
        var minute = p.GetMinute(datetime);
        var second = p.GetSecond(datetime);
        return new DateTime(year, month, day, hours, minute, second);
    }
    public static DateTime ConvertMiladiToShamsi(DateTime datetime)
    {
        var p = new PersianCalendar();
        var year = p.GetYear(datetime);
        var month = p.GetMonth(datetime);
        var day = p.GetDayOfMonth(datetime);
        var hours = p.GetHour(datetime);
        var minute = p.GetMinute(datetime);
        var second = p.GetSecond(datetime);
        var milisecond = p.GetMilliseconds(datetime);
        return new DateTime(year, month, day, hours, minute, second, p);

    }

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static DateTime GetDateTime(int intDate, string strTime)
    {
        PersianCalendar pc = new PersianCalendar();
        var date = intDate.ToString();
        var year = Convert.ToInt32(date.Substring(0, 4));
        var month = Convert.ToInt32(date.Substring(4, 2));
        var day = Convert.ToInt32(date.Substring(6, 2));
        DateTime dt = new DateTime(year, month, day, pc);
        var hour = Convert.ToInt32(strTime.Substring(0, 2));
        var minute = Convert.ToInt32(strTime.Substring(2, 2));
        return dt + new TimeSpan(hour, minute, 0);
    }

    // TODO: Uncomment the content later 
    //public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
    //{
    //    controllerContext.Controller.ViewData.Model = model;
    //    using (var sw = new StringWriter())
    //    {
    //        var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
    //        var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
    //        viewResult.View.Render(viewContext, sw);
    //        viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
    //        return sw.GetStringBuilder().ToString();
    //    }

    //    return "";
    //}
}