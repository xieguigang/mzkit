% Demonstration of smoothing with unequally spaced samples
% Optimal smoothing by cross-validation
%
% Paul Eilers, 2003

% Simulate data
m = 500;
rand('seed', pi)
randn('seed', pi)
x = (1:m)';
y = sin(10 * x / m) + randn(m, 1) * 0.3;

% Smooth for series of lambdas
lambdas = 10 .^ (0:0.5:10);
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
plot( [z-1 y z] )    % Downward shift for visibility
title('Data and optimal smooth')
xlabel('Channel')
ylabel('Signal strength')

% Plot CV profile
subplot(2, 1, 2)
semilogx(lambdas, cvs)
title('Cross-validation error')
xlabel('\lambda')
ylabel('CVE')




