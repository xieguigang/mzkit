function [xW,R] = coshift(xT,xP,n,options)
% function [xW,R] = coshift(xT,xP,n,options)
% Correlation optimized shifting
% 060206 (FvdB)
%
% Simply shift a vector left-right, or a matrix left-right (row direction) and up-down (column direction) to get maximum
% correlation or matrix-correlation from the RV-coefficient, missing parts after shifting are filled with "closest"
% value.
%
% If the target ("xT") is a row-vector, and the sample ("xP") is a matrix, it is assumed that all the rows in the
% matrix have to be pre-processed individually.
%
% in:  xT (1 x mT) target vector or matrix (nT x mT)
%      xP (1 x mT) sample vector or matrix (nT x mT) to be aligned by correlation optimized shifting
%      xP (nP x mT) sample vectors to be aligned as a sample-set towards common target xT
%      n (1 x 1 or 1 x 2) maximum shift correction in data points in x/rows (first entry) and y/columns (first entry or second entry)
%      options (1 x 3) 1 : triggers plot (default = no)
%                      2 : use N-points X-step in grid-search (default = 1)
%                      3 : use N-points Y-step in grid-search (default = 1, if omitted options(2) is used)
%                       e.g. if n=[50 30] and options(2:3) = [5 3],
%                       grid-search is 1:5:50 1:3:30, followed by a spline interpolation to find the optimal shift
%                       Use e.g. n=[10 1] and options=[0 1 1] to shift only
%                       in x/row direction
%
% out: xW (1 x mT or nT x mT) shift corrected vector or matrix
%      R (2*n(1)+1 [one vector] or 2*n(1)+1 x 2*n(2)+1 [one matrix] or nP x 2*n(1)+1 [sample set]) correlation matrix for shifted sample
%
% loosely base on : V.G. van Mispelaar et.al. 'Quantitative analysis of target components by comprehensive two-dimensional gas chromatography'
%                   J. Chromatogr. A 1019(2003)15-29
%  
% "Expert users": alter "eval_vector" or "eval_matrix" (bottom of the function) to define other segment comparisons during alignment.
%
% Author:
% Frans van den Berg - Royal Agricultural and Veterinary University - Department of Food Science
% Quality and Technology - Spectroscopy and Chemometrics group - Denmark
% email: fb@kvl.dk - www.models.kvl.dk

if (nargin == 0)
    help coshift
    return
end
if (nargin < 3)
    error('ERROR: number of input arguments must be minimal three (target "xT", sample "xP" and shift-range "n")');
end
if (nargin == 3)
    options = [0 1 1];
end
if (length(options) == 1)
    options = [options 1 1];
elseif (length(options) == 2)
    options = [options options(2)];
end

[nT,mT] = size(xT);
[nP,mP] = size(xP);
if (mT ~= mP)
    error('ERROR: target "xT" and sample "xP" must be of compatible size (same vectors, same matrices or row + matrix of rows)');
end
if any(n < 0)
    error('ERROR: shift(s) "n" must be larger than zero');
end

D1 = 0;
if (nT == 1)
    D1 = 1;
end
if (length(n) == 1)
    n = [n n];
end

xW = zeros(nP,mP);
if D1
    Rindex = 1:options(2):2*n(1)+1;
    Rsmall = zeros(1,length(Rindex));
    R = zeros(nP,2*n(1)+1);
    for loop = 1:nP
        x = [ones(1,n(1))*xP(loop,1) xP(loop,:) ones(1,n(1))*xP(loop,end)];
        for a=1:length(Rindex)
            Rsmall(a) = eval_vector(xT,x(Rindex(a):Rindex(a)+mP-1));
        end
        R(loop,:) = interp1(Rindex,Rsmall,1:2*n(1)+1,'spline');
        [a,index] = max(R(loop,:));
        xW(loop,:) = x(index:index+mP-1);
        temp = -n(1):n(1);
        index = temp(index);
    end
else
    Rindexr = 1:options(2):2*n(1)+1;
    Rindexc = 1:options(3):2*n(2)+1;
    R = zeros(length(Rindexr),length(Rindexc));
    [nxP,mxP] = size(xP);
    x = zeros(nxP+2*n(1),mxP+2*n(2));
    x(n(1)+1:n(1)+nxP,n(2)+1:n(2)+mxP) = xP;
    x(n(1)+1:n(1)+nxP,1:n(2)) = xP(:,1)*ones(1,n(2));
    x(n(1)+1:n(1)+nxP,n(2)+1+mxP:n(2)+n(2)+mxP) = xP(:,end)*ones(1,n(2));
    x(1:n(1),n(2)+1:n(2)+mxP) = ones(n(1),1)*xP(1,:);
    x(n(1)+1+nxP:n(1)+n(1)+nxP,n(2)+1:n(2)+mxP) = ones(n(1),1)*xP(end,:);
    x(1:n(1),1:n(2)) = x(n(1)+1,1);
    x(n(1)+nxP+1:n(1)+nxP+n(1),1:n(2)) = x(n(1)+nxP,1);
    x(1:n(1),n(2)+mxP+1:n(2)+mxP+n(2)) = x(n(1)+1,end);
    x(n(1)+nxP+1:n(1)+nxP+n(1),n(2)+mxP+1:n(2)+mxP+n(2)) = x(n(1)+nxP,end);
    for a=1:length(Rindexr)
        for b=1:length(Rindexc)
            R(a,b) = eval_matrix(xT,x(Rindexr(a):Rindexr(a)+nxP-1,Rindexc(b):Rindexc(b)+mxP-1));
        end
    end
    if ~n(1)
        R = interp1(Rindexc,R,1:2*n(2)+1,'spline');
    elseif ~n(2)
        R = interp1(Rindexr,R,1:2*n(1)+1,'spline');
    else
        [gx,gy] = meshgrid(Rindexc,Rindexr);
        [gxi,gyi] = meshgrid(1:2*n(2)+1,1:2*n(1)+1);
        R = interp2(gx,gy,R,gxi,gyi,'spline');
    end
    if n(2) & ~n(1)
        [a,indexrows] = max(R);
        a = -n(2):n(2);
        indexrows = a(indexrows);
        indexcols = 0;
    elseif n(1) & ~n(2)
        [a,indexcols] = max(R);
        a = -n(1):n(1);
        indexcols = a(indexcols);
        indexrows = 0;
    else
        [a,indexrows] = max(max(R,[],1));
        [a,indexcols] = max(max(R,[],2));
        a = -n(1):n(1);
        indexrows = a(indexrows);
        a = -n(2):n(2);
        indexcols = a(indexcols);
    end
    xW = x(n(1)+1+indexcols:n(1)+nxP+indexcols,n(2)+1+indexrows:n(2)+mxP+indexrows);
end

if options(1)
    if D1
        subplot(3,1,1);
        plot(-n(1):n(1),R(end,:),'k'); grid
        if (nP == 1)
            legend('correlation');
        else
            legend(['correlation (last sample, total ' num2str(nP) ')']);
        end
        xlabel('shift');
        s1 = ['optimal shift found at ' num2str(index) ' data points'];
        s2 = ['(xT = target; xP = sample; xW = sample after correction)'];
        title({s1;s2});
        subplot(3,1,2);
        plot(1:mP,xT,'b',1:mP,xP(end,:),'g',1:mP,xW(end,:),'r'); grid
        legend('xT','xP','xW');
        subplot(3,1,3);
        plot(1:mP,xT-xP(end,:),'c',1:mP,xT-xW(end,:),'m'); grid
        legend('xT - xP','xT - xW');
        xlabel('index');
    else
        subplot(3,1,1);
        if ~n(2)
            plot(-n(1):n(1),R,'k'); grid
            s = ['shift (R), optimal ' num2str(indexcols) ' cols'];
        elseif ~n(1)
            plot(-n(2):n(2),R,'k'); grid
            s = ['shift (R), optimal ' num2str(indexrows) ' rows'];
        else
            mesh(-n(2):n(2),-n(1):n(1),R);
            s = ['shift (R), optimal ' num2str(indexrows) ' rows and ' num2str(indexcols) ' columns'];
        end
        title(s);
        subplot(3,1,2);
        mesh(1:mxP,1:nxP,xT-xP);
        title('xT - xP');
        subplot(3,1,3);
        mesh(1:mxP,1:nxP,xT-xW);
        title('xT - xW');
    end
end

function c = eval_vector(a,b);
% comparison function for vector segments
r = corrcoef(a,b);
c = r(1,2);
% c = 1/norm(a-b); 

function c = eval_matrix(a,b);
% comparison function for matrix segments
c = trace(a'*b)/sqrt(trace(a'*a)*trace(b'*b));
% c = 1/norm(a-b);
