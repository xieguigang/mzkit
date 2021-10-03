#pragma once
#define API extern "C" __declspec(dllexport)

/*
* Computes pascal triangle row basing on passed gauss triangle row.
*/
int* ComputePascalRow(int n);