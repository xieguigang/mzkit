using imagefilter.Interop;
using System.Runtime.InteropServices;

namespace imagefilter
{
    internal class unsafeLibrary
    {
        #region DLL Imports

        [DllImport("gauss_blur.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ComputeGaussBlur")]
        private static extern void ComputeGaussBlurCpp(ThreadParameters threadParameters);

        #endregion

        /// <summary>
        /// call cpp code at here
        /// </summary>
        /// <param name="argv"></param>
        static internal unsafe void RunUnsafeComputeGaussBlur(ThreadParameters argv, byte[] sourceFile)
        {
            int rowPadded = (argv.ImgWidth * 3 + 3) & (~3);
            var tmpArray = new byte[argv.ImgHeight * rowPadded];

            fixed (byte* imgArrayPtr = sourceFile)
            fixed (byte* tmpArrayPtr = tmpArray)
            {
                argv.ImgByteArrayPtr = (uint*)(&imgArrayPtr[54]);
                argv.TempImgByteArrayPtr = (uint*)(tmpArrayPtr);

                // call cpp library
                ComputeGaussBlurCpp(argv);
            }
        }
    }
}
