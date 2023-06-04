using System;
using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public static class ImageHelpers
    {
        public static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
        {
            if (aBottom.width != aTop.width || aBottom.height != aTop.height)
                throw new InvalidOperationException("AlphaBlend only works with two equal sized images");
            var bData = aBottom.GetPixels();
            var tData = aTop.GetPixels();
            int count = bData.Length;
            var rData = new Color[count];
            for(int i = 0; i < count; i++)
            {
                Color B = bData[i];
                Color T = tData[i];
                float srcF = T.a;
                float destF = 1f - T.a;
                float alpha = srcF + destF * B.a;
                Color R = (T * srcF + B * B.a * destF)/alpha;
                R.a = alpha;
                rData[i] = R;
            }
            var res = new Texture2D(aTop.width, aTop.height);
            res.SetPixels(rData);
            res.Apply();
            return res;
        }
        
        public static Texture2D rotateTexture(this Texture2D image )
        {
 
            Texture2D target = new Texture2D(image.height, image.width, image.format, false);    //flip image width<>height, as we rotated the image, it might be a rect. not a square image
         
            Color32[] pixels = image.GetPixels32(0);
            pixels = rotateTextureGrid(pixels, image.width, image.height);
            target.SetPixels32(pixels);
            target.Apply();
 
            //flip image width<>height, as we rotated the image, it might be a rect. not a square image
 
            return target;
        }
 
 
        public static Color32[] rotateTextureGrid(Color32[] tex, int wid, int hi)
        {
            Color32[] ret = new Color32[wid * hi];      //reminder we are flipping these in the target
 
            for (int y = 0; y < hi; y++)
            {
                for (int x = 0; x < wid; x++)
                {
                    ret[(hi-1)-y + x * hi] = tex[x + y * wid];         //juggle the pixels around
                 
                }
            }
 
            return ret;
        }
    }
    
    
}