#include <iostream>

#include "stdafx.h"
#include "console.h"

using namespace std;

namespace console {

	/**
	 * C++ std stream wrapper for run debug echo
	*/
	void println(const char* line) {
		cout << line << endl;
	}

	void echo(const char* text) {
		cout << text;
	}

	void newline() {
		cout << endl;
	}
}