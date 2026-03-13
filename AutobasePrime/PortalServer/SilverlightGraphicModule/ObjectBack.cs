using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightGraphicModule
{
    public class ObjectBack : ObjectPublic
    {
        int nWidth;
        int nHeight;
        public int nStartX, nStartY;

        AnimationClass animation = new AnimationClass();
        Image imageChild;

        public ObjectBack(ObjectCommonProperty ocp, Canvas parent_canvas, string filename, int startx, int starty, bool tile_flag)
            : base(ocp)
        {
            //
            // TODO: Add constructor logic here
            //

            SetFileName(filename);
            
            nStartX = startx;
            nStartY = starty;

            if (tile_flag)  // 타일모드가 없어서 확대로 변경
            {
                /*
                Canvas child = new Canvas();

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++){
                        Image image = new Image();

                        image.Source = animation.GetImageSource(0);
                        image.Stretch = Stretch.Fill;

                        child.Children.Add(image);
                    }
                }

                parent_canvas.Children.Add(child);

                child.IsHitTestVisible = false;

                //Canvas.SetLeft(child, startx);
                //Canvas.SetTop(child, starty);
                */
                Image child = new Image();

                child.Source = animation.GetImageSource(0);
                child.Stretch = Stretch.Fill;

                parent_canvas.Children.Add(child);

                child.IsHitTestVisible = false;
                //child.Margin = new Thickness(0, 0, 0, 0);

                //Canvas.SetLeft(child, startx);
                //Canvas.SetTop(child, starty);

                imageChild = child;
                parent_canvas.SizeChanged += new SizeChangedEventHandler(parent_canvas_SizeChanged);
            }
            else 
            {
                Image child = new Image();

                child.Source = animation.GetImageSource(0);
                child.Stretch = Stretch.Fill;

                parent_canvas.Children.Add(child);

                child.IsHitTestVisible = false;

                Canvas.SetLeft(child, startx);
                Canvas.SetTop(child, starty);
            }
            
        }

        void parent_canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            imageChild.Width = ((Canvas)sender).Width;
            imageChild.Height = ((Canvas)sender).Height;
        }

        void SetFileName(string filename)
        {
            string path;

            path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

            animation.LoadImage(path, false, 0);

            nWidth = animation.Width();
            nHeight = animation.Height();
        }

        /*
        public void Display(Graphics g, Rectangle rcPaint, int tile_flag, Point scroll_pos)
        {
            SetBasePoint(scroll_pos.X, scroll_pos.Y);

            int x1, y1, x2, y2;

            x1 = GetViewPosX(nStartX);
            y1 = GetViewPosY(nStartY);
            x2 = GetViewPosX(nStartX + nWidth - 1);
            y2 = GetViewPosY(nStartY + nHeight - 1);

            if (hBitmap != null)
            {
                if (tile_flag == 1)
                {
                    int x, y;

                    for (y = y1; y <= rcPaint.Bottom; y += nHeight)
                    {
                        if (y + nHeight < rcPaint.Top) continue;
                        for (x = x1; x <= rcPaint.Right; x += nWidth)
                        {
                            if (x + nWidth < rcPaint.Left) continue;

                            g.DrawImageUnscaled(hBitmap, x, y);
                        }
                    }
                }
                else
                {
                
                    if (nWidth == x2 - x1 + 1 && nHeight == y2 - y1 + 1)
                    {	// 확대 축소 할 필요가 없다.
                        g.DrawImageUnscaled(hBitmap, x1, y1);

                    }
                    else
                    {
                        g.DrawImage(hBitmap, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                    }
                
                }
            }
            else
            {
                
            }
        }

        public void GetZone(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            x1 = 0;
            y1 = 0;
            x2 = nWidth - 1;
            y2 = nHeight - 1;
        }

        
        public void GetFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            family.filename = sFileName;
            block.Add(family);
        }

        public void ChangeFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family;
            family = ObjectGroup.GetMatchFamilyFile(sFileName, block);
            if (family == null) return;
            sFileName = family.change;
        }*/
    }
}
