function [ref,refs,N] = ref_select(X,varlabels,options)
% function [ref,refs,N] = ref_select(X,varlabels,options);
% Assists in select a reference vector for warping
% Thomas Skov / Frans van den Berg 060515 (FvdB)
%
% in:  X (n x m) matrix - objects x variables - with data for warping/correction
%      varlabels (1 x m) Variable labels for plotting
%      options (1 x 2) 1 : reference selection
%                           0 - interactive (all options below - plotting is on)
%                           1 - mean signal vector
%                           2 - median signal vector
%                           3 - bi-weighted mean (iterative, slow, but robust)
%                           4 - maximum signal vector
%                           5 - maximum cumulative product of correlation coefficients
%                      2 : plotting of the selection
%                      default [0 1] (interactive) type "ref_select" for more details
% 
% out: ref (1 x m) target/reference vector selected
%      refs (5 x m) target/reference vectors from all five methods
%      N (1x1) if options(1) == 5 --> index in "X" of "ref" selected,
%           otherwise []
%
% Authors: 
% Thomas Skov / Frans van den Berg
% Royal Agricultural and Veterinary University - Department of Food Science
% Quality and Technology - Spectroscopy and Chemometrics group - Denmark
% email: thsk@kvl.dk / fb@kvl.dk - www.models.kvl.dk

if (nargin < 1)
    help ref_select;
    say_what;
    return;
end
[Xn,Xm] = size(X);
if (nargin == 1)
    varlabels = [];
    options = [0 1];
elseif (nargin == 2)
    options = [0 1];
end
if isempty(varlabels)
    varlabels = 1:Xm;
end
if (Xm ~= length(varlabels))
    error('ERROR: number of entries in "variables" and number of columns in "X" must be the same');
end
if any(isnan(X))
    error('ERROR: function "ref_select" can not handle missing values');
end
N = [];

if (options(1) == 0)
    interactive = 1;
    close all
else
    interactive = 0;
end

if (options(1) == 1) || interactive
    refs(1,:) = mean(X,1);
    if options(2) || interactive
        figure
        plot(varlabels,X,'b',varlabels,refs(1,:),'g');
        grid; title('Reference (green) = mean signal (criterion #1)');
    end
end

if (options(1) == 2) || interactive
    refs(2,:) = median(X,1);
    if options(2) || interactive
        figure
        plot(varlabels,X,'b',varlabels,refs(2,:),'g');
        grid; title('Reference (green) = median signal (criterion #2)');
    end
end

if (options(1) == 3) || interactive
    for a=1:Xm
        refs(3,a) = biwmean(X(:,a));
    end
    if options(2) || interactive
        figure
        plot(varlabels,X,'b',varlabels,refs(3,:),'g');
        grid; title('Reference (green) = bi-weighted mean signal (criterion #3)');
    end
end

if (options(1) == 4) || interactive
    refs(4,:) = max(X,[],1);
    if options(2) || interactive
        figure
        plot(varlabels,X,'b',varlabels,refs(4,:),'g');
        grid; title('Reference (green) = maximum signal (criterion #4)');
    end
end

if (options(1) == 5) || interactive
    R = eye(Xn);
    for a=1:Xn-1
        for b=2:Xn
            xx1 = X(a,:) - sum(X(a,:))/Xm;
            xx2 = X(b,:) - sum(X(b,:))/Xm;
            R(a,b) = (xx1*xx2'/(norm(xx1)*norm(xx2)))^2;
            R(b,a) = R(a,b);
        end
    end
    Rcp = prod(R,2);
    [dummy,N] = max(Rcp);
    refs(5,:) = X(N,:);
    if options(2) || interactive
        figure
        semilogy(1:Xn,Rcp,N,Rcp(N),'og','LineWidth',2);
        grid; title(['Reference (green) = maximum cumulative product of corr. coefs. Object #' num2str(N)]);
        xlabel('Object (#)');
        figure
        plot(varlabels,X,'b',varlabels,refs(5,:),'g');
        grid; title('Reference (green) = maximum cumulative product of correlation coefficients (criterion #5)');
    end
end

if interactive
    figure
    plot(varlabels,refs);
    grid; title('References'); legend('#1','#2','#3','#4','#5');
    choice = 1;
    while choice
        options(1) = input('Which reference method (1-5)? :');
        if (options(1) < 1) || (options(1) > 5)
            disp('Incorrect choice!')
        else
            choice = 0;
        end
    end
end
ref = refs(options(1),:);

% Internal functions
function biwm = biwmean(x)
nx = length(x);
niqr = round(nx*0.25);
sx = sort(x);
medianx = median(x);
iqr = sx(nx-niqr) - sx(niqr);
if (2*iqr < eps)
    biwm = medianx;
    return
end
zx = (x - medianx)/(3*iqr);
biw = (1-zx.^2).^2;
biw(abs(zx)>1) = 0;
biwm = sum(biw.*x)/sum(biw);
oldbiwm = medianx + eps;
iter = 0;
biwmiter(iter+1) = biwm;
while ((oldbiwm - biwm)^2/oldbiwm^2) > 1e-8 && (iter <= 100)
    iter = iter + 1;
    zx = (x - biwm)/(3*iqr);
    biw = (1-zx.^2).^2;
    biw(abs(zx)>1) = 0;
    oldbiwm = biwm;
    biwm = sum(biw.*x)/sum(biw);
    biwmiter(iter+1) = biwm;
end

function say_what
disp('Explanation "reference selection" (options(1)):');
disp('0 - All the methods below are computed and shown, and the user can select her/his preferred method.');
disp('1 - The mean vector from matrix "X" is used as reference.');
disp('    Usually this results in a little broader/"wobbly" signal compared to the individual')
disp('    measurements due to the shifting.');
disp('2 - The median vector from matrix "X" is used as reference.');
disp('    Usually this is a little "sharper" then the choice 1 (mean).');
disp('3 - The so-called bi-weighted mean vector from matrix "X" is used as reference.');
disp('    This is based on a robust estimator somewhere between 1 (mean) and 2 (median).');
disp('4 - The reference vector is composed of the maximum value in each variable in matrix "X".');
disp('    This is a strange (and very broad) reference that is sometimes useful to see in which way the');
disp('    alignment correction "wants to go".');
disp('5 - The object from matrix "X" with the maximum cumulative product of correlation coefficients with');
disp('    all other objects (notice that this is the only criterion selecting a real sample).');
disp('    Each object get a score, e.g. score(1) = r(1,2)*r(1,3)*... , where r(1,2) is the (squared) correlation');
disp('    coefficient between object 1 and 2, etc. Object with the highest score matches best with all others,');
disp('    and is thus selected as reference');
disp(' ');
