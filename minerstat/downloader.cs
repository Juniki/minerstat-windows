﻿using System;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.ComponentModel;
using System.Threading.Tasks;

namespace minerstat {
 class Downloader {

  private static string fileName;
  private static string fileNameReal;
  private static int counter;
  private static string downloadUrl = "https://static.minerstat.farm/miners/windows/";
  public static string minerVersion;
  private static string minerType;

   internal static bool downloadFile(string v, string n, string cli) {
   bool retVal = false;
   fileName = v;
   fileNameReal = n;
   minerType = cli;
   try {
    using(WebClient webClient = new WebClient()) {

     webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgressChanged);
     webClient.DownloadFileAsync(new Uri(downloadUrl + v), v);
     webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DoSomethingOnFinish);

    }

   } catch (Exception value) {
    Program.NewMessage(value.ToString().Substring(0, 42) + "...", "ERROR");
   }

   return retVal;
  }


  private static void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {

   bool flag = counter % 100 == 0;
   if (flag) {
   Program.NewMessage("DOWNLOAD => " + fileNameReal.ToUpper() + " (" + e.ProgressPercentage + " %  )", "INFO");
   Program.SyncStatus = false;
   }

  }


        async private static void DoSomethingOnFinish(object sender, AsyncCompletedEventArgs e) {

   try {

                File.WriteAllText(Directory.GetCurrentDirectory() + "/clients/" + fileNameReal.ToLower() + "/minerVersion.txt", minerVersion);

                decompressFile();

                File.WriteAllText(Directory.GetCurrentDirectory() + "/clients/" + fileNameReal.ToLower() + "/minerUpdated.txt", minerVersion);

                await Task.Delay(2000);
    mining.downloadConfig(Program.token, Program.worker);
    await Task.Delay(1000);

                Program.NewMessage("NODE => Waiting for the first sync..", "INFO");

                if (minerType.Equals("main"))
                {
                    mining.startMiner(true, false);
                    Program.SyncStatus = true;

                    // Start watchDog
                    Program.watchDogs.Start();

                    // Start SYNC & Remote Command
                    Program.syncLoop.Start();

                } else
                {
                    mining.startMiner(false, true);
                    Program.SyncStatus = true;
                }
                
                

            } catch (Exception) {


   }

  }

  public static void Decompress(string filename, string targetdir) {

   Program.NewMessage("DOWNLOAD => " + fileNameReal.ToUpper() + " ( COMPLETE )", "INFO");
   Program.NewMessage(fileNameReal.ToUpper() + " => Decompressing", "INFO");

            using (ZipFile zipFile = ZipFile.Read(filename))
            {
                zipFile.ExtractProgress +=
               new EventHandler<ExtractProgressEventArgs>(zip_ExtractProgress);
                zipFile.ExtractAll(targetdir, ExtractExistingFileAction.OverwriteSilently);
            }

        }

  private static string decompressFile() {
   Decompress(fileName.ToLower(), Directory.GetCurrentDirectory() + "/clients/" + fileNameReal.ToLower() + "/");
   return "done";
  }

        async static void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer > 0)
            {
                Program.NewMessage("UNZIP => " + Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer) + "%", "");

                if (Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer) == 100)
                {
                    try
                    {
                        string safe = fileName.ToLower();
                        await Task.Delay(10000);
                        File.Delete(safe);

                    } catch (Exception ex)
                    {
                        //Program.NewMessage("ERROR" + ex.ToString(), "");
                    }
                }

            }
        }

    }

}