using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TzWxSDK
{
    /************************************************************************/
    /* 微信网页WebAPI返回的WebAccessToken                                   */
    /************************************************************************/
    public class WxWebApiRetAccessToken
    {
        //网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
        public string access_token { get; set; }

        //access_token接口调用凭证超时时间，单位（秒）
        public int expires_in { get; set; }

        //用户刷新access_token
        public string refresh_token { get; set; }

        //用户唯一标识
        public string openid { get; set; }

        //用户授权的作用域，使用逗号（,）分隔
        public string scope { get; set; }
    }
}
