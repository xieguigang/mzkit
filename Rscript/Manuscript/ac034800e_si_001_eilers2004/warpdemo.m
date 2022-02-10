% Smooth warping demo

% Paul Eilers, 2002

% Load and select data
load demodata
Y = G(:, 2501:7500)';
m = size(Y, 1);
t = (1:m)' - 0.5;

% Select signals, remove baseline 
jx = 1;
jy = 16;
lambda = 1e5;
x0 = Y(:, jx);
bx0 = asysm(x0, 1e7, 0.001, 2);
x = x0 - bx0;
y0 = Y(:, jy);
by0 = asysm(y0, 1e7, 0.001, 2);
y = y0 - by0;

% Heavy smoothing, to broaden peaks
xs = difsm(x, lambda, 2);   % reference
ys = difsm(y, lambda, 2);   % 

% Compute warping function and use on unsmoothed signal
[w sel] = quadwarp(xs, ys);
xw = interpol(w, x);

% Plot raw data
figure(1)
fs = 9;
subplot(2, 1, 1)
plot(t, x, 'b', t, y, 'r')
set(gca, 'FontSize', fs)
title('Raw data')
xlabel('Signal sample number')
ylabel('Signal')
subplot(2,1,  2)
plot(t, y - x, 'k')
set(gca, 'FontSize', fs)
title('Differences between raw data traces')
set(gca, 'YLim', 800 * [-1 1])
xlabel('Signal sample number')
ylabel('Signal')

% Plot warped data
figure(2)
subplot(2, 1, 1)
plot(t(sel), y(sel), 'k')
hold on
plot(t(sel), xw, 'r')
hold off
title('Warped data')
set(gca, 'FontSize', fs)
xlabel('Signal sample number')
ylabel('Signal')
subplot(2, 1, 2)
plot(t(sel), y(sel) - xw, 'k')
title('Differences between warped data traces')
set(gca, 'YLim', 800 * [-1 1])
xlabel('Signal sample number')
ylabel('Signal')

% Plot warping function
figure(3)
subplot(1, 3, 1)
plot(w)
fs = 8;
set(gca, 'FontSize', fs)
title('Warping function w(t)')
subplot(1, 3, 2);
plot(w - t);
set(gca, 'FontSize', fs)
title('w(t) - t')
subplot(1, 3, 3)
plot(diff(w))
set(gca, 'FontSize', fs)
title('\Delta w(t)')
