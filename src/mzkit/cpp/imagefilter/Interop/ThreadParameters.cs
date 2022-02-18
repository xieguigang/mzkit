using System;
using System.Runtime.InteropServices;

namespace imagefilter.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ThreadParameters
    {
        public int ProcessId;
        public int GaussMaskSize;
        public int CurrentImgOffset;
        public int ImgWidth;
        public int ImgHeight;
        public int IdOfImgPart;
        public int NumOfImgParts;
        public unsafe uint* ImgByteArrayPtr;
        public unsafe uint* TempImgByteArrayPtr;

        public override unsafe string ToString()
        {
            return string.Format("ProcessID: {7}; " +
                                 "ThreadID: {0}; " +
                                 "Width: {1}; " +
                                 "Height: {2}; " +
                                 "NumOfParts: {3}; " +
                                 "ThreadOffset: {4}; " +
                                 "ImgPtr: {5}; " +
                                 "TempImgPtr: {6}",
                                 IdOfImgPart,
                                 ImgWidth,
                                 ImgHeight,
                                 NumOfImgParts,
                                 CurrentImgOffset,
                                 new IntPtr(ImgByteArrayPtr).ToInt32(),
                                 new IntPtr(TempImgByteArrayPtr).ToInt32(),
                                 ProcessId);
        }
    }
}