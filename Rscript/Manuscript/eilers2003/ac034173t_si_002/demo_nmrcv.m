% Demonstration NMR spectrum smoothing with Whittaker smoother
% Optimal smoothing by cross-validation
%
% Paul Eilers, 2003

% Get the data
y = load('nmr.dat');
m = 1000;
y = y(1:m);

% Smooth for series of lambdas
lambdas = 10 .^ (-2:0.2:3);
cvs = [];
for lambda = lambdas
   [z cv] = whitsm(y, lambda, 2);
   cvs = [cvs cv];
end

% Choose optimal lambda
[cm ci] = min(cvs);
lambda = lambdas(ci);
[z cv] = whitsm(y, lambda, 2);

% Plot data and smooth
subplot(2, 1, 1);
plot([z-10 y] )    % Downward shift for visibility
title('NMR spectrum and optimal smooth')
xlabel('Channel')
ylabel('Signal strength')

% Plot CV profile
subplot(2, 1, 2)
semilogx(lambdas, cvs)
title('Cross-validation error')
xlabel('\lambda')
set(gcf, 'PaperPosition', [1 2 6 6])
ylabel('CVE')




