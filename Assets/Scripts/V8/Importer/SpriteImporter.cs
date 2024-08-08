using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace V8
{
    public static class SpriteImporter
    {
        public static async UniTask<Dictionary<string, Sprite>> Import(Dictionary<string, SpriteData> spriteDataDict, bool forceDownload = false)
        {
            var spriteDict = new Dictionary<string, Sprite>();
            foreach (var spriteData in spriteDataDict)
            {
                var spriteKey = spriteData.Key;
                var spriteValue = spriteData.Value;
                
                if(spriteDict.ContainsKey(spriteKey)) continue;
                
                var sprite = await DownloadAndSetSprite(spriteKey, spriteValue, forceDownload);
                
                spriteDict.Add(spriteKey, sprite);
            }

            return spriteDict;
        }
        
        [Obsolete]
        public static async UniTask<Dictionary<string, Sprite>> Import(IEnumerable<SpriteData> spriteDataList, bool forceDownload = false)
        {
            var spriteMap = new Dictionary<string, Sprite>();
            foreach (var spriteData in spriteDataList)
            {
                if (spriteMap.ContainsKey(spriteData.id)) continue;
        
                var sprite = await DownloadAndSetSprite(spriteData, forceDownload);
                spriteMap[spriteData.id] = sprite;
            }
        
            return spriteMap;
        }
        
        private static async UniTask<Sprite> DownloadAndSetSprite(string id, SpriteData spriteData, bool forceDownload)
        {
            var fileExtension = Path.GetExtension(spriteData.url).ToLower();
            var filePath = Path.Combine(Application.persistentDataPath, id + fileExtension);

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
            if (!texture.LoadImage(imageData))
            {
                throw new Exception("Failed to load image data into texture");
            }

            var sprite = CreateSprite(texture, spriteData);
            return sprite;
        }
        
        [Obsolete]
        private static async UniTask<Sprite> DownloadAndSetSprite(SpriteData spriteData, bool forceDownload)
        {
            var fileExtension = Path.GetExtension(spriteData.url).ToLower();
            var filePath = Path.Combine(Application.persistentDataPath, spriteData.id + fileExtension);

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