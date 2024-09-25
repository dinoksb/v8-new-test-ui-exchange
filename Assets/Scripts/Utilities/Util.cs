using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utilities
{
    public static class Util
    {
        public const double ZERO_POINT_ZERO_ONE = 0.01;
        
        public static Quaternion DirectionToRotation(Vector2 direction, bool directionless)
        {
            const float INVERTED_DEGREE = 180f;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (!directionless && direction.x < 0)
            {
                angle += INVERTED_DEGREE;
            }

            var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            return rotation;
        }
        
        public static string SanitizeInput(string dirtyString)
        {
            return Regex.Replace(dirtyString, "[^0-9.]", "");
        }

        public static string GetGuid()
        {
            var key = "client_guid";
            if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetString(key);

            var guid = Guid.NewGuid();
            var guidString = guid.ToString();

            PlayerPrefs.SetString(key, guidString);
            return guidString;
        }

        private static string GetProfile()
        {
#if UNITY_EDITOR
            var hashedBytes = new MD5CryptoServiceProvider()
                .ComputeHash(Encoding.UTF8.GetBytes(Application.dataPath));
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes).ToString("N")[..5];
#else
            return string.Empty;
#endif
        }

        public static string ScriptContent(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var scriptContent = File.ReadAllText(filePath);
            return scriptContent;
        }
        
        public static string GetRevisionFromURI(string dataURI)
        {
            var parts = dataURI.Split('/');
            return parts.Length > 0 ? parts.Last() : "0";
        }


        public static string GenerateDummyEthereumAddress() // 유효한 주소x. 테스트용
        {
            var result = new StringBuilder("0x");
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[20];
                rng.GetBytes(randomBytes);
                foreach (var b in randomBytes) result.Append(b.ToString("x2"));
            }

            return result.ToString();
        }

        public static void OpenBrowser(string url)
        {
            Application.OpenURL(url);
        }
        
        public static byte[] ConvertListToByteArray<T>(List<T> intList)
        {
            var formatter = new BinaryFormatter();
            using var ms = new MemoryStream();
            formatter.Serialize(ms, intList);
            return ms.ToArray();
        }

        public static List<T> ConvertByteArrayToList<T>(byte[] byteArray)
        {
            var formatter = new BinaryFormatter();
            using var ms = new MemoryStream(byteArray);
            return (List<T>)formatter.Deserialize(ms);
        }
        
        public static byte[] ConvertToByteArray<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        
        public static T DeserializeObject<T>(byte[] byteArray)
        {
            if (byteArray == null)
            {
                return default(T);
            }

            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream(byteArray);
            return (T)formatter.Deserialize(stream);
        }
        
        public static bool CopyFile(string source, string dest)
        {
            try
            {
                var destinationDirectory = Path.GetDirectoryName(dest);
                if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory!);

                File.Copy(source, dest, true);
            }
            catch (IOException e)
            {
                Debug.LogError($"Error copying file: {e.Message}");
                return false;
            }

            return true;
        }
        
        public static bool SaveFilesRecursive(string destPath, string fileExtension, string currentPath, string relativePath)
        {
            var fullPath = Path.Combine(currentPath, relativePath);
            if (!Directory.Exists(fullPath)) return true;

            // Copy files in the current directory
            var files = Directory.GetFiles(fullPath, $"*.{fileExtension}");
            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                var savePath = Path.Combine(relativePath, fileName);
                var dest = Path.Combine(destPath, savePath);
                if (!Util.CopyFile(source: filePath, dest: dest))
                {
                    return false;
                }
            }

            // Get subdirectories and call this method recursively
            var subDirectories = Directory.GetDirectories(fullPath);
            foreach (var subDir in subDirectories)
            {
                var subDirName = Path.GetFileName(subDir);
                var newRelativePath = Path.Combine(relativePath, subDirName);
                if (!SaveFilesRecursive(destPath, fileExtension, currentPath, newRelativePath))
                {
                    return false;
                }
            }

            return true;
        }
        
        public static List<string> FindFilePaths(string rootPath, string targetFileName)
        {
            var result = new List<string>();

            try
            {
                foreach (var dir in Directory.GetDirectories(rootPath))
                {
                    var files = Directory.GetFiles(dir, $"{targetFileName}.asset", SearchOption.AllDirectories);
                    result.AddRange(files);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return result;
        }
    }
}
