using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace V8
{
    public static class WebRequestUtility
    {
        public static async UniTask<string> GetData(string url, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            var webRequest = UnityWebRequest.Get(url);
            var isDisposed = false;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            var tcs = new TaskCompletionSource<string>();

            webRequest.SendWebRequest().completed += _ =>
            {
                if (!isDisposed)
                {
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        tcs.TrySetResult(webRequest.downloadHandler.text);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception(webRequest.error));
                    }
                }
                else
                {
                    return;
                }

                webRequest.Dispose();
                isDisposed = true;
            };

            cancellationToken.Register(() =>
            {
                if (isDisposed) return;
                if (!tcs.Task.IsCompleted) tcs.TrySetCanceled();
                webRequest.Abort();
                webRequest.Dispose();
                isDisposed = true;
            });

            try
            {
                return await tcs.Task;
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException)
                {
                    return null;
                }

                Debug.LogException(e);
                return null;
            }
        }

        public static async UniTask<string> PostData(string url, string postData, List<KeyValuePair<string, string>> headers = null, CancellationToken cancellationToken = default)
        {
            //var webRequest = UnityWebRequest.Post(url, postData);
            var webRequest = new UnityWebRequest(url, "POST");
            var isDisposed = false;
            if (postData != null)
            {
                var bodyRaw = Encoding.UTF8.GetBytes(postData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
            }

            if (headers != null)
            {
                foreach (var header in headers.Where(header => !string.IsNullOrEmpty(header.Key) && header.Value != null))
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            var tcs = new TaskCompletionSource<string>();

            webRequest.SendWebRequest().completed += _ =>
            {
                if (!isDisposed)
                {
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        tcs.SetResult(webRequest.downloadHandler.text);
                    }
                    else
                    {
                        tcs.SetException(new Exception(webRequest.error));
                    }
                }
                else
                {
                    return;
                }
                
                webRequest.Dispose();
                isDisposed = true;
            };
            
            cancellationToken.Register(() =>
            {
                if (isDisposed) return;
                if (!tcs.Task.IsCompleted) tcs.TrySetCanceled();
                webRequest.Abort();
                webRequest.Dispose();
                isDisposed = true;
            });
            
            try
            {
                return await tcs.Task;
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException)
                {
                    return null;
                }
                
                Debug.LogError($"{e}, PostData url({url})");
                return null;
            }
        }

        public static async UniTask<(AssetBundleManifest manifest, AssetBundle manifestBundle)> GetAssetBundleManifest(string manifestUrl)
        {
            var manifestRequest = UnityWebRequestAssetBundle.GetAssetBundle(manifestUrl);

            var tcs = new TaskCompletionSource<(AssetBundleManifest manifest, AssetBundle manifestBundle)>();

            manifestRequest.SendWebRequest().completed += _ =>
            {
                if (manifestRequest.result == UnityWebRequest.Result.Success)
                {
                    var manifestBundle = DownloadHandlerAssetBundle.GetContent(manifestRequest);
                    var manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    tcs.SetResult((manifest, manifestBundle));
                }
                else
                {
                    tcs.SetException(new Exception(manifestRequest.error));
                }

                manifestRequest.Dispose();
            };

            return await tcs.Task;
        }

        public static async UniTask<AssetBundle> GetAssetBundle(string url)
        {
            var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
            var tcs = new TaskCompletionSource<AssetBundle>();

            webRequest.SendWebRequest().completed += _ =>
            {
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    tcs.SetResult(DownloadHandlerAssetBundle.GetContent(webRequest));
                }
                else
                {
                    tcs.SetException(new Exception(webRequest.error));
                }
                
                webRequest.Dispose();
            };

            return await tcs.Task;
        }

        public static async UniTask<AssetBundle> GetAssetBundleUsingManifest(AssetBundleManifest manifest, string url, string assetBundleName)
        {
            var assetBundleRequest =  UnityWebRequestAssetBundle.GetAssetBundle(url, manifest.GetAssetBundleHash(assetBundleName));
            var tcs = new TaskCompletionSource<AssetBundle>();

            assetBundleRequest.SendWebRequest().completed += _ =>
            {
                if (assetBundleRequest.result == UnityWebRequest.Result.Success)
                {
                    tcs.SetResult(DownloadHandlerAssetBundle.GetContent(assetBundleRequest));
                }
                else
                {
                    tcs.SetException(new Exception(assetBundleRequest.error));
                }
                
                assetBundleRequest.Dispose();
            };

            return await tcs.Task;
        }

        public static async UniTask DownloadAndSaveFile(string url, string saveFilePath, CancellationToken cancellationToken = default)
        {
            using var www = UnityWebRequest.Get(url);

            var webRequestTask = www.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);
            await webRequestTask;

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception("Failed to download file: " + www.error);
            }

            var directoryPath = Path.GetDirectoryName(saveFilePath);
            if (directoryPath != null) Directory.CreateDirectory(directoryPath);
            var results = www.downloadHandler.data;

            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);

            await File.WriteAllBytesAsync(saveFilePath, results ?? new byte[] { }, cancellationToken);
        }
    }
}