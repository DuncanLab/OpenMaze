namespace data
{
    using UnityEngine;
    using System.Collections;
    using System.IO;
 
    public static class Img2Sprite
    {
 
        //Static class instead of _instance
        // Usage from any other script:
        // MySprite = IMG2Sprite.instance.LoadNewSprite(FilePath, [PixelsPerUnit (optional)])
 
        public static Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
 
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

            var spriteTexture = LoadTexture(filePath);
            var newSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit, 0 , spriteType);
 
            return newSprite;
        }
 
        public static Texture2D LoadTexture(string filePath)
        {
 
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            if (File.Exists(filePath))
            {
                var fileData = File.ReadAllBytes(filePath);
                var tex2D = new Texture2D(2, 2);
                if (tex2D.LoadImage(fileData))           // Load the imagedata into the texture (size is set automatically)
                    return tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }
    }
 
}