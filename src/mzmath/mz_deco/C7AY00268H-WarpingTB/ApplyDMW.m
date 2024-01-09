function [YWarped,WarpPath] = ApplyDMW(Y,RefPos,PadLen,Span,MaxStep,Band)
% FvdB 050104
% [YWarped,WarpPath] = ApplyDMW(Y,RefPos,PadLen,Span,MaxStep,Band)
%
% Y       : matrix of spectra (one row = one spectrum)
% RefPos  : position of the reference sample (i.e. row number)
% PadLen  : initial and final padding in points ([0 0] is the default)
% Span    : length of the "transition rule" (the number of diagonal steps allowed is
%           equal to Span - MaxStep). Default: 20
% MaxStep : max n. of consecutive horizontal or vertical transitions in the warping path
%           Default: 1
% Band    : band width in percentage (Default is given by the max width of the lozenge 
%           associated with the slope constraints)
%
% YWarped : warped spectra (including reference)
% WarpPath: structure with the warping Paths and sample Index in "Y" (NOT including reference)
%
% Reference: Correlation optimized warping and dynamic time warping as preprocessing methods for chromatographic Data
%            Giorgio Tomasi, Frans van den Berg and Claus Andersson, Journal of Chemometrics 18(2004)231-241
%
% Author: 
% Giorgio Tomasi 
% Royal Agricultural and Veterinary University 
% MLI, LMT, Chemometrics group 
% Rolighedsvej 30 
% DK-1958 Frederiksberg C 
% Danmark 

if ~exist('PadLen','var') || isempty(PadLen)
    PadLen = [0,0];
end
if ~exist('Span','var') || isempty(Span)
    Span = 20;
end
if ~exist('MaxStep','var') || isempty(MaxStep)
    MaxStep = 1;
end
if ~exist('Band','var') || isempty(Band)
    Band = fix((size(Y,2) + sum(PadLen))/Span) * 2 * MaxStep /(size(Y,2) + sum(PadLen));
end
[Yn,Ym] = size(Y);
Ind = {':',[ones(1,PadLen(1),1),1:size(Y,2),ones(1,PadLen(2)) * size(Y,2)]};
X   = Y(Ind{:});
ref = X(RefPos,:);
X = X([1:RefPos-1 RefPos+1:Yn],:);

Table = struct('type','SlopeCon','maxsteps',MaxStep,'span',Span,'check_maxsteps',false);
Check = struct('band',Band,'endpoints',[0 0 0 0],'in_points',false);
Dist  = struct('miss',false);

% Interchange the next two lines if instead of interpolation the "average" of the vertical transitions
% is desired
% Sync  = struct('synchronise',true,'interpolate','Average','type','linear');
Sync  = struct('synchronise',true,'interpolate','Type II','type','linear');

disp('DMW warping pre-processing');
fprintf('Table = T(%i,%i); Reference position = %i \n',Span * 2 - MaxStep,MaxStep,RefPos)
disp('Sample time (s)');
% Warp
ShowProgressBar = false;
[SyncSam,WarpPath.Path] = DMW(X,ref,ShowProgressBar,Dist,Table,Check,Sync);
WarpPath.Index = [1:RefPos-1 RefPos+1:Yn]';
disp(' ');

YWarped = cat(1,SyncSam{:});
YWarped = [YWarped(1:RefPos-1,:); ref; YWarped(RefPos:Yn-1,:)];
YWarped = YWarped(:,PadLen(1) + 1:min(size(YWarped,2),size(YWarped,2) - PadLen(2)));
