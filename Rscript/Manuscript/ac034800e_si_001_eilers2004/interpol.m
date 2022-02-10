function [f, s, g] = interpol(t, y);
% Linear interpolation 
% Implicit assumption: y given on grid 1:m
% t: points at which to compute interpolated values
% y: signal to be interpolated
% f: interpolated signal
% s: points of t with 1 <= t & t <= m
% g: gradient of y at points t

% Paul Eilers, 2002

m = length(y);
s = find(1 < t & t < m);
ti = floor(t(s));
tr = t(s) - ti;
g = y(ti + 1) - y(ti);
f = y(ti) + tr .* g;

