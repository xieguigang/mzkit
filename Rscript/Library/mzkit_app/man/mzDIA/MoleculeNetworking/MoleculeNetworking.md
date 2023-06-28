# MoleculeNetworking

Molecular Networking (MN) is a computational strategy that may help visualization and interpretation of the complex data arising from MS analysis.
> MN is able to identify potential similarities among all MS/MS spectra within 
>  the dataset and to propagate annotation to unknown but related molecules 
>  (Wang et al., 2016). This approach exploits the assumption that structurally
>  related molecules produce similar fragmentation patterns, and therefore they 
>  should be related within a network (Quinn et al., 2017). In MN, MS/MS data 
>  are represented in a graphical form, where each node represents an ion with 
>  an associated fragmentation spectrum; the links among the nodes indicate 
>  similarities of the spectra. By propagation of the structural information within
>  the network, unknown but structurally related molecules can be highlighted
>  and successful dereplication can be obtained (Yang et al., 2013); this may
>  be particularly useful for metabolite and NPS identification.
>  
>  MN has been implemented In different fields, particularly metabolomics And 
>  drug discovery (Quinn et al., 2017); MN In forensic toxicology was previously
>  used by Allard et al. (2019) For the retrospective analysis Of routine 
>  cases involving biological sample analysis. Yu et al. (2019) also used MN 
>  analysis For the detection Of designer drugs such As NBOMe derivatives And 
>  they showed that unknown compounds could be recognized As NBOMe-related 
>  substances by MN.
>  
>  In the present work the Global Natural Products Social platform (GNPS) was 
>  exploited to analyze HRMS/MS data obtained from the analysis of seizures 
>  collected by the Italian Department of Scientific Investigation of Carabinieri 
>  (RIS). The potential of MN to highlight And support the identification of
>  unknown NPS belonging to chemical classes such as fentanyls And synthetic
>  cannabinoids has been demonstrated.

+ [uniqueNames](MoleculeNetworking/uniqueNames.1) makes the spectrum data its unique id reference uniqued!
+ [as.graph](MoleculeNetworking/as.graph.1) convert the cluster tree into the graph model
+ [tree](MoleculeNetworking/tree.1) do spectrum data clustering
+ [splitClusterRT](MoleculeNetworking/splitClusterRT.1) 
+ [clustering](MoleculeNetworking/clustering.1) Do spectrum clustering on a small bundle of the ms2 spectrum from a single raw data file
+ [msBin](MoleculeNetworking/msBin.1) populate a list of peak ms2 cluster data
+ [representative](MoleculeNetworking/representative.1) create representative spectrum data
