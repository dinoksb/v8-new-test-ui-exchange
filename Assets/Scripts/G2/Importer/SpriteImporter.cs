using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using G2.Model;
using Utilities;

namespace G2.Importer
{
    public static class SpriteImporter
    {
        public static async UniTask<Dictionary<string, Sprite>> Import(Dictionary<string, ResourceData> textureDataDict, Dictionary<string, SpriteSheetData> spriteDataDict, string saveFilePath, bool forceDownload = false)
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
                if(CheckIsValidUri(textureValue.Url))
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

                var texture = textureDict[spriteValue.TextureId];
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
            var fileExtension = Path.GetExtension(resourceData.Url).ToLower();
            var fileNameWithExtension = id + fileExtension;
            var filePath = Path.Combine(saveFolderPath, fileNameWithExtension);

            if (forceDownload || !File.Exists(filePath))
            {
                using var www = UnityWebRequestTexture.GetTexture(resourceData.Url);
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
            byte[] pngBytes = System.IO.File.ReadAllBytes(resourceData.Url);
            
            var texture = new Texture2D(2, 2);
            texture.name = id;
            if (!texture.LoadImage(pngBytes))
            {
                throw new Exception("Failed to load image data into texture");
            }

            return texture;
        }
        
        private static Sprite CreateSprite(Texture2D texture, SpriteSheetData spriteSheetData)
        {
            var offset = TypeConverter.ToVector2(spriteSheetData.Offset);
            var size = TypeConverter.ToVector2(spriteSheetData.CellSize);
            var border = TypeConverter.ToVector4(spriteSheetData.Border);
            var pivot = TypeConverter.ToVector2(spriteSheetData.Pivot).ToReverseYAxis();
            var pixelsPerUnit = spriteSheetData.PixelsPerUnit;
            var sprite = TypeConverter.ToSprite(texture, offset, size, border, pivot, pixelsPerUnit);
            return sprite;
        }
        
        private static bool CheckIsValidUri(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out var uriResult) 
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
