% FvdB 061128
clear all;
close all;

clc;
disp('Small sample script for the use of user-defined segment boundaries in "cow.m"')
disp('Make example data (three Gaussian curves + noise).');
X(1,:) = 1:500;
X(2,:) = sum(gauss(500,[150 250 350],[10 11 9],0.01));
X(3,:) = sum(gauss(500,[160 240 360],[11 10 9],0.01));

disp('Plotting data. [Press key...]');
figure(1); plot(X(1,:),X(2,:),X(1,:),X(3,:));
title('original data'); grid; legend('reference','sample');
pause;

disp('Use COW with many equidistant segments and small slack to align');
[warping,Xw_cow,diagnos] = cow(X(2,:),X(3,:),10,1,[1 1 0]);
disp('Default graphical output for "cow.m". [...]'); pause;

disp('Peak 1 and 3 want to go to the left, peak 2 to the right.');
disp('Let''s plot the warping-path.');
figure
plot(X(1,:),X(2,:)*2e3,'b',X(3,:)*2e3,1:length(X(1,:)),'g',warping(1,:,2),warping(1,:,2),'-k',warping(1,:,2),warping(1,:,1),'--k','LineWidth',2);
xlabel('reference'); ylabel('sample'); grid; axis tight; axis square

disp('The result is not very satisfying.');
disp('Try COW with user defined segments and large slack to align. [...]'); pause;

index = [1 50 100 180 220 280 320 400 450 500];
index = [index; index];
[warping,Xw_cow,diagnos] = cow(X(2,:),X(3,:),index,30,[1 1 0]);
disp('Let''s plot the warping-path again. [...]'); pause;
figure
plot(X(1,:),X(2,:)*2e3,'b',X(3,:)*2e3,1:length(X(1,:)),'g',warping(1,:,2),warping(1,:,2),'-k',warping(1,:,2),warping(1,:,1),'--k','LineWidth',2);
xlabel('reference'); ylabel('sample'); grid; axis tight; axis square
disp('[Finished]');