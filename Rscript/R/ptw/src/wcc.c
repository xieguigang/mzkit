/* wcc.c: for comparing patters using the WCC criterion
   This version: Sep 14, 2009. Based on functions in the wccsom package
   Author: Ron Wehrens
   */

#include <R.h>

double wcc_crosscorr(double *, double *, int, double *, int);
double wcc_autocorr(double *, int, double *, int);
double wcc_corr(double *, double *, int);
void wccdist(double *, double *, int *, double *, int *, double *);
void wacdist(double *, int *, double *, int *, double *);


double wcc_crosscorr(double *p1, double *p2, int np, 
		     double *wghts, int trwdth) {
  int i;
  double crosscov;

  crosscov = wcc_corr(p1, p2, np);
  
  for (i=1; i<trwdth; i++) {
    crosscov+=(wcc_corr(p1, p2+i, np-i)*wghts[i]);
    crosscov+=(wcc_corr(p1+i, p2, np-i)*wghts[i]);
  }
    
  return(crosscov);
}

double wcc_autocorr(double *p1, int np, double *wghts, int trwdth) {
  int i;
  double autocov;

  autocov = wcc_corr(p1, p1, np);

  for (i=1; i<trwdth; i++) 
    autocov += 2*(wcc_corr(p1, p1+i, np-i)*wghts[i]);

  return(sqrt(autocov));
}

double wcc_corr(double *p1, double *p2, int npoints)
{
  int i;
  double anum;

  anum = 0.0;
  for (i=0; i<npoints; i++)
    anum += p1[i]*p2[i];

  return(anum);
}

void wccdist(double *p1, double *p2, int *pnpoints, 
	     double *wghts, int *ptrwdth, double *WCC)
{
  int npoints = *pnpoints, trwdth=*ptrwdth;

  *WCC = wcc_crosscorr(p1, p2, npoints, wghts, trwdth);
}

void wacdist(double *p1, int *pnpoints, 
	     double *wghts, int *ptrwdth, double *ACC) 
{
  int npoints = *pnpoints, trwdth=*ptrwdth;

  *ACC = wcc_autocorr(p1, npoints, wghts, trwdth);
}


