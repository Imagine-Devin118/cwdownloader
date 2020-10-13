using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ULib.Imagine.Server
{
    internal class WebRequestQueueOperation
    {
        public UnityWebRequestAsyncOperation Result;
        public Action<UnityWebRequestAsyncOperation> OnComplete;

        public bool IsDone => Result != null;

        internal UnityWebRequest webRequest;

        public WebRequestQueueOperation(UnityWebRequest request)
        {
            webRequest = request;
        }

        internal void Complete(UnityWebRequestAsyncOperation asyncOp)
        {
            Result = asyncOp;
            OnComplete?.Invoke(Result);
        }

        public void Abort()
        {
            webRequest.Abort();
        }
    }

    internal static class WebRequestQueue
    {
        private static readonly int MaxRequest = 99;
        internal static Queue<WebRequestQueueOperation> QueuedOperations = new Queue<WebRequestQueueOperation>();
        internal static List<UnityWebRequestAsyncOperation> ActiveRequests = new List<UnityWebRequestAsyncOperation>();

        public static WebRequestQueueOperation QueueRequest(UnityWebRequest request)
        {
            WebRequestQueueOperation queueOperation = new WebRequestQueueOperation(request);
            if (ActiveRequests.Count < MaxRequest)
            {
                UnityWebRequestAsyncOperation webRequestAsyncOp = request.SendWebRequest();
                webRequestAsyncOp.completed += OnWebAsyncOpComplete;
                ActiveRequests.Add(webRequestAsyncOp);
                queueOperation.Complete(webRequestAsyncOp);
            }
            else
            {
                QueuedOperations.Enqueue(queueOperation);
            }

            return queueOperation;
        }

        private static void OnWebAsyncOpComplete(AsyncOperation operation)
        {
            ActiveRequests.Remove((operation as UnityWebRequestAsyncOperation));

            while (QueuedOperations.Count > 0)
            {
                WebRequestQueueOperation nextQueuedOperation = QueuedOperations.Dequeue();
                UnityWebRequestAsyncOperation webRequestAsyncOp = nextQueuedOperation.webRequest.SendWebRequest();
                if (webRequestAsyncOp == null)
                {
                    nextQueuedOperation.webRequest.Dispose();
                    nextQueuedOperation.webRequest = null;
                    continue;
                }
                webRequestAsyncOp.completed += OnWebAsyncOpComplete;
                ActiveRequests.Add(webRequestAsyncOp);
                nextQueuedOperation.Complete(webRequestAsyncOp);
                break;
            }
        }
    }
}