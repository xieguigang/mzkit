function [z, cve, h] = whitsmdd(x, y, lambda, d)
% Whittaker smoother with divided differences (arbitrary spacing of x)
% Input:
%   x:      data series of sampling positions (must be increasing)
%   y:      data series, assumed to be sampled at equal intervals
%   lambda: smoothing parameter; large lambda gives smoother result
%   d:      order of differences (default = 2)
% Output:
%   z:      smoothed series
%   cve:    RMS leave-one-out prediction error
%   h:      diagonal of hat matrix
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
C = chol(E + lambda * D' * D);
z = C \ (C' \ y);

% Computation of hat diagonal and cross-validation
if nargout > 1
   if m <= 100    % Exact hat diagonal
      H = inv(E + lambda * D' * D);
      h = diag(h);
   else           % Map to diag(H) for n = 100
      n = 100;
      E1 = speye(n);
      g = round(((1:n) - 1) * (m - 1) / (n - 1) + 1);
      D1 = ddmat(x(g), d);
      lambda1 = lambda * (n / m) ^ (2 * d);
      H1 = inv(E1 + lambda1 * D1' * D1);
      h1 = diag(H1);
      u = zeros(m, 1);
      k = floor(m / 2);
      k1 = floor(n / 2);
      u(k) = 1;
      v = C \ (C' \ u);
      hk = v(k);
      f = round(((1:m)' - 1) * (n - 1)/ (m - 1) + 1);
      h = h1(f) * v(k) / h1(k1);
   end
   r = (y - z) ./ (1 - h);
   cve = sqrt(r' * r / m);
end



