using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace V8
{
    public static class SpriteImporter
    {
        public static async UniTask<Dictionary<string, Sprite>> Import(Dictionary<string, SpriteData> spriteDataDict, bool forceDownload = false)
        {
            return await Import(spriteDataDict, string.Empty, forceDownload);
        }
        
        public static async UniTask<Dictionary<string, Sprite>> Import(Dictionary<string, SpriteData> spriteDataDict, string saveFilePath, bool forceDownload = false)
        {
            var spriteDict = new Dictionary<string, Sprite>();
            foreach (var spriteData in spriteDataDict)
            {
                var spriteKey = spriteData.Key;
                var spriteValue = spriteData.Value;
                
                if(spriteDict.ContainsKey(spriteKey)) continue;
                
                var sprite = await DownloadAndSetSprite(spriteKey, saveFilePath, spriteValue, forceDownload);
                
                spriteDict.Add(spriteKey, sprite);
            }

            return spriteDict;
        }
        
        public static Texture2D GetTextureInfo(string filePath)
        {
            var imageData = File.ReadAllBytes(filePath);
            var texture = new Texture2D(2, 2);
            texture.name = Path.GetFileNameWithoutExtension(filePath);
            if (!texture.LoadImage(imageData))
            {
                throw new Exception("Failed to load image data into texture");
            }
            return texture;
        }
        
        private static async UniTask<Sprite> DownloadAndSetSprite(string id, SpriteData spriteData, bool forceDownload)
        {
            return await DownloadAndSetSprite(id, string.Empty, spriteData, forceDownload);
        }
        
        private static async UniTask<Sprite> DownloadAndSetSprite(string id, string saveFilePath, SpriteData spriteData, bool forceDownload)
        {
            var fileExtension = Path.GetExtension(spriteData.url).ToLower();
            var fileNameWithExtension = id + fileExtension;
            var filePath = string.IsNullOrEmpty(saveFilePath) ? Path.Combine(Application.persistentDataPath, fileNameWithExtension) : Path.Combine(saveFilePath, fileNameWithExtension);

            if (forceDownload || !File.Exists(filePath))
            {
                using var www = UnityWebRequestTexture.GetTexture(spriteData.url);
                await www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception("Failed to download file: " + www.error);
                }

                var downloadedTexture = DownloadHandlerTexture.GetContent(www);
                var fileData = fileExtension is ".jpg" or ".jpeg"
                    ? downloadedTexture.EncodeToJPG()
                    : downloadedTexture.EncodeToPNG();
                File.WriteAllBytes(filePath, fileData);
            }

            var imageData = File.ReadAllBytes(filePath);
            var texture = new Texture2D(2, 2);
            texture.name = id;
            if (!texture.LoadImage(imageData))
            {
                throw new Exception("Failed to load image data into texture");
            }

            var sprite = CreateSprite(texture, spriteData);
            return sprite;
        }
        
        private static Sprite CreateSprite(Texture2D texture, SpriteData spriteData)
        {
            var offset = TypeConverter.ToVector2(spriteData.offset);
            var size = TypeConverter.ToVector2(spriteData.size);
            var border = TypeConverter.ToVector4(spriteData.border);
            var pivot = TypeConverter.ToVector2(spriteData.pivot);
            var pixelsPerUnit = spriteData.pixelsPerUnit;
            var sprite = TypeConverter.ToSprite(texture, offset, size, border, pivot, pixelsPerUnit);
            return sprite;
        }
    }
}