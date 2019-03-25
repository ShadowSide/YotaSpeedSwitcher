using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
//using System.Runtime.Serialization.Json;
//using System.Web.Script.Serialization;.

namespace YotaSpeedSwitcher
{
    class Yota
    {
        private string GetSplitedResult(string rstr)
        {
            var spl = rstr.Split('|', StringSplitOptions.None);
            if (spl.Length < 2)
                throw new ApplicationException($"Remote server call resulted with wrong answer '{rstr}'.");
            if (spl[0].ToLower() != "ok")
                throw new ApplicationException($"Remote server call resulted with answer with error status '{rstr}'.");
            return string.Join('|', spl.Skip(1));
        }
        
        public void Login()
        {
            /*var r = _httpClient.GetAsync(@"/").Result;
            Console.WriteLine(r.Headers.ToString());
            UpdateCookies(r);
            r.EnsureSuccessStatusCode();
            var rstr = r.Content.ReadAsStringAsync().Result;
            r.Dispose();
            _httpClient.CancelPendingRequests();
            PrintCookies();*/
            var r = _httpClient.GetAsync(@"/selfcare/login").Result;
            r.EnsureSuccessStatusCode();
            var rstr = r.Content.ReadAsStringAsync().Result;
            PrintCookies();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
            var cont = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "value", "Sergey4Zhuravlev@gmail.com" }
            });
            foreach(var domain in Domains)
                _cookieContainer.Add(domain, new Cookie("username", "Sergey4Zhuravlev@gmail.com"));
            r = _httpClient.PostAsync(@"/selfcare/login/getUidByMail", cont).Result;
            PrintCookies();
            r.EnsureSuccessStatusCode();
            Console.WriteLine(r.Headers.ToString());
            var userUid = GetSplitedResult(r.Content.ReadAsStringAsync().Result);
            Console.WriteLine($"UserId:{userUid}");
            PrintCookies();
            cont = new FormUrlEncodedContent(new Dictionary<string,string>
            {
                { "IDToken1", userUid },
                { "IDToken2", "TESIII"},
                { "goto", @"https://my.yota.ru/selfcare/loginSuccess" },
                { "gotoOnFail", @"https://my.yota.ru/selfcare/loginErrorOlolo"},
                { "org", "customer" },
                { "ForceAuth", "true" },
                { "login", "Sergey4Zhuravlev@gmail.com&" },
                { "password","TESIII" }
            });
            r = _httpClient.PostAsync(@"https://login.yota.ru/UI/Login", cont).Result;
            Console.WriteLine(r.Headers.ToString());
            PrintCookies();
            r.EnsureSuccessStatusCode();
            var o= r.Content.ReadAsStringAsync().Result;
            Console.WriteLine(o);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://my.yota.ru");
            _httpClient.DefaultRequestHeaders.Add("Referer", @"https://my.yota.ru/selfcare/devices");
            cont = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "product", "318228061" },
                { "offerCode","POS-MA30-0012" },
                { "areOffersAvailable", "false" },
                { "period", "19 дней" },
                { "status", "custom" },
                { "autoprolong", "1" },
                { "isSlot", "false" },
                { "finished", "false" },
                { "blocked", "false" },
                { "freeQuotaActive", "false" },
                { "pimpaPosition", "1.0" },
                { "specialOffersExpanded", "false" },
                { "resourceId","90446202" },
                { "currentDevice", "1" },
                { "username", "" },
                { "isDisablingAutoprolong", "false" }
            });
            r = _httpClient.PostAsync(@"/selfcare/devices/changeOffer", cont).Result;
            PrintCookies();
            r.EnsureSuccessStatusCode();
            o = r.Content.ReadAsStringAsync().Result;
            Console.WriteLine(o);
        }

        void UpdateCookies(HttpResponseMessage resp)
        {
            foreach(var header in resp.Headers)
            {
                if (header.Key != "Set-Cookie")
                    continue;
                foreach (var domain in Domains)
                    foreach (var value in header.Value)
                    {
                        var val = value.Split(';')[0].Split('=');
                        if(val[1]!="null")
                            _cookieContainer.Add(domain, new Cookie(val[0], val[1]));
                    }
            }
        }

        void PrintCookies()
        {
            Console.WriteLine("=========================");
            Hashtable table = (Hashtable)_httpClientHandler.CookieContainer.GetType().InvokeMember("m_domainTable",
                                                                         BindingFlags.NonPublic |
                                                                         BindingFlags.GetField |
                                                                         BindingFlags.Instance,
                                                                         null,
                                                                         _httpClientHandler.CookieContainer,
                                                                         new object[] { });



            foreach (string key in table.Keys)
            {
                var keyQuoted = key;
                Console.WriteLine($"CookieKey: '{keyQuoted}'");
                if (key.StartsWith("."))
                    keyQuoted = key.Substring(1);
                foreach (Cookie cookie in _httpClientHandler.CookieContainer.GetCookies(new Uri(string.Format("http://{0}/", keyQuoted))))
                {
                    Console.WriteLine("Name = {0} ; Value = {1} ; Domain = {2}", cookie.Name, cookie.Value,
                                      cookie.Domain);
                }
            }
            Console.WriteLine("=========================");
        }
        
        public Yota ()
        {
            _httpClientHandler = new HttpClientHandler();
            _cookieContainer = new CookieContainer();
            _httpClientHandler.CookieContainer = _cookieContainer;
            _httpClientHandler.AllowAutoRedirect = true;
            _httpClientHandler.UseCookies = true;
            _httpClient = new HttpClient(_httpClientHandler);
            //_httpClientHandler.Proxy = new WebProxy("127.0.0.1:8888");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Referer", @"https://www.yota.ru/modem/");
            _httpClient.BaseAddress = new Uri(@"https://my.yota.ru");
            _httpClient.Timeout = TimeSpan.FromSeconds(3);
        }

        private HttpClientHandler _httpClientHandler;
        private HttpClient _httpClient;
        private CookieContainer _cookieContainer;

        private readonly Uri[] Domains = new[] { new Uri(@"https://my.yota.ru"), new Uri(@"https://yota.ru"), new Uri(@"https://login.yota.ru") };
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
