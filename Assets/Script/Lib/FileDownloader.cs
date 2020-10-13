using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ULib.Imagine.Server
{
    public static class FileDownloader
    {
        private class FileData
        {
            public string url;
            public string savePath;
            public int maxRetryCount;
            public int retryCount;
            public WebRequestQueueOperation webRequestQueueOperation;
        }

        public static Action<string, string> OnError;
        public static Action<string, string> OnComplete;
        private static readonly List<FileData> downloadingFiles = new List<FileData>();

        public static void QueueDownload(string url, string savePath, int maxRetryCount = 5)
        {
            if (downloadingFiles.Find(item => item.url == url && item.savePath == savePath) != null)
            {
                return;
            }

            FileData fileData = new FileData
            {
                url = url,
                savePath = savePath,
                maxRetryCount = maxRetryCount,
            };

            QueueRequest(fileData);
            downloadingFiles.Add(fileData);
        }

        public static void CancelAll()
        {
            foreach (FileData downloadingFile in downloadingFiles)
            {
                if (downloadingFile.webRequestQueueOperation.IsDone)
                {
                    downloadingFile.webRequestQueueOperation.Result.completed -= WebRequestOperationCompleted;
                }
                downloadingFile.webRequestQueueOperation.Abort();
            }

            downloadingFiles.Clear();
        }

        private static void QueueRequest(FileData fileData)
        {
            UnityWebRequest req = new UnityWebRequest
            {
                url = fileData.url,
                method = UnityWebRequest.kHttpVerbGET,
                downloadHandler = new DownloadHandlerFile(fileData.savePath)
                {
                    removeFileOnAbort = true,
                },
            };

            WebRequestQueueOperation webRequestQueueOperation = WebRequestQueue.QueueRequest(req);

            if (webRequestQueueOperation.IsDone)
            {
                UnityWebRequestAsyncOperation requestOperation = webRequestQueueOperation.Result;
                requestOperation.completed += WebRequestOperationCompleted;
            }
            else
            {
                webRequestQueueOperation.OnComplete += asyncOP =>
                {
                    UnityWebRequestAsyncOperation requestOperation = asyncOP as UnityWebRequestAsyncOperation;
                    requestOperation.completed += WebRequestOperationCompleted;
                };
            }

            fileData.webRequestQueueOperation = webRequestQueueOperation;
        }

        private static void WebRequestOperationCompleted(AsyncOperation op)
        {
            UnityWebRequestAsyncOperation remoteReq = op as UnityWebRequestAsyncOperation;
            UnityWebRequest webReq = remoteReq.webRequest;
            FileData fileData = downloadingFiles.Find(item => item.webRequestQueueOperation.Result == remoteReq);

            if (string.IsNullOrEmpty(webReq.error))
            {
                downloadingFiles.Remove(fileData);
                OnComplete?.Invoke(fileData.url, fileData.savePath);
            }
            else
            {
                if (fileData.retryCount++ < fileData.maxRetryCount)
                {
                    Debug.LogFormat("web request {0} failed with error '{1}, retrying ({2}/{3})...", fileData.url, webReq.error, fileData.retryCount, fileData.maxRetryCount);
                    QueueRequest(fileData);
                }
                else
                {
                    Debug.LogErrorFormat("download file {0} failed after {1} attempts.", fileData.url, fileData.maxRetryCount);
                    downloadingFiles.Remove(fileData);
                    OnError?.Invoke(fileData.url, fileData.savePath);
                }
            }

            webReq.Dispose();
        }
    }
}
