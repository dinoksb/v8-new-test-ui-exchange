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
        public static async UniTask<Dictionary<string, Sprite>> Import(Dictionary<string, TextureData> textureDataDict, Dictionary<string, SpriteData> spriteDataDict, bool forceDownload = false)
        {
            return await Import(textureDataDict, spriteDataDict, string.Empty, forceDownload);
        }
        
        public static async UniTask<Dictionary<string, Sprite>> Import(Dictionary<string, TextureData> textureDataDict, Dictionary<string, SpriteData> spriteDataDict, string saveFilePath, bool forceDownload = false)
        {
            var spriteDict = new Dictionary<string, Sprite>();
            var textureDict = new Dictionary<string, Texture2D>();
            
            // download texture
            foreach (var textureData in textureDataDict)
            {
                var textureKey = textureData.Key;
                var textureValue = textureData.Value;
                
                if(textureDict.ContainsKey(textureKey)) continue;
                
                var texture = await DownloadTexture(textureKey, saveFilePath, textureValue, forceDownload);
                
                textureDict.Add(textureKey, texture);
            }
            
            // create sprite
            foreach (var spriteData in spriteDataDict)
            {
                var spriteKey = spriteData.Key;
                var spriteValue = spriteData.Value;
                
                if(spriteDict.ContainsKey(spriteKey)) continue;

                var texture = textureDict[spriteValue.textureId];
                var sprite = CreateSprite(texture, spriteValue);
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
        
        private static async UniTask<Texture2D> DownloadTexture(string id, string saveFilePath, TextureData textureData, bool forceDownload)
        {
            var fileExtension = Path.GetExtension(textureData.url).ToLower();
            var fileNameWithExtension = id + fileExtension;
            var filePath = string.IsNullOrEmpty(saveFilePath) ? Path.Combine(Application.persistentDataPath, fileNameWithExtension) : Path.Combine(saveFilePath, fileNameWithExtension);

            if (forceDownload || !File.Exists(filePath))
            {
                using var www = UnityWebRequestTexture.GetTexture(textureData.url);
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

            return texture;
        }
        
        private static Sprite CreateSprite(Texture2D texture, SpriteData spriteData)
        {
            var offset = TypeConverter.ToVector2(spriteData.offset);
            var size = TypeConverter.ToVector2(spriteData.size);
            var border = TypeConverter.ToVector4(spriteData.border);
            var pivot = TypeConverter.ToVector2(spriteData.pivot).ToReverseYAxis();
            var pixelsPerUnit = spriteData.pixelsPerUnit;
            var sprite = TypeConverter.ToSprite(texture, offset, size, border, pivot, spriteData.name, pixelsPerUnit);
            return sprite;
        }
    }
}