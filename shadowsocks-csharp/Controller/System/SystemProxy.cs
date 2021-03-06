﻿using System;
using Shadowsocks.Model;
using Shadowsocks.Util.SystemProxy;

namespace Shadowsocks.Controller
{
    public static class SystemProxy
    {
        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        public static void Update(Configuration config, bool forceDisable, PACServer pacSrv)
        {
            bool global = config.global;
            bool enabled = config.enabled;

            if (forceDisable)
            {
                enabled = false;
            }

            try
            {
                if (enabled)
                {
                    if (global)
                    {
                        Sysproxy.SetIEProxy(true, true, "127.0.0.1:" + config.localPort.ToString(), "");
                    }
                    else
                    {
                        string pacUrl;
                        if (config.useOnlinePac && !config.pacUrl.IsNullOrEmpty())
                        {
                            pacUrl = config.pacUrl;
                        }
                        else
                        {
                            pacUrl =
                                $"http://127.0.0.1:{config.localPort}/pac?t={GetTimestamp(DateTime.Now)}{pacSrv.PacSecret}";
                        }
                        Sysproxy.SetIEProxy(true, false, "", pacUrl);
                    }
                }
                else
                {
                    Sysproxy.SetIEProxy(false, false, "", "");
                }
            }
            catch (ProxyException ex)
            {
                Logging.LogUsefulException(ex);
            }
        }
    }
}