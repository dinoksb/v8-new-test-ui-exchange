using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Utilities
{
    public static class ResourceDownloader
    {
        private const string _JPG_EXTENSION = ".jpg";
        private const string _JPEG_EXTENSION = ".jpeg";

        public static string GetPath(string url, string savePath, string wantFileName = null)
        {
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath!);

            var fileExtension = Path.GetExtension(url);
            var fileName = string.IsNullOrEmpty(wantFileName) ? Path.GetFileName(url) : wantFileName + fileExtension;
            var filePath = Path.Combine(savePath, fileName);

            return filePath;
        }

        public static async UniTask<string> Download(
            string url,
            string savePath,
            string wantFileName = null,
            bool forceDownload = false,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);

            var filePath = GetPath(url: url, savePath: savePath, wantFileName: wantFileName);

            try
            {
                if (forceDownload || !File.Exists(filePath))
                {
                    using var www = UnityWebRequest.Get(url);
                    await www.SendWebRequest();
                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        throw new Exception("Failed to download file: " + www.error);
                    }

                    var fileData = www.downloadHandler.data;
#if UNITY_WEBGL
                    File.WriteAllBytes(filePath, fileData);
#else
                    await File.WriteAllBytesAsync(filePath, fileData, cancellationToken);
#endif
                }
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }

            return filePath;
        }

        public static async UniTask<string> DownloadTexture(
            string url,
            string savePath,
            string wantFileName = null,
            bool forceDownload = false,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);

            var fileExtension = Path.GetExtension(url);
            var filePath = GetPath(url: url, savePath: savePath, wantFileName: wantFileName);
            
            try
            {
                if (forceDownload || !File.Exists(filePath))
                {
                    using var www = UnityWebRequestTexture.GetTexture(url);
                    await www.SendWebRequest();
                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        throw new Exception("Failed to download file: " + www.error);
                    }

                    var downloadedTexture = DownloadHandlerTexture.GetContent(www);
                    var fileData = fileExtension.ToLower() is _JPG_EXTENSION or _JPEG_EXTENSION ? downloadedTexture.EncodeToJPG() : downloadedTexture.EncodeToPNG();
#if UNITY_WEBGL
                    File.WriteAllBytes(filePath, fileData);
#else
                    await File.WriteAllBytesAsync(filePath, fileData, cancellationToken);
#endif
                }
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }

            return filePath;
        }

        public static async UniTask<byte[]> GetFileData(string filePath, CancellationToken cancellationToken = default)
        {
            byte[] fileData;
#if UNITY_WEBGL
            fileData = File.ReadAllBytes(filePath);
#else
            fileData = await File.ReadAllBytesAsync(filePath, cancellationToken);
#endif
            return fileData;
        }
    }
}
