using System;

namespace imagefilter.Interop
{
    public struct Size<T> where T : struct, IConvertible
    {
        public T Width { get; set; }
        public T Height { get; set; }

        public Size(T width, T height)
            : this()
        {
            Width = width;
            Height = height;
        }
    }
}
