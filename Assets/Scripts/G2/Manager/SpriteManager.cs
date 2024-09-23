using System.Collections.Generic;
using UnityEngine;

namespace G2.Manager
{
    public static class SpriteManager
    {
        private static readonly Dictionary<Sprite, string> _spriteKeys = new();
        private static readonly Dictionary<string, Sprite> _sprites = new();
        private static readonly Dictionary<Sprite, int> _spriteRefCount = new();

        public static Sprite CreateSprite(
            string key,
            Texture2D texture,
            Vector2 offset,
            Vector2 size,
            Vector4 border,
            Vector2 pivot,
            float pixelsPerUnit)
        {
            if (_sprites.ContainsKey(key))
            {
                Debug.LogWarning($"Sprite with key {key} already exists.");
                return _sprites[key];
            }

            var width = Mathf.Min(size.x, texture.width - offset.x);
            var height = Mathf.Min(size.y, texture.height - offset.y);
            var rect = new Rect(offset.x, offset.y, width, height);
            var sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            sprite.name = key;
            _sprites[key] = sprite;
            _spriteKeys[sprite] = key;
            _spriteRefCount[sprite] = 0;

            return sprite;
        }

        public static Sprite GetSprite(string key)
        {
            return _sprites.TryGetValue(key, out var sprite) ? sprite : null;
        }

        public static void OnSpriteUsed(Sprite sprite)
        {
            if (_spriteRefCount.ContainsKey(sprite))
            {
                _spriteRefCount[sprite]++;
            }
            else
            {
                Debug.LogWarning("Trying to use a sprite that is not managed by SpriteManager.");
            }
        }

        public static void OnSpriteUnused(Sprite sprite)
        {
            if (_spriteRefCount.ContainsKey(sprite))
            {
                _spriteRefCount[sprite]--;
                if (_spriteRefCount[sprite] > 0) return;

                Object.Destroy(sprite);
                _sprites.Remove(_spriteKeys[sprite]);
                _spriteKeys.Remove(sprite);
                _spriteRefCount.Remove(sprite);
            }
            else
            {
                Debug.LogWarning("Trying to release a sprite that is not managed by SpriteManager.");
            }
        }

        public static void Release()
        {
            foreach (var (_, sprite) in _sprites)
            {
                Object.Destroy(sprite);
            }
            
            _spriteKeys.Clear();
            _sprites.Clear();
            _spriteRefCount.Clear();
        }
    }
}
