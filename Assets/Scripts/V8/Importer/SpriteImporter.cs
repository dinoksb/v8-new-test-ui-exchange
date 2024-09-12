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
        public static async UniTask<Dictionary<string, Sprite>> Import(Dictionary<string, ResourceData> textureDataDict, Dictionary<string, SpriteData> spriteDataDict, string saveFilePath, bool forceDownload = false)
        {
            var spriteDict = new Dictionary<string, Sprite>();
            var textureDict = new Dictionary<string, Texture2D>();
            
            // download texture
            foreach (var textureData in textureDataDict)
            {
                var textureKey = textureData.Key;
                var textureValue = textureData.Value;
                
                if(textureDict.ContainsKey(textureKey)) continue;

                Texture2D texture = null;
                if(CheckIsValidUri(textureValue.url))
                    texture = await DownloadTexture(textureKey, saveFilePath, textureValue, forceDownload);
                else
                    texture = DownloadTextureFromLocalPath(textureKey, textureValue, forceDownload);
                
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
        
        private static async UniTask<Texture2D> DownloadTexture(string id, string saveFolderPath, ResourceData resourceData, bool forceDownload)
        {
            var fileExtension = Path.GetExtension(resourceData.url).ToLower();
            var fileNameWithExtension = id + fileExtension;
            var filePath = Path.Combine(saveFolderPath, fileNameWithExtension);

            if (forceDownload || !File.Exists(filePath))
            {
                using var www = UnityWebRequestTexture.GetTexture(resourceData.url);
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
        
        private static Texture2D DownloadTextureFromLocalPath(string id, ResourceData resourceData, bool forceDownload)
        {
            byte[] pngBytes = System.IO.File.ReadAllBytes(resourceData.url);
            
            var texture = new Texture2D(2, 2);
            texture.name = id;
            if (!texture.LoadImage(pngBytes))
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
        
        private static bool CheckIsValidUri(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out var uriResult) 
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}