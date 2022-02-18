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
        /// <param name="currentThreadParams"></param>
        static internal unsafe void RunUnsafeImageGenerationCode(ThreadParameters currentThreadParams, out byte[] sourceFile)
        {
            int rowPadded = (currentThreadParams.ImgWidth * 3 + 3) & (~3);
            var tmpArray = new byte[currentThreadParams.ImgHeight * rowPadded];

            fixed (byte* imgArrayPtr = sourceFile)
            fixed (byte* tmpArrayPtr = tmpArray)
            {
                currentThreadParams.ImgByteArrayPtr = (uint*)(&imgArrayPtr[54]);
                currentThreadParams.TempImgByteArrayPtr = (uint*)(tmpArrayPtr);

                // call cpp library
                ComputeGaussBlurCpp(currentThreadParams);
            }
        }
    }
}
