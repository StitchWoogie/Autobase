using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace AniEditLib
{
    public class GifImage
    {
        private Image gifImage;
        private FrameDimension dimension;
        private int frameCount;
        private int currentFrame = -1;
        private bool reverse;
        private int step = 1;
        private int frameDelay; // 단일 지연 시간 2024-12-11 PSU

        public GifImage(string path)
        {
            gifImage = Image.FromFile(path); //initialize
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); //gets the GUID
            frameCount = gifImage.GetFrameCount(dimension); //total frames in the animation

            // 첫 프레임의 지연 시간만 가져오기 2024-12-11 PSU
            try
            {
                PropertyItem item = gifImage.GetPropertyItem(0x5100);
                if (item != null && item.Value != null && item.Value.Length >= 4)
                {
                    frameDelay = BitConverter.ToInt32(item.Value, 0) * 10;
                }
                else
                {
                    frameDelay = 100; // 기본값 설정
                }
            }
            catch (ArgumentException)
            {
                frameDelay = 100; // 프레임 딜레이 정보가 없을 경우 기본값
            }
        }

        public bool ReverseAtEnd //whether the gif should play backwards when it reaches the end
        {
            get { return reverse; }
            set { reverse = value; }
        }

        public Image GetNextFrame()
        {
            currentFrame += step;          //if the animation reaches a boundary...
            if (currentFrame >= frameCount || currentFrame < 1)
            {
                if (reverse)
                {
                    step *= -1; //...reverse the count
                    currentFrame += step; //apply it
                }
                else
                    currentFrame = 0; //...or start over
            }
            return GetFrame(currentFrame);
        }

        public Image GetFrame(int index)
        {
            gifImage.SelectActiveFrame(dimension, index); //find the frame
            return (Image)gifImage.Clone(); //return a copy of it
        }

        public int GetCount()
        {
            return frameCount;
        }


        public int GetFrameDelay()    //추가 2024-12-11 PSU
        {
            return frameDelay;
        }

        // 모든 프레임이 동일한 지연시간을 가질 경우의 간단한 계산  ,추가 2024-12-11 PSU
        public int GetSimpleRPM()
        {
            // 한 프레임의 지연시간 * 총 프레임 수 = 1회전 시간
            int totalDelayMs = frameDelay * frameCount;

            // 밀리초를 분으로 변환하고 RPM 계산
            return (int)((1000.0 * 60.0) / (double)totalDelayMs);
        }
    }
}
