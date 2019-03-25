using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace YotaSpeedSwitcherService
{
    public class Yota : IInternetProviderService
    {
        private HttpClientHandler _httpClientHandler;
        private CookieContainer _cookieContainer;
        private HttpClient _httpClient;
        private Uri[] Domains = new[] { new Uri(@"https://my.yota.ru"), new Uri(@"https://yota.ru"), new Uri(@"https://login.yota.ru") };
        private string _userUid = null;
        private readonly ProviderAuthorization _providerAuthorization;

        public Yota (ProviderAuthorization providerAuthorization)
        {
            _providerAuthorization = providerAuthorization;
        }

        public Fare GetCurrentFare()
        {
            throw new NotImplementedException();
        }

        public IList<Fare> GetFares()
        {
            throw new NotImplementedException();
        }

        public void Login()
        {
            Reset();
            var r = _httpClient.GetAsync(@"/selfcare/login").Result;
            r.EnsureSuccessStatusCode();
            var rstr = r.Content.ReadAsStringAsync().Result;
            UpdateRequestHeaders(null);
            var cont = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "value", _providerAuthorization.Login }
            });
            foreach (var domain in Domains)
                _cookieContainer.Add(domain, new Cookie("username", _providerAuthorization.Login));
            r = _httpClient.PostAsync(@"/selfcare/login/getUidByMail", cont).Result;
            r.EnsureSuccessStatusCode();
            _userUid = GetSplitedResult(r.Content.ReadAsStringAsync().Result);
            cont = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "IDToken1", _userUid },
                { "IDToken2", _providerAuthorization.Password},
                { "goto", @"https://my.yota.ru/selfcare/loginSuccess" },
                { "gotoOnFail", @"https://my.yota.ru/selfcare/loginErrorOlolo"},
                { "org", "customer" },
                { "ForceAuth", "true" },
                { "login", _providerAuthorization.Login },
                { "password", _providerAuthorization.Password }
            });
            r = _httpClient.PostAsync(@"https://login.yota.ru/UI/Login", cont).Result;
            r.EnsureSuccessStatusCode();
            var o = r.Content.ReadAsStringAsync().Result;
            UpdateRequestHeaders(@"https://my.yota.ru/selfcare/devices");
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
            r.EnsureSuccessStatusCode();
            o = r.Content.ReadAsStringAsync().Result;
            Console.WriteLine(o);
        }

        public void SetFare(int fareIndex)
        {
            throw new NotImplementedException();
        }

        private void Reset()
        {
            _httpClientHandler = new HttpClientHandler();
            _cookieContainer = new CookieContainer();
            _httpClientHandler.CookieContainer = _cookieContainer;
            _httpClientHandler.AllowAutoRedirect = true;
            _httpClientHandler.UseCookies = true;
            _httpClient = new HttpClient(_httpClientHandler);
            //_httpClientHandler.Proxy = new WebProxy("127.0.0.1:8888");
            UpdateRequestHeaders(@"https://www.yota.ru/modem/");
            _httpClient.BaseAddress = new Uri(@"https://my.yota.ru");
            _httpClient.Timeout = TimeSpan.FromSeconds(3);
        }

        private string GetSplitedResult(string rstr)
        {
            var spl = rstr.Split('|', StringSplitOptions.None);
            if (spl.Length < 2)
                throw new ApplicationException($"Remote server call resulted with wrong answer '{rstr}'.");
            if (spl[0].ToLower() != "ok")
                throw new ApplicationException($"Remote server call resulted with answer with error status '{rstr}'.");
            return string.Join('|', spl.Skip(1));
        }

        private void UpdateCookies(HttpResponseMessage resp)
        {
            foreach (var header in resp.Headers)
            {
                if (header.Key != "Set-Cookie")
                    continue;
                foreach (var domain in Domains)
                    foreach (var value in header.Value)
                    {
                        var val = value.Split(';')[0].Split('=');
                        if (val[1] != "null")
                            _cookieContainer.Add(domain, new Cookie(val[0], val[1]));
                    }
            }
        }

        private void UpdateRequestHeaders(string referer /*@"https://my.yota.ru/selfcare/devices"*/)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://my.yota.ru");
            if(!(referer is null))
                _httpClient.DefaultRequestHeaders.Add("Referer", referer);
        }
    }
}
