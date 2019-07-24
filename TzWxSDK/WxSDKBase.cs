using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TzWxSDK
{
    /************************************************************************/
    /* 微信SDK类的基类                                                      */
    /************************************************************************/
    public class WxSDKBase
    {
        public WxSDKBase()
        {

        }

        //当前dll所在目录
        public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
    
}
