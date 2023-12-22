% FvdB 070821
clear all;
close all;

clc;
disp('Small script to explain the use of "ApplyDMW.m" and "cow.m"');
disp('Loading example data ("gauss.txt"; two Gaussians plus some noise).');
load gauss.txt; X = gauss; clear gauss;
figure(1); subplot(3,2,1); plot(X'); grid; title('Data (blue=target, green=sample)'); disp('[Press key...]'); pause;

disp('First, try Dynamic Time Warping (DTW) in it''s original form');
disp('(so-called T(1,1) table) with a 10% band-limit.');
[Xw_dtw1,WP_dtw1] = ApplyDMW(X(1:2,:),1,[0 0],1,1,0.05);
figure(1); subplot(3,2,2); plot(Xw_dtw1'); grid; title('DTW T(1,1)');
disp('Seems to work "to well", especially for the peak on the right! [...]'); pause;

disp('Next, try DTW with the default settings (which gives less flexibility).');
[Xw_dtw2,WP_dtw2] = ApplyDMW(X(1:2,:),1,[0 0],20,2,0.05); 
figure(1); subplot(3,2,3); plot(Xw_dtw2'); grid; title('DTW T(38,2)');
disp('Looks better (less flexibility) [...]'); pause;

disp('We can achieve similar results with COW (first too flexible)');
[warping1,Xw_cow1,diagnos1] = cow(X(1,:),X(2,:),5,1,[1 1 0]);
disp('This is the default graphical output for "cow.m". [...]'); pause;
figure(1); subplot(3,2,4); plot([X(1,:); Xw_cow1]'); grid; title('COW(3,1)');
disp('Distortion due to flexibility, especially in the left peak. [...]'); pause;

disp('COW less flexible (longer segments with more slack = possibility to move)');
[warping1,Xw_cow1,diagnos1] = cow(X(1,:),X(2,:),50,10);
figure(1); subplot(3,2,5); plot([X(1,:); Xw_cow1]'); grid; title('COW(25,5)');
disp('Looks better - the trick is of course to find the right settings for your own problem');

disp('In COW we can also place the segment boundaries in a specific location (e.g. at points 50 125 and 275)');
disp('(Have a close look at the build-in diagnostics plot)');
[warping2,Xw_cow2,diagnos2] = cow(X(1,:),X(2,:),[1 50 125 275 300; 1 50 125 275 300],10,[1 1 0]);
disp('This is again the default graphical output for "cow.m". [...]'); pause;
figure(1); subplot(3,2,6); plot([X(1,:); Xw_cow2]'); grid; title('COW(assigned,10)');
disp('The trick is of course to find the right settings for your own problem. [Finished]');