// export R# package module type define for javascript/typescript language
//
// ref=mzkit.MoleculeNetworking@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Molecular Networking (MN) is a computational strategy that may help visualization and interpretation of the complex data arising from MS analysis.
 * 
 * > MN is able to identify potential similarities among all MS/MS spectra within 
 * >  the dataset and to propagate annotation to unknown but related molecules 
 * >  (Wang et al., 2016). This approach exploits the assumption that structurally
 * >  related molecules produce similar fragmentation patterns, and therefore they 
 * >  should be related within a network (Quinn et al., 2017). In MN, MS/MS data 
 * >  are represented in a graphical form, where each node represents an ion with 
 * >  an associated fragmentation spectrum; the links among the nodes indicate 
 * >  similarities of the spectra. By propagation of the structural information within
 * >  the network, unknown but structurally related molecules can be highlighted
 * >  and successful dereplication can be obtained (Yang et al., 2013); this may
 * >  be particularly useful for metabolite and NPS identification.
 * >  
 * >  MN has been implemented In different fields, particularly metabolomics And 
 * >  drug discovery (Quinn et al., 2017); MN In forensic toxicology was previously
 * >  used by Allard et al. (2019) For the retrospective analysis Of routine 
 * >  cases involving biological sample analysis. Yu et al. (2019) also used MN 
 * >  analysis For the detection Of designer drugs such As NBOMe derivatives And 
 * >  they showed that unknown compounds could be recognized As NBOMe-related 
 * >  substances by MN.
 * >  
 * >  In the present work the Global Natural Products Social platform (GNPS) was 
 * >  exploited to analyze HRMS/MS data obtained from the analysis of seizures 
 * >  collected by the Italian Department of Scientific Investigation of Carabinieri 
 * >  (RIS). The potential of MN to highlight And support the identification of
 * >  unknown NPS belonging to chemical classes such as fentanyls And synthetic
 * >  cannabinoids has been demonstrated.
*/
declare namespace MoleculeNetworking {
   module as {
      /**
       * convert the cluster tree into the graph model
       * 
       * 
        * @param tree A cluster tree which is created via the ``tree`` function.
        * @param ions the source data for create the cluster tree
      */
      function graph(tree: object, ions: object): object;
   }
   /**
    * populate a list of peak ms2 cluster data
    * 
    * 
     * @param tree -
     * @param ions -
     * @return a set of ms2 data groups, in format of ``[guid => peakms2]`` vector tuples
   */
   function msBin(tree: object, ions: object): object;
   /**
    * create representative spectrum data
    * 
    * > @``P:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.PeakMs2.collisionEnergy`` is tagged as the cluster size
    * 
     * @param mzdiff -
     * 
     * + default value Is ``'da:0.3'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function representative(tree: object, mzdiff?: any, env?: object): object;
   /**
    * do spectrum data clustering
    * 
    * 
     * @param ions A set of the spectrum data
     * @param mzdiff the ms2 fragment mass tolerance when used for compares 
     *  ms2 spectrum data
     * 
     * + default value Is ``0.3``.
     * @param intocutoff intensity cutoff value that used for make the spectrum 
     *  centroid and noise cleanup
     * 
     * + default value Is ``0.05``.
     * @param equals -
     * 
     * + default value Is ``0.85``.
   */
   function tree(ions: object, mzdiff?: number, intocutoff?: number, equals?: number): object;
   /**
    * makes the spectrum data its unique id reference uniqued!
    * 
    * 
     * @param ions -
   */
   function uniqueNames(ions: object): object;
}
