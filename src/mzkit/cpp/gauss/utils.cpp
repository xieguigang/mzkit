#include "stdafx.h"

int* ComputePascalRow(int n) {
	int* row = new int[n + 1];

	// First element is always 1
	row[0] = 1; 
	
	for (int i = 1; i < n / 2 + 1; i++) { 
		// Progress up, until reaching the middle value
		row[i] = row[i - 1] * (n - i + 1) / i;
	}
	
	for (int i = n / 2 + 1; i <= n; i++) { 
		// Copy the inverse of the first part
		row[i] = row[n - i];
	}
	
	return row;
}