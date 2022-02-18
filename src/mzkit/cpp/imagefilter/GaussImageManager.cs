using imagefilter.Interop;
using System;
using System.IO;
using System.Threading.Tasks;

namespace imagefilter
{
    public class GaussImageManager
    {

        private byte[] SourceFile { get; set; }
        private int i;

        public GaussImageManager(string filename)
        {
            this.SourceFile = File.ReadAllBytes(filename);
        }

        public async Task<byte[]> GenerateBlurredImageAsync(GeneratorParameters generatorParams)
        {
            var tasks = new Task[generatorParams.NumberOfThreads];
            var imgSizes = GetLoadedImageSizes();
            var sourceFile = this.SourceFile;
            var processId = 0;

            while (generatorParams.BlurLevel-- > 0)
            {
                processId++;

                for (var threadNum = 0; threadNum < tasks.Length; threadNum++)
                {
                    var num = threadNum;
                    var id = processId;

                    tasks[threadNum] = Task.Run(() =>
                    {
                        var currentThreadParams = ComputeThreadParams(
                            threadId: num,
                            generatorParams: generatorParams,
                            imageSizes: imgSizes);

                        currentThreadParams.ProcessId = id;

                        Console.WriteLine("Start {0}", currentThreadParams);
                        unsafeLibrary.RunUnsafeImageGenerationCode(currentThreadParams: currentThreadParams, out sourceFile);
                        Console.WriteLine("Stop {0}", currentThreadParams);
                    });
                }

                await Task.WhenAll(tasks);
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