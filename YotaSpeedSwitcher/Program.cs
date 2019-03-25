using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
//using System.Runtime.Serialization.Json;
//using System.Web.Script.Serialization;

namespace YotaSpeedSwitcher
{
    class Yota
    {
        public void Login()
        {
            
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
