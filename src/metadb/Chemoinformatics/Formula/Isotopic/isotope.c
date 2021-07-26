/*

 I S O T O P E . C

 A program to calculate isotope patterns from a given formula.

--------------------------------------------------------------------
 Copyright (c) 1996...2009 Joerg Hau <joerg.hau(at)dplanet.ch>.

 Web: http://isopat.sourceforge.net/

 This program is free software; you can redistribute it and/or
 modify it under the terms of version 2 of the GNU General Public
 License as published by the Free Software Foundation. See the
 file LICENSE for details.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.
--------------------------------------------------------------------

 Acknowledgement: This program is based on a file named MASS.C,
 which one of my (former) colleagues found around 1994 "somewhere
 on Internet". He could not recall the source, and the file did
 neither carry any copyright nor was the author identified
 somehow.
 As the original file was already publicly available, I put
 this modified version under the GNU Public License.

 Note: Version code is reflected by date in ISO writing (20020125)

 Modification history:

 1996-xx-xx, put element and isotope tables IN the code (JHa).
 1998-03-23, extended comments and output (JHa).
 2002-01-25, some minor (stylistic) fixes (JHa)
 2002-01-26, fix error message when entering new element
 2005-06-17, added command line calculation (JHa)
 2008-05-08, fixed potential security holes (JHa)
 2009-03-07, updated to reflect hosting on sourceforge.net (JHa)

To run the program interactively, enter formula when asked. Element symbols
must be correctly typed, with upper and lower case as usual. Elements can be
repeated in a formula, but brackets are not understood: CH3OH is ok, but
Ni(CO)4 is not; this has to be typed as NiC4O4.

If you run the program, the output is by defaut normalized to 100.00
percent for the bigest peak. If you launch it with option '-f' instead, the
output peaks will be printed as fractions of the total intensity.

You can redirect the output to a file or printer:

	isotope > filename
or
	isotope > printername.

You can specify a formula on the command line, but only the first formula
on the line is taken into account. Example:

	isotope C12H22O11

Calculation of a series of spectra can be done in batch mode. Create an
input file of all the formula, one per line, with a 'q' (quit) as the first
character of the last line. Then type

	isotope < infile > outfile.

(... but don't make any mistakes in the formulas, if you do it this way!).

 This is ANSI C and should compile with any C compiler; use
 something along the lines of "gcc -Wall -O3 -o isotope isotope.c".
 Optimize for speed!

*/

#define VERSION "20090307"	/* string! */
#define CUTOFF 1e-7

#include <stdio.h>
#include <ctype.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>

typedef struct { int m; float fr; }                  isotope;    /* mass, abundance */
typedef struct { char* sym; int niso;  isotope* p; } element;    /* symbol, no. of isotopes, ptr */
typedef struct { int atno; int count; }              atom;
typedef struct { int mass; float intens; }           peak;

element el[] =
{
	{ "H",  2, NULL },
	{ "He", 2, NULL },
	{ "Li", 2, NULL },
	{ "Be", 1, NULL },
	{ "B",  2, NULL },
	{ "C",  2, NULL },
	{ "N",  2, NULL },
	{ "O",  3, NULL },
	{ "F",  1, NULL },
	{ "Ne", 3, NULL },
	{ "Na", 1, NULL },
	{ "Mg", 3, NULL },
	{ "Al", 1, NULL },
	{ "Si", 3, NULL },
	{ "P",  1, NULL },
	{ "S",  4, NULL },
	{ "Cl", 2, NULL },
	{ "Ar", 3, NULL },
	{ "K",  3, NULL },
	{ "Ca", 6, NULL },
	{ "Sc", 1, NULL },
	{ "Ti", 5, NULL },
	{ "V",  2, NULL },
	{ "Cr", 4, NULL },
	{ "Mn", 1, NULL },
	{ "Fe", 4, NULL },
	{ "Co", 1, NULL },
	{ "Ni", 5, NULL },
	{ "Cu", 2, NULL },
	{ "Zn", 5, NULL },
	{ "Ga", 2, NULL },
	{ "Ge", 5, NULL },
	{ "As", 1, NULL },
	{ "Se", 6, NULL },
	{ "Br", 2, NULL },
	{ "Kr", 6, NULL },
	{ "Rb", 2, NULL },
	{ "Sr", 4, NULL },
	{ "Y",  1, NULL },
	{ "Zr", 5, NULL },
	{ "Nb", 1, NULL,  },
	{ "Mo", 7, NULL },
	{ "Tc", 1, NULL },
	{ "Ru", 7, NULL },
	{ "Rh", 1, NULL },
	{ "Pd", 6, NULL },
	{ "Ag", 2, NULL },
	{ "Cd", 8, NULL },
	{ "In", 2, NULL },
	{ "Sn", 10, NULL },
	{ "Sb", 2, NULL },
	{ "Te", 8, NULL },
	{ "I" , 1, NULL },
	{ "Xe", 7, NULL },
	{ "Cs", 1, NULL },
	{ "Ba", 7, NULL },
	{ "La", 2, NULL },
	{ "Ce", 4, NULL },
	{ "Pr", 1, NULL },
	{ "Nd", 7, NULL },
	{ "Pm", 1, NULL },
	{ "Sm", 7, NULL },
	{ "Eu", 2, NULL },
	{ "Gd", 7, NULL },
	{ "Tb", 1, NULL },
	{ "Dy", 7, NULL },
	{ "Ho", 1, NULL },
	{ "Er", 6, NULL },
	{ "Tm", 1, NULL },
	{ "Yb", 7, NULL },
	{ "Lu", 2, NULL },
	{ "Hf", 6, NULL },
	{ "Ta", 2, NULL },
	{ "W" , 5, NULL },
	{ "Re", 2, NULL },
	{ "Os", 7, NULL },
	{ "Ir", 2, NULL },
	{ "Pt", 5, NULL },
	{ "Au", 1, NULL },
	{ "Hg", 7, NULL },
	{ "Tl", 2, NULL },
	{ "Pb", 4, NULL },
	{ "Bi", 1, NULL },
	{ "Po", 1, NULL },
	{ "At", 1, NULL },
	{ "Rn", 1, NULL },
	{ "Fr", 1, NULL },
	{ "Ra", 1, NULL },
	{ "Ac", 1, NULL },
	{ "Th", 1, NULL },
	{ "Pa", 1, NULL },
	{ "U" , 2, NULL },
	{ "Np", 1, NULL },
	{ "Pu", 1, NULL }
};

isotope iso[] =
{
	{  1,  .99985   /* H */ },
	{  2,  .00015 },
	{  3,  .000001  /* He */ },
	{  4,  .999999   },
	{  6,  .0742    /* Li */ },
	{  7,  .9258 },
	{  9,  1.00     /* Be */ },
	{ 10,  .197     /* B */ },
	{ 11,  .803 },
	{ 12,  .98892   /* C */ },
	{ 13,  .01108 },
	{ 14,  .99635   /* N */ },
	{ 15,  .00365 },
	{ 16,  .99759   /* O */ },
	{ 17,  .00037 },
	{ 18,  .00204 },
	{ 19,  1.00     /* F */ },
	{ 20,  .9092    /* Ne */ },
	{ 21,  .00257 },
	{ 22,  .0882 },
	{ 23,  1.00     /* Na */ },
	{ 24,  .7870    /* Mg */ },
	{ 25,  .1013 },
	{ 26,  .1117 },
	{ 27,  1.00     /* Al */ },
	{ 28,  .9221    /* Si */ },
	{ 29,  .0470 },
	{ 30,  .0309 },
	{ 31,  1.00     /* P */ },
	{ 32,  .950     /* S */ },
	{ 33,  .0076 },
	{ 34,  .0422 },
	{ 36,  .00014 },
	{ 35,  .7553   /* Cl */ },
	{ 37,  .2447 },
	{ 36,  .00337   /* Ar */ },
	{ 38,  .00063 },
	{ 40,  .9960 },
	{ 39,  .9326    /* K */ },
	{ 40,  .0001 },
	{ 41,  .0673 },
	{ 40,  .9697    /* Ca */ },
	{ 42,  .0064 },
	{ 43,  .00145 },
	{ 44,  .0206 },
	{ 46,  .000033 },
	{ 48,  .0018 },
	{ 45,  1.00     /* Sc */ },
	{ 46,  .0793    /* Ti */ },
	{ 47,  .0728 },
	{ 48,  .7394 },
	{ 49,  .0551 },
	{ 50,  .0534 },
	{ 50,  .0025    /* V */ },
	{ 51,  .9975 },
	{ 50,  .0431    /* Cr */ },
	{ 52,  .8376 },
	{ 53,  .0955 },
	{ 54,  .0238 },
	{ 55,  1.00     /* Mn */ },
	{ 54,  .0582    /* Fe */ },
	{ 56,  .9166 },
	{ 57,  .0219 },
	{ 58 , .0033 },
	{ 59,  1.00     /* Co */ },
	{ 58,  .6788    /* Ni */ },
	{ 60,  .2623 },
	{ 61,  .0119 },
	{ 62,  .0366 },
	{ 64,  .0108 },
	{ 63,  .6909,    /* Cu */ },
	{ 65,  .3091 },
	{ 64,  .4889    /* Zn */ },
	{ 66,  .2781 },
	{ 67,  .0411 },
	{ 68,  .1857 },
	{ 70,  .0062 },
	{ 69,  .604     /* Ga */ },
	{ 71,  .396 },
	{ 70,  .2051    /*Ge */ },
	{ 72,  .2743 },
	{ 73,  .0776 },
	{ 74,  .3654 },
	{ 76,  .0776 },
	{ 75,  1.00     /* As */ },
	{ 74,  .0087    /* Se */ },
	{ 76,  .0902 },
	{ 77,  .0758 },
	{ 78,  .2352 },
	{ 80,  .4982 },
	{ 82,  .0919 },
	{ 79,  .5054    /* Br */ },
	{ 81,  .4946 },
	{ 78,  .0035    /* Kr */ },
	{ 80,  .0227 },
	{ 82,  .1156 },
	{ 83,  .1155 },
	{ 84,  .5690 },
	{ 86,  .1737 },
	{ 85,  .7215    /* Rb */ },
	{ 87,  .2785  },
	{ 84,  .0056    /* Sr */ },
	{ 86,  .0986 },
	{ 87,  .0702 },
	{ 88,  .8256   },
	{ 89,  1.00     /* Y */ },
	{ 90,  .5146    /* Zr */ },
	{ 91,  .1123 },
	{ 92,  .1711 },
	{ 94,  .1740 },
	{ 96,  .0280 },
	{ 93,  1.00     /* Nb */ },
	{ 92,  .1584    /* Mo */ },
	{ 94,  .0904 },
	{ 95,  .1572 },
	{ 96,  .1653 },
	{ 97,  .0946 },
	{ 98,  .2378 },
	{100,  .0963 },
	{ 98,  1.00     /* Tc */ },
	{ 96,  .0551    /* Ru */ },
	{ 98,  .0187 },
	{ 99,  .1272 },
	{100,  .1262 },
	{101,  .1707 },
	{102,  .3161 },
	{104,  .1858 },
	{103,  1.00     /* Rh */ },
	{102,  .0096    /* Pd */ },
	{104,  .1097 },
	{105,  .2223 },
	{106,  .2733 },
	{108,  .2671 },
	{109,  .1181 },
	{107,  .5182    /* Ag */ },
	{109,  .4818 },
	{106,  .0122    /* Cd */ },
	{108,  .0088 },
	{110,  .1239 },
	{111,  .1275 },
	{112,  .2407 },
	{113,  .1226 },
	{114,  .2886 },
	{116,  .0758 },
	{113,  .0428    /* In */ },
	{115,  .9572 },
	{112,  .0096    /* Sn */ },
	{114,  .0066 },
	{115,  .0035 },
	{116,  .1430 },
	{117,  .0761 },
	{118,  .2403 },
	{119,  .0858 },
	{120,  .3258 },
	{122,  .0472               },
	{124,  .0594 },
	{121,  .5725    /* Sb */ },
	{123,  .4275 },
	{120,  .00089   /* Te */ },
	{122,  .0246 },
	{123,  .0087 },
	{124,  .0461 },
	{125,  .0699 },
	{126,  .1871 },
	{128,  .3179 },
	{130,  .3448 },
	{127,  1.00     /* I */ },
	{128,  .0192    /* Xe */ },
	{129,  .2644 },
	{130,  .0408 },
	{131,  .2118 },
	{132,  .2689 },
	{134,  .1044 },
	{136,  .0887 },
	{133,  1.00     /* Cs */ },
	{130,  .00101   /* Ba */ },
	{132,  .00097 },
	{134,  .0242 },
	{135,  .0659 },
	{136,  .0781 },
	{137,  .1132 },
	{138,  .7166 },
	{138,  .00089   /* La */ },
	{139,  .99911 },
	{136,  .00193   /* Ce */ },
	{138,  .0025 },
	{140,  .8848 },
	{142,  .1107 },
	{141,  1.00     /* Pr */ },
	{142,  .2713    /* Nd */ },
	{143,  .1220 },
	{144,  .2387 },
	{145,  .0830 },
	{146,  .1718 },
	{148,  .0572 },
	{150,  .0560 },
	{145,  1.00     /* Pm */ },
	{144,  .0316    /* Sm */ },
	{147,  .1507 },
	{148,  .1127 },
	{149,  .1384 },
	{150,  .0747 },
	{152,  .2663 },
	{154,  .2253 },
	{151,  .4777    /* Eu */ },
	{153,  .5223 },
	{152,  .0020    /* Gd */ },
	{154,  .0215 },
	{155,  .1478 },
	{156,  .2059 },
	{157,  .1571 },
	{158,  .2478 },
	{160,  .2179 },
	{159,  1.00     /* Tb */ },
	{156,  .0005    /* Dy */ },
	{158,  .0009 },
	{160,  .02294 },
	{161,  .1888 },
	{162,  .2553 },
	{163,  .2497 },
	{164,  .2818 },
	{165,  1.00     /* Ho */ },
	{162,  .001     /* Er */ },
	{164,  .015 },
	{166,  .329 },
	{167,  .244 },
	{168,  .269 },
	{170,  .142 },
	{169,  1.00     /* Tm */ },
	{168,  .0006    /* Yb */ },
	{170,  .0421 },
	{171,  .1426 },
	{172,  .2149 },
	{173,  .1702 },
	{174,  .2958 },
	{176,  .1338 },
	{175,  .975     /* Lu */ },
	{176,  .025 },
	{174,  .0018    /* Hf */ },
	{176,  .0520 },
	{177,  .1850 },
	{178,  .2714 },
	{179,  .1375 },
	{180,  .3524 },
	{180,  .000123  /* Ta */ },
	{181,  .99988 },
	{180,  .0014    /* W */ },
	{182,  .2641 },
	{183,  .1440 },
	{184,  .3064 },
	{186,  .2841 },
	{185,  .3707    /* Re */ },
	{187,  .6293 },
	{184,  .00018   /* Os */ },
	{186,  .0159 },
	{187,  .0164 },
	{188,  .133 },
	{189,  .161 },
	{190,  .264 },
	{192,  .410 },
	{191,  .373     /* Ir */ },
	{193,  .627 },
	{190,  .000127,  /* Pt */ },
	{192,  .0078 },
	{194,  .329 },
	{195,  .338 },
	{196,  .253 },
	{198,  .0721 },
	{197,  1.00     /* Au */ },
	{196,  .00146   /* Hg */ },
	{198,  .1002 },
	{199,  .1684 },
	{200,  .2313 },
	{201,  .1322 },
	{202,  .2980 },
	{204,  .0685 },
	{203,  .295     /* Tl */ },
	{205,  .705 },
	{204,  .0148,    /* Pb */ },
	{206,  .236 },
	{207,  .226 },
	{208,  .523 },
	{209,  1.00     /* Bi */ },
	{209,  1.00     /* Po */ },
	{210,  1.00     /* At */ },
	{222,  1.00     /* Rn */ },
	{223,  1.00     /* Fr */ },
	{226,  1.00     /* Ra */ },
	{227,  1.00     /* Ac */ },
	{231,  1.00     /* Pa */ },
	{235,  .0071    /* U */ },
	{238,  .9928 },
	{237,  1.00      /* Np */ },
	{244,  1.00      /* Pu */ },
};

#define ADDBASE 100        /* above the natural elements */
#define MAXADD 10          /* space for user-defined 'elements' */
#define MAXAT  50

element	addel[MAXADD];       /* e.g. isotope enriched ones, etc. */
isotope	addiso[5 * MAXADD];
atom	atoms[MAXAT];        /* up to MAXAT different atoms in one formula */
int 	natoms;              /* number of atoms */
int 	eadd = 0, iadd = 0;  /* added elements & isotopes */

int nel  = sizeof(el) / sizeof(element);
int niso = sizeof(iso) / sizeof(isotope);

/* --- some variables needed for reading the cmd line --- */

char* optarg;		    /* global: pointer to argument of current option */
static int optind = 1;	/* global: index of which argument is next. Is used
						   as a global variable for collection of further
						   arguments (= not options) via argv pointers. */



				/* --- Function prototypes --- */

void 	setpointers(void);
void 	addelement(void);
int 	squob(char* s);
void 	foutput(int a, int n);
isotope	imin(int atno);
isotope	imax(int atno);
int 	atno(char* s);
int 	formula(char* in);
int     getopt(int argc, char* argv[], char* optionS);



void setpointers(void)      /* set pointers in el entries to start of isotopes */
{                           /* for that element in the iso table */
	int i;
	isotope* p;

	p = iso;
	for (i = 0; i < nel; i++)
	{
		el[i].p = p;
		p += el[i].niso;
	}
}


int squob(char* buf)      /* squeeze out the blanks in s and return the length */
			              /* of the resultant string not counting terminating null*/
{
	int i, j;
	char c;

	i = j = 0;
	while ((c = buf[i++]) != '\0')
		if (c != ' ') buf[j++] = c;
	buf[j] = '\0';
	return(j);
}


void foutput(int oz, int nr)
{
	atoms[natoms].atno = oz;		/* Ordnungszahl */
	atoms[natoms].count = nr;		/* No. of atoms of this element */
	natoms++;
	if (natoms >= MAXAT)
	{
		printf("Formula too long\n");
		exit(1);
	}
}


isotope imin(int atno)        /* isotope of lowest mass of element atno */
{
	if (atno < ADDBASE)
		return (*(el[atno - 1].p));  /* element 1 is in table entry 0, etc. */
	else return(*(addel[atno - ADDBASE].p));
}


isotope imax(int atno)        /* isotope of highest mass of element atno */
{
	if (atno < ADDBASE)
		return(*(el[atno - 1].p + el[atno - 1].niso - 1));
	else return(*(addel[atno - ADDBASE].p + addel[atno - ADDBASE].niso - 1));
}


int atno(char* str)  			/* return atom # or 0 if str not valid element symbol */
{
	int i;

	for (i = 0; i < nel; i++)             /* try natural elements first */
		if (0 == strcmp(el[i].sym, str))	/* if symbol is found in elem. table */
			return(i + 1);			/* return correct atom number */

	for (i = 0; i < eadd; i++)		/* try 'user-def.' elements */
		if (0 == strcmp(addel[i].sym, str))	/* if symbol is found in 'user-def.'elem. table */
			return(i + ADDBASE);            /* return a high 'atomic #' on user-defined elements */

	return 0;				/* this is 'else' */
}


int formula(char* in)
/* Determine if input is valid formula. Set composition in table *atoms.
   return 1 if OK, 0 if not call foutput() */
{
	int 	a, n,      /* to handle (element, count) pairs. a is El.No., n is the number */
		state;     /* number of chars read */
	char 	ch, buf[3];     /* this is safe here */

	state = a = n = 0;				/* here, no char was read */

	while (0 != (ch = *(in++)))		/* while not end of string */
	{
		if (ch == '\n' || ch == '\r')	/* junk at end from fgets ? */
			continue;
		switch (state)
		{
		case 0:
			if (isupper(ch))		/* uppercase letter ? */
			{
				buf[0] = ch;		/* take first char of element symbol */
				buf[1] = 0;			/* ASCII zero to terminate string */
				state = 1;			/* i.e. 1st char was read */
			}
			else
				goto error;
			break;

		case 1:
			if (isdigit(ch))			/* is it a number ? */
			{
				if (0 == (a = atno(buf)))	/* if NOT an element */
					goto error;
				n = ch - '0';		/* just like 'atoi()' */
				state = 2;			/* 2nd char was read */
			}
			else if (islower(ch))
			{
				buf[1] = ch;       	 	/* take char as 2nd letter */
				buf[2] = 0;			/* terminate string */
				if (0 == (a = atno(buf)))	/* see above */
					goto error;
				state = 3;
			}
			else if (isupper(ch))
			{
				if (0 == (a = atno(buf)))	/* see above */
					goto error;
				n = 1;                      /* 1st char read */
				foutput(a, n);
				buf[0] = ch;		/* take first char of element symbol */
				buf[1] = 0;			/* ASCII zero to terminate string */
				state = 1;			/* i.e. 1st char was read */
			}
			else
				goto error;
			break;

		case 2:
			if (isdigit(ch))                    /* is it a number */
				n = 10 * n + ch - '0';		/* YES -> get value (tens) */
			else if (isupper(ch))
			{
				foutput(a, n);
				buf[0] = ch;		/* take first char of element symbol */
				buf[1] = 0;			/* ASCII zero to terminate string */
				state = 1;			/* i.e. 1st char was read */
			}
			else
				goto error;
			break;

		case 3:
			if (isdigit(ch))      		/* is it a number */
			{				/* YES */
				n = ch - '0';			/* get value */
				state = 2;
			}
			else if (isupper(ch))
			{
				if (0 == (a = atno(buf)))
					goto error;
				n = 1;
				foutput(a, n);
				buf[0] = ch;			/* take first char of element symbol */
				buf[1] = 0;			/* ASCII zero to terminate string */
				state = 1;			/* i.e. 1st char was read */
			}
			else
				goto error;
			break;

		}  /* end of case */
	}          /* end of while */

	if (state == 1 || state == 3) n = 1;
	if (state != 0)
	{
		if (0 == (a = atno(buf))) goto error;
		foutput(a, n);
		return(1);
	}

error:
	printf("Bad formula\n\n");
	return(0);
}



void addelement(void)            /* user-defined element */
{
	int i, j, k, first;
	float f, sumpc;
	char buf1[81], buf2[81], * p;

	while (1)
	{
		printf("Enter symbol for element: ");
		fgets(buf1, 80, stdin);
		for (i = 0; i < strlen(buf1); i++)     /* strlen is safe since the string is null-terminated */
			if (buf1[i] == '\n' || buf1[i] == '\r')
				buf1[i] = ' ';
		squob(buf1);
		buf1[2] = 0;
		if (!isupper(buf1[0]) || (buf1[1] != 0 && (!islower(buf1[1]))))
		{
			printf("Bad symbol\n\n");
			continue;
		}
		if (0 == atno(buf1))     /* not already known symbol */
		{
			addel[eadd].sym = (char*)malloc(3);
			strncpy(addel[eadd].sym, buf1, 3);
			addel[eadd].sym[2] = 0x0;   /* make sure this is null-terminated */
			break;
		}
		printf("This symbol is already in use.\n\n");
	}
	printf("\nEnter mass - percent abundance pairs, one pair per line, with space between.\n");
	printf("Enter an empty line to finish.\n");

startiso:
	addel[eadd].niso = 0;
	first = 1;
	sumpc = 0.0;
	while (1)
	{
		printf(">");
		fgets(buf1, 80, stdin);
		if (strlen(buf1) <= 1)      /* strlen is safe since the string is null-terminated */
		{
			if (sumpc > 99.5 && sumpc < 100.5) break;
			else if (sumpc <= 99.5)
			{
				printf("Sum of isotope percentages is %.1f, press Y if ok, N to enter more:", sumpc);
				fgets(buf2, 80, stdin);
				if (buf2[0] == 'y' || buf2[0] == 'Y') break;
				else continue;
			}
			else
			{
				printf("Sum of isotope percentages is %.1f, press Y if ok, N to start over:", sumpc);
				fgets(buf2, 80, stdin);
				if (buf2[0] == 'y' || buf2[0] == 'Y') break;
				else goto startiso;
			}
		}
		j = strtol(buf1, &p, 10);

		if (j > 0 && j < 300)
			addiso[iadd].m = j;
		else if (j <= 0)
		{
			printf("Negative or zero mass. Re-enter line.");
			continue;
		}
		else
		{
			printf("Mass > 300. Type Y to confirm, or N to re-enter line. ");
			fgets(buf2, 80, stdin);
			k = buf2[0];
			if (k != 'y' && k != 'Y')
				continue;
			addiso[iadd].m = j;
		}

		f = atof(p);
		if (f > 0 && f <= 100)
		{
			addiso[iadd].fr = f / 100;
			sumpc += f;
		}
		else
		{
			printf("Impossible percentage. Re-enter line\n");
			continue;
		}

		addel[eadd].niso++;
		if (first)
		{
			addel[eadd].p = addiso + iadd;
			first = 0;
		}
		iadd++;
	}
	eadd++;
}



int main(int argc, char* argv[])
{
	int i, j, m, q, nold, nnew, oldmin, oldmax, newmin, newmax, ii, ix, fraction, ns, read_cmd, tmp;
	register int k;
	char buf[81], stars[71];
	float fr, maxintens, sumintens;
	isotope s;
	element e;
	peak* old, * new;
	register peak* pp;

	static char* id =
		"isotope. Copyright (C) by Joerg Hau 1996...2009, version";

	static char* msg =
		"\nusage: isotope [-h] [-v] [-f] [formula]\n\nValid command line options are:\n"
		"    -h       This Help screen.\n"
		"    -v       Display version information.\n"
		"    -f       Print fractional intensities (default: scaled to 100%).\n"
		"    formula  Chemical formula, e.g. 'C12H11O11'.\n";

	static char* disclaimer =
		"\nThis program is free software; you can redistribute it and/or modify it under\n"
		"the terms of version 2 of the GNU General Public License as published by the\n"
		"Free Software Foundation.\n\n"
		"This program is distributed in the hope that it will be useful, but WITHOUT ANY\n"
		"WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A\n"
		"PARTICULAR PURPOSE. See the GNU General Public License for details.\n";

	fraction = 0;   /* normalize to 100 max, or print fractions on -f cmd switch */
	read_cmd = 0;   /* != 0 if formula is read via cmd line */
	setpointers();
	nnew = 0;
	new = NULL;

	/* decode and read the command line */

	while ((tmp = getopt(argc, argv, "hvf")) != EOF)
		switch (tmp)
		{
		case 'h':     	  		/* help me */
			printf("%s %s\n", id, VERSION);
			printf("%s", msg);
			printf("%s\n", disclaimer);
			return 0;
		case 'v':     	  		/* version */
			printf("%s %s\n", id, VERSION);
			return 0;
		case 'f':    			/* print fractional intensities */
			fraction = 1;
			continue;
		case '~':    	  	/* invalid arg */
		default:
			printf("'%s -h' for help.\n", argv[0]);
			return 1;
		}

	if (argv[optind] != NULL)	 /* remaining parameter on cmd line? */
	{
		strncpy(buf, argv[optind], 80);     /* read it */
		buf[80] = 0x0;          /* force null-termination */
		read_cmd = 1;           /* set flag */
	}

	while (1)
	{
		if (!read_cmd)          /* if NOT read via cmd line, use interactive mode */
		{
			printf("Enter a formula (no brackets). Q to quit, E to define an extra element.\n:");
			fgets(buf, 80, stdin);				/* read line from stdin */
		}

		squob(buf);					/* clean up */
		for (k = 0; k < strlen(buf); k++)		/* search for CR/LF */
		{                                   /* strlen is safe since the string is null-terminated */
			if (buf[k] == '\n' || buf[k] == '\r')
			{
				buf[k] = 0;			/* stop at CR or LF */
				break;
			}
		}

		if (buf[0] == 'q' || buf[0] == 'Q')
			exit(0);

		if ((buf[0] == 'e' || buf[0] == 'E') && strlen(buf) == 1)	/* add -e-lement */
		{                                   /* strlen is safe since the string is null-terminated */
			addelement();
			continue;
		}

		natoms = 0;					/* init. */
		k = formula(buf);
		if (k == 0)                 /* problem ? */
		{
			if (read_cmd)           /* if formula was read via cmd line, */
				exit(1);           /* quit here */
			continue;
		}

		old = (peak*)malloc(sizeof(peak));		/* init. */
		old->mass = 0;
		old->intens = 1;
		nold = 1;
		oldmin = oldmax = 0;

		for (i = 0; i < natoms; i++)				/* for all elements */
		{
			for (j = 0; j < atoms[i].count; j++)		/* for all atoms of an element */
			{
				s = imin(atoms[i].atno);
				newmin = oldmin + s.m;			/* min. mass */
				s = imax(atoms[i].atno);
				newmax = oldmax + s.m;			/* max. mass */
				nnew = newmax - newmin + 1;		/* number */
				new = (peak*)malloc(nnew * sizeof(peak));
				if (new == NULL)
				{
					printf("\nOut of Memory!\n");
					exit(1);
				}
				for (k = 0; k < nnew; k++)
					new[k].intens = 0;		/* init. */

				if (atoms[i].atno < ADDBASE)
					e = el[atoms[i].atno - 1];
				else
					e = addel[atoms[i].atno - ADDBASE];

				for (k = 0; k < e.niso; k++)          /* for all isotopes */
				{
					m = (k + e.p)->m;		/* mass */
					fr = (k + e.p)->fr;		/* inty */
					for (q = 0; q < nold; q++)
					{
						ix = m + old[q].mass - newmin;
						new[ix].mass = m + old[q].mass;		/* shift mass */
						new[ix].intens += fr * old[q].intens; 	/* add inty */
					}
				}	/* end of 'k' loop (isotopes) */

			   /* normalize to maximum intensity of 1.0 */
				maxintens = 0;
				for (ii = 0; ii < nnew; ii++)
					if (new[ii].intens > maxintens)		/* find max. value */
						maxintens = new[ii].intens;
				for (ii = 0; ii < nnew; ii++)
					new[ii].intens /= maxintens;

				/* throw away very small peaks */
				for (ii = 0; ii < nnew; ii++)
				{
					if (new[ii].intens < CUTOFF)
					{
						for (k = ii, pp = new + ii; k < nnew - 1; k++, pp++)
						{
							pp->mass = (pp + 1)->mass;
							pp->intens = (pp + 1)->intens;   /* inner loop...*/
						}                            /* avoid structure copy */
						nnew--;
						ii--;
					}
				}
				free(old);
				old = new;
				nold = nnew;
				oldmin = newmin;
				oldmax = newmax;
			}	/* end of 'j' loop (atoms) */
		}		/* end of 'i' loop (elements) */

		maxintens = sumintens = 0;
		for (ii = 0; ii < nnew; ii++)		/* find max. */
		{
			sumintens += new[ii].intens;
			if (new[ii].intens > maxintens)
				maxintens = new[ii].intens;
		}
		for (ii = 0; ii < nnew; ii++)		/* calculate fraction/percent */
		{
			if (fraction)
				new[ii].intens /= sumintens;
			else
				new[ii].intens *= 100;  /* they are already normalized to max=1 */
		}
		if (fraction)
			maxintens /= sumintens;
		else
			maxintens *= 100;

		for (ii = 0; ii < nnew; ii++)
		{
			ns = .5 + 60.0 * new[ii].intens / maxintens;	/* no. of stars */
			for (k = 0; k < ns; k++)
				stars[k] = '*';
			stars[ns] = 0;
			printf(fraction ? "%5d%8.4f  |%s\n" : "%5d%8.2f  |%s\n",
				new[ii].mass, new[ii].intens, stars);
		}
		printf("\n");
		free(new);
		if (read_cmd)          /* if formula was read via cmd line, quit here */
			exit(0);
	}		/* end of 'while (1)...' */
}



/***************************************************************************
* GETOPT: Command line parser, system V style.
*
*  This routine is widely (and wildly) adapted from code that was
*  made available by Borland International Inc.
*
*  Standard option syntax is:
*
*    option ::= SW [optLetter]* [argLetter space* argument]
*
*  where
*    - SW is '-'
*    - there is no space before any optLetter or argLetter.
*    - opt/arg letters are alphabetic, not punctuation characters.
*    - optLetters, if present, must be matched in optionS.
*    - argLetters, if present, are found in optionS followed by ':'.
*    - argument is any white-space delimited string.  Note that it
*      can include the SW character.
*    - upper and lower case letters are distinct.
*
*  There may be multiple option clusters on a command line, each
*  beginning with a SW, but all must appear before any non-option
*  arguments (arguments not introduced by SW).  Opt/arg letters may
*  be repeated: it is up to the caller to decide if that is an error.
*
*  The character SW appearing alone as the last argument is an error.
*  The lead-in sequence SWSW ("--") causes itself and all the rest
*  of the line to be ignored (allowing non-options which begin
*  with the switch char).
*
*  The string *optionS allows valid opt/arg letters to be recognized.
*  argLetters are followed with ':'.  Getopt () returns the value of
*  the option character found, or EOF if no more options are in the
*  command line. If option is an argLetter then the global optarg is
*  set to point to the argument string (having skipped any white-space).
*
*  The global optind is initially 1 and is always left as the index
*  of the next argument of argv[] which getopt has not taken.  Note
*  that if "--" or "//" are used then optind is stepped to the next
*  argument before getopt() returns EOF.
*
*  If an error occurs, that is an SW char precedes an unknown letter,
*  then getopt() will return a '~' character and normally prints an
*  error message via perror().  If the global variable opterr is set
*  to false (zero) before calling getopt() then the error message is
*  not printed.
*
*  For example, if
*
*    *optionS == "A:F:PuU:wXZ:"
*
*  then 'P', 'u', 'w', and 'X' are option letters and 'A', 'F',
*  'U', 'Z' are followed by arguments. A valid command line may be:
*
*    aCommand  -uPFPi -X -A L someFile
*
*  where:
*    - 'u' and 'P' will be returned as isolated option letters.
*    - 'F' will return with "Pi" as its argument string.
*    - 'X' is an isolated option.
*    - 'A' will return with "L" as its argument.
*    - "someFile" is not an option, and terminates getOpt.  The
*      caller may collect remaining arguments using argv pointers.
***************************************************************************/
int getopt(int argc, char* argv[], char* optionS)
{
	static char* letP = NULL;		/* remember next option char's location */
	static char SW = '-';		/* switch character */

	int opterr = 1;				/* allow error message	*/
	unsigned char ch;
	char* optP;

	if (argc > optind)
	{
		if (letP == NULL)
		{
			if ((letP = argv[optind]) == NULL || *(letP++) != SW)
				goto gopEOF;

			if (*letP == SW)
			{
				optind++;
				goto gopEOF;
			}
		}
		if (0 == (ch = *(letP++)))
		{
			optind++;
			goto gopEOF;
		}
		if (':' == ch || (optP = strchr(optionS, ch)) == NULL)
			goto gopError;
		if (':' == *(++optP))
		{
			optind++;
			if (0 == *letP)
			{
				if (argc <= optind)
					goto  gopError;
				letP = argv[optind++];
			}
			optarg = letP;
			letP = NULL;
		}
		else
		{
			if (0 == *letP)
			{
				optind++;
				letP = NULL;
			}
			optarg = NULL;
		}
		return ch;
	}

gopEOF:
	optarg = letP = NULL;
	return EOF;

gopError:
	optarg = NULL;
	errno = EINVAL;
	if (opterr)
		perror("Command line option");
	return ('~');
}
