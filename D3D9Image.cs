using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;

namespace AVRecorder
{

    internal class D3D9Image : D3DImage
    {
        private DXG_Interop.Direct3DSurface9 surface;

        private D3D9Image() { }

        static public Tuple<D3DImage, DXG_Interop.Direct3DSurface9> createD3D9Image()
        {
            D3D9Image lImageSource = new D3D9Image();

            return lImageSource.init() ?
                Tuple.Create<D3DImage, DXG_Interop.Direct3DSurface9>(lImageSource, lImageSource.surface)
                :
                null;
        }


        private bool init()
        {
            bool lresult = false;

            do
            {

                // Free the old surface
                if (surface != null)
                {
                    surface.Dispose();
                    surface = null;
                }

                using (var device = CreateDevice(DXG_Interop.NativeMethods.GetDesktopWindow()))
                {
                    surface = GetSharedSurface(device);

                    Lock();

                    this.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativeInterface);

                    Unlock();

                    lresult = true;
                }

            } while (false);

            return lresult;
        }

        private DXG_Interop.Direct3DSurface9 GetSharedSurface(DXG_Interop.Direct3DDevice9Ex device)
        {

            IntPtr handle = new IntPtr(0);

            return device.CreateRenderTarget(
                800,//1920,
                600,//1080,
                21, // D3DFMT_A8R8G8B8
                0,
                0,
                0,  // UNLOCKABLE
                ref handle);

        }

        private static DXG_Interop.Direct3DDevice9Ex CreateDevice(IntPtr handle)
        {
            const int D3D_SDK_VERSION = 32;
            using (var d3d9 = DXG_Interop.Direct3D9Ex.Create(D3D_SDK_VERSION))
            {
                var present = new DXG_Interop.NativeStructs.D3DPRESENT_PARAMETERS();
                present.Windowed = 1; // TRUE
                present.SwapEffect = 1; // D3DSWAPEFFECT_DISCARD
                present.hDeviceWindow = handle;
                present.PresentationInterval = unchecked((int)0x80000000); // D3DPRESENT_INTERVAL_IMMEDIATE;

                return d3d9.CreateDeviceEx(
                    0, // D3DADAPTER_DEFAULT
                    1, // D3DDEVTYPE_HAL
                    handle,
                    70, // D3DCREATE_HARDWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE
                    present,
                    null);
            }
        }
    }
}

