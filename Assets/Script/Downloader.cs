using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using ImagineClass.Library;
using ImagineClass;
using System;

public class Downloader
{
    private HttpDownLoad httpDownLoad;
    private Model<DownloaderState> model;
    private bool isFinished;
    private string tip, fileName, filesize;

    public Downloader(Model<DownloaderState> model)
    {
        this.model = model;
        var state = model.GetState();
        state.clickConfirm += UnZip;
        model.SetState(state);
    }

    public void UnZip()
    {
        var state = model.GetState();
        //state
        PlaceUnZipFiles(state.filePath);
    }

    private async void StartDownload()
    {
        isFinished = false;
        tip = "文件将安装到互动课堂资源目录";

        var state = model.GetState();
        fileName = Path.GetFileName(state.filePath);
        var savepath = Path.Combine(Application.persistentDataPath, fileName).Replace('\\', '/');
        state.progress = 0;
        model.SetState(state);

        httpDownLoad = new HttpDownLoad(state.filePath, Application.persistentDataPath, 8000);
        httpDownLoad.DownLoad();
        Debug.Log($"httpDownLoad.TotalLength:{httpDownLoad.TotalLength}");
        await new WaitForSeconds(0.5f);
        filesize = HumanReadableFilesize(httpDownLoad.TotalLength);
        httpDownLoad.OnComplete = () =>
        {
            isFinished = true;
        };
        Update();
    }

    private void PlaceUnZipFiles(string savepath)
    {
        var count = ZipHelper.FileInZipCount(savepath);
        var unzipPath = Path.GetDirectoryName(savepath);
        var curIndex = 0;

        var st1 = model.GetState();
        st1.tip = savepath + "|" + unzipPath;
        st1.progress = (float)curIndex * 100f / (float)count;
        model.SetState(st1);

        ZipHelper.UnZip(savepath, unzipPath, (s) =>
        {
            var st = model.GetState();
            st.tip = $"正在安装:[{curIndex++}/{count}]{s}";
            st.progress = (float)curIndex * 100f / (float)count;
            model.SetState(st);
        },
        async () =>
        {
            var st = model.GetState();
            st.tip = "安装完成, 软件即将关闭";
            st.progress = 100f;
            model.SetState(st);
            // var endst = model.GetState();
            // endst.page = "complete";
            // model.SetState(endst);
            await new WaitForSeconds(1f);
            Application.Quit();
        });
    }

    ~Downloader()
    {
        httpDownLoad.OnComplete = null;
        httpDownLoad = null;
    }

    async void Update()
    {
        while (true)
        {
            if (isFinished)
            {
                httpDownLoad.Close();
                httpDownLoad = null;
                var savepath = Path.Combine(Application.persistentDataPath, fileName).Replace('\\', '/');
                PlaceUnZipFiles(savepath);
                isFinished = false;
            }
            else
            {
                var state = model.GetState();
                if (httpDownLoad != null)
                {
                    if (httpDownLoad.Progress > 0.3f && httpDownLoad.Progress < 0.5f)
                    {
                        state.tip = $"下载及解压过程中请勿关闭软件[{HumanReadableFilesize((long)(httpDownLoad.fileLength))}/{HumanReadableFilesize(httpDownLoad.TotalLength)}]({HumanReadableFilesize(httpDownLoad.BytePerSecond)}/S)";
                    }
                    else
                    {
                        state.tip = $"{tip}[{HumanReadableFilesize((long)(httpDownLoad.fileLength))}/{HumanReadableFilesize(httpDownLoad.TotalLength)}]({HumanReadableFilesize(httpDownLoad.BytePerSecond)}/S)";
                    }
                    state.progress = httpDownLoad.Progress * 100;
                }
                model.SetState(state);
            }
            await new WaitForEndOfFrame();
        }
    }

    public string HumanReadableFilesize(long size)
    {
        var num = 1024.00;

        if (size < num)
            return size + "B";
        if (size < Math.Pow(num, 2))
            return (size / num).ToString("f2") + "KB";
        if (size < Math.Pow(num, 3))
            return (size / Math.Pow(num, 2)).ToString("f2") + "M";
        if (size < Math.Pow(num, 4))
            return (size / Math.Pow(num, 3)).ToString("f2") + "G";

        return (size / Math.Pow(num, 4)).ToString("f2") + "T";
    }
}
