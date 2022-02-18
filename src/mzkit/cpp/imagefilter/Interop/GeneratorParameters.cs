using System;

namespace imagefilter.Interop
{
    public class GeneratorParameters
    {
        /// <summary>
        /// number of threads for run gauss blur image filter
        /// </summary>
        public int NumberOfThreads { get; set; }

        /// <summary>
        /// blur levels, value range of this level can be [1,100]
        /// </summary>
        public int BlurLevel { get; set; }

        /// <summary>
        /// pixel block size, value range of this value can be [3,9]
        /// </summary>
        public int GaussMaskSize { get; set; }

        public override string ToString()
        {
            return String.Format("NumOfThreads: {0}, BlurLvl: {1}", NumberOfThreads, BlurLevel);
        }
    }
}