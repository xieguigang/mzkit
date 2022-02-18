#include "stdafx.h"
#include "gauss_blur.h"
#include "utils.h"
#include "console.h"

/**
 * Computes gaussian blur.
 * Takes struct with current thread parameters.
*/
void ComputeGaussBlur(ThreadParameters argv) {
	// The pixel array must begin at a memory address that is a multiple of 4 bytes 
	int rowPadded = (argv.ImageWidth * 3 + 3) & (~3);
	// Compute difference between rowPadded and real rowWidth
	int rowPaddedDiff = rowPadded - argv.ImageWidth * 3;
	int gaussHalf = argv.GaussMaskSize / 2;

	BYTE* temp = argv.TempImgByteArrayPtr;

	// must be odd
	const int gaussWidth = argv.GaussMaskSize;
	// Compute specific row of a pascal triangle
	int* mask = ComputePascalRow(gaussWidth - 1, gaussWidth);
	// Compute gauss mask sum
	int gauss_sum = mask[gaussWidth];
	int maxY = argv.ImageHeight - gaussWidth + 1;

	// Stores current thread bitmap part offset
	BYTE* imgOffset = &argv.ImgByteArrayPtr[argv.CurrentImgOffset];

	console::println("processing of image with size in pixels:");
	console::echo(std::to_string(argv.ImageWidth).c_str());
	console::echo(" ");
	console::echo(std::to_string(argv.ImageHeight).c_str());
	console::newline();

	// Vertical iteration part
	VerticalScan(
		argv.ImageWidth,
		argv.ImageHeight,
		gaussWidth,
		gaussHalf,
		gauss_sum,
		rowPadded,
		rowPaddedDiff,
		maxY,
		imgOffset,
		temp,
		mask
	);

	int maxX = argv.ImageWidth - gaussWidth + 1;
	int beginCopy = 0;
	int endCopy = argv.ImageHeight;

	// If current thread doesn't work with first part of the bitmap
	if (argv.IdOfImgPart != 0) {
		beginCopy = gaussHalf;
		imgOffset += rowPadded * gaussHalf;
	}

	// If current thread doesn't work with last part of the bitmap
	if (argv.IdOfImgPart != argv.NumOfImgParts - 1) {
		// shrink area we are working on (because of gauss filters interleaves)
		endCopy -= gaussHalf;
	}

	// Horizontal iteration part
	HorizontalScan(
		beginCopy,
		endCopy,
		rowPadded,
		rowPaddedDiff,
		gaussHalf,
		argv.ImageWidth,
		maxX,
		gaussWidth,
		gauss_sum,
		mask,
		temp,
		imgOffset
	);
}

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
	int* mask) {

	// stores position (in bytes) of current position 
	// of temporary bitmap array data
	int currPos = 0;

	console::println("Run vertical iteration part:");

	for (int y = 0; y < ImageHeight; y++) {
		int currY = y - gaussHalf;

		console::echo(std::to_string(y).c_str());
		console::echo(" ");

		// Compute offset to the current line of source bitmap 
		BYTE* offset1 = imgOffset + rowPadded * currY;

		/*
		* If |current line - gaussHalf| is in bounds of data array
		* (edges of the bitmap)
		*/
		if (currY >= 0 && currY < maxY) {
			// Iterate over pixels in line
			for (int x = 0; x < ImageWidth; x++) {

				// Compute offset to the current pixel structure
				BYTE* offset2 = offset1 + x * 3;

				// Clear total sums of R, G, B
				double linc_b = 0;
				double linc_g = 0;
				double linc_r = 0;

				// For each up/down pixel surrounding the current source pixel
				for (int k = 0; k < gaussWidth; k++) {
					/*
					* Multiply current mask value by corresponding,
					* surrounding pixel took into consideration of
					* averaging process and add to the total sums
					* splitted on R, G, B components.
					*/
					linc_b += offset2[0] * mask[k];
					linc_g += offset2[1] * mask[k];
					linc_r += offset2[2] * mask[k];

					offset2 += rowPadded;
				}

				/*
				* Divide R, G, B components by the gauss mask sum
				* and save averaged value to the temporary data array.
				*/
				temp[currPos++] = linc_b / gauss_sum;
				temp[currPos++] = linc_g / gauss_sum;
				temp[currPos++] = linc_r / gauss_sum;
			}
		}
		/*
		* If |current line - gaussHalf| is not in bounds of data array
		* (edges of the bitmap)
		*/
		else {
			// Compute pointer to the current source bitmap pixel structure
			BYTE* offset2 = offset1 + gaussHalf * rowPadded;

			// Iterate over pixels in line
			for (int x = 0; x < ImageWidth; x++) {
				/*
				* For pixels which are so close to border of the bitmap
				* that they cannot be averaged by the full mask,
				* we simply rewrite pixel to the temporary data array.
				*/
				temp[currPos++] = offset2[0];
				temp[currPos++] = offset2[1];
				temp[currPos++] = offset2[2];

				offset2 += 3;
			}
		}

		/*
		* Because bitmaps has to have new lines aligned every 4 bytes
		* we have to add row padded difference to make sure we are
		* on the appropriate position of temporary (destination) bitmap
		*/
		currPos += rowPaddedDiff;
	}

	console::newline();
}

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
	BYTE* imgOffset) {

	int currPos = 0;

	console::println("Run horizontal iteration part!");
	console::echo("begin copy: ");
	console::println(std::to_string(beginCopy).c_str());
	console::echo("end copy: ");
	console::println(std::to_string(endCopy).c_str());

	for (int y = beginCopy; y < endCopy; y++) {
		// Compute offset to the current line of source bitmap 
		BYTE* offset1 = temp + rowPadded * y - gaussHalf * 3;

		// Iterate over pixels in line
		for (int x = 0; x < ImageWidth; x++) {
			// Clear total sums of R, G, B
			double linc_b = 0;
			double linc_g = 0;
			double linc_r = 0;

			int currX = x - gaussHalf;

			// Compute offset to the current source bitmap pixel structure
			BYTE* offset2 = offset1 + x * 3;

			/*
			* If |current pixel X position - gaussHalf| is in bounds of data array
			* (edges of the bitmap)
			*/
			if (currX >= 0 && currX < maxX) {
				// For each left/right pixel surrounding the current source pixel
				for (int k = 0; k < gaussWidth; k++) {
					/*
					* Multiply current mask value by corresponding,
					* surrounding pixel took into consideration of
					* averaging process and add to the total sums
					* splitted on R, G, B components.
					*/
					linc_b += offset2[0] * mask[k];
					linc_g += offset2[1] * mask[k];
					linc_r += offset2[2] * mask[k];

					offset2 += 3;
				}

				/*
				* Divide R, G, B components by the gauss mask sum
				* and save averaged value to the temporary data array.
				*/
				imgOffset[currPos++] = linc_b / gauss_sum;
				imgOffset[currPos++] = linc_g / gauss_sum;
				imgOffset[currPos++] = linc_r / gauss_sum;
			}
			/*
			* If |current pixel X position - gaussHalf| is not in bounds of data array
			* (edges of the bitmap)
			*/
			else {
				/*
				* For pixels which are so close to border of the bitmap
				* that they cannot be averaged by the full mask,
				* we simply rewrite pixel to the temporary data array.
				*/
				offset2 += gaussHalf * 3;
				imgOffset[currPos++] = offset2[0];
				imgOffset[currPos++] = offset2[1];
				imgOffset[currPos++] = offset2[2];
			}
		}

		/*
		* Because bitmaps has to have new lines aligned every 4 bytes
		* we have to add row padded difference to make sure we are
		* on the appropriate position of temporary (destination) bitmap
		*/
		currPos += rowPaddedDiff;
	}
}