function f = savgol(x, y, n, d)
% Savitzky-Golay filter

% Prepare
m = length(x);
nr = floor(n / 2);
f = y;

% Filter left end
r = 1:n;
a = polyfit(x(r) - x(n), y(r), d);
f(r) = polyval(a, x(r) - x(n));

% Filter right end
r = (m - n + 1):m;
a = polyfit(x(r) - x(m - n), y(r), d);
f(r) = polyval(a, x(r) - x(m - n));

% Filter rest
for i = (nr + 1):(m-nr);
    r = (i - nr):(i + nr);
    a = polyfit(x(r) - x(i), y(r), d);
    f(i) = polyval(a, 0);
end
