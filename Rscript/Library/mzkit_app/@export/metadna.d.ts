// export R# package module type define for javascript/typescript language
//
//    imports "metadna" from "mzDIA";
//
// ref=mzkit.metaDNAInfer@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Metabolic Reaction Network-based Recursive Metabolite Annotation for Untargeted Metabolomics
 * 
*/
declare namespace metadna {
   /**
    * Create the kegg compound ms1 annotation query engine.
    * 
    * 
     * @param kegg a set of kegg/pubchem/chebi/hmdb compound data.
     * @param precursors a character vector of the ms1 precursor ion names or 
     *  a list of the given mzcalculator object models.
     * 
     * + default value Is ``'[M]+|[M+H]+|[M+H-H2O]+'``.
     * @param mzdiff the mass tolerance value to match between the 
     *  experiment m/z value and the reference m/z value
     *  which is calculated from the compound exact mass
     *  with a given specific ion precursor type.
     * 
     * + default value Is ``'ppm:20'``.
     * @param excludes 
     * + default value Is ``null``.
     * @param mass_range 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a data query engine model to run ms1 data search 
     *  for the kegg metaolite compounds.
   */
   function annotationSet(kegg: any, precursors?: any, mzdiff?: any, excludes?: any, mass_range?: any, env?: object): object;
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function graph(result: any, env?: object): object;
      /**
       * create seeds from mgf file data
       * 
       * 
        * @param seeds A set of the mzkit @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.PeakMs2`` clr object that could 
        *  be used for the seeds for run the metadna annotation.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function seeds(seeds: any, env?: object): object;
      /**
       * Extract the annotation result from metaDNA algorithm module as data table
       * 
       * 
        * @param metaDNA -
        * @param result a collection of the @``T:BioNovoGene.BioDeep.MetaDNA.Infer.CandidateInfer``.
        * @param unique -
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return A collection of the @``T:BioNovoGene.BioDeep.MetaDNA.MetaDNAResult`` data objects that could be
        *  used for represented as the result table.
      */
      function table(metaDNA: object, result: any, unique?: boolean, env?: object): object;
      /**
      */
      function ticks(metaDNA: object): object;
   }
   module DIA {
      /**
       * apply of the metadna annotation workflow
       * 
       * 
        * @param metaDNA -
        * @param sample -
        * 
        * + default value Is ``null``.
        * @param seeds -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function infer(metaDNA: object, sample?: any, seeds?: any, env?: object): object;
   }
   module kegg {
      /**
       * load kegg compounds
       * 
       * 
        * @param repo the file path to the messagepack data repository
      */
      function library(repo: string): object;
      /**
       * load the kegg reaction class data.
       * 
       * 
        * @param repo -
      */
      function network(repo: string): object;
   }
   module load {
      /**
       * Set kegg compound library
       * 
       * 
        * @param metadna -
        * @param kegg should be a collection of the @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound`` data.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function kegg(metadna: object, kegg: any, env?: object): object;
      /**
       * set the kegg reaction class data links for the compounds
       * 
       * 
        * @param metadna -
        * @param links should be a collection of the @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.ReactionClass`` data
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function kegg_network(metadna: object, links: any, env?: object): object;
      /**
       * load the ontology tree as the network graph for search
       * 
       * 
        * @param metadna -
        * @param obo raw data for build @``T:BioNovoGene.BioDeep.MetaDNA.OntologyTree``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function ontology(metadna: object, obo: object, env?: object): any;
      /**
       * set ms2 spectrum data for run the annotation
       * 
       * 
        * @param metadna -
        * @param sample a collection of the mzkit peak ms2 data objects
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function raw(metadna: object, sample: any, env?: object): object;
   }
   /**
    * Create an algorithm module for run metaDNA inferance
    * 
    * 
     * @param ms1ppm the mass tolerance error for matches the ms1 ion
     * 
     * + default value Is ``'ppm:20'``.
     * @param mzwidth -
     * 
     * + default value Is ``'da:0.3'``.
     * @param dotcutoff -
     * 
     * + default value Is ``0.5``.
     * @param allowMs1 -
     * 
     * + default value Is ``true``.
     * @param maxIterations -
     * 
     * + default value Is ``1000``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function metadna(ms1ppm?: any, mzwidth?: any, dotcutoff?: number, allowMs1?: boolean, maxIterations?: object, env?: object): object;
   /**
    * Configs the precursor adducts range for the metaDNA algorithm
    * 
    * 
     * @param metadna -
     * @param precursorTypes -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function range(metadna: object, precursorTypes: any, env?: object): object;
   module reaction_class {
      /**
       * load kegg reaction class data in table format from given file
       * 
       * 
        * @param file csv table file or a directory with raw xml model data file in it.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return A collection of the reaction class table for provides 
        *  the data links between the compounds.
      */
      function table(file: string, env?: object): object;
   }
   module read {
      module metadna {
         /**
          * Load network graph model from the kegg metaDNA infer network data.
          * 
          * 
           * @param debugOutput -
           * @param env -
           * 
           * + default value Is ``null``.
         */
         function infer(debugOutput: any, env?: object): object;
      }
   }
   module result {
      /**
       * get result alignments raw data for data plots.
       * 
       * 
        * @param DIAinfer the result candidates of clr data type in mzkit: @``T:BioNovoGene.BioDeep.MetaDNA.Infer.CandidateInfer``
        * @param table the @``T:BioNovoGene.BioDeep.MetaDNA.MetaDNAResult`` data table
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function alignment(DIAinfer: any, table: any, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function setLibrary(metadna: object, library: any, env?: object): any;
   /**
   */
   function setNetworking(metadna: object, networking: object): any;
}
