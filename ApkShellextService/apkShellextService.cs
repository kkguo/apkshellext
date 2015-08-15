using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Collections.Generic;

namespace ApkShellext2 {
    class apkShellextService : ServiceBase {
        public apkShellextService() {
            ServiceName = "ApkShellext Service";
            EventLog.Log = "Application";
            
            CanHandlePowerEvent = false;
            CanHandleSessionChangeEvent = false;
            CanPauseAndContinue = false;
            CanShutdown = false;
            CanStop = true;
        }

        static void Main() {
            ServiceBase.Run(new apkShellextService());
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private WebServer ws;

        private static string LocalIPAddress() {
            var card = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            var str = card.GetIPProperties().GatewayAddresses;
            
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        protected override void OnStart(string[] args) {
            base.OnStart(args);

            string[] prefixes = new string[] {
                @"http://*:42728",
                @"http://localhost:42728/",
                @"http://"+LocalIPAddress()+@":42728/",
                @"http://127.0.0.1:42728/"
            };

            ws = new WebServer(SendResponse, prefixes);
            ws.Run();
        }

        protected override void OnStop() {
            base.OnStop();
            ws.Stop();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command) {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            base.OnCustomCommand(command);
        }

        Dictionary<string, string> pathList = new Dictionary<string,string>();
        
        public string SendResponse(HttpListenerRequest request) {
            if (request.QueryString["md5"] != null) {
                if (!pathList.ContainsKey(request.QueryString["md5"])) {
                    pathList.Add(request.QueryString["md5"], request.QueryString["path"]);
                }
                return "";
            } else {
                string md5 = request.RawUrl.Replace(@"/", "");
                if (pathList.ContainsKey(md5))
                    return pathList[md5];
                return "";
            }
        }
    }


    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;
 
        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");
 
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");
 
            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");
 
            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);
 
            _responderMethod = method;
            _listener.Start();
        }
 
        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }
 
        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                if (rstr != "") {
                                    string filename = Path.GetFileName(rstr);
                                    using (FileStream fs = new FileStream(rstr, FileMode.Open)) {
                                        using (BinaryReader sr = new BinaryReader(fs)) {
                                            byte[] buf = sr.ReadBytes((int)sr.BaseStream.Length);
                                            ctx.Response.ContentType = "application/octet-stream";
                                            ctx.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                                            ctx.Response.ContentLength64 = buf.Length;
                                            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                                        }
                                    }
                                }                         
                            }
                            catch (Exception ex){
                                EventLog log = new EventLog();
                                log.WriteEntry(ex.Message);
                            } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }
 
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
