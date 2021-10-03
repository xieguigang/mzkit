#pragma once
#define API extern "C" __declspec(dllexport)

/*
* Structure representing the bitmap pixel
*/
typedef struct Pixel {
	unsigned char B;
	unsigned char G;
	unsigned char R;
} Pixel;

/*
* Structure representing thread parameters structure
* used to store & pass information about thread and calculated
* bitmap settings to the ComputeGaussBlur method.
*/
struct ThreadParameters
{
	int ProcessId;                      // Id of the current thread (for debugging)
	int GaussMaskSize;                  // Gauss mask size (width)
	int CurrentImgOffset;               // Offset relative to the bitmap pixels data start (bytes)
	int ImageWidth;                     // Width of the bitmap in pixels
	int ImageHeight;                    // Height of the bitmap in pixels
	int IdOfImgPart;                    // Number of part of the bitmap passed to the current thread
	int NumOfImgParts;                  // Total number of bitmap parts (number of threads)
	unsigned char* ImgByteArrayPtr;     // Pointer to the beginning of bitmap pixels data section
	unsigned char* TempImgByteArrayPtr; // Pointer to the beginning of the temporary bimap 
};

/**
Computes gaussian blur.
Takes struct with current thread parameters.
*/
API void ComputeGaussBlur(ThreadParameters threadParameters);


/*
@brief Horizontal iteration part,
Iterate over selected range of lines

@param[in] ImageWidth  ThreadParameters image width
@param[in] rowPadded   The pixel array must begin at a memory address that is a multiple of 4 bytes
@param[in] imgOffset   Stores current thread bitmap part offset
*/
void HorizontalScan(
	int beginCopy,
	int endCopy,
	int rowPadded,
	int rowPaddedDiff,
	int gaussHalf,
	int ImageWidth,
	int maxX,
	int gaussWidth,
	int gauss_sum,
	int* mask,
	BYTE* temp,
	BYTE* imgOffset
);

/*
Vertical iteration part
Iterate over lines

@param ImageHeight   argv ImageHeight

*/
void VerticalScan(
	int ImageWidth,
	int ImageHeight,
	int gaussWidth,
	int gaussHalf,
	int gauss_sum,
	int rowPadded,
	int rowPaddedDiff,
	int maxY,
	BYTE* imgOffset,
	BYTE* temp,
	int* mask
);