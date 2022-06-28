function [xgrid, ygrid, cv] = scatsm(x, y, lambda, d, n)
% Smoothing of a scatterplot, based on Whittaker smoother
% 
% Input
%   x:      data series x
%   y:      data series y
%   lambda: smoothing parameter
%   d:      order of difference in penalty (usually 2 or 3)
%   n:      number of bins to use (optional, default = 100)
% Output
%   xgrid:  grid on which smooth curve is computed
%   ygrid:  computed smooth curve on grid
%   cv:     RMS crosss-validation error;
%
% Paul Eilers, 2003

if nargin < 5
   n = 100;
end

% Compute bin index
m = length(x);
xmin = min(x);
xmax = max(x);
dx = (xmax - xmin) / (n - 1e-6);
bin = floor(((x - xmin) / dx) + 1);

% Do penalized regression
w = full(sparse(bin, 1, 1));
s = full(sparse(bin, 1, y));
D = diff(eye(n), d);
ygrid = (diag(w) + lambda * D' * D) \ s;
xgrid = ((1:n)' - 0.5) * dx + xmin;

% Cross-validation
if nargout > 2
   H = (diag(w) + lambda * D' * D) \ diag(w);
   u = s ./ (w + 1e-9);
   r = (u - ygrid) ./ (1 - diag(H));
   cv = sqrt(r' * (w .* r) / n);
end



