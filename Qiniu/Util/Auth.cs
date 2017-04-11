using System;
using System.Text;

namespace Qiniu.Util
{
    public class Auth
    {
        public static string createUploadToken(PutPolicy putPolicy, Mac mac)
        {
            string jsonData = putPolicy.ToString();
            return mac.SignWithData(Encoding.UTF8.GetBytes(jsonData));
        }

        public static string createManageToken(string url, byte[] reqBody, Mac mac)
        {
            return string.Format("QBox {0}", mac.SignRequest(url, reqBody));
        }

        public static string createDownloadToken(string rawUrl, Mac mac)
        {
            return mac.Sign(Encoding.UTF8.GetBytes(rawUrl));
        }

        public static string PrivateDownloadUrl(string baseUrl, Mac mac, int seconds = 3600)
        {
            return PrivateDownloadUrl(baseUrl, seconds, mac);
        }

        /// <summary>
        /// Privates the download URL.
        /// </summary>
        /// <returns>The download URL.</returns>
        /// <param name="baseUrl">Base URL.</param>
        /// <param name="expires">Expires.</param>
        public static string PrivateDownloadUrl(string baseUrl, long expires, Mac mac)
        {
            return PrivateDownloadUrlWithDeadline(baseUrl, Deadline(expires), mac);
        }

        public delegate long Clock();

        private static Clock clock = delegate () {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        };

        private static long Deadline(long expires)
        {
            return clock() + expires;
        }


        private static string PrivateDownloadUrlWithDeadline(string baseUrl, long deadline, Mac mac)
        {
            StringBuilder b = new StringBuilder();
            b.Append(baseUrl);
            int pos = baseUrl.IndexOf("?");
            if (pos > 0)
            {
                b.Append("&e=");
            }
            else
            {
                b.Append("?e=");
            }
            b.Append(deadline);

            String token = createDownloadToken(b.ToString(), mac);
            b.Append("&token=");
            b.Append(token);
            return b.ToString();
        }


    }
}
