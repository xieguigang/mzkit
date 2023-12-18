% FvdB 070821
% Not correct, needs revision!!!
clear all;
close all;

clc;
disp('Small sample script for the use of "cow.m" output to warp different data.')
disp('(useful e.g. to give the same shift-correction to additional channels in hyper-signals)');
disp('(MatLab programmers only!!!)');

disp('Loading example data (two chromatograms).');
load dtw_cow_demo.txt; X = dtw_cow_demo; clear dtw_cow_demo;

disp('Plotting data (first two rows are chromatograms, third one is the time axis).');
figure; subplot(3,1,1); plot(X(3,:),X(1,:),X(3,:),X(2,:));
xlabel('time (min)'); ylabel('FID'); title('original data'); grid; legend('reference','sample');

disp('Use Correlation Optimized Warping (COW) to align');
[warping,Xw_cow,diagnos] = cow(X(1,:),X(2,:),10,1,[0 1 0]);

disp('Plot COW results from routine.');
subplot(3,1,2); plot(X(3,:),Xw_cow); grid; title('"cow.m" function output');

% [START] This is the code-snippet to apply the (previously found) shift correction to the same signal
[xw] = cow_apply(X(2,:),warping);
% [STOP]

subplot(3,1,3); plot(X(3,:),xw); grid; title('"manual" reconstruction');
disp('Plot COW results from "manual" reconstruction (obviously the same). [Finished]');