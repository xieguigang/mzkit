#include <stdlib.h> // for NULL
#include <R_ext/Rdynload.h>

/* FIXME: 
   Check these declarations against the C/Fortran source code.
*/

/* .C calls */
extern void smooth1(void *, void *, void *, void *, void *, void *, void *);
extern void smooth2(void *, void *, void *, void *, void *, void *, void *, void *);
extern void st_WAC(void *, void *, void *, void *);
extern void st_WCC(void *, void *, void *, void *, void *, void *);
extern void wacdist(void *, void *, void *, void *, void *);
extern void wccdist(void *, void *, void *, void *, void *, void *);

static const R_CMethodDef CEntries[] = {
    {"smooth1", (DL_FUNC) &smooth1, 7},
    {"smooth2", (DL_FUNC) &smooth2, 8},
    {"st_WAC",  (DL_FUNC) &st_WAC,  4},
    {"st_WCC",  (DL_FUNC) &st_WCC,  6},
    {"wacdist", (DL_FUNC) &wacdist, 5},
    {"wccdist", (DL_FUNC) &wccdist, 6},
    {NULL, NULL, 0}
};

void R_init_ptw(DllInfo *dll)
{
    R_registerRoutines(dll, CEntries, NULL, NULL, NULL);
    R_useDynamicSymbols(dll, FALSE);
}
