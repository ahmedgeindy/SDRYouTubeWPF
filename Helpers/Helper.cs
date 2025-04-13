using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers
{
    static internal class Helper
    {
        internal static string DateTimeFormate => "dd/MM/yyyy HH:mm";

        internal static DateTime GetDateTimeFromUnixTimeStamp(double timestamp)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(timestamp * TimeSpan.TicksPerSecond);

            DateTime utc = new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
            DateTime local = utc.ToLocalTime();

            return utc.ToLocalTime();
        }

        internal static DateTime GetDateTimeFromUnixTimeStamp(string timestampstr)
        {
            double timestamp = Convert.ToDouble(timestampstr);

            return GetDateTimeFromUnixTimeStamp(timestamp);
        }

        internal static int ConvertStringToInt(string strNum)
        {
            int num = 0;
            if (int.TryParse(strNum, out num))
            {
                return num;
            }

            return 0;
        }

        public static byte[] ToByteArray(object image)
        {
            if (image.GetType() == typeof(System.Byte[]))
                return image as byte[];

            var str = image.ToString();
            var byteImage = System.Convert.FromBase64String(str);
            return byteImage;
        }

        public static byte[] ToByteArray(string str)
        {
            return System.Convert.FromBase64String(str);
        }

        internal static BitmapImage LoadImage(string imagestr)
        {
            byte[] imageData = ToByteArray(imagestr);

            return LoadImage(imageData);
        }

        internal static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            //  byte[] decom = DecompressGZip(imageData);

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Seek(0, SeekOrigin.Begin);
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();

            return image;
        }

        internal static byte[] GetBytes()
        {
            string someUrl = "http://www.google.com/images/logos/ps_logo2.png";
            using (var webClient = new WebClient())
            {
                return webClient.DownloadData(someUrl);
            }
        }

        internal static byte[] GetResourseBytes(string strURL)
        {
            var uri = new System.Uri(strURL);
            var converted = uri.AbsoluteUri;

            var info = Application.GetResourceStream(uri);
            var memoryStream = new MemoryStream();
            info.Stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        internal static BitmapImage GetImageFromImageSrc(string strURL)
        {
            var imageData = GetResourseBytes(strURL);

            return LoadImage(imageData);
        }

        public static byte[] DecompressGZip(byte[] compressed)
        {
            byte[] buffer = new byte[4096];
            using (MemoryStream ms = new MemoryStream(compressed))
            using (GZipStream gzs = new GZipStream(ms, CompressionMode.Decompress))
            using (MemoryStream uncompressed = new MemoryStream())
            {
                for (int r = -1; r != 0; r = gzs.Read(buffer, 0, buffer.Length))
                    if (r > 0) uncompressed.Write(buffer, 0, r);
                return uncompressed.ToArray();
            }
        }

    }

    static internal class DateTimeHelper
    {
        internal static string DateTimeFormate => "dd/MM/yyyy HH:mm";

        internal static DateTime GetLocalDate(string dateStr)
        {

           var date = DateTime.Now;
            if(DateTime.TryParse(dateStr, out date))
            {
                return date;
            }

            return GetLocalDateFromUnix(dateStr);
        }

        internal static DateTime GetLocalDateFromUnix(double timestamp)
        {
            var utcDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
            return utcDate.ConvertToLocal();
        }

        internal static DateTime GetLocalDateFromUnix(string timestampstr)
        {
            double timestamp = 0;
            if (Double.TryParse(timestampstr, out timestamp))
            {
                return GetLocalDateFromUnix(timestamp);
            }

            return DateTime.UtcNow;
        }

        internal static string GetLocalDateStringFromUnix(string timeStampStr)
        {
            var date = GetLocalDateFromUnix(timeStampStr);
            return date.ToString();
        }

    }

    public static class DateTimeExtension
    {
        public static string DateFormat;
        public static string TimeFormat;

        public static string ToFormatedString(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                return date.ToString(DateFormat, CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        public static string ToFormatedTimeStr(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                return date.ToString("HH:mm", CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        public static string ToFormatedDateTimeStr(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                var dateTimeFormate = DateFormat + " " + TimeFormat;
                return date.ToString(dateTimeFormate, CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }


        public static string ConvertToUTCStr(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                date = UTCDateHelper.ConvertToUTC(date);
                return date.ToFormatedString();
            }

            return string.Empty;
        }

        public static string ConvertToLocalStr(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                date = UTCDateHelper.ConvertToLocal(date);
                return date.ToFormatedString();
            }

            return string.Empty;
        }

        public static string ToLocalDateTimeStr(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                date = UTCDateHelper.ConvertToLocal(date);
                return date.ToFormatedDateTimeStr();
            }

            return string.Empty;
        }
        //change name as there is ambiguaty
        public static string ToUTCTimeStampStrcom(this DateTime date)
        {
            return date.ConvertToUTC().ToUnixTime().ToString();
        }

        public static double ToUnixTime(this DateTime input)
        {
            return input.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        // time
        public static string ToUTCDateTimeStr(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                date = UTCDateHelper.ConvertToUTC(date);
                return date.ToFormatedDateTimeStr();
            }

            return string.Empty;
        }

        public static string ToLocalTimeStr(this DateTime date)
        {
            if (date != null && date != default(DateTime))
            {
                date = UTCDateHelper.ConvertToLocal(date);
                return date.ToFormatedTimeStr();
            }

            return string.Empty;
        }

        static DateTimeExtension()
        {
            DateFormat = "dd/MM/yyyy";
            TimeFormat = "HH:mm";
        }
    }

    public static class UTCDateHelper
    {
        public static DateTime ConvertToUTC(this DateTime date)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Local);
            return date.ToUniversalTime();
        }

        public static DateTime ConvertToLocal(this DateTime date)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return date.ToLocalTime();
        }

        public static DateTime GetDefaultUTCDate()
        {
            return ConvertToUTC(default(DateTime));
        }

        public static DateTime GetDefaultLocalDate()
        {
            return ConvertToLocal(default(DateTime)); // check it
        }

        public static DateTime GetUTCDateNow()
        {
            return DateTime.UtcNow;
        }

        public static DateTime GetLocalDateNow()
        {
            return DateTime.Now;
        }
    }
}
