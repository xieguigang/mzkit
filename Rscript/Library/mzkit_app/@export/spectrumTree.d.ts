// export R# package module type define for javascript/typescript language
//
//    imports "spectrumTree" from "mzkit";
//
// ref=mzkit.ReferenceTreePkg@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Provides R language interface for mass spectrometry data processing and metabolite annotation using spectrum tree-based reference libraries.
 * 
 * > This module enables cross-language interoperability between VB.NET mass spectrometry algorithms and R scripting environments.
 * >  
 * >  Key features include:
 * >  
 * >  1. Reference library management (Pack/Binary/Tree formats)
 * >  2. Spectrum similarity searches with multiple algorithms
 * >  3. Metabolite annotation pipeline integration
 * >  4. Library compression and optimization
 * >  5. Test dataset generation
 * >  6. Embedding generation for machine learning applications
 * >  
 * >  Supported data types:
 * >  
 * >  - mzPack containers
 * >  - PeakMs2 spectra
 * >  - LibraryMatrix objects
 * >  - BioDeep metabolite metadata
 * >  
 * >  Search algorithms implemented:
 * >  
 * >  - Cosine similarity (dot product)
 * >  - Jaccard index
 * >  - Entropy-based scoring
 * >  - Forward/reverse match validation
*/
declare namespace spectrumTree {
   /**
    * push the reference spectrum data into the spectrum reference tree library
    * 
    * 
     * @param tree The reference spectrum database, which the spectrum data 
     *  is store in family tree style
     * @param x A new spectrum data to push into the reference database
     * @param ignore_error 
     * + default value Is ``false``.
     * @param args additional parameters for create the spectrum library in spectrum pack format:
     *  
     *  1. uuid, BioDeepID, biodeep_id is used for the metabolite unique reference id
     *  2. chemical_formula, formula is used for the metabolite exact mass value
     *  
     *  and the spectrum input of x should be the same metabolite if save data as 
     *  the spectrum pack data.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function addBucket(tree: any, x: any, ignore_error?: boolean, args?: object, env?: object): any;
   module as {
      /**
       * Create metabolite annotation result dataset for a set of the spectrum annotation candidates result.
       * 
       * 
        * @param hits A set of the spectrum annotation hits candidates
        * @param metadb A metabolite annotation data repository, which could be pull annotation information by a unique reference id.
        * @return An array of AnnotationData(Of xref) with metabolite annotations.
      */
      function annotation_result(hits: object, metadb: object): object;
   }
   /**
    * Extract all reference id from a set of spectrum annotation candidate result
    * 
    * 
     * @param result An array of AlignmentOutput objects.
     * @param env The R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return A string array of metabolite reference IDs.
   */
   function candidate_ids(result: any, env?: object): any;
   /**
    * Compresses and optimizes a spectrum library, removing redundant entries.
    * 
    * 
     * @param spectrumLib The SpectrumReader source library.
     * @param file Output file path for compressed library.
     * @param metadb Metadata repository for annotations.
     * @param nspec Maximum spectra per metabolite entry.
     * 
     * + default value Is ``5``.
     * @param xrefDb Cross-reference database name.
     * 
     * + default value Is ``null``.
     * @param test Number of test entries to process (-1 for all).
     * 
     * + default value Is ``-1``.
     * @param env The R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return TRUE on successful compression.
   */
   function compress(spectrumLib: object, file: any, metadb: object, nspec?: object, xrefDb?: string, test?: object, env?: object): any;
   /**
    * set @``P:BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query.Ms2Search.discardPrecursorFilter`` to value true, and make cache of the spectrum data
    * 
    * 
     * @param pack -
   */
   function discard_precursor_filter(pack: object): object;
   /**
    * set dot cutoff parameter for the cos score similarity algorithm
    * 
    * 
     * @param search The spectrum library stream engine
     * @param cutoff cutoff threshold value of the cos score
   */
   function dotcutoff(search: object, cutoff: number): object;
   /**
    * Generates vector embeddings for spectral data (e.g., for machine learning).
    * 
    * 
     * @param x Input spectra (mzPack, PeakMs2 array, or mzPack list).
     * @param mslevel MS level for spectra extraction (1 or 2). 
     *  only works when the input dataset is @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.mzPack``
     * 
     * + default value Is ``2``.
     * @param env The R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return A VectorModel containing spectral embeddings.
   */
   function embedding(x: any, mslevel?: object, env?: object): object;
   /**
    * export all reference spectrum from the given library object
    * 
    * 
     * @param pack The PackAlignment object containing spectral data.
     * @param ionMode 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
     * @return An array of PeakMs2 objects representing reference spectra.
   */
   function export_spectrum(pack: any, ionMode?: object, env?: object): object;
   /**
    * Extract the test sample data for run evaluation of the annotation workflow
    * 
    * 
     * @param packlib The SpectrumReader object containing reference spectra.
     * @param n The number of test samples to generate.
     * 
     * + default value Is ``30``.
     * @param rtmax The maximum retention time for generated test spectra.
     * 
     * + default value Is ``840``.
     * @param source_name A fake source name for label this generated test dataset.
     * 
     * + default value Is ``'get_testSample'``.
   */
   function get_testSample(packlib: object, n?: object, rtmax?: number, source_name?: string): object;
   /**
    * construct a fragment set library for run spectrum search in jaccard index matches method
    * 
    * 
     * @param libname Array of library spectrum identifiers.
     * @param mz Array of precursor m/z values.
     * @param mzset Array of fragment m/z strings (comma-separated).
     * @param rt Array of retention times (optional).
     * 
     * + default value Is ``null``.
     * @param cutoff Jaccard similarity threshold (0.0 to 1.0).
     * 
     * + default value Is ``0.1``.
     * @param filter_complex_adducts Exclude spectra with multiple adducts if TRUE.
     * 
     * + default value Is ``false``.
     * @param env The R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return A JaccardSearch object for fragment-based searches.
   */
   function jaccardSet(libname: string, mz: number, mzset: string, rt?: number, cutoff?: number, filter_complex_adducts?: boolean, env?: object): object;
   /**
    * Creates a new reference spectrum database file for storing spectral data.
    * 
    * 
     * @param file The file path where the spectrum database will be saved.
     * @param type The type of cluster structure to create (Pack, Binary, or Tree).
     * 
     * + default value Is ``null``.
     * @param env The R environment for error handling and output.
     * 
     * + default value Is ``null``.
     * @return An instance of ReferenceTree, ReferenceBinaryTree, or SpectrumPack based on the specified type.
   */
   function new(file: any, type?: object, env?: object): object|object|object;
   /**
    * ### open the spectrum reference database
    *  
    *  open the reference spectrum database file and 
    *  then create a host to run spectrum cluster 
    *  search
    * 
    * > the data format is test via the magic header
    * 
     * @param file -
     * @param dotcutoff 
     * + default value Is ``0.6``.
     * @param adducts the precursor types for build the mass index for the reference library, this 
     *  parameter is required for reference library model in stream pack object type.
     * 
     * + default value Is ``["[M]+","[M+H]+"]``.
     * @param target_uuid a character vector of the target metabolite biodeep_id, default value
     *  is NULL means load all reference spectrum from the required reference 
     *  database file. this function will just load a subset of the reference 
     *  spectrum data from the database file is this parameter value is not 
     *  NULL.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return the reference library object in different search mode, all library object 
     *  is inherits based on the @``T:BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query.Ms2Search`` object.
   */
   function open(file: any, dotcutoff?: number, adducts?: any, target_uuid?: any, env?: object): object|object;
   /**
    * Enables or disables parallel processing for spectral searches.
    * 
    * > this function only works for the @``T:BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib.PackAlignment`` method.
    * 
     * @param search The PackAlignment object to configure.
     * @param enable TRUE to enable parallel processing, FALSE otherwise.
     * @return The modified PackAlignment object.
   */
   function parallel(search: any, enable: boolean): any;
   /**
    * do spectrum family alignment via cos similarity
    * 
    * 
     * @param tree The reference spectrum tree object to search
     * @param x The query spectrum data from the sample raw data files
     * @param maxdepth The max depth of the tree search
     * 
     * + default value Is ``1024``.
     * @param treeSearch Do alignment in family tree search mode?
     * 
     * + default value Is ``false``.
     * @param top_hits the top n hits of the candidate result populated for the each query input,
     *  set this parameter value to zero or negative value means no limits.
     * 
     * + default value Is ``3``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return function returns nothing means no query hits or the 
     *  given input query sample data **`x`**
   */
   function query(tree: object, x: any, maxdepth?: object, treeSearch?: boolean, top_hits?: object, env?: object): object;
   /**
    * open the spectrum pack reference database file
    * 
    * 
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function readpack(file: any, env?: object): object;
   /**
    * Retrieves the top candidate matches from a metabolite library search.
    * 
    * 
     * @param libs The metabolite library (Library(Of MetaLib)).
     * @param x The query spectrum (PeakMs2 or GCMSPeak).
     * @param top The maximum number of top candidates to return.
     * 
     * + default value Is ``9``.
     * @return An array of AlignmentOutput objects representing top matches.
   */
   function top_candidates(libs: object, x: any, top?: object): object;
}
