using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TzWxSDK
{
    /************************************************************************/
    /* 小程序WebAPI使用的SDK                                                */
    /************************************************************************/
    public class SmallRoutine : WxSDKBase
    {
        private string _app_id;
        private string _app_secret;

        public SmallRoutine(string app_id, string app_secret)
        {
            if (string.IsNullOrEmpty(app_id) || string.IsNullOrEmpty(app_id.Trim())
                || string.IsNullOrEmpty(app_id) || string.IsNullOrEmpty(app_id.Trim()))
                throw new Exception("参数错误");
            _app_id = _app_id.Trim();
            _app_secret = app_secret.Trim();
        }

        public WxUserInfo GetByCode(string code)
        {
            string url = @"https://api.weixin.qq.com/sns/jscode2session?appid=" + _app_id + @"&secret=" + _app_secret + @"&js_code=" + code + "&grant_type=authorization_code";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage res = httpClient.GetAsync(url).Result;
            //string statusCode = response.StatusCode.ToString();
            if (res.IsSuccessStatusCode)
            {
                string result = res.Content.ReadAsStringAsync().Result;
                WxUserInfo info = JsonConvert.DeserializeObject<WxUserInfo>(result);
                return info;
            }
            return null;
        }
    }
}
