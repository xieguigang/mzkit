function [Warping,XWarped,Diagnos] = cow(T,X,Seg,Slack,Options)
% function [Warping,XWarped,Diagnos] = cow(T,X,Seg,Slack,Options);
% Correlation Optimized Warping function with linear interpolation
% Giorgio Tomasi / Frans van den Berg 070821 (GT)
%
% Thomas Skov 061219 - line 277 changed to work in MATLAB version 6.5
%
% in:  T (1 x nt) target vector
%      X (mP x nP) matrix with data for mP row vectors of length nP to be warped/corrected
%      Seg (1 x 1) segment length; number of segments N = floor(nP/m)
%       or (2 x N+1) matrix with segment (pre-determined) boundary-points
%                    first row = index in "xt", must start with 1 and end with "nt"
%                    second row = index in "xP", must start with 1 and end with "nP"
%      Slack (1 x 1) 'slack' - maximum range or degree of warping in segment length "m"
%      Options (1 x 5) 1 : triggers plot and progress-text (note: only last row/object in "xP" is plotted)
%                      2 : correlation power (minimum 1th power, maximum is 4th power)
%                      3 : force equal segment lengths in "xt" and "xP" instead of filling up "xt" with N boundary-points
%                          (notice that different number of boundaries in "xt" and "xP" will generate an error)
%                      4 : fix maximum correction to + or - options(4) points from the diagonal
%                      5 : save in "diagnos" the table with the optimal values of loss function and predecessor (memory
%                          consuming for large problems - on how to read the tables are in the m-file
%              default [0 1 0 0 0] (no plot; power 1; no forced equal segment lengths; no band constraints; no Table in "diagnos")
% 
% out: Warping (mP x N x 2) interpolation segment starting points (in "nP"
%          units) after warping (first slab) and before warping (second slab)
%          (difference of the two = alignment by repositioning segment
%          boundaries; useful for comparing correction in different/new objects/samples)
%      XWarped (mP x nt) corrected vectors (from "xP" warped to mach "xt")
%      Diagnos (struct) warping diagnostics: options, segment, slack, 
%          index in target ("xt", "warping" is shift compared to this) and sample ("xP"), search range in "xP", computation time
%          (note: diagnostics are only saved for one - the last - signal in "xP")
%
% based on: Niels-Peter Vest Nielsen, Jens Micheal Carstensen and Jørn Smedegaard 'Aligning of singel and multiple
%           wavelength chromatographic profiles for chemometric data analysis using correlation optimised warping' 
%           J. Chrom. A 805(1998)17-35
%
% Reference: Correlation optimized warping and dynamic time warping as preprocessing methods for chromatographic Data
%            Giorgio Tomasi, Frans van den Berg and Claus Andersson, Journal of Chemometrics 18(2004)231-241
%
% Authors: 
% Giorgio Tomasi / Frans van den Berg
% Royal Agricultural and Veterinary University - Department of Food Science
% Quality and Technology - Spectroscopy and Chemometrics group - Denmark
% email: gt@kvl.dk / fb@kvl.dk - www.models.kvl.dk

%% Check Input values
if (nargin < 4)
   help cow;
   return;
end
if (nargin < 5)
   Options = [0 1 0 0 0];
end
if (length(Options) < 5)
   Options_def = [0 1 0 0 0];
   Options(length(Options) + 1:5) = Options_def(length(Options) + 1:5);
end
if (Options(2) < 1) || (Options(2) > 4)
   error('ERROR: "Options(2)" (correlation power) must be in the range 1:4');
end
if any(isnan(T)) || any(isnan(X(:)))
   error('ERROR: function "cow" can not handle missing values');
end

%% Initialise
[nX,pX] = size(X);         % nX     : number of signals that are to be aligned
                           % pX     : number of data points in each signal
pT      = size(T,2);       % pT     : number of data points in the target
XWarped = zeros(nX,pT);    % XWarped: initialise matrix of warped signals
Time    = zeros(1,1);      % Time   : processing time
%% Initialise segments
Seg        = round(Seg);      % Only integers are currently allowed as segment boundaries
Pred_Bound = length(Seg) > 1; % True if segment boundaries are predefined
if Pred_Bound 
   if not(isequal(Seg(:,1),ones(2,1)) & isequal(Seg(:,end),[pT,pX]'))
      error('End points must be equal to 1 and to the length of the pattern/target');
   end
   LenSeg = diff(Seg,1,2);    % LenSeg(1,:): Length of the segments in the - 1
   if not(all(LenSeg >= 2))
      error('Segments must contain at least two points');
   end
   nSeg   = size(LenSeg,2);   % nSeg: number of segments
else
   if Seg > min(pX,pT)
      error('Segment length is larger than length of the signal');
   end
   if Options(3) % Segments in the signals can have different length from those in the target
      nSeg             = floor((pT - 1)/Seg);
      LenSeg(1,1:nSeg) = floor((pT - 1)/nSeg);
      LenSeg(2,1:nSeg) = floor((pX - 1)/nSeg);
      fprintf('\n Segment length adjusted to best cover the remainders')
   else
      nSeg               = floor((pT - 1) / (Seg - 1));
      LenSeg(1:2,1:nSeg) = Seg - 1;
      if floor((pX - 1) / (Seg - 1)) ~= nSeg
         error('For non-fixed segment lengths the target and the signal do not have the same number of segments (try Options(3))');
      end
   end
   temp = rem(pT - 1,LenSeg(1,1)); % The remainders are attached to the last segment in the target and in the reference
   if (temp > 0)
      LenSeg(1,nSeg) = LenSeg(1,nSeg) + temp;
      if Options(1)
         fprintf('\n Segments: %i points x %i segments + %i (target)',LenSeg(1,1) + 1,nSeg - 1,LenSeg(1,end) + 1);
      end
   else
      if Options(1)
         fprintf('\n Segments: %i points x %i segments (target)',LenSeg(2,1) + 1,nSeg);
      end
   end
   temp = rem(pX - 1,LenSeg(2,1));
   if temp > 0
      LenSeg(2,nSeg) = LenSeg(2,nSeg) + temp;
      if Options(1)
         fprintf('\n           %i points x %i segments + %i (signals)\n',LenSeg(2,1) + 1,nSeg - 1,LenSeg(2,end) + 1);
      end
   else 
      if Options(1)
         fprintf('\n           %i points x %i segments (signals)\n',LenSeg(2,1) + 1,nSeg);
      end
   end
end
if any(LenSeg(:) <= Slack + 2) % Two points are the minimum required for linear interpolation
   error('The slack cannot be larger than the length of the segments');
end

bT      = cumsum([1,LenSeg(1,:)]);
bP      = cumsum([1,LenSeg(2,:)]);
Warping = zeros(nX,nSeg + 1);

%% Check slack
if length(Slack) > 1 % Different slacks for the segment boundaries will be implemented
   if size(Slack,2) <= nSeg
      error('The number of slack parameters is not equal to the number of optimised segments');
   end
   fprintf('\n Multiple slacks have not been implemented yet')
   return
end
Slacks_vec  = -Slack:Slack;                     % All possible slacks for a segment boundary
%% Set feasible points for boundaries
Bounds      = ones(2,nSeg + 1);
% Slope Constraints
offs        = (Slack * [-1,1]') * (0:nSeg);
Bounds_a    = bP(ones(2,1),1:nSeg + 1) + offs;
Bounds_b    = bP(ones(2,1),1:nSeg + 1) + offs(:,nSeg + 1:-1:1);
Bounds(1,:) = max(Bounds_a(1,:),Bounds_b(1,:));
Bounds(2,:) = min(Bounds_a(2,:),Bounds_b(2,:));
% Band Constraints
if Options(4)
   if abs(pT - pX) > Options(4)
      error('The band is too narrow and proper correction is not possible');
   end
   Bounds(1,:) = max(Bounds(1,:),max(0,pX/pT * bT - Options(4)));
   Bounds(2,:) = min(Bounds(2,:),min(pX,pX/pT * bT + Options(4)));
   if any(diff(Bounds < 0))
      error('The band is incompatible with the fixed boundaries');
   end
end

%% Calculate first derivatives for interpolation
Xdiff = diff(X,1,2);

%% Calculate coefficients and indexes for interpolation
Int_Coeff = cell(nSeg,1);
Int_Index = Int_Coeff;
if ~Pred_Bound
   [A,B]                             = InterpCoeff(LenSeg(1,1) + 1,LenSeg(2,1) + Slacks_vec + 1,Slacks_vec); 
   [Int_Coeff{1:nSeg - 1}]           = deal(A);
   [Int_Index{1:nSeg - 1}]           = deal(B);
   [Int_Coeff{nSeg},Int_Index{nSeg}] = InterpCoeff(LenSeg(1,nSeg) + 1,LenSeg(2,nSeg) + Slacks_vec + 1,Slacks_vec);   
else
   for i_seg = 1:nSeg
      [Int_Coeff{i_seg},Int_Index{i_seg}] = InterpCoeff(LenSeg(1,i_seg) + 1,LenSeg(2,i_seg) + Slacks_vec + 1,Slacks_vec);
   end
end

%% Dynamic Programming Section
Table_Index    = cumsum([0,diff(Bounds) + 1]);       % Indexes for the first node (boundary point) of each segment in Table
Table          = zeros(3,Table_Index(nSeg + 2),nX);  % Table: each column refer to a node
                                                     %        (1,i) position of the boundary point in the signal
                                                     %        (2,i) optimal
                                                     %        value of the loss function up to node (i)
                                                     %        (3,i) pointer to optimal preceding node (in Table)
Table(2,2:end,1:nX) = -Inf;                          % All loss function values apart from node (1) are set to -Inf
for i_seg = 1:nSeg + 1                               % Initialise Table
   v                                                        = (Bounds(1,i_seg):Bounds(2,i_seg))';
   Table(1,Table_Index(i_seg) + 1:Table_Index(i_seg + 1),:) = v(:,ones(nX,1));
end
warning('off','MATLAB:divideByZero')              % To avoid warning if division for zero occurs
   
tic
% Forward phase
for i_seg = 1:nSeg                             % Loop over segments
   a             = Slacks_vec + LenSeg(2,i_seg);               % a,b,c: auxiliary values that depend only on segment number and not node
   b             = Table_Index(i_seg) + 1 - Bounds(1,i_seg);
   c             = LenSeg(1,i_seg) + 1;
   Count         = 1;                                          % Counter for local table for segment i_seg
   Node_Z        = Table_Index(i_seg + 2);                     % Last node for segment i_seg
   Node_A        = Table_Index(i_seg + 1) + 1;                 % First node for segment i_seg
   Bound_k_Table = zeros(2,Node_Z - Node_A + 1,nX);            % Initialise local table for boundary

   Int_Index_Seg = Int_Index{i_seg}' - (LenSeg(2,i_seg) + 1);  % Indexes for interpolation of segment i_seg
   Int_Coeff_Seg = Int_Coeff{i_seg}';                          % Coefficients for interpolation of segment i_seg

   TSeg          = T(bT(i_seg):bT(i_seg + 1));                 % Segment i_seg of target T
   TSeg_centred  = TSeg - sum(TSeg)/size(TSeg,2);              % Centred TSeg (for correlation coefficients)
   Norm_TSeg_cen = norm(TSeg_centred);                         % (n - 1) * standard deviation of TSeg

   for i_node = Node_A:Node_Z                                  % Loop over nodes (i.e. possible boundary positions) for segment i_seg
      Prec_Nodes         = Table(1,i_node) - a;                                           % Possible predecessors given the allowed segment lengths
      Allowed_Arcs       = Prec_Nodes >= Bounds(1,i_seg) & Prec_Nodes <= Bounds(2,i_seg); % Arcs allowed by local and global constraints
      Nodes_TablePointer = b + Prec_Nodes(Allowed_Arcs);                                  % Pointer to predecessors in Table
      N_AA               = sum(Allowed_Arcs);                                             % Number of allowed arcs
      if N_AA % Sometimes boundaries are ineffective and few nodes are allowed that cannot be reached
         % It has to be further investigated
         Index_Node                    = Table(1,i_node) + Int_Index_Seg(:,Allowed_Arcs);                % Interpolation signal indexes for all the allowed arcs for node i_node
         Coeff_b                       = Int_Coeff_Seg(:,Allowed_Arcs);                                  % Interpolation coefficients for all the allowed arcs for node i_node
         Coeff_b                       = Coeff_b(:)';
         Coeff_b                       = Coeff_b(ones(nX,1),:);
         Xi_Seg                        = X(:,Index_Node);
         Xi_diff                       = Xdiff(:,Index_Node);
         Xi_Seg                        = reshape((Xi_Seg + Coeff_b .* Xi_diff)',c,N_AA * nX);            % Interpolate for all allowed predecessors
         Xi_Seg_mean                   = sum(Xi_Seg)/size(Xi_Seg,1);                                     % Means of the interpolated segments
         Norm_Xi_Seg_cen               = sqrt(sum(Xi_Seg.^2) - size(Xi_Seg,1) * Xi_Seg_mean.^2);         % Fast method for calculating the covariance of T and X (no centering of X is needed)
         CCs_Node                      = (TSeg_centred * Xi_Seg)./(Norm_TSeg_cen * Norm_Xi_Seg_cen);     % Correlation coefficients relative to all possible predecessors
         CCs_Node(~isfinite(CCs_Node)) = 0;                                                              % If standard deviation is zero, update is not chosen
         CCs_Node                      = reshape(CCs_Node,N_AA,nX);
         if Options(2) == 1
            Cost_Fun = reshape(Table(2,Nodes_TablePointer,:),N_AA,nX) + CCs_Node;                        % Optimal value of loss function from all predecessors
         else
            Cost_Fun = reshape(Table(2,Nodes_TablePointer,:),N_AA,nX) + CCs_Node.^Options(2);
         end
         [ind,pos]                = max(Cost_Fun,[],1);                                                  % Optimal value of loss function from all predecessors
         Bound_k_Table(1,Count,:) = ind;
         Bound_k_Table(2,Count,:) = Nodes_TablePointer(pos);                                             % Pointer to optimal predecessor
         Count                    = Count + 1;
      end
   end % i_node
   Table(2:3,Node_A:Node_Z,:) = Bound_k_Table;       % Update general table (it turned out to be faster than using Table directly in the loop over nodes
end % i_seg
Time = toc;

for i_sam = 1:nX                                  % Loop over samples/signals
   % Backward phase
   Pointer                 = size(Table,2);           % Backtrace optimal boundaries using the pointers in Table
   Warping(i_sam,nSeg + 1) = pX;
   for i_bound = nSeg:-1:1
      Pointer                = Table(3,Pointer,i_sam);
      Warping(i_sam,i_bound) = Table(1,Pointer,i_sam);
   end
%    if Options(1)                                      % Some output if requested
%       fprintf('\n Sample %i: %g sec',i_sam,Time(i_sam));
%    end
end
Warping(:,:,2) = bT(ones(nX,1),:);
warning('on','MATLAB:divideByZero')
% fprintf('\n')

%% Output
if (nargout > 1) || Options(1)   % Reconstruct aligned signals
   for i_seg = 1:nSeg
      indT = bT(i_seg):bT(i_seg + 1);
      lenT = bT(i_seg + 1) - bT(i_seg);
      for i_sam = 1:nX
         indX                = Warping(i_sam,i_seg):Warping(i_sam,i_seg + 1);
         lenX                = Warping(i_sam,i_seg + 1) - Warping(i_sam,i_seg);
         % NB the right handside expression must be transposed to fit MATLAB version 6.5
         XWarped(i_sam,indT) = interp1q(indX' - Warping(i_sam,i_seg) + 1,X(i_sam,indX)',(0:lenT)'/lenT * lenX + 1)';
      end
   end
end
if (nargout > 2)    % Save some diagnostics if requested
   Diagnos = struct('indexP',bP,'indexT',bT,'Nsegments',nSeg,'options',Options,'rangeP',Bounds',...
      'segment_length',LenSeg,'slack',Slack,'table',[],'time',Time);
   if Options(5)
      Diagnos.table = Table;
   end
end

%% Plot
if Options(1)
   figure
   minmaxaxis = [1 max([pT pX]) min([T X(nX,:)]) max([T X(nX,:)])] ;
   subplot(2,1,1);
   plot(1:pT,T,'b',bT,T(bT),'.b',1:pX,X(nX,:),'g',bP,X(nX,bP),'.g');
   hold on
   for a = 2:length(Warping(nX,:,1))
      plot([bT(a) Warping(nX,a,1)],[T(Warping(nX,a,2)) T(Warping(nX,a,2))],'r');
      if (Warping(nX,a,2) > Warping(nX,a,1))
         plot(Warping(nX,a,2),T(Warping(nX,a,2)),'>r');
      else
         plot(Warping(nX,a,2),T(Warping(nX,a,2)),'<r');
      end
   end
   hold off
   axis(minmaxaxis)
   grid
   title(['COW reference = blue, Sample ' num2str(nX) '(/' num2str(nX) ') = green, Segment-boundary movement = red']);
   subplot(2,1,2);
   plot(1:pT,T,'b',1:pT,XWarped(nX,:),'g');
   grid;
   axis(minmaxaxis);
   title('Warped sample')
end

%% Function to calculate coefficients for interpolation
function [Coeff,Index] = InterpCoeff(n,nprime,offs)
p     = length(nprime);
q     = n - 1;
Coeff = zeros(p,n);
Index = zeros(p,n);
for i_p = 1:p
   pp                  = 1:nprime(i_p);
   p                   = (0:q) * (nprime(i_p) - 1)/q + 1;
   [ignore,k]          = histc(p,pp);
   k(p < 1)            = 1;
   k(p >= nprime(i_p)) = nprime(i_p) - 1;
   Coeff(i_p,:)        = (p - pp(k));
   Index(i_p,:)        = k - offs(i_p);
end