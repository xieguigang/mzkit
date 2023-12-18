% FvdB 061128
clear all;
close all;

clc;
disp('Small sample script for the use of "cow.m" with augmentation.')
disp('Loading example data (two chromatograms).');
load dtw_cow_demo.txt; X = dtw_cow_demo; clear dtw_cow_demo;

disp('Plotting data (first two rows are chromatograms, third one is the time axis). [Press key...]');
figure(1); subplot(3,1,1); plot(X(3,:),X(1,:),X(3,:),X(2,:));
xlabel('time (min)'); ylabel('FID'); title('original data'); grid; legend('reference','sample');
pause;

disp('Use Correlation Optimized Warping (COW) to align');
[warping,Xw_cow,diagnos] = cow(X(1,:),X(2,:),10,1,[0 1 0]);
disp('Plot COW results.');
subplot(3,1,2); plot(X(3,:),X(1,:),X(3,:),Xw_cow);
xlabel('time (min)'); ylabel('FID'); title('COW(10,1) data'); grid; legend('reference','sample');

disp('The last peak (around 16.7min) is not aligned well because there is not enough freedom for correction after this point.');
disp('We can use e.g. 25% of the lowest points in the chromatogram to estimate base-line noise,');
disp('and augment reference and sample with 200 points to get better alignment for the last part. [...]');
pause;

temp = sort(X(1,:));
std_ref = std(temp(1:round(length(temp)*.25)));
aug_ref = [X(1,:) X(1,end) + randn(1,200)*std_ref];
temp = sort(X(2,:));
std_samp = std(temp(1:round(length(temp)*.25)));
aug_samp = [X(2,:) X(2,end) + randn(1,200)*std_samp];
N = length(X(1,:));

[warping_aug,Xw_cow_aug,diagnos_aug] = cow(aug_ref,aug_samp,10,1,[1 1 0]);
disp('Default graphical output "cow.m". [...]'); pause;

disp('Plot COW results after augmentation. Notice that the last peak is now well aligned. [Finished]');
figure(1); subplot(3,1,3); plot(X(3,:),X(1,:),X(3,:),Xw_cow_aug(1:N));
xlabel('time (min)'); ylabel('FID'); title('COW(10,1) augmented data'); grid; legend('reference','sample');
