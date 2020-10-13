using System;
using System.Net;
using System.IO;
using System.Threading;
using UnityEngine;
/// <summary>
/// 文件下载
/// </summary>
public class HttpDownLoad
{
    private string postfix = ".temp";
    public long fileLength;
    public float Progress
    {
        get;
        private set;
    }
    public long BytePerSecond
    {
        get;
        private set;
    }
    public string Error
    {
        get;
        set;
    }
    public long TotalLength
    {
        get;
        private set;
    }
    private string savePath;
    private string url;
    private int timeout;
    private Thread thread;
    public bool isStop;
    public Action OnComplete;
    public Action<float> OnProgress;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="url">url地址</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="callback">回调函数</param>
    public HttpDownLoad(string _url, string _savePath, int _timeout)
    {
        savePath = _savePath;
        url = _url;
        timeout = _timeout;
    }

    /// <summary>
    /// 开启下载
    /// </summary>
    public void DownLoad()
    {
        isStop = false;

        // 开启线程下载
        thread = new Thread(StartDownLoad);
        thread.IsBackground = true;
        thread.Start();
    }

    public void DownLoad(string _url, string _savePath, int _timeout)
    {
        savePath = _savePath;
        url = _url;
        timeout = _timeout;

        DownLoad();
    }

    int preTime, curTime;
    long preBytePerSecond;
    /// <summary>
    /// 开始下载
    /// </summary>
    private void StartDownLoad()
    {
        try
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string fileName = url.Split('/')[url.Split('/').Length - 1];
            string pathName = savePath + "/" + ErasePostfix(fileName) + postfix;

            FileStream fs = new FileStream(pathName, FileMode.OpenOrCreate, FileAccess.Write);
            fileLength = fs.Length;
            TotalLength = GetDownLoadFileSize();
            if (fileLength < TotalLength)
            {
                fs.Seek(fileLength, SeekOrigin.Begin);
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.AddRange((int)fileLength);
                request.Timeout = timeout;
                Stream stream = request.GetResponse().GetResponseStream();
                if (stream.CanTimeout)
                {
                    stream.ReadTimeout = timeout;
                }
                byte[] buff = new byte[4096];
                int len = -1;
                preBytePerSecond = fileLength;
                preTime = DateTime.Now.Second;
                while ((len = stream.Read(buff, 0, buff.Length)) > 0)
                {
                    curTime = DateTime.Now.Second;
                    if (isStop)
                    {
                        break;
                    }
                    fs.Write(buff, 0, len);
                    fileLength += len;
                    Progress = fileLength * 1.0f / TotalLength;
                    if (curTime != preTime)
                    {
                        BytePerSecond = fileLength - preBytePerSecond;
                        preBytePerSecond = fileLength;
                        preTime = curTime;
                    }
                }
                stream.Close();
                stream.Dispose();
            }
            else
            {
                Progress = 1;
            }
            fs.Close();
            fs.Dispose();
            if (Progress == 1)
            {
                string filepathName = savePath + "/" + fileName;
                if (File.Exists(filepathName))
                    File.Delete(filepathName);
                File.Move(pathName, filepathName);
                OnComplete?.Invoke();
                Debug.Log("下载" + filepathName + "完毕");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message + e.StackTrace);
            Error = e.Message;
        }
    }

    /// <summary>
    /// 获取下载的文件大小
    /// </summary>
    /// <returns>文件大小</returns>
    public long GetDownLoadFileSize()
    {
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        request.Method = "HEAD";
        request.Timeout = timeout;
        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        return response.ContentLength;
    }

    /// <summary>
    /// 停止下载
    /// </summary>
    public void Close()
    {
        thread.Abort();
        Thread.Sleep(50);
        isStop = true;
    }

    private string ErasePostfix(string filePath)
    {
        return filePath.Substring(0, filePath.LastIndexOf('.'));
    }
}