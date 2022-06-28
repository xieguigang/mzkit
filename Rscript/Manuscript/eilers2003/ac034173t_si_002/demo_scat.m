% Demo scatterplot smoother
%
% Paul Eilers, 2003

m = 200;
x = rand(m, 1);
y = sin(4 * x) + randn(m, 1) * 0.2;

cvs = [];
lambdas = 10 .^ (0:.2:6);
for lambda = lambdas
   [xgrid, ygrid, cv] = whitscat(x, y, lambda, 2);
   cvs = [cvs cv];
end

[cvm cvi] = min(cvs);
[xgrid, ygrid] = whitscat(x, y, lambdas(cvi), 2);

subplot(2, 1, 1);
plot(x, y, 'o', 'MarkerSize', 4)
line(xgrid, ygrid)
title('Simulated data and optimal smooth')

subplot(2, 1, 2);
semilogx(lambdas, cvs)
title('Cross-validation profile')
xlabel('\lambda')
ylabel('RMS CVE')

