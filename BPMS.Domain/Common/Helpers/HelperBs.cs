using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BPMS.Domain.Common.Helpers;

public static class HelperBs
{
    public static bool IsValidDate(int date, string message = null)
    {
        if (!Validate(Util.MakeDate(date.ToString()), @"\b(?'Year'(1[3-4])\d{2})/(?'Month'10|11|12|0[1-9])/(?'Day'1[0-9]|2[0-9]|30|31|0[1-9])\b"))
            throw new Exception(message ?? "فرمت تاریخ اشتباه وارد شده است");
        return true;
    }

    public static bool IsValidMiladiDate(int date, string message = null)
    {
        if (!Validate(Util.MakeDate(date.ToString()), @"\b(?'Year'([1-2])\d{4})/(?'Month'10|11|12|0[1-9])/(?'Day'1[0-9]|2[0-9]|30|31|0[1-9])\b"))
            throw new Exception(message ?? "فرمت تاریخ اشتباه وارد شده است");
        return true;
    }

    public static bool IsValidTime(string time, string message = null)
    {
        if (!Validate(MakeTime(time), @"\b(?'Houre'0[0-9]|1[0-9]|2[0-3]):(?'Min'0[0-9]|1[0-9]|2[0-9]|3[0-9]|4[0-9]|5[0-9])\b"))
            throw new Exception(message ?? "فرمت ساعت اشتباه وارد شده است");
        return true;
    }

    public static bool IsValidPersianText(string text)
    {
        return Validate(text, @"[ةپچجحخهعغفقثصضشسیبلاتنمکگوئدذرزطظژؤإآأءًٌٍَُِّۀ\،;\؛ـي«»]");
    }

    public static bool Validate(string value, string expression)
    {
        var regex = new
            Regex(expression);
        var match = regex.Match(value);
        if (match.Success)
            return true;
        else
            return false;
    }

    public static string MakeTime(string value)
    {
        return Util.MakeTime(value);
    }

    public static string MakeDate(string value)
    {
        return Util.MakeDate(value);
    }

    public static int ConvertMiladyToShamsi(string date)
    {
        return Util.ConvertMiladyToShamsi(date);
    }

    public static int ConvertMiladyToIntShamsi(DateTime date)
    {
        var p = new PersianCalendar();
        var year = p.GetYear(date).ToString("D4");
        var month = p.GetMonth(date).ToString("D2");
        var day = p.GetDayOfMonth(date).ToString("D2");
        return int.Parse(year + month + day);
    }

    public static DateTime ConvertMiladyToShamsi(DateTime date)
    {
        return Util.ConvertMiladyToShamsi(date);
    }

    public static DateTime ConvertMiladiToShamsi(DateTime date)
    {
        return Util.ConvertMiladiToShamsi(date);
    }
    public static DateTime GetDateTime(int date, string time)
    {
        return Util.GetDateTime(date, time);
    }

    public static string GetDayByPersianDate(int date)
    {
        //  var dayofweek = Util.ConvertShamsiToMilady(date).DayOfWeek;
        string day = string.Empty;
        DayOfWeek dayofweek = DayOfWeek.Friday;
        switch (dayofweek)
        {
            case DayOfWeek.Saturday:
                day = "شنبه";
                break;
            case DayOfWeek.Sunday:
                day = "یک شنبه";
                break;
            case DayOfWeek.Monday:
                day = "دوشنبه";
                break;
            case DayOfWeek.Tuesday:
                day = "سه شنبه";
                break;
            case DayOfWeek.Wednesday:
                day = "چهار شنبه";
                break;
            case DayOfWeek.Thursday:
                day = "پنج شنبه";
                break;
        }
        return day;
    }

    public static bool IsValidNationalNo(string nationalNo)
    {
        var illigalNationalNo = new string[]
        {
            "1111111111", "2222222222", "3333333333", "444444444", "5555555555",
            "6666666666", "7777777777", "8888888888", "9999999999", "0000000000"
        };
        var dic = new Dictionary<int, int>();
        if (string.IsNullOrWhiteSpace(nationalNo)) throw new Exception("کد ملی وارد نشده است");
        if (nationalNo.Length != 10) throw new Exception("طول کد ملی باید ده رقم باشد");
        if (!Validate(nationalNo, "[0-9]+$")) throw new Exception("کد ملی اشتباه وارد شده است");
        if (illigalNationalNo.Contains(nationalNo)) throw new Exception("کد ملی اشتباه وارد شده است");

        for (int i = nationalNo.Length - 1; i >= 0; i--)
        {
            dic.Add(i + 1, int.Parse(nationalNo[9 - i].ToString()));
        }

        var sumValue = dic.Where(item => item.Key != 1).Sum(item => item.Key * item.Value);

        var isValid = sumValue % 11 < 2
            ? (sumValue % 11) == dic.Last().Value ? true : false
            : 11 - (sumValue % 11) == dic.Last().Value ? true : false;

        if (!isValid)
            throw new Exception("فرمت کدملی اشتباه وارد شده است");
        return true;
    }

    public static bool IsZero(double value)
    {
        return Math.Abs(value) < 2.2204460492503131E-15;
    }

    public static byte[] ConvertStreamToBytes(Stream stream)
    {
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static Stream ConvertBytesToStream(byte[] values)
    {
        return new MemoryStream(values);
    }

    public static string CreateHash(string password, string salt)
    {
        var hashTool = new SHA256Managed();
        var passwordAsByte = Encoding.UTF8.GetBytes(string.Concat(password, salt));
        var encryptedBytes = hashTool.ComputeHash(passwordAsByte);
        hashTool.Clear();
        return Convert.ToBase64String(encryptedBytes);
    }



    public static string EncodeUri(string text)
    {
        var sb = new StringBuilder();
        var limit = 32766;
        var loop = text.Length / limit;
        for (var i = 0; i <= loop; i++)
        {
            if (i < loop)
                sb.Append(Uri.EscapeDataString(text.Substring(limit * i, limit)));
            else
                sb.Append(Uri.EscapeDataString(text.Substring(limit * i)));
        }
        return sb.ToString();
    }

    public static string GetCssStyleFromByteArray_Unicode(byte[] styleInByte)
    {
        var encoding = new UnicodeEncoding();
        var css = encoding.GetString(styleInByte);
        return css;
    }

    public static string ConvertArraysToCommaDelimited(string exp)
    {
        var token = JToken.Parse(exp);
        if (token is JObject)
        {
            var jObject = token.ToObject<JObject>();

            if (jObject.Property("TargetValue") != null)
            {


                if (jObject.Property("TargetValue").Value.GetType().Name == "JArray")
                {
                    jObject.Property("TargetValue").Value = string.Join(",", jObject.Property("TargetValue").Value);
                }
            }

            if (jObject.Property("Rules") != null)
            {
                var arr = JArray.Parse(jObject.Property("Rules").Value.ToString());
                for (var i = 0; i < arr.Count; i++)
                {

                    arr[i] = JObject.Parse(ConvertArraysToCommaDelimited(arr[i].ToString()));

                }

                jObject.Property("Rules").Value = arr;
            }

            exp = jObject.ToString();
        }

        return exp;
    }

    public static byte[] ConvertToExcel<TEntity>(IEnumerable<TEntity> data)
    {
        ExcelPackage pck = new ExcelPackage();

        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
        ws.View.RightToLeft = true;
        for (int i = 1; i <= 10; i++)
        {
            ws.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }

        Type T = typeof(TEntity);
        PropertyInfo[] props = T.GetProperties();

        int A_AsciiCode = 65;
        int rowStart = 2;


        foreach (PropertyInfo item in props)
        {
            char charName = (char)A_AsciiCode;

            string headName = "";

            if ((DisplayNameAttribute)item.GetCustomAttribute(typeof(DisplayNameAttribute)) != null)
            {
                headName = ((DisplayNameAttribute)item.GetCustomAttribute(typeof(DisplayNameAttribute)))
                    .DisplayName;
            }
            else
            {
                headName = item.Name;
            }

            ws.Cells[$"{charName}{rowStart - 1}"].Value = headName;
            ws.Cells[$"{charName}{rowStart - 1}"].Style.Font.Bold = true;
            ws.Cells[$"{charName}{rowStart - 1}"].Style.Fill.PatternType = ExcelFillStyle.DarkGray;
            ws.Cells[$"{charName}{rowStart - 1}"].Style.Fill.BackgroundColor.SetColor(1, 62, 62, 62);

            A_AsciiCode++;
        }

        foreach (var entity in data)
        {
            Type entityType = entity.GetType();
            PropertyInfo[] entityProps = entityType.GetProperties();

            int A_AsciiCode2 = 65;
            foreach (var prop in entityProps)
            {
                var propValue = prop.GetValue(entity).ToString();

                string colChar = ((char)A_AsciiCode2).ToString();

                ws.Cells[$"{colChar}{rowStart}"].Value = propValue;

                A_AsciiCode2++;
            }

            rowStart++;
        }

        ws.Cells["A:AZ"].AutoFitColumns();

        var arrayData = pck.GetAsByteArray();

        return arrayData;
    }

    public static bool IsValidJson(string strInput)
    {
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException jex)
            {
                //Exception in parsing json
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        return false;
    }

    public static DateTime ConvertShamsiToDateTime(string date)
    {
        var year = date.Substring(0, 4) + "/";
        var month = date.Substring(4, 2) + "/";
        var day = date.Substring(6, 2);

        return DateTime.Parse(year + month + day);
    }

    public static DateTime ConvertShamsiToDateTime(string date, string time)
    {
        var year = Convert.ToInt32(date.Substring(0, 4));
        var month = Convert.ToInt32(date.Substring(4, 2));
        var day = Convert.ToInt32(date.Substring(6, 2));

        var hh = Convert.ToInt32(time.Substring(0, 2));
        var mm = Convert.ToInt32(time.Substring(2, 2));

        PersianCalendar pc = new PersianCalendar();

        return pc.ToDateTime(year, month, day, hh, mm, 0, 0, 0);
    }

    public static int GetIntTime(DateTime date)
    {
        var time = (date.Hour < 10 ? $"0{date.Hour}" : date.Hour.ToString()) + "" + (date.Minute < 10 ? $"0{date.Minute}" : date.Minute.ToString());
        return Convert.ToInt32(time);
    }
}