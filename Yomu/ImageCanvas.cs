using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Yomu
{
    class ImageCanvas : Canvas
    {
        private BitmapSource page1;
        public BitmapSource Page1
        {
            get
            {
                return page1;
            }
            set
            {
                page1 = value;
                UpdateRenderedImage();
                InvalidateVisual();
            }
        }
        public BitmapSource Source
        {
            get
            {
                return Page1;
            }
            set
            {
                Page1 = value;
            }
        }
        private BitmapSource page2;
        public BitmapSource Page2
        {
            get
            {
                return page2;
            }
            set
            {
                page2 = value;
                UpdateRenderedImage();
                InvalidateVisual();
            }
        }

        private bool twoPageView;
        public bool TwoPageView
        {
            get
            {
                return twoPageView;
            }
            set
            {
                twoPageView = value;
                UpdateRenderedImage();
                InvalidateVisual();
            }
        }

        private bool rightToLeft;
        public bool RightToLeft
        {
            get
            {
                return rightToLeft;
            }
            set
            {
                rightToLeft = value;
                InvalidateVisual();
            }
        }

        private BitmapSource scaledPage1;
        private BitmapSource scaledPage2;

        private WriteableBitmap renderedImage;
        public WriteableBitmap RenderedImage
        {
            get
            {
                return renderedImage;
            }
        }

        public ImageCanvas()
        {
            SizeChanged += OnResize;
            TwoPageView = true;
            RightToLeft = true;
        }

        private void UpdateRenderedImage()
        {
            if (Page1 == null) return;
            if (TwoPageView)
            {
                double scale;
                if (Page2 != null)
                    scale = ActualWidth / (float)(Page1.PixelWidth + Page2.PixelWidth);
                else
                    scale = (ActualWidth / 2) / (float)Page1.PixelWidth;
                scaledPage1 = new TransformedBitmap(Page1, new ScaleTransform(scale, scale));
                if (Page2 != null)
                    scaledPage2 = new TransformedBitmap(Page2, new ScaleTransform(scale, scale));
                else
                    scaledPage2 = null;
                /*var tb1c = new TransformedBitmap(Page1, new ScaleTransform(scale, scale));
                TransformedBitmap tb1;
                if (tb1c.Format.BitsPerPixel == 32)
                    tb1 = tb1c;
                else
                {
                    tb1 = tb1c;
                }
                var tb2c = new TransformedBitmap(Page2, new ScaleTransform(scale, scale));
                TransformedBitmap tb2;
                if (tb2c.Format.BitsPerPixel == 32)
                    tb2 = tb2c;
                {
                    tb2 = tb2c;
                }*/
                /*renderedImage = new WriteableBitmap(tb1.PixelWidth + tb2.PixelWidth, Math.Max(tb1.PixelHeight, tb2.PixelHeight), 96, 96, PixelFormats.Bgra32, null);
                var stride1c = tb1.PixelWidth * (tb1.Format.BitsPerPixel / 8);
                var stride2c = tb2.PixelWidth * (tb2.Format.BitsPerPixel / 8);
                byte[] data1c = new byte[tb1.PixelHeight * stride1c];
                tb1.CopyPixels(data1c, stride1c, 0);
                byte[] data1;
                int stride1;
                if (tb1.Format.BitsPerPixel != 32)
                {
                    var s = tb1.PixelWidth * 4;
                    data1 = new byte[tb1.PixelHeight * s];
                    for (int y = 0; y < tb1.PixelHeight; ++y)
                    {
                        for (int x = 0; x < tb1.PixelWidth; ++x)
                        {
                            data1[y * s + x * 4 + 0] = data1c[y * stride1c + x];
                            data1[y * s + x * 4 + 1] = data1c[y * stride1c + x];
                            data1[y * s + x * 4 + 2] = data1c[y * stride1c + x];
                            data1[y * s + x * 4 + 3] = 255;
                        }
                    }
                    stride1 = s;
                }
                else
                {
                    data1 = data1c;
                    stride1 = stride1c;
                }
                byte[] data2c = new byte[tb2.PixelHeight * stride2c];
                tb2.CopyPixels(data2c, stride2c, 0);
                byte[] data2;
                int stride2;
                if (tb2.Format.BitsPerPixel != 32)
                {
                    var s = tb2.PixelWidth * 4;
                    data2 = new byte[tb2.PixelHeight * s];
                    for (int y = 0; y < tb2.PixelHeight; ++y)
                    {
                        for (int x = 0; x < tb2.PixelWidth; ++x)
                        {
                            data2[y * s + x * 4 + 0] = data2c[y * stride2c + x];
                            data2[y * s + x * 4 + 1] = data2c[y * stride2c + x];
                            data2[y * s + x * 4 + 2] = data2c[y * stride2c + x];
                            data2[y * s + x * 4 + 3] = 255;
                        }
                    }
                    stride2 = s;
                }
                else
                {
                    data2 = data2c;
                    stride2 = stride2c;
                }*/
                // Add condition for right-to-left / left-to-right later
                //renderedImage.WritePixels(new Int32Rect(tb2.PixelWidth, 0, tb1.PixelWidth, tb1.PixelHeight), data1, stride1, 0);
                //renderedImage.WritePixels(new Int32Rect(0, 0, tb2.PixelWidth, tb2.PixelHeight), data2c, stride2c, 0
                //scaledPage1 = tb1;
                //scaledPage2 = tb2;
                if (scaledPage2 != null)
                    Height = Math.Max((Parent as ScrollViewer).ActualHeight, Math.Max(scaledPage1.PixelHeight, scaledPage2.PixelHeight));
                else
                    Height = Math.Max((Parent as ScrollViewer).ActualHeight, scaledPage1.PixelHeight);
            }
            else
            {
                var scale = ActualWidth / (float)Source.PixelWidth;
                scaledPage1 = new TransformedBitmap(Page1, new ScaleTransform(scale, scale));
                /*renderedImage = new WriteableBitmap(tb.PixelWidth, tb.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
                var stride = tb.PixelWidth * (tb.Format.BitsPerPixel / 8);
                byte[] data = new byte[tb.PixelHeight * stride];
                tb.CopyPixels(data, stride, 0);
                //renderedImage.WritePixels(new Int32Rect(0, 0, tb.PixelWidth, tb.PixelHeight), data, stride, 0);
                scaledPage1 = tb;*/
                Height = Math.Max((Parent as ScrollViewer).ActualHeight, scaledPage1.PixelHeight);
            }
            //Height = renderedImage.PixelHeight;
        }

        private void OnResize(object sender, SizeChangedEventArgs e)
        {
            if (Source != null)
            {
                UpdateRenderedImage();
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
            if (TwoPageView)
            {
                if (scaledPage1 != null)
                {
                    //dc.DrawImage(RenderedImage, new Rect(0, 0, RenderedImage.PixelWidth, RenderedImage.PixelHeight));
                    if (RightToLeft)
                    {
                        if (scaledPage2 != null)
                        {
                            dc.DrawImage(scaledPage1, new Rect(scaledPage2.PixelWidth, 0, scaledPage1.PixelWidth, scaledPage1.PixelHeight));
                            dc.DrawImage(scaledPage2, new Rect(0, 0, scaledPage2.PixelWidth, scaledPage2.PixelHeight));
                        }
                        else
                        {
                            dc.DrawImage(scaledPage1, new Rect(ActualWidth / 2, 0, scaledPage1.PixelWidth, scaledPage1.PixelHeight));
                        }
                    }
                    else
                    {
                        if (scaledPage2 != null)
                        {
                            dc.DrawImage(scaledPage1, new Rect(0, 0, scaledPage1.PixelWidth, scaledPage1.PixelHeight));
                            dc.DrawImage(scaledPage2, new Rect(scaledPage1.PixelWidth, 0, scaledPage2.PixelWidth, scaledPage2.PixelHeight));
                        }
                        else
                        {
                            dc.DrawImage(scaledPage1, new Rect(0, 0, scaledPage1.PixelWidth, scaledPage1.PixelHeight));
                        }
                    }
                }
            }
            else
            {
                if (scaledPage1 != null)
                {
                    //dc.DrawImage(RenderedImage, new Rect(0, 0, RenderedImage.PixelWidth, RenderedImage.PixelHeight));
                    dc.DrawImage(scaledPage1, new Rect(0, 0, scaledPage1.PixelWidth, scaledPage1.PixelHeight));
                }
            }
        }
    }
}
