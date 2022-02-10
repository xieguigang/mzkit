function z = difsmw(y, lambda, w, d)
% Weighted smoothing with a finite difference penalty
% y:      signal to be smoothed
% lambda: smoothing parameter 
% w:      weights (use0 zeros for missing values)
% d:      order of differences in penalty (generally 2)

% Paul Eilers, 2002

m = length(y);
W = spdiags(w, 0, m, m);
D = diff(speye(m), d);
C = chol(W + lambda * D' * D);
z = C \ (C' \ (w .* y));

