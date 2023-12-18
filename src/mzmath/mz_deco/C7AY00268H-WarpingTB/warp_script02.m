% FvdB 070821
clear all;
close all;

clc;
disp('Small sample script for the use of "coshift.m", "ApplyDMW.m" and "cow.m".');
disp('Loading example data (two chromatograms).');
load dtw_cow_demo.txt; X = dtw_cow_demo; clear dtw_cow_demo;

disp('Plotting data (first two rows are chromatograms, third one is the time axis). [Press key...]');
figure(1); subplot(5,1,1); plot(X(3,:),X(1,:),X(3,:),X(2,:)); grid;
xlabel('time (min)'); ylabel('FID'); title('original data'); legend('reference','sample');
pause;

disp('For this chromatographic example a simple, "linear"');
disp('Correlation Optimized Shifting (COShift) correction is not enough.');
figure(2); [Xw_cs,R_cs] = coshift(X(1,:),X(2,:),10,[1 1 1]);
disp('This is the default graphical output for "coshift.m". [...]'); pause;

disp('Plot coshift results. [...]');
figure(1); subplot(5,1,2); plot(X(3,:),X(1,:),X(3,:),Xw_cs); grid; legend('reference','samp. coshift');
pause;

disp('First, try Dynamic Time Warping (DTW) in it''s original form');
disp('(so-called T(1,1) table) with a 10% band-limit.');
[Xw_dtw1,WP_dtw1] = ApplyDMW(X(1:2,:),1,[0 0],1,1,.01);

disp('Plot DTW #1 results.');
figure(1); subplot(5,1,3); plot(X(3,:),X(1,:),X(3,:),Xw_dtw1(2,:));
grid; legend('reference','samp. DTW #1');

disp('Seems to work fine, but this approach is often to flexible, leading to artifacts');
disp('e.g. zoom-in on the peak around 5.4 minutes to see what happens. [...]'); pause;

disp('Next, try DTW with the default settings');
disp('(a T(39,1) table, which gives less flexibility).');
[Xw_dtw2,WP_dtw2] = ApplyDMW(X(1:2,:),1);

disp('Plot DTW #2 results.');
figure(1); subplot(5,1,4); plot(X(3,:),X(1,:),X(3,:),Xw_dtw2(2,:)); grid; legend('reference','samp. DTW #2');
disp('Zoom-in on the same peak (around 5.4 minutes) to see the effect. [...]'); pause

disp('Last, try Correlation Optimized Warping (COW) to align');
disp('the chromatograms (using the function to plot the results).');
disp('(In this case segment lengths of 10 data-points and a slack of 1 data-point.)');

[warping,Xw_cow,diagnos] = cow(X(1,:),X(2,:),10,1,[1 1 0]);
disp('Default graphical output for "cow.m". [...]'); pause;

disp('Plot COW results.');
figure(1); subplot(5,1,5); plot(X(3,:),X(1,:),X(3,:),Xw_cow); grid; legend('reference','samp. COW');
disp('Again, zoom-in on the same peak (around 5.4 minutes). [Finished]');