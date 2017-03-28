using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Net;
using System.Text;

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Net;
using Microsoft.SPOT.Net.NetworkInformation;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Networking;
using Gadgeteer.SocketInterfaces;

namespace HelloWebService
{
    public partial class Program
    {
        const int std_port = 80;
        GT.Networking.WebEvent webEventPicture;

        void ProgramStarted()
        {
            ledStrip.TurnLedOn(0);
            ledStrip.TurnLedOn(1);

            string[] dnsEntries = { "8.8.8.8", "8.8.4.4" };

            ethernet.UseThisNetworkInterface();
         //   ethernet.UseDHCP();

            ethernet.NetworkSettings.EnableDhcp();
         //   ethernet.NetworkSettings.EnableStaticDns(dnsEntries);

         //   ethernet.UseStaticIP("127.0.0.1", "255.255.255.0", "127.0.0.1");

         //   ethernet.NetworkUp += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkUp);
         //   ethernet.NetworkDown += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkDown);

            ethernet.NetworkUp += ethernet_NetworkUp;
            ethernet.NetworkDown += ethernet_NetworkDown;

            Debug.Print("Program Started");

        //    HttpListener listener = new HttpListener("http", std_port);
        //    listener.Start();

        //    while (true)
        //    {
        //        HttpListenerResponse response = null;
        //        HttpListenerContext context = null;

        //        try
        //        {
        //            context = listener.GetContext();
        //            response = context.Response;
        //            // We are ignoring the request, assuming GET
        //            // HttpListenerRequest request = context.Request;
        //            // Sends response:
        //            response.StatusCode = (int)HttpStatusCode.OK;
        //            byte[] HTML = Encoding.UTF8.GetBytes(
        //            "<html><body>" +
        //            "<h1>FEZ Panda</h1>" +
        //            "<p>Embedded Systems were never easier!</p>" +
        //            "</body></html>");
        //            response.ContentType = "text/html";
        //            response.OutputStream.Write(HTML, 0, HTML.Length);
        //            response.Close();
        //        }

        //        catch
        //        {
        //            if (context != null)
        //            {
        //                context.Close();
        //            }
        //        }
        //    }
        }
        
        //void ethernet_NetworkUp(EthernetJ11D.NetworkModule sender, EthernetJ11D.NetworkState state)
        void ethernet_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Connected to network.");

            WebServer.StartLocalServer(ethernet.NetworkSettings.IPAddress, 80);
 
            webEventPicture = WebServer.SetupWebEvent("picture");
            webEventPicture.WebEventReceived += webEventPicture_WebEventReceived;

            ledStrip.TurnLedOn(2);
            ledStrip.TurnLedOn(3);
        }

        //void ethernet_NetworkDown(EthernetJ11D.NetworkModule sender, EthernetJ11D.NetworkState state)
        void ethernet_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down");
        }
 
        void webEventPicture_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            //string content = "<html><body><h1>Hello World!</h1></body></html>";
            //byte[] bytes = new System.Text.UTF8Encoding().GetBytes(content);
            //responder.Respond(bytes, "text/html");

            if (sdCard.IsCardMounted)
            {
                using (FileStream stream = sdCard.StorageDevice.OpenRead("\\SD\\bitmaps\\image1.bmp"))
                {
                    try
                    {
                        byte[] bytest = new byte[stream.Length];
                        stream.Read(bytest, 0, (int)stream.Length);
                        responder.Respond(bytest, "image/bmp");
                    }
                    catch (Exception ex)
                    {
                        responder.Respond("Exception: " + ex.Message + "  Inner Exception: " + ex.InnerException);
                    }
                }
            }
        }
    }
}