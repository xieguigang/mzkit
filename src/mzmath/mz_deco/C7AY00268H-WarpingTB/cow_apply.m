function [Xw] = cow_apply(X,Warping)
% Apply Correlation Optimized Warping path on a data matrix
% Thomas Skov / Frans van den Berg 061219
%
% in:  X (mP x nP) matrix with data for mP row vectors of length nP to be warped/corrected
%      Warping (1 x N x 2) or (mP x nP) interpolation segment starting points (in "nP"
%          units) after warping (first slab) and before warping (second slab)
%          (difference of the two = alignment by repositioning segment
%          boundaries; useful for comparing correction in different/new objects/samples)
%          NOTE: if only one warping path is given this will be applied to all objects/samples in "X".
%
% out: Xw (mP x nP) warped/corrected data matrix
%
% Reference: Correlation optimized warping and dynamic time warping as preprocessing methods for chromatographic Data
%            Giorgio Tomasi, Frans van den Berg and Claus Andersson, Journal of Chemometrics 18(2004)231-241
%
% Authors: 
% Thomas Skov / Frans van den Berg
% Royal Agricultural and Veterinary University - Department of Food Science
% Quality and Technology - Spectroscopy and Chemometrics group - Denmark
% email: thsk@kvl.dk / fb@kvl.dk - www.models.kvl.dk

if (nargin < 2)
    help cow_apply
    return;
end
[nX,pX] = size(X);
[nW,pW,oW] = size(Warping);
if (oW~=2)
    error('ERROR: "Warping" must have two slabs');
end
if (nX~=nW) & (nW~=1)
    error('ERROR: "Warping" must have second dimention 1 or equal to the number of samples in "X"');
end

for i_seg = 1:pW-1
    indT = Warping(1,i_seg,2):Warping(1,i_seg + 1,2);
    lenT = Warping(1,i_seg + 1,2) - Warping(1,i_seg,2);
    if (nW == 1)
        for i_sam = 1:nX
            indX = Warping(1,i_seg):Warping(1,i_seg + 1);
            lenX = Warping(1,i_seg + 1) - Warping(1,i_seg);
            Xw(i_sam,indT) = interp1q(indX' - Warping(1,i_seg) + 1,X(i_sam,indX)',(0:lenT)'/lenT * lenX + 1)';
        end
    else
        for i_sam = 1:nX
            indX = Warping(i_sam,i_seg):Warping(i_sam,i_seg + 1);
            lenX = Warping(i_sam,i_seg + 1) - Warping(i_sam,i_seg);
            Xw(i_sam,indT) = interp1q(indX' - Warping(i_sam,i_seg) + 1,X(i_sam,indX)',(0:lenT)'/lenT * lenX + 1)';
        end
    end
end