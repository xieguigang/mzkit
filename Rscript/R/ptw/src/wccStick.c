/* wccStick.c: for comparing stick patters using the WCC criterion
   This version: July 2, 2013. Earlier version has existed but got
   lost when moving to Italy...

   Rather than looping over all shifts r, from -l to +l (notation from
   Hageman2000), we only need to consider those differences actually
   present in the data and smaller than the triangle width. The result
   is therefore much faster than the full-profile WCC. Data structures
   are a little bit more complex than the simple equal-length vectors
   of the full-profile version. 

   Eventually this file can be incorporated in the src directory of
   the ptw package.

   Author: Ron Wehrens
   */

#include <R.h>

double st_wght(double, double);
double st_Cfg(double *, int, double *, int, double);
void st_WAC(double *, int *, double *, double *);
void st_WCC(double *, int *, double *, int *, double *, double *);

double st_wght(double dst, double trwdth) {
  double wght;

  wght = 1.0 - fabs(dst) / trwdth;
  return(wght);
}

double st_Cfg(double *p1, int np1, double *p2, int np2, double trwdth)
{
  int i, j;
  double diff, wght, anum;

  anum = 0.0;
  for (i=0; i<np1; i++)
    for (j=0; j<np2; j++) {
      diff = fabs(p1[i] - p2[j]);

      if (diff < trwdth) {
	wght = st_wght(diff, trwdth);
	anum += p1[i+np1]*p2[j+np2]*wght;
      }
    }
  
  return(anum);
}

void st_WAC(double *p1, int *pnp1, double *ptrwdth, double *WAC) 
{
  int np1 = *pnp1;
  double trwdth = *ptrwdth;
  
  *WAC = st_Cfg(p1, np1, p1, np1, trwdth);
  *WAC = sqrt(*WAC);
}

void st_WCC(double *p1, int *pnp1, double *p2, int *pnp2, 
	    double *ptrwdth, double *WCC) 
{
  int np1 = *pnp1, np2 = *pnp2;
  double trwdth = *ptrwdth;
  
  *WCC = st_Cfg(p1, np1, p2, np2, trwdth);
}

