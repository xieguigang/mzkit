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
