% Demonstration of smoothing with uneaually spaced samples
% Optimal smoothing by cross-validation
%
% Paul Eilers, 2003

% Simulate data
m = 500;
rand('seed', pi)
randn('seed', pi)
x = cumsum(rand(m, 1) + 0.1);
x = x / max(x);
y = sin(10 * x) + randn(m, 1) * 0.3;
w = rand(m, 1) < 0.9;

% Smooth for series of lambdas
lambdas = 10 .^ (-6:0.2:0);
cvs = [];
for lambda = lambdas
   [z cv] = whitsmddw(x, y, w, lambda, 2);
   cvs = [cvs cv];
end

% Choose optimal lambda
[cm ci] = min(cvs);
lambda = lambdas(ci);
[z cv h] = whitsmddw(x,y,w, lambda, 2);

% Plot data and smooth
subplot(2, 1, 1);
f = find(w == 0);
yn = y;
yn(f) = NaN;
plot(x, [z-1 yn z] )    % Downward shift for visibility
title('Data and optimal smooth')
xlabel('Channel')
ylabel('Signal strength')

% Plot CV profile
subplot(2, 1, 2)
semilogx(lambdas, cvs)
title('Cross-validation error')
xlabel('\lambda')
ylabel('CVE')




