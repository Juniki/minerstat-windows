﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;

namespace Launcher
{
    static class Program
    {
        // Resources
        static string lib, browser, locales, res;

        // minerstat Direcories
        public static string currentDir;
        public static string tempDir;
        public static string minerstatDir;

        [STAThread]
        static void Main()
        {
            // Directories
            //currentDir = System.Environment.CurrentDirectory;
            currentDir = AppDomain.CurrentDomain.BaseDirectory;
            tempDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            minerstatDir = tempDir + "/minerstat";

            // Assigning file paths to varialbles
            lib = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\libcef.dll");
            browser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\CefSharp.BrowserSubprocess.exe");
            locales = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\locales\");
            res = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\");

            var libraryLoader = new CefLibraryHandle(lib);
            bool isValid = !libraryLoader.IsInvalid;
            libraryLoader.Dispose();

            var settings = new CefSettings();
            settings.BrowserSubprocessPath = browser;
            settings.LocalesDirPath = locales;
            settings.ResourcesDirPath = res;

            Cef.Initialize(settings);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LauncherForm());
        }

        public class getData
        {
            private WebRequest request;
            private Stream dataStream;
            private string status;

            public String Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                }
            }

            public getData(string url)
            {
                // Create a request using a URL that can receive a post.

                request = WebRequest.Create(url);
            }

            public getData(string url, string method) : this(url)
            {

                if (method.Equals("GET") || method.Equals("POST"))
                {
                    // Set the Method property of the request to POST.
                    request.Method = method;
                }
                else
                {
                    throw new Exception("Invalid Method Type");
                }
            }

            public getData(string url, string method, string data) : this(url, method)
            {
                try
                {

                    // Create POST data and convert it to a byte array.
                    string postData = data;
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                    // Set the ContentType property of the WebRequest.
                    request.ContentType = "application/x-www-form-urlencoded";

                    // Set the ContentLength property of the WebRequest.
                    request.ContentLength = byteArray.Length;

                    // Get the request stream.
                    dataStream = request.GetRequestStream();

                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    // Close the Stream object.
                    dataStream.Close();

                }
                catch (Exception)
                { }

            }

            public static string responseFromServer;

            public string GetResponse()
            {
                // Get the original response.

                try
                {

                    WebResponse response = request.GetResponse();
                    this.Status = ((HttpWebResponse)response).StatusDescription;

                    // Get the stream containing all content returned by the requested server.
                    dataStream = response.GetResponseStream();

                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content fully up to the end.
                    responseFromServer = reader.ReadToEnd();

                    // Clean up the streams.
                    reader.Close();
                    dataStream.Close();
                    response.Close();


                }
                catch (Exception)
                { }

                return responseFromServer;


            }

        }

    }
}
