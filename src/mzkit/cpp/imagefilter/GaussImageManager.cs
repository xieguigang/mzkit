using imagefilter.Interop;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace imagefilter
{
    public class GaussImageManager
    {
        private byte[] SourceFile;

        /// <summary>
        /// run in sequence mode if this flag is set to true,
        /// otherwise run image processing in parallel
        /// </summary>
        public bool debugMode { get; set; }

        /// <summary>
        /// create bitmap buffer from file in any kind of image format
        /// </summary>
        /// <param name="filename"></param>
        public GaussImageManager(string filename)
        {
            this.SourceFile = gdiStream.getBitmapStream(fileName: filename);
            this.debugMode = false;
        }

        /// <summary>
        /// should be a data buffer of bitmap data!
        /// </summary>
        /// <param name="bitmap"></param>
        public GaussImageManager(byte[] bitmap)
        {
            this.SourceFile = bitmap;
            this.debugMode = false;
        }

        /// <summary>
        /// create bitmap stream data from any kind of image data object
        /// </summary>
        /// <param name="img"></param>
        public GaussImageManager(Image img)
        {
            this.SourceFile = gdiStream.getBitmapStream(img);
            this.debugMode = false;
        }

        public byte[] GenerateBlurredImage(GeneratorParameters generatorParams)
        {
            var tasks = new Task[generatorParams.NumberOfThreads];
            var imgSizes = GetLoadedImageSizes();
            var processId = 0;

            while (generatorParams.BlurLevel-- > 0)
            {
                processId++;

                for (var threadNum = 0; threadNum < tasks.Length; threadNum++)
                {
                    var num = threadNum;
                    var id = processId;
                    var task = new Action(() =>
                    {
                        var currentThreadParams = ComputeThreadParams(
                            threadId: num,
                            generatorParams: generatorParams,
                            imageSizes: imgSizes
                            );

                        currentThreadParams.ProcessId = id;

                        Console.WriteLine("Start {0}", currentThreadParams);
                        unsafeLibrary.RunUnsafeComputeGaussBlur(argv: currentThreadParams, sourceFile: SourceFile);
                        Console.WriteLine("Stop {0}", currentThreadParams);
                    });

                    if (debugMode)
                    {
                        task();
                    }
                    else
                    {
                        tasks[threadNum] = Task.Run(task);
                    }
                }
            }

            if (!debugMode)
            {
                Task.WaitAll(tasks);
            }

            return SourceFile;
        }

        private unsafe Size<int> GetLoadedImageSizes()
        {
            int width, height;

            fixed (byte* imgArray = SourceFile)
            {
                width = *(int*)&imgArray[18];
                height = *(int*)&imgArray[22];
            }

            return new Size<int>(width, height);
        }

        private ThreadParameters ComputeThreadParams(int threadId, GeneratorParameters generatorParams, Size<int> imageSizes)
        {
            int rowPadded = (imageSizes.Width * 3 + 3) & (~3);
            int currentThreadImgHeight = 0;
            int sumOfOffsetLines = 0;

            for (int i = 0; i <= threadId; i++)
            {
                var numOfLinesOfCurrentThread = ComputeNumberOfLinesPerThread(
                    threadId: i,
                    numOfThreads: generatorParams.NumberOfThreads,
                    gaussMaskSize: generatorParams.GaussMaskSize,
                    imgHeight: imageSizes.Height);

                if (i == threadId)
                    currentThreadImgHeight = numOfLinesOfCurrentThread;
                else
                    sumOfOffsetLines += (numOfLinesOfCurrentThread - (generatorParams.GaussMaskSize - 1));
            }

            return new ThreadParameters
            {
                CurrentImgOffset = sumOfOffsetLines * rowPadded,
                GaussMaskSize = generatorParams.GaussMaskSize,
                ImgWidth = imageSizes.Width,
                ImgHeight = currentThreadImgHeight,
                IdOfImgPart = threadId,
                NumOfImgParts = generatorParams.NumberOfThreads,
            };
        }

        private int ComputeNumberOfLinesPerThread(int threadId, int numOfThreads, int gaussMaskSize, int imgHeight)
        {
            var numOfLinesPerThread = imgHeight / numOfThreads;

            if (numOfThreads > 1)
            {
                if ((threadId == 0 || threadId == (numOfThreads - 1)))
                    numOfLinesPerThread += gaussMaskSize / 2;
                else
                    numOfLinesPerThread += gaussMaskSize - 1;
            }

            return numOfLinesPerThread;
        }
    }
}