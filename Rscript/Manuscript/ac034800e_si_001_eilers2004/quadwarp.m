function [w, sel, a] = quadwarp(x, y, astart)
% Parametric warping with quadratic polynomial
% Input
%   x: template signal
%   y: signal to fit
% Output
%   w: warping function
%   sel: part of y that can be fitted; y(sel) 
%   a: coefficients of constant, time and time squared
% Notes
%   1) time implicit: integer series starting at 1, in steps of 1
%   2) plot results as interpol(w, x), i.e. warped x, and y(sel)
%
% Paul Eilers, 2003

% Construct a basis for the warping function, use (t/m) ^ 2 for stability
m = max(length(x), length(y));
t = (1:m)';
B = [ones(m, 1), t, (t /m) .^ 2];
n = size(B, 2);

% Initialize the coefficients: w(t) = t, thus no warping
a = [0; 1; 0];
if nargin == 3
   a = astart;
end

% Do the iterative computations 
rms_old = 0;
for it = 1:40   
   
   % Compute warping function, w  
   w = B * a;
   
   % Warp template x, to give z = x(w(x)) and its derivative g
   [z sel g] = interpol(w, x);
    
   % Compute residuals and check convergence
   r = y(sel) - z;
   rms = sqrt(r' * r / m);
   drms = abs((rms - rms_old) / (rms + 1e-10));
   fprintf('%3d%12.1e\n', [it drms])
   if drms < 1e-6
      break
   end
   rms_old = rms;
   
   % Improve coeffcients with linear regression
   G = repmat(g, 1, n);
   Q = G .* B(sel, :);
   da = Q \ r;
   a = a + da;
   
end

% Transform quadratic coeffcient back to real time
a(3) = a(3) / m ^ 2;
