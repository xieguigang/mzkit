function [z, cve, h] = whitsmddw(x, y, w, lambda, d)
% Whittaker smoother with divided differences and weights
% Input:
%   x:      data series of sampling positions (must be increasing)
%   y:      data series, assumed to be sampled at equal intervals
%   w:      weights
%   lambda: smoothing parameter; large lambda gives smoother result
%   d:      order of differences (default = 2)
% Output:
%   z:      smoothed series
%   cve:    RMS leave-one-out prediction error
%   h:      diagonal of hat matrix
%
% Remark: the computation of the hat diagonal for m > 100 is experimental;
% with many missing observation it may fail.
%
% Paul Eilers, 2003

% Default order of differences
if nargin < 3
   d = 2;
end

% Smoothing
m = length(y);
E = speye(m);
D = ddmat(x, d);
W = spdiags(w, 0, m, m);
C = chol(W + lambda * D' * D);
z = C \ (C' \ (w .*y));

% Computation of hat diagonal and cross-validation
if nargout > 1
   if m <= 100    % Exact hat diagonal
      W = diag(w);
      H = (W + lambda * D' * D) \ W;
      h = diag(H);
   else           % Map to diag(H) for n = 100
      n = 100;
      E1 = speye(n);
      g = round(((1:n) - 1) * (m - 1) / (n - 1) + 1);
      D1 = ddmat(x(g), d);
      W1 = diag(w(g));
      lambda1 = lambda * (n / m) ^ (2 * d);
      H1 = inv(W1 + lambda1 * D1' * D1);
      h1 = diag(H1);
      u = zeros(m, 1);
      k = floor(m / 2);
      k1 = floor(n / 2);
      u(k) = 1;
      v = C \ (C' \ u);
      hk = v(k);
      f = round(((1:m)' - 1) * (n - 1)/ (m - 1) + 1);
      h = w .* h1(f) * v(k) / h1(k1);
   end
   r = (y - z) ./ (1 - h);
   cve = sqrt(r' * (w .* r) / sum(w));
end



