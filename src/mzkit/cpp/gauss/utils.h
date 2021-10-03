#pragma once
#define API extern "C" __declspec(dllexport)

/*
* Computes pascal triangle row basing on passed gauss triangle row.
*
* @return the last element of the array is the gauss sum value
*/
int* ComputePascalRow(int n, int gaussWidth);