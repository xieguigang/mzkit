#include "stdafx.h"
#include "console.h"

int* ComputePascalRow(int n, int gaussWidth) {
	// the last element of the array is gauss sum value
	int* mask = new int[n + 2];

	// First element is always 1
	mask[0] = 1;

	for (int i = 1; i < n / 2 + 1; i++) {
		// Progress up, until reaching the middle value
		mask[i] = mask[i - 1] * (n - i + 1) / i;
	}

	for (int i = n / 2 + 1; i <= n; i++) {
		// Copy the inverse of the first part
		mask[i] = mask[n - i];
	}

	// Compute gauss mask sum
	int gauss_sum = 0;

	for (int i = 0; i < gaussWidth; i++) {
		gauss_sum += mask[i];
	}

	mask[n + 1] = gauss_sum;

	console::println("gauss width value: ");
	console::println(std::to_string(mask[n + 1]).c_str());

	return mask;
}