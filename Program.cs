using System;
using System.Net;
using System.IO;
using System.Linq;

namespace YotaSpeedSwitcher
{
    class Yota
    {
        private string ReadResponse(HttpWebResponse resp)
        {
            var respStream = resp.GetResponseStream();
            using (var reader = new StreamReader(respStream))
            {
                var result = reader.ReadToEnd();
                if (result == null)
                    throw new ApplicationException($"Remote server call {resp.Method} {resp.ResponseUri} resulted response is null error.");
                return result;
            }
        }
        private HttpWebRequest MakeRequest(string url, bool isPost = true)
        {
            var req = HttpWebRequest.CreateHttp(url);
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            req.CookieContainer = _cookies;
            req.Credentials = _credentialManager;
            req.Method = isPost ? "POST" : "GET";
            req.KeepAlive = false;
            req.Timeout = 3 * 1000;
            req.ContinueTimeout = 3 * 1000;
            return req;
        }

        private HttpWebResponse GetResponse(string url, string data, bool isPost = true)
        {
            var req = MakeRequest(url, isPost:isPost);
            Console.WriteLine(req.Headers);
            if (!string.IsNullOrEmpty(data))
            {
                req.ContentLength = data.Length;
                req.ContentType = @"application/x-www-form-urlencoded";
                var contentStream = req.GetRequestStream();
                using (var writer = new StreamWriter(contentStream))
                {
                    writer.Write(data);
                }
            }
            HttpWebResponse resp = null;
            try
            {
                resp = req.GetResponse() as HttpWebResponse;
                if (resp == null)
                    throw new WebException(@"Response is null");
                /*foreach (var a in resp.Cookies)
                    Console.WriteLine(a);*/
                _cookies.Add(resp.Cookies);
                Console.WriteLine(resp.ResponseUri);
            }
            catch (WebException wex)
            {
                var httpResponse = wex.Response as HttpWebResponse;
                if (httpResponse != null)
                    throw new ApplicationException($"Remote server call {req.Method} {url} resulted in a http error {httpResponse.StatusCode} {httpResponse.StatusDescription}.", wex);
                else
                    throw new ApplicationException($"Remote server call {req.Method} {url} resulted in an error.", wex);
            }
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException($"Remote server call {req.Method} {url} resulted in an StatusCode error.");
            return (HttpWebResponse)resp;
        }

        private string GetSplitedResult(HttpWebResponse resp)
        {
            var str = ReadResponse(resp);
            var spl = str.Split('|', StringSplitOptions.None);
            if (spl.Length < 2)
                throw new ApplicationException($"Remote server call {resp.Method} {resp.ResponseUri} resulted with wrong answer.");
            if (spl[0].ToLower() != "ok")
                throw new ApplicationException($"Remote server call {resp.Method} {resp.ResponseUri} resulted with answer with error status.");
            return string.Join('|', spl.Skip(1));
        }
        
        public void Login()
        {
            var r = GetResponse(@"https://my.yota.ru/", null, isPost: false);
            r = GetResponse(@"https://my.yota.ru/devices", null, isPost: false);
            var o = ReadResponse(r);
            var userUid = GetSplitedResult(GetResponse(@"https://my.yota.ru/selfcare/login/getUidByMail", "value=Sergey4Zhuravlev%40gmail.com"));
            Console.WriteLine(userUid);
            r = GetResponse(@"https://login.yota.ru/UI/Login", @"IDToken1=6054601495&IDToken2=TESIII&goto=https%3A%2F%2Fmy.yota.ru%3A443%2Fselfcare%2FloginSuccess&gotoOnFail=https%3A%2F%2Fmy.yota.ru%3A443%2Fselfcare%2FloginError&org=customer&ForceAuth=true&login=Sergey4Zhuravlev%40gmail.com&password=TESIII");
            o = ReadResponse(r);
            if (r.ResponseUri.ToString() != @"https://my.yota.ru/selfcare/devices")
                throw new ApplicationException($"Login response url is strange {r.ResponseUri}");
            r = GetResponse(@"https://my.yota.ru/selfcare/devices/changeOffer", @"product=317976993&offerCode=POS-MA30-0012&areOffersAvailable=false&period=19+%D0%B4%D0%BD%D0%B5%D0%B9&status=custom&autoprolong=1&isSlot=false&finished=false&blocked=false&freeQuotaActive=false&pimpaPosition=1.0&specialOffersExpanded=false&resourceId=90446202&currentDevice=1&username=&isDisablingAutoprolong=false");
            o = ReadResponse(r);
            Console.ReadLine();
            Console.WriteLine(o);
        }
       
        private CookieContainer _cookies = new CookieContainer();
        private CredentialCache _credentialManager = new CredentialCache();
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Yota speed switcher!");

            try
            {
                var yoda = new Yota();
                yoda.Login();
                Console.WriteLine("Yota ok!");
            } catch (Exception ex) {
                Console.WriteLine("Yota speed switch error " + ex);
                Environment.ExitCode = -1;
            }
            Console.ReadLine();
        }
    }
}
