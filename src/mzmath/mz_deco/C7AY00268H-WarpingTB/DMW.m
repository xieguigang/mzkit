function [SyncSam,WarpPath,SyncRef,CumDist,DistMap] = DMW(SamMat,Ref,Show,varargin);
% Dynamic Multi-way warping
% Function for Warping of n-way arrays
%
% [WarpSam,WarpPath,WarpRef] = DMW(Sam,Ref,Show,Distance,Table,Check,Sync);
%
% INPUTS
% Sam     : Sample matrix, time must be in the last mode (e.g. row vector for one dimensional data)
%           If size(Sample,1) > 1 each horizontal slab (rows if Sample is a matrix) is treated as a separate sample
% Ref     : Reference, time must be in the last mode (e.g. row vector for one dimensional data), size in the first mode must be equal to 1
% Show    : 1/0 "show/do not show" waiting bars
%
% Distance: structure with fields
%           'function': function computing distance
%                       D = distance(R,S,Distance)
%                       OUTPUT
%                       D: distance between R and S
%                       INPUTS
%                       R: reference element(s)
%                       S: sample element(s)
%                       Additional inputs may be passed as fields in the Distance structure
%
%                       Default: @Distance_eucl (internal), squared euclidean distance 
%
% Table   : structure with options for look-up table, fields
%           (all of them are optional, default values are used if nothing fields are not present)
%           'type': 'nocon'  - no constraints
%                   'smooth' - slope constraints
%                   'cow'    - COW-like constraints 
%           'maxsteps': max number of consecutive horizontal/vertical steps (slack for 'cow')
%           'span'    : necessary for type 'cow', segment length (Default: 25)
%           'function': (Optional) function handle (or name) for a function returning a look-up table in form of a cell vector
%                       T = LookupTable(Table);
%                       OUTPUT
%                       T: look-up table as a cell vector
%                          Each element is a legal transition
%                          One element is an n x 3 array, n being the number of substeps.
%                          Substeps are [i j w]: i = delta along x (reference)
%                                                j = delta along y (sample)
%                                                w = weight for the substep
%                      INPUT
%                      Table: structure (it can also contain additional inputs as fields)
%                      
%                      Default: @LookUpTable (internal); handles give types
%
% Check   : structure with the following fields (all of them are optional, default values are used if nothing fields are not present)
%           'band'     : width in % of the the band constraints (default: 15%)
%           'endpoints': not available,
%           'in_points': set to true if banded (e.g. for cow like constraints) feasible area; (default: false)
%           'function' : (Optional) function handle (or name) with the following input and outputs
%                        [feas,feasdist] = checkstep_gen(M,N,Table,Options)
%                         OUTPUTS
%                         feas    : M x N logical array identifying legal transition endpoints (true if feasible, false if not)
%                         feasdist: M x N logical array identifying points interested in a legal transition albeit maybe not legal endpoints themselves
%                                   (true if feasible, false if not)
%
%                         INPUTS
%                         M,N     : sample/reference time-points length
%                         Table   : look-up table in form of a cell vector (see above)
%                         Options : structure with fields:
%                                   'table'   : Table structure
%                                   'check'   : Check structure
%                                   'distance': Distance structure
%                                   'sync'    : Sync structure
%                         Additional inputs may be passed as fields in the Check structure
%
%                         Default: @checkstep_gen (internal), handles all the default tables
%
% Sync    : structure with fields
%           'synchronise': true if syncronise
%           'interpolate': 'Average', use average for synchronisation
%                          'Type II', use interpolation
%           'type'       : type of interpolation (default: 'linear') see interp1 for more
%
%           
% OUTPUTS   
% WarpSam : cell vector, i-th element is the i-th warped sample in Sample
% WarpPath: cell vector i-th element is the warping path for the i-th sample in Sample.
%           Each warping path is a s x 2 array
%           WarpPath{i}(:,1): sample warp path (NB reverse order!)
%           WarpPath{i}(:,2): reference warp path (NB reverse order!)
% WarpRef : cell vector, i-th element is the warped reference relative to the i-th Sample, if syncronisation is applied each element is empty
%
% Set up examples:
% No Constraints
%    Table = struct('type','nocon','maxsteps',0);
%    Check = struct('band',.05,'in_points',false);
%    Dist  = [];
%    Sync  = struct('synchronise',true,'interpolate','Average');
% 
% Slope constraints, max consecutive vertical/horizontal transition: 1 - band 5%
%    Table = struct('type','smooth','maxsteps',1);
%    Check = struct('band',.05,'in_points',false);
%    Dist  = [];
%    Sync  = struct('synchronise',true,'interpolate','Average','type','linear');
%
% COW like segment length: 50 - slack: 2 - band 5%
%    Table = struct('type','cow','maxsteps',2,'span',50);
%    Check = struct('band',.05,'in_points',true);
%    Dist  = [];
%    Sync  = struct('synchronise',true,'interpolate','Type II','type','linear');
%
%
% Author: 
% Giorgio Tomasi 
% Royal Agricultural and Veterinary University 
% MLI, LMT, Chemometrics group 
% Rolighedsvej 30 
% DK-1958 Frederiksberg C 
% Denmark 
% 

%INPUTS check
if isempty(varargin)
   varargin = {};
end
if isempty(Show)
   Show = true;
end
   
Options = DMWOptions(varargin{:});

%Some initial values
OptStep = [];

%To be general the warped dimension is the last
M        = size(SamMat,ndims(SamMat));         %Sample length
N        = size(Ref,ndims(Ref));         %Reference length
WarpPath = cell(size(SamMat,1),1);
SyncRef  = cell(size(SamMat,1),1);
SyncSam  = cell(size(SamMat,1),1);

%Create look-up table
Table                                                                  = feval(Options.table.function,Options.table);    
Options.check.table                                                    = Table;
[Tini,Tfin,Tcat,Tlen,Tcumstep,Thor,Tver,Tflat,Tsize,Min_m,Min_n,Table] = Table2Double(Table,Options);

%Fill-in distance table and feasibility table
%General indexing for multi-way data
if Show
   h2      = waitbar(0,'Creating feasibility map');
end
[FeasMap,FeasDist] = feval(Options.check.function,M,N,Table(1:Tlen),Options);
if ~FeasMap(1,1)

   M                  = M - min(find(any(FeasMap,2))) + 1;
   N                  = N - min(find(any(FeasMap,1))) + 1;
   [FeasMap,FeasDist] = feval(Options.check.function,M,N,Table(1:Tlen),Options);
   if Options.check.endpoints(3) & Show
      fprintf('\nRemainders and C endpoint constraint will be taken care of\n')
   end
   
end
FeasDist = sparse(FeasDist);
FeasMap = sparse(FeasMap);
if Show
   waitbar(1,h2,'Creating distance map')
   pause(.5)
   delete(h2)
end
if isempty(FeasDist)
   FeasDist = FeasMap;
end

IndRef             = repmat({':'},ndims(Ref),N); %Reference index
IndSam             = repmat({':'},ndims(SamMat),M); %Sample index
IndRef(end,:)      = num2cell(1:N);
IndSam(end,:)      = num2cell(1:M);
IndI               = repmat({':'},ndims(Ref),1);

%Initialize distance map
for i = 1:size(SamMat,1)
   
   tic
   IndI{1} = i;
   Sam     = SamMat(IndI{:});
   if Show
      h2 = waitbar(0/3,'Creating distance map');
   end
   if any(isnan(Ref(:))) | any(isnan(Sam(:)))
      %Activates the check for missing values.
      %It is an option as it slows down the algorithm
      Options.distance.miss = true;
   else
      Options.distance.miss = false;
   end
   
   %    DistMap = repmat(Inf,size(FeasMap));
   %    for m = 1:M %iterate over sample
   %       
   %       indn = find(FeasDist(m,:));
   %       for n = indn
   %          % if instead of feval the code is in here it is slightly (<10%) faster
   %          DistMap(m,n) = feval(Options.distance.function,Sam(IndSam{:,m}),Ref(IndRef{:,n}),Options.distance);
   %       end
   %       
   %    end
   Pow     = 1;
   DistMap = repmat(Ref(1:N),M,1);
   DistMap = DistMap - repmat(Sam(1:M)',1,N);
   DistMap = DistMap.^2;
   DistMap = DistMap.^Pow;
   DistMap(~FeasDist) = inf;

   
   %Cumulated distances. Can be memory optimised. 
   %Ideally only the M x Min_n or Min_M x N are necessary. It would require book-keeping and update. 
   CumDist      = repmat(inf,M,N);
   CumDist(1,1) = DistMap(1,1); 
   OptStep      = char(zeros(M,N));
   %Compute map of accumulated distances according to Table
   
   if Show
      waitbar(1/3,h2,'Creating mapping grid')
   end
   %Fill in the first column and the first row for relaxed endpoints      
   if Options.check.endpoints(1) 
      [CumDist(:,1),OptStep(:,1)] = FillBorderAB(DistMap(:,1),FeasMap(:,1),Table{Tver}(end,3),Tver);
   end
   if Options.check.endpoints(2)
      [CumDist(1,:),OptStep(1,:)] = FillBorderAB(DistMap(1,:),FeasMap(1,:),Table{Thor}(end,3),Thor);
   end
   CumDist(~FeasMap) = inf;
   
   %Slightly Suboptimal, the last row and column are filled twice
   [OptStep,CumDist] = DMWWarp(FeasMap,DistMap,CumDist,Table(1:Tlen),Options,OptStep);
   
   if Options.check.endpoints(4)
      RowSpan = [M + min(Tcumstep(:,1)):M];
      [CumDist(RowSpan,:),OptStep(RowSpan,:)] = FillBorderCD(DistMap(RowSpan,:),FeasMap(RowSpan,:),CumDist(RowSpan,:),OptStep(RowSpan,:),0,Table,Options);      
   end
   if Options.check.endpoints(3) 
      ColSpan = [N + min(Tcumstep(:,2)):N];
      [CumDist(:,ColSpan),OptStep(:,ColSpan)] = FillBorderCD(DistMap(:,ColSpan),FeasMap(:,ColSpan),CumDist(:,ColSpan),OptStep(:,ColSpan),1,Table,Options);      
   end
   
   if Show
      waitbar(2/3,h2,'Reconstructing warping path & synchronysing')
   end
   [WarpPath{i},SyncRef{i},SyncSam{i}] = DMWSynchronise(Ref,Sam,OptStep,Table,Options);
   if Show
      pause(.5)
      waitbar(3/3,h2,'Warped!')
      pause(.5)
      delete(h2)
   end
   if isempty(SyncRef)
      SyncRef = Ref;
   end
   fprintf('\n %i\t\t%3.2f',i,toc);
end
%---------------------------------------------------------------------------------------------------------------------------------------------

function [OptStep,CumDist] = DMWWarp(FeasMap,DistMap,CumDist,Table,Options,OptStep)
FlatStep_count = [];
[M,N]          = size(FeasMap);
Alg            = any(Options.check.endpoints);
if ~exist('OptStep','var') | isempty(OptStep)
   %Optimal step look-up. May speed up recovery of Optimal warping path
   %As it is, it does not support Tables with more than 65535 steps ...
   OptStep = char(zeros(M,N));
end
%Speed-up measures: indexes and operations are stored in arrays instead of cell vectors giving faster access
%                   some operations that would be uselessly repeated in the inner loops are also executed here    
[Tini,Tfin,Tcat,Tlen,Tcumstep,Thor,Tver,Tflat,Tsize,Min_m,Min_n,Table,Temp] = Table2Double(Table,Options);
Tempa = Temp(:,1);
Tempb = Temp(:,2);
aa    = ~Tsize(1:Tlen);
wm    = -Tcumstep(1:Tlen,1);
wn    = -Tcumstep(1:Tlen,2);
%Build the mapping grid CumDist according to local and global (via the FeasMap) constraints.
if any(Tsize)
   
   %CumDist(~FeasMap) = inf;
   for m = Min_m:M
      
      indn  = find(FeasMap(m,Min_n:N)) + Min_n - 1;
      %indsm = -Tcumstep(1:Tlen,1) < m;
      indsm0 = wm < m & aa;
      indsm1 = wm < m & ~aa;
      %T(1)  = m;
      for n = indn
         
         indsmn0 = find(wn < n & indsm0);
         indsmn1 = find(wn < n & indsm1);
         inda    = m - Tempa;
         indb    = n - Tempb;
         %Initialise step distance to inf
         %d0     = inf;
         CumDist(m,n) = inf;
         for S = 1:length(indsmn0)
            s    = indsmn0(S);
            a    = m + Tcumstep(s,1);
            b    = n + Tcumstep(s,2);
            if FeasMap(a,b)
               
               %Go through the sub-steps of the s-th step
               d = CumDist(a,b) + DistMap(m,n) * Tcat(Tfin(s),3);
               if d < CumDist(m,n)%d0
                  %d0           = d;
                  CumDist(m,n) = d; %Optimal cumulated distance
                  OptStep(m,n) = s; %Optimal step from the current point.
               end
               
            end
         end
         for S = 1:length(indsmn1)
            s    = indsmn1(S);
            a    = m + Tcumstep(s,1);
            b    = n + Tcumstep(s,2);
            if FeasMap(a,b)
               
               %Go through the sub-steps of the s-th step
               d = CumDist(a,b) + DistMap(m,n) * Tcat(Tfin(s),3);
               for r = 0:Tsize(s)
                  d = d + DistMap(inda(Tini(s) + r),indb(Tini(s) + r)) * Tcat(Tini(s) + r,3);
               end
               if d < CumDist(m,n)%d0
                  %d0           = d;
                  CumDist(m,n) = d; %Optimal cumulated distance
                  OptStep(m,n) = s; %Optimal step from the current point.
               end
               
            end
         end
         if ~(isequal(m,1) || isequal(n,1) || ~isequal(m,M) || ~isequal(n,N)) && ~isfinite(CumDist(m,n))%d0)
            %It is necessary to handle remainders < eps in the determination
            %of the feasible map
            FeasMap(m,n) = false;
            %             if  %T(1) == 1 | T(2) == 1 | T(1) == M | T(2) == N
            %                FeasMap(m,n) = true;
            %             end
         end
         
      end
      
   end
   
else
   
   if ~Options.table.check_maxsteps
      
      %No constraints at all
      for m = Min_m:M
         
         indn  = find(FeasMap(m,Min_n:N));
         indsm = find(-Tcumstep(1:Tlen,1) < m);
         T(1)  = m;
         for n = indn
            
            indsmn = indsm(-Tcumstep(indsm,2) < n);
            d0     = inf;
            %Initialise step distance to inf
            T(2) = n;
            for S = 1:length(indsmn)%1:length(Table)
               
               s   = indsmn(S);
               ind = T + Tcumstep(s,:);
               if FeasMap(ind(1),ind(2))
                  
                  %Go through the sub-steps of the s-th step
                  d = CumDist(ind(1),ind(2)) + DistMap(m,n) * Tcat(Tfin(s),3);
                  if d < d0
                     d0           = d;
                     CumDist(m,n) = d; %Optimal cumulated distance
                     OptStep(m,n) = s; %Optimal step from the current point.
                  end
                  
               end
               
            end
            
         end
         
      end
      
   else
      
      FlatStep_count                             = zeros(2,N);                 %Matrix keeping track of the previous steps
      FlatStep_acc                               = 1 - Tcumstep(:,1);
      FlatStep_acc2                              = repmat(1:N,length(Table),1) + repmat(Tcumstep(:,2),1,N);
      MaxSteps                                   = Options.table.maxsteps - 1; %Just to avoid having to do this thousands of times within the various loops
      
      [CumDist(:,1),OptStep(:,1)]                = FillBorderAB(DistMap(:,1),FeasMap(:,1),Table{Tver}(end,3),Tver);
      [CumDist(1,:),OptStep(1,:),FlatStep_count] = FillBorderAB(DistMap(1,:),FeasMap(1,:),Table{Thor}(end,3),Thor,FlatStep_count);
      for m = 2:M
         FlatStep_count(2,:) = FlatStep_count(1,:);
         FlatStep_count(1,:) = ones(1,N);
         indn                = find(FeasMap(m,2:end)) + 1;
         indsm               = find(-Tcumstep(1:Tlen,1) < m);
         T(1)                = m;
         for n = indn
            
            indsmn = indsm(-Tcumstep(indsm,2) < n);
            %Initialise step distance to inf
            d0   = inf;
            T(2) = n;
            for S = 1:length(indsmn)
               
               s   = indsmn(S);
               ind = T + Tcumstep(s,:);
               d   = CumDist(ind(1),ind(2)) + DistMap(m,n) * Tcat(Tfin(s),3);
               if FeasMap(ind(1),ind(2))
                  
                  if (Thor == s | Tver == s) & (~Alg | (m ~= M & n ~= N))  
                     
                     if s == OptStep(ind(1),ind(2))
                        if FlatStep_count(FlatStep_acc(s),FlatStep_acc2(s,n)) > MaxSteps
                           d  = inf;
                        end
                     end
                     
                  end   
                  if d < d0
                     d0           = d; %Optimal cumulated distance
                     OptStep(m,n) = s; %Optimal step from the current point.
                     OptStepPrev  = OptStep(ind(1),ind(2));
                  end
                  
               end
               
            end
            CumDist(m,n) = d0;
            if isfinite(d0)  
               
               if (Thor == OptStep(m,n) | Tver == OptStep(m,n))
                  if OptStep(m,n) == OptStepPrev 
                     FlatStep_count(1,n) = FlatStep_count(FlatStep_acc(OptStep(m,n)),FlatStep_acc2(OptStep(m,n),n)) + 1;
                  end
               else
                  FlatStep_count(1,n) = 0;
               end
               
            else
               FeasMap(m,n) = false;
            end
            
         end
         
      end                                
      
   end
   
end

%---------------------------------------------------------------------------------------------------------------------------------------------

function [CumDistBord,OptStepBord,FlatStep_count] = FillBorderAB(DistMapBord,FeasMapBord,Weight,Tflat,FlatStep_count)
CumDistBord = DistMapBord;
OptStepBord = char(zeros(size(CumDistBord)));
indn        = find(FeasMapBord(2:end)) + 1;
for j = 1:length(indn)
   CumDistBord(indn(j)) = CumDistBord(indn(j) - 1) + DistMapBord(indn(j)) * Weight;
end
if nargout > 2 & nargin > 4 & size(FeasMapBord,1) == 1 %First row
   OptStepBord(1,[1,indn(:)'])    = Tflat;
   FlatStep_count(1,[1,indn(:)']) = 1;
else
   OptStepBord(indn) = Tflat;
end

%----------------------------------------------------------------------------------------------------------------------------------------------

function [CumDistBord,OptStepBord,FlatStep_count] = FillBorderCD(DistMapBord,FeasMapBord,CumDistBord,OptStepBord,RowCol,Table,Options)
[Tini,Tfin,Tcat,Tlen,Tcumstep,Thor,Tver,Tflat,Tsize,Min_m,Min_n] = Table2Double(Table);
if ~isequal(size(DistMapBord),size(FeasMapBord),size(CumDistBord))
   error('The passed arguments do not fit with each other')
end
[M,N] = size(FeasMapBord);
if RowCol %Column
   
   if N <= Min_n
      error('Table''s width exceed feasible map width')
   end
   indm  = find(FeasMapBord(:,N));
   indsn = find(-Tcumstep(unique([1:Tlen,Tver]),2) < N);
   T(2)  = N;
   for mm = 1:length(indm)
      
      m      = indm(mm);
      indsmn = indsn(-Tcumstep(indsn,1) < M);
      d0     = inf;
      T(1)   = m;
      for S = 1:length(indsmn)
         
         s   = indsmn(S);
         ind = T + Tcumstep(s,:);
         if FeasMapBord(ind(1),ind(2))
            
            %Go through the sub-steps of the s-th step
            d         = CumDistBord(ind(1),ind(2)) + DistMapBord(m,N) * Tcat(Tfin(s),3);
            ind2(1:2) = T;
            ind2(3)   = 0;
            for r = 1:Tsize(s)
               ind2 = ind2 - Tcat(Tini(s) + r - 1,:);
               %The minus is for the weight, that changes sign
               d    = d - DistMapBord(ind2(1),ind2(2)) * ind2(3);
            end
            if d < d0
               d0               = d; %Optimal cumulated distance
               CumDistBord(m,N) = d0;
               OptStepBord(m,N) = s; %Optimal step from the current point.
            end
            
         end
         
      end
      if ~isfinite(d0)
         FeasMapBord(m,N) = false;
      end
      
   end
   
else
   
   if M <= Min_m
      error('Table''s width exceed feasible map height')
   end
   indn  = find(FeasMapBord(M,:));
   indsm = find(-Tcumstep(unique([1:Tlen,Thor]),1) < M);
   T(1)  = M;
   for n = indn
      
      indsmn = indsm(-Tcumstep(indsm,2) < N);
      d0     = inf;
      T(2)   = n;
      for S = 1:length(indsmn)
         
         s   = indsmn(S);
         ind = T + Tcumstep(s,:);
         if FeasMapBord(ind(1),ind(2))
            
            %Go through the sub-steps of the s-th step
            d         = CumDistBord(ind(1),ind(2)) + DistMapBord(M,n) * Tcat(Tfin(s),3);
            ind2(1:2) = T;
            ind2(3)   = 0;
            for r = 1:Tsize(s)
               ind2 = ind2 - Tcat(Tini(s) + r - 1,:);
               %The minus is for the weight, that changes sign
               d    = d - DistMapBord(ind2(1),ind2(2)) * ind2(3);
            end
            if d < d0
               d0               = d; %Optimal cumulated distance
               CumDistBord(M,n) = d0;
               OptStepBord(M,n) = s; %Optimal step from the current point.
            end
            
         end
         
      end
      if ~isfinite(d0)
         FeasMapBord(m,N) = false;
      end
      
   end
   
end

%-------------------------------------------------------------------------------------------------------------------------------------------------

function [Tini,Tfin,Tcat,Tlen,Tcumstep,Thor,Tver,Tflat,Tsize,Min_m,Min_n,Table,Temp] = Table2Double(Table,Options)
if nargin < 2
   Options.table.weight    = NaN;
   Options.check.endpoints = zeros(1,4);
end
Thor  = [];
Tver  = [];
Tflat = [];
Tlen  = length(Table);
for s = 1:Tlen
   
   if isequal(sum(Table{s}(:,1:2),1),[0 1]) 
      Thor = s;
   elseif isequal(sum(Table{s}(:,1:2),1),[1 0])
      Tver = s;
   end
   
end
%Check if horizontal step is included in the table
if isempty(Thor)
   Thor = length(Table) + 1;
end
if length(Table) < Thor
   Tflat            = [Tflat;Thor];
   Table(Thor)      = ComputeWeight({[0 1 NaN]},Options.table.weight);
end
%Check if vertical step is included in the table
if isempty(Tver)
   Tver = length(Table) + 1;
end
if length(Table) < Tver
   Tflat            = [Tflat;Tver];
   Table(Tver)      = ComputeWeight({[1 0 NaN]},Options.table.weight);
end
Tsize = cellfun('size',Table(:),1);      %Size of each step in the table
Tfin  = [cumsum(Tsize)];                 %Final position of the step in the table
Tini  = [1;Tfin(1:end-1)+1];             %Initial position of the step in the table
Tcat  = cat(1,Table{:});                 %Transforms the table in a matrix (faster)
Tsize = Tsize - 1;                       %For the cycle with the substeps
for s = 1:length(Table)
   Tcumstep(s,1:2)           = -sum(Table{s}(:,1:2),1);
   Temp(Tini(s):Tfin(s),1:2) = cumsum(Table{s}(:,1:2));
end
Min_m = min(-Tcumstep(1:Tlen,1)) + 1;    %Minimum sample span of look-up table
Min_n = min(-Tcumstep(1:Tlen,2)) + 1;    %Minimum reference span of look-up table


%----------------------------------------------------------------------------------------------------------------------------------------------

function [WarpPath,SyncRef,SyncSam] = DMWSynchronise(Ref,Sam,OptStep,Table,Options)
[M,N]   = size(OptStep);
IndI    = repmat({':'},ndims(Sam),1);   %n-way array index for sample
IndF    = repmat({':'},ndims(Sam),1);   %n-way array index for warped sample
miss    = any(isnan(Sam(:)));
TcatSam = {};
TcatRef = {};
SyncSam = [];
SyncRef = [];
for s = 1:length(Table);
   TcatSam{s}      = -cumsum(Table{s}(:,1));
   TcatRef{s}      = -cumsum(Table{s}(:,2));
   Tsize(s)        = size(Table{s},1);
   Tcumstep(s,1:2) = sum(Table{s}(:,1:2),1);
end
%Initial point on the map. Limited by global constraints
[m,WarpPath(1,1),m_in] = deal(M);
[n,WarpPath(1,2),n_in] = deal(N);

Mrem = size(Sam,ndims(Sam)) - M;
Nrem = size(Ref,ndims(Ref)) - N;
C    = 0;
if or(Mrem,Nrem)
   
   %Remember to insert remainders in the warppath
   if isequal(sum(Table{OptStep(M,N)}(:,1:2),1),[1 0]);
      
      while isequal(sum(Table{OptStep(m,n)}(:,1:2),1),[1 0]);
      
         len                                           = size(WarpPath,1);
         WarpPath(len + 1:len + Tsize(OptStep(m,n)),1) = WarpPath(len,1) + TcatSam{OptStep(m,n)};; 
         WarpPath(len + 1:len + Tsize(OptStep(m,n)),2) = WarpPath(len,2) + TcatRef{OptStep(m,n)};; 
         m                                             = WarpPath(end,1);
         n                                             = WarpPath(end,2);
         C                                             = C + 1;
         
      end
      Mrem = size(Sam,ndims(Sam)) - m;
      
   end
   if Mrem >= 2 %For interpolation more than two points are necessary (if it is only one...)
      
      IndI{end}        = m : size(Sam,ndims(Sam));
      temp1            = Sam(IndI{:});
      s                = size(temp1);
      nd               = ndims(temp1);
      ord              = [nd,1:nd-1];
      np               = 1:Mrem + 1;
      nr               = linspace(1,length(np),Nrem + 1);
      temp1            = reshape(permute(temp1,ord),s(ord));                     %Time on the first mode, necessary for interp1
      temp1            = interp1(np',temp1,nr(1:end)',Options.sync.type);         %Interpolate
      s(nd)            = size(temp1,1);
      temp1            = ipermute(reshape(temp1,s(ord)),ord);                    %Permute to right mode order               
      IndF{end}        = [1:Nrem + 1];                                               %Index on warped sample
      SyncSam(IndF{:}) = temp1;
      
   else
      IndI{end}        = M;
      SyncSam = Sam(IndI{:});
   end
   
else
   IndI{end} = M;
   SyncSam   = Sam(IndI{:});
end
s        = size(Sam);
nd       = ndims(Sam);
ord      = [nd,1:nd-1];
SyncSam  = cat(ndims(Sam),repmat(NaN,[s(1:end-1) WarpPath(1,2) - 1]),SyncSam);

if Options.sync.synchronise & (isequal(Options.sync.interpolate,'Type II') | isequal(Options.sync.interpolate,'Type III'))
   
   while ~isequal(WarpPath(end,1),WarpPath(end,2),1)
      
      %Overall delta for the step
      m        = WarpPath(end,1);
      n        = WarpPath(end,2);
      if isequal(Tcumstep(OptStep(m,n),1:2),[1 0])
         
         m2 = m; 
         delta_m = [];
         while m2 ~= 1 & isequal(Tcumstep(OptStep(m2(end),n),1:2),[1 0])
            delta_m(end + 1) = -1;    %Sample
            m2(end + 1)      = m2(end) -1;
         end
         delta_m          = cumsum(delta_m);
         delta_n          = zeros(size(delta_m));
         IndF{end}        = n;
         IndI{end}        = m2;
         temp1            = Sam(IndI{:});
         st               = size(temp1);
         np               = [1:st(nd)]';
         temp1            = reshape(permute(temp1,ord),st(ord));                    %Time on the first mode, necessary for interp1
         temp1            = interp1(np,temp1,mean(np),Options.sync.type);           %Interpolate
         st(nd)           = size(temp1,1);
         SyncSam(IndF{:}) = ipermute(reshape(temp1,st(ord)),ord);                   %Permute to right mode order               
                        
      elseif ~isequal(Tcumstep(OptStep(m,n),1:2),[0 1])
         
         %An advancement in both indexes is the optimal step
         delta_m   = TcatSam{OptStep(m,n)};     %Sample
         delta_n   = TcatRef{OptStep(m,n)};     %Reference
         m2        = [m,m + delta_m'];
         n2        = [n,n + delta_n'];
         unin      = unique(n2);                %Unique sorts the points in the opposite order
         unim      = unique(m2);                                  %Unique sorts the points in the opposite order
         IndF{end} = n - 1:-1:n2(end);
         IndI{end} = m - 1:-1:m2(end);
         if ~isequal(m - m2(end),n - n2(end),length(unin) - 1,length(unim) - 1)
            
            %Interpolate
            if isequal(Options.sync.interpolate,'Type III') & isequal(n - n2(end),length(unim) - 1)
               %If the number of points in the sample is equal to the number of points spanned by the step 
               %on the reference, interpolation may be skipped
               IndI{end}        = unim(end - 1:-1:1);
               SyncSam(IndF{:}) = Sam(IndI{:});
            else
               
               np               = [length(unim):-1:1]';
               nr               = linspace(m - m2(end) + 1,1,n - n2(end) + 1)';
               IndI{end}        = unim(end:-1:1);
               temp1            = Sam(IndI{:});
               st               = size(temp1);
               temp1            = reshape(permute(temp1,ord),st(ord));                    %Time on the first mode, necessary for interp1
               temp1            = interp1(np,temp1,nr(2:end),Options.sync.type);                 %Interpolate
               st(nd)           = size(temp1,1);
               SyncSam(IndF{:}) = ipermute(reshape(temp1,st(ord)),ord);                   %Permute to right mode order               
               
            end
            
         else
            SyncSam(IndF{:}) = Sam(IndI{:});
         end
         
      elseif isequal(Tcumstep(OptStep(m,n),1:2),[0 1])
         
         n2      = n; 
         delta_n = [];
         while n2(end) ~= 1 & isequal(Tcumstep(OptStep(m,n2(end)),1:2),[0 1])
            delta_n(end + 1) = -1;    %Sample
            n2(end + 1)      = n2(end) -1;
         end
         delta_n          = cumsum(delta_n);
         delta_m          = zeros(size(delta_n));
         IndI{end}        = m(ones(length(n2),1));
         IndF{end}        = n2;
         SyncSam(IndF{:}) = Sam(IndI{:});
         
      end
      len                                       = size(WarpPath,1);
      WarpPath(len + 1:len + length(delta_m),1) = WarpPath(len,1) + delta_m(:); %Update warp path for sample
      WarpPath(len + 1:len + length(delta_n),2) = WarpPath(len,2) + delta_n(:); %Update warp path for references
      
   end   
   
else
   
   while ~isequal(m,n,1)
      len                                           = size(WarpPath,1);
      WarpPath(len + 1:len + Tsize(OptStep(m,n)),1) = WarpPath(len,1) + TcatSam{OptStep(m,n)};; 
      WarpPath(len + 1:len + Tsize(OptStep(m,n)),2) = WarpPath(len,2) + TcatRef{OptStep(m,n)};; 
      m                                             = WarpPath(end,1);
      n                                             = WarpPath(end,2);
   end
   if Options.sync.synchronise
      
      Ref_path = unique(WarpPath(:,2));
      if ~miss
         
         for i = 1:length(Ref_path) - 1
            
            IndF{end} = Ref_path(i);
            IndI{end} = WarpPath(WarpPath(:,2) == Ref_path(i),1);
            if ~isequal(length(IndI{end}),length(IndF{end}),1)
            
               if isequal(Options.sync.interpolate,'Average')
                  SyncSam(IndF{:}) = mean(Sam(IndI{:}),nd);
               else
                  
                  %Interpolation
                  temp1            = Sam(IndI{:});
                  st               = size(temp1);
                  np               = [1:st(nd)]';
                  temp1            = reshape(permute(temp1,ord),st(ord));                           %Time on the first mode, necessary for interp1
                  temp1            = interp1(np,temp1,mean(np),Options.sync.type);                %Interpolate
                  SyncSam(IndF{:}) = ipermute(reshape(temp1,[1,s(1:nd-1)]),ord);                   %Permute to right mode order               
                  
               end
               
            else
               SyncSam(IndF{:}) = Sam(IndI{:});
            end
            
         end
         
      else
      end

   else
      
      if or(Mrem,Nrem)
         IndI{end} = WarpPath(end:-1:C + 1,1);
         SyncSam   = cat(ndims(Sam),Sam(IndI{:}),SyncSam);
         IndI{end} = cat(1,WarpPath(end:-1:C + 1,2),[WarpPath(C + 1,2) + 1:size(Ref,ndims(Ref))]');
         SyncRef   = Ref(IndI{:});
      else   
         IndI{end} = flipud(WarpPath(:,1));
         SyncSam   = Sam(IndI{:});
         IndI{end} = flipud(WarpPath(:,2));
         SyncRef   = Ref(IndI{:});
      end
      
   end
   
end

%-----------------------------------------------------------------------------------------------------------------------------------------------

function Options = DMWOptions(Distance,Table,Check,Interp)
ver       = version;
TableStr  = struct('function',@LookupTable,'weight',NaN,'type','NoCon','maxsteps',0,'check_maxsteps',false,'span',25);
CheckStr  = struct('function',@checkstep_gen,'in_points',false,'band',1,'endpoints',[0 0 0 0]);
DistStr   = struct('function',@Distance_eucl,'miss',1);
InterpStr = struct('type','linear','interpolate','Average','synchronise',true);
if nargin >= 1
   
   if isa(Table,'cell')
      TableStr.function       = Table;
      TableStr.weight         = [];
      TableStr.maxsteps       = [];
   elseif ischar(Table)
      
      TableStr.weight   = [];
      TableStr.maxsteps = [];
      if ~any([2:6] == exist('Table'))
         error('Check step function is not in the path')
      end
      TableStr.function = Table;
      
   elseif isa(Table,'struct')
      
      FN = fieldnames(Table);
      if str2num(ver(1:3)) >= 6.5
         for i = 1:length(FN)
            TableStr.(FN{i}) = Table.(FN{i});
         end
      else
         for i = 1:length(FN)
            TableStr = setfield(TableStr,FN{i},getfield(Table,FN{i}));
         end
      end
      
   end
   if ~TableStr.maxsteps
      TableStr.check_maxsteps = false;
   end
   
end
if nargin >= 2
   
   if isa(Check,'function_handle')
      CheckStr.function = Check;
   elseif ischar(Check)
      if ~any([2:6] == exist('Check'))
         error('Check step function is not in the path')
      end
      CheckStr.function = Check;
   elseif isa(Check,'struct')
      
      FN = fieldnames(Check);
      if str2num(ver(1:3)) >= 6.5
         for i = 1:length(FN)
            CheckStr.(FN{i}) = Check.(FN{i});
         end
      else
         for i = 1:length(FN)
            CheckStr = setfield(CheckStr,FN{i},getfield(Check,FN{i}));
         end
      end
      
   end
   
end
if nargin >= 3
   
   if isa(Distance,'function_handle')
      DistStr.function = Distance;
   elseif ischar(Distance)
      if ~any([2:6] == exist('Distance'))
         error('Check step function is not in the path')
      end
      DistStr.function = Distance;
   elseif isa(Distance,'struct')
      
      FN = fieldnames(Distance);
      if str2num(ver(1:3)) >= 6.5
         for i = 1:length(FN)
            DistStr.(FN{i}) = Distance.(FN{i});
         end
      else
         for i = 1:length(FN)
            DistStr = setfield(DistStr,FN{i},getfield(Distance,FN{i}));
         end
      end
      
   end
   
end
if nargin >= 4
   
   if isa(Interp,'struct')
      
      FN = fieldnames(Interp);
      if str2num(ver(1:3)) >= 6.5
         for i = 1:length(FN)
            InterpStr.(FN{i}) = Interp.(FN{i});
         end
      else
         for i = 1:length(FN)
            InterpStr = setfield(InterpStr,FN{i},getfield(Interp,FN{i}));
         end
      end
      
   end
   
end
Options = struct('table',TableStr,'check',CheckStr,'distance',DistStr,'sync',InterpStr);

%-------------------------------------------------------------------------------------------------------------------------------------------------

function [feas,feasdist] = checkstep_gen(M,N,Table,Options)
% Checks feasibility for point in general DTW look-up table
%
% The previous step is not taken into consideration (see: checkstep_ams)
%
% feas = checkstep(M,N,m,n,Flag,Options)
%
% INPUTS
% M,N        : Length of the Reference and of the Sample respectively 
% Table      : Table is cell format
% Options    : DMW options
%
% OUTPUTS
% feas       : 0 not feasible transition-endpoint
%              1 feasible transition-endpoint) 
%
% Author: 
% Giorgio Tomasi 
% Royal Agricultural and Veterinary University 
% MLI, LMT, Chemometrics group 
% Rolighedsvej 30 
% DK-1958 Frederiksberg C 
% Danmark 
% 
Alpha_min = inf;
Alpha_max = -inf;
%DistAdd   = [0 0];
if any(~Options.check.endpoints)
   Options.check.endpoints(~Options.check.endpoints) = 1;
end
if ~Options.table.check_maxsteps
   
   for i = 1:length(Table)
      
      a = sum(Table{i}(:,1:2),1);
      b = [0 0];
      for s = 1:size(Table{i},1)
         b(1) = b(1) + isequal(Table{i}(s,1:2),[0 1]);
         b(2) = b(2) + isequal(Table{i}(s,1:2),[1 0]);
      end
      %DistAdd = max(DistAdd,b);
      warning off
      if Alpha_min > a(1)/a(2) %Minimum slope
         Alpha_min = a(1)/a(2);
      end
      if Alpha_max < a(1)/a(2) %Maximum slope
         Alpha_max = a(1)/a(2);
      end
      warning on
      
   end
   
else
   [Alpha_min,Alpha_max]   = deal(1/(Options.table.maxsteps + 1),Options.table.maxsteps + 1);
   Options.check.endpoints = max(Options.check.endpoints,Options.table.maxsteps + 1);
   %DistAdd                = Options.table.maxsteps([1 1]);
end
[A,B,C,D] = deal(Options.check.endpoints(1),Options.check.endpoints(2),Options.check.endpoints(3),Options.check.endpoints(4));

feas     = true(M,N);
feasdist = true(M,N);
if isfinite(Alpha_max)
   
   for n = 1:N
      %Feasible transition end-points
      feas(fix([1:Alpha_min * (n - B - 1) + 1]),n)    = false;
      feas(fix([M:-1:Alpha_max * (n - 1) + A + 1]),n) = false;
      feas(fix([1:Alpha_max * (n - N) + M - C]),n)    = false;
      feas(fix([M:-1:Alpha_min * (n - N + D) + M]),n) = false;
   end
   feasdist = feas;
   %    if ~Options.table.check_maxsteps
   %        
   %       for n = 1:N
   %          %Feasible intermediate points distances, necessary for complex transitions
   %          feasdist([1:Alpha_min * (n - B - 1 - DistAdd(1)) + 1],n)    = false;
   %          feasdist([M:-1:Alpha_max * (n - 1) + A + 1 + DistAdd(2)],n) = false;
   %          feasdist([1:Alpha_max * (n - N) + M - C - DistAdd(2)],n)    = false;
   %          feasdist([M:-1:Alpha_min * (n - N + D + DistAdd(1)) + M],n) = false;      
   %       end
   
end

if Options.table.check_maxsteps
   feasdist = feas;
end
if Options.check.band & Options.check.band ~= 1
   
   BandWidth = abs(M - N) + Options.check.band * min(M,N) + 1;
   BandSlope = M/N;
   for n = 1:N
       cols = round([1:BandSlope * (n - BandWidth) - 1]); %%% FvdB 041026
       feas(cols,n) = false;
       cols = round([M:-1:BandWidth + BandSlope * (n - 1)]); %%% FvdB 041026
       feas(cols,n) = false;
       cols = round([1:BandSlope * (n - BandWidth) - 1]); %%% FvdB 041026
       feasdist(cols,n) = false;
       cols = round([M:-1:BandWidth + BandSlope * (n - 1)]); %%% FvdB 041026
       feasdist(cols,n) = false;
   end
   
end
if Options.check.in_points
   
   notfeas = ~feas;
   for s = 1:length(Table)
      Tcumstep(s,1:2) = sum(Table{s}(:,1:2));
   end
   Hor = unique(Tcumstep(:,2));
   Ver = unique(Tcumstep(:,1));
   if length(Hor) == 1
      
      for b = 1:B
         for d = 1:D
            notfeas(:,end - d + 1:-Hor:b) = true;
         end
      end
      
   elseif length(Ver) == 1
      notfeas(end:-Ver:1,:) = true;
   end
   feas = feas & notfeas;
   
end

%-------------------------------------------------------------------------------------------------------------------------------------------

function d = Distance_eucl(R,S,varargin)
% Computes euclidean distance between vectors or vectorised arrays
% Missing values are ignored.
% Returns inf if R or S are empty or only NaNs
%
%
% Author: 
% Giorgio Tomasi 
% Royal Agricultural and Veterinary University 
% MLI, LMT, Chemometrics group 
% Rolighedsvej 30 
% DK-1958 Frederiksberg C 
% Danmark 
% 

if varargin{1}.miss
   
   nonmiss = find(~(isnan(R(:)) | isnan(S(:))));
   if isempty(nonmiss) | isempty(R) | isempty(S)
      d = inf;
      return
   end
   d = sum((R(nonmiss)-S(nonmiss)).^2);
   
else
   d = sum((R(:)-S(:)).^2);
end

%-------------------------------------------------------------------------------------------------------------------------------------------

function T = LookupTable(TableStruct)
% General DMW look-up table
%
% T = Table(TableStruct)
%  
% INPUTS
% TableStruct = table structure with options
% Options' fields are
% type  : 'NoCon'  => T = T(2,1) look-up table, no weighing
%         'Smooth' => T = T(u+2,u) look-up table
%         'COW'    => T = TCOW(u,v) look-up table
% maxlen  : longest step for tablen == 3. Used only if TableN == 3
% weight  : weight for ComputeWeight.
%           elseif NaN => weight = step block distance/n substeps
%           elseif 0   => weight = 1 for all substeps (no weighing)
%
% OUTPUT
% T: look-up table (as a cell vector). Each element is a feasible step
%    The first column is the advancement along the sample
%    The second column is the advancement along the reference
%    The third column contain the weights for the sub-step (if no weighing, 1 for all sub-steps)
%
%
% Author: 
% Giorgio Tomasi 
% Royal Agricultural and Veterinary University 
% MLI, LMT, Chemometrics group 
% Rolighedsvej 30 
% DK-1958 Frederiksberg C 
% Danmark 
% 
if ~isa(TableStruct,'struct')
   error('Input must be a structure');
end
switch lower(TableStruct.type)
   case 'nocon'
      %Amsterdam with no weighing
      T = {[1 0 1];[1 1 1];[0 1 1]};
      T = ComputeWeight(T,TableStruct.weight);
      
   case 'smooth'
      T = TableSmooth(TableStruct);
      
   case 'cow'
      T = TableCOW(TableStruct);
      
   case 'slopecon'
      T = TableSlopeCon(TableStruct);
      
end

%-------------------------------------------------------------------------------------------------------------------------------------------

function Table = TableSmooth(T)
if ~nargin
   Weight   = 0;
   maxsteps = 2;
end
if any(strcmp(fieldnames(T),'weight'))
   Weight = T.weight;
end
if ~exist('Weight','var')
   Weight = 0;
end
if any(strcmp(fieldnames(T),'maxsteps'))
   maxsteps = T.maxsteps;
end
if ~exist('Weight','var')
   Weight = 0;
end
for i = 0:maxsteps
   Table{i + 1}            = [ones(i+1,1),[zeros(i,1);1],ones(i+1,1)];
   Table{i + maxsteps + 1} = [[zeros(i,1);1],ones(i+1,1),ones(i+1,1)];
end
Table = ComputeWeight(Table,Weight);

%---------------------------------------------------------------------------------------------------------------------------------------------

function Table = TableCOW(T)
if ~nargin
   T = struct('weight',0,'maxsteps',3,'span',10);
end
if any(strcmp(fieldnames(T),'weight'))
   Weight = T.weight;
end
if ~exist('Weight','var')
   Weight = 0;
end
if any(strcmp(fieldnames(T),'span'))
   span = T.span;
end
if ~exist('span','var')
   span = 10;
end
if any(strcmp(fieldnames(T),'maxsteps'))
   maxsteps = T.maxsteps;
end
if ~exist('Weight','var')
   Weight = 0;
end
for i = 0:maxsteps
   Table{i + 1}            = [ones(span + i,1),[zeros(i,1);ones(span,1)],ones(span + i,1)];
   Table{i + maxsteps + 1} = [[zeros(i,1);ones(span - i,1)],ones(span,2)];
end

Table = ComputeWeight(Table,Weight);

%---------------------------------------------------------------------------------------------------------------------------------------------

function Table = TableSlopeCon(T)
if ~nargin
   T = struct('weight',0,'maxsteps',3,'span',10);
end
if any(strcmp(fieldnames(T),'weight'))
   Weight = T.weight;
end
if ~exist('Weight','var')
   Weight = 0;
end
if any(strcmp(fieldnames(T),'span'))
   span = T.span;
end
if ~exist('span','var')
   span = 10;
end
if any(strcmp(fieldnames(T),'maxsteps'))
   maxsteps = T.maxsteps;
end
if ~exist('Weight','var')
   Weight = 0;
end
for i = 0:maxsteps
   Table{i + 1}            = [ones(span,1),[zeros(i,1);ones(span - i,1)],ones(span,1)];
   Table{i + maxsteps + 1} = [[zeros(i,1);ones(span - i,1)],ones(span,2)];
end
Table{1} = [1 1 1];
Table    = ComputeWeight(Table,Weight);

%---------------------------------------------------------------------------------------------------------------------------------------------

function T = ComputeWeight(T,Weight)
% Computes exponential weights for a generic DMW look-up table
% If a scalar, the step weights exponentially increase the longest is the single step
% The highest value depends on the longest block distance spanned in T
% and on the number of substeps that the step includes.
% Maximum weight is given to the step with the maximum ratio
% between spanned block distance and number of substeps.
% If Weight is 0, all the substeps are given weight 1. This is equivalent to no-weighing.
% If Weight is NaN, the weight is equal to the block distance for the whole step divided
% by the number of substeps.
%
% T = ComputeWeight(T,Weight)
%  
% INPUTS
% T     : look-up table (as a cell vector)
% Weight: -0.5 (i.e. square root), is default.
%         if NaN   => weight = step block distance/n substeps
%         elseif 0 => weight = 1 for all substeps (no weighing)
%
% OUTPUT
% T: look-up table (as a cell vector) including weights
%
%
% Author: 
% Giorgio Tomasi 
% Royal Agricultural and Veterinary University 
% MLI, LMT, Chemometrics group 
% Rolighedsvej 30 
% DK-1958 Frederiksberg C 
% Danmark 
% 
if nargin < 2
   Weight = .5;
end
MaxStep = 0;
if isnan(Weight)
   for i = 1:length(T)
      T{i}(:,3) = sum(sum(T{i}(:,1:2)))/size(T{i},1);
   end
elseif Weight == 0
   for i = 1:length(T)
      T{i}(:,3) = 1;
   end
else
   for i = 1:length(T)
      MaxStep = max(MaxStep,sum(sum(T{i}(:,1:2))));
   end
   for i = 1:length(T)
      md = ((MaxStep - sum(sum(T{i}(:,1:2))) + 1).^Weight)/size(T{i},1);
      T{i}(:,3) = md;
   end
end
