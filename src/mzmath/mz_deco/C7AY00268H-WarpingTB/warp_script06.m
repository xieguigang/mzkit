% FvdB 070821
clear all;
close all;

clc;
disp('Small sample script for automatic search of "segment length" and "slack size" parameter in COW')
disp('using "optim_cow.m"')
disp('Load example data (ten samples of three shifted Gaussian peaks). [Press key...]');
load gauss_optim.txt; X = gauss_optim; clear gauss_optim
pause;

disp('Use "ref_select" to select the reference sample. [...]');
ref = ref_select(X,1:size(X,2),[5 1]);

pause;
disp('Use "cow_optim" to to find the optimal segment length and slack size parameter');
disp('In this example we use segment length 5 to 30 and slack size 1 to 10 points.');
disp('The routine starts with a 5x5 grid search in the segment-slack space,')
disp('followed by three further optimizations of the three highest grid points (maximum 50 steps).');
[optim_pars,OS,diagnos] = optim_cow(X,[5 30 1 10],[1 3 50 0.15],ref);

disp('The diagnostic plots show: the overall best segment-slack parameters, the optimum for "Simplicity" only,');
disp('the optimum for "Peak factor" only and the data before and after correction');
disp('[Finished]');