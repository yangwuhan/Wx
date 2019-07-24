using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TzWxSDK
{
    /************************************************************************/
    /* 微信网页WebAPI使用的SDK                                                */
    /************************************************************************/
    public class WebPage : WxSDKBase
    {
        protected string _root_url;
        protected string _code_url;
        protected string _error_url;
        protected string _app_id;
        protected string _app_secret;
        protected static readonly string _session_key_for_openid = "wx-openid";
        public static string SessionKeyForOpenId { get { return _session_key_for_openid; } }

        public WebPage(string root_url, string code_url, string error_url, string app_id, string app_secret)
        {
            if (string.IsNullOrEmpty(root_url) || string.IsNullOrEmpty(root_url.Trim())
                || string.IsNullOrEmpty(code_url) || string.IsNullOrEmpty(code_url.Trim())
                || string.IsNullOrEmpty(error_url) || string.IsNullOrEmpty(error_url.Trim())
                || string.IsNullOrEmpty(app_id) || string.IsNullOrEmpty(app_id.Trim())
                || string.IsNullOrEmpty(app_id) || string.IsNullOrEmpty(app_id.Trim()))
                throw new Exception("参数错误");
            _root_url = root_url.Trim();
            _code_url = code_url.Trim();
            _error_url = error_url.Trim();
            _app_id = app_id.Trim();
            _app_secret = app_secret.Trim();
        }

        #region 获取openid和access_token相关

        /*获取OpenId
         * 首先检查session，session有则直接返回
         *                  session无则从微信服务器获取                         */
        public void GetOpenId(HttpSessionStateBase session, HttpResponseBase response, out string openid)
        {
            openid = "";
            if (session.Keys != null && session.Keys.Count > 0)
            {
                for (int i = 0; i < session.Keys.Count; ++i)
                {
                    if(session.Keys[i] == _session_key_for_openid)
                    {
                        openid = session[_session_key_for_openid].ToString();
                        return;
                    }
                }
            }
            _FirstPage_AuthToGetCode(response, true, _code_url);
        }

        /* Web网站首页，调用的函数，获取Code后跳转到RedirectUrl【中间可能会(is_base为False)临时跳转到微信授权页】
         * is_base为True时，不显示授权页面，用户感知的是直接进入页面
         * is_base为False时，显示授权页面，用户感知的是先点击授权，然后进入页面 */
        protected void _FirstPage_AuthToGetCode(HttpResponseBase response, bool is_base, string redirect_url)
        {
            redirect_url = HttpUtility.UrlEncode(redirect_url);
            string url = @"https://open.weixin.qq.com/connect/oauth2/authorize" +
                            "?appid=" + _app_id +
                            "&redirect_uri=" + redirect_url +
                            "&response_type=code" +
                            "&scope=" + (is_base ? "snsapi_base" : "snsapi_userinfo") +
                            "&state=123" +
                            "#wechat_redirect";
            response.Redirect(url);
            response.End();
        }

        /* 微信授权后跳转到的页面中通过code获取用户信息                         
         */
        public void SecondPage_RedirectUrl_GetUserInfoByCode(HttpSessionStateBase session, HttpResponseBase response, string code)
        {
            string url = @"https://api.weixin.qq.com/sns/oauth2/access_token"+
                            "?appid=" + _app_id + 
                            "&secret=" + _app_secret + 
                            "&code=" + code + 
                            "&grant_type=authorization_code";
            try
            {
                if (string.IsNullOrEmpty(code))
                    throw new Exception("code为空");
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = httpClient.GetAsync(url).Result;
                //string statusCode = response.StatusCode.ToString();
                if (res.IsSuccessStatusCode)
                {
                    string result = res.Content.ReadAsStringAsync().Result;
                    JObject o = JObject.Parse(result);
                    JToken jt_errcode;
                    if (o.TryGetValue("errcode", out jt_errcode))
                        throw new Exception("微信服务器返回错误：【" + o["errcode"].ToString() + "】" + o["errmsg"].ToString());
                    else
                    {
                        WxWebApiRetAccessToken at = new WxWebApiRetAccessToken();
                        at.access_token = o["access_token"].ToString();
                        at.expires_in = int.Parse(o["expires_in"].ToString());
                        at.refresh_token = o["refresh_token"].ToString();
                        at.openid = o["openid"].ToString();
                        at.scope = o["scope"].ToString();
                        for (int i = 0; i < session.Keys.Count; ++i)
                        {
                            if (session.Keys[i] == _session_key_for_openid)
                            {
                                session.Remove(_session_key_for_openid);
                                break;
                            }
                        }
                        session.Add(_session_key_for_openid, at.openid);
                        response.Redirect(_root_url);
                        response.End();
                    }                    
                }
                else
                    throw new Exception("服务器返回：" + res.StatusCode.ToString());
            }
            catch(System.Exception ex)
            {
                response.Redirect(_error_url + "?error=" + HttpUtility.UrlEncode(ex.Message));
                response.End();
            }
        }

        #endregion

    }
}
