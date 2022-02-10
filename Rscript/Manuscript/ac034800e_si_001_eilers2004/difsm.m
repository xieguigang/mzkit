function z = difsm(y, lambda, d)
% Smoothing with a finite difference penalty
% y:      signal to be smoothed
% lambda: smoothing parameter 
% d:      order of differences in penalty (generally 2)

% Paul Eilers, 2002

m = length(y);
E = speye(m);
D = diff(E, d);
C = chol(E + lambda * D' * D);
z = C \ (C' \ y);

