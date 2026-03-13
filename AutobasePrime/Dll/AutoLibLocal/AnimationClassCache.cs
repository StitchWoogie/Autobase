using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AutoLibLocal
{
    public class AnimationClassCache
    {
        static List<AnimationClassCache> arrayCache = new List<AnimationClassCache>();

        public string sFilename;
        public AnimationClass animation;

        public static void Add(string filename, AnimationClass ac)
        {
            AnimationClassCache cache = new AnimationClassCache();
            cache.sFilename = filename;
            cache.animation = ac;

            arrayCache.Add(cache);
        }

        public static AnimationClass SeekMatch(string filename)
        {
            for (int i = 0; i < arrayCache.Count; i++)
            {
                if (String.Compare(filename, arrayCache[i].sFilename, true) == 0)
                {
                    return arrayCache[i].animation;            
                }
            }

            return null;
        }
    }
}
