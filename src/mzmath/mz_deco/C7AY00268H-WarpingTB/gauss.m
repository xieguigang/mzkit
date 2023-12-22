function Z = gauss(n,mu,sigma,options)
% function Z = gauss(n,mu,sigma,options)
% 030407 FvdB
% Computes Gaussian profiles.
%
% in: n (1 x 1) number of points in z
%     mu (objects x 1) first moments of gaussian distributions in units 'points'
%     sigma (objects x 1) second moment of gaussian distributions in units 'points'
%     options (1 x 1) trigger N(0,s)-noise addition of fraction  (default = 0 = no noise; e.g. 0.1 adds 10%-max-value noise) 
%
% out: Z (objects x n) data table with gaussian curves

if (nargin < 3)
    help gauss
    return
elseif (nargin == 3)
    options = 0;
end

nZ = length(mu);
if nZ ~= length(sigma)
    error('Error: number of entries for first (mu) and second moment (sigma) must be the same')
end

x = 1:n;
for a=1:nZ
   Z(a,:) = exp(-0.5*((x-mu(a))/sigma(a)).^2)./(sigma(a)*(2*pi)^0.5);
end

if options(1)
    nmax = max(Z(:))*options(1);
    Z = Z + randn(size(Z))*nmax;
end