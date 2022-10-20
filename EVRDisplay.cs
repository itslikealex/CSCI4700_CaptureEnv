using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AVRecorder
{
    internal class EVRDisplay : ContentControl
    {
        #region Public Functions / Vars
        public DXG_Interop.Direct3DSurface9 Surface { get { return surface; } }

        public EVRDisplay()
        {
            var lTuple = D3D9Image.createD3D9Image();

            if (lTuple != null)
            {
                imageSource = lTuple.Item1;

                surface = lTuple.Item2;
            }

            if (imageSource != null)
            {
                var image = new Image();
                image.Stretch = Stretch.Uniform;
                image.Source = imageSource;
                AddChild(image);

                // To greatly reduce flickering we're only going to AddDirtyRect
                // when WPF is rendering.
                CompositionTarget.Rendering += CompositionTargetRendering;
            }
        }
        #endregion

        #region Private Functions / Vars
        private System.Windows.Interop.D3DImage imageSource = null;

        private DXG_Interop.Direct3DSurface9 surface = null;

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (imageSource != null && imageSource.IsFrontBufferAvailable)
            {
                imageSource.Lock();
                imageSource.AddDirtyRect(new Int32Rect(0, 0, imageSource.PixelWidth, imageSource.PixelHeight));
                imageSource.Unlock();
            }
        }
        #endregion

    }
}
