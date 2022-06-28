function D = ddmat(x, d)
% Compute divided differencing matrix of order d
%
% Input
%   x:  vector of sampling positions
%   d:  order of diffferences
% Output
%   D:  the matrix; D * Y gives divided differences of order d
%
% Paul Eilers, 2003

m = length(x);
if d == 0
    D = speye(m);
else
    dx = x((d + 1):m) - x(1:(m - d));
    V = spdiags(1 ./ dx, 0, m - d, m - d);
    D = V * diff(ddmat(x, d - 1));
end
