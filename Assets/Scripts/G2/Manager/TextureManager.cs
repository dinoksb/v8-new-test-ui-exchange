using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace G2.Manager
{
    public static class TextureManager
    {
        private static readonly Dictionary<Texture2D, string> _textureFilePaths = new();
        private static readonly Dictionary<string, Texture2D> _textures = new();
        private static readonly Dictionary<Texture2D, int> _textureRefCount = new();

        public static async UniTask<Texture2D> LoadTextureAsync(string filePath, CancellationToken cancellationToken)
        {
            if (_textures.ContainsKey(filePath)) return _textures[filePath];
            
            var fileData = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(fileData))
            {
                Debug.LogError($"Failed to load texture from path: {filePath}");
                return null;
            }

            _textures[filePath] = texture;
            _textureFilePaths[texture] = filePath;

            return _textures[filePath];
        }

        public static void OnTextureUsed(Texture2D texture)
        {
            if (texture == null || !_textureRefCount.ContainsKey(texture)) return;
            
            _textureRefCount[texture]++;
        }

        public static void OnTextureUnused(Texture2D texture)
        {
            if (texture == null || !_textureRefCount.ContainsKey(texture)) return;

            _textureRefCount[texture]--;
            if (_textureRefCount[texture] > 0) return;

            _textureRefCount.Remove(texture);
            _textures.Remove(_textureFilePaths[texture]);
            
            Object.Destroy(texture);
        }

        public static void Release()
        {
            foreach (var (_, texture) in _textures)
            {
                Object.Destroy(texture);
            }
        
            _textureFilePaths.Clear();
            _textures.Clear();
            _textureRefCount.Clear();
        }
    }
}
