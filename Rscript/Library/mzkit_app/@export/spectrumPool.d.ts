// export R# package module type define for javascript/typescript language
//
//    imports "spectrumPool" from "mzDIA";
//
// ref=mzkit.MolecularSpectrumPool@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Spectrum clustering/inference via molecule networking method, 
 *  this api module is working with the biodeep public cloud service
 * 
*/
declare namespace spectrumPool {
   /**
    * add sample peaks data to spectrum pool
    * 
    * > the spectrum data for run clustering should be 
    * >  processed into centroid mode at first!
    * 
     * @param pool -
     * @param x the spectrum data collection
     * @param biosample -
     * 
     * + default value Is ``'unknown'``.
     * @param organism -
     * 
     * + default value Is ``'unknown'``.
     * @param project 
     * + default value Is ``'unknown'``.
     * @param instrument 
     * + default value Is ``'unknown'``.
     * @param file 
     * + default value Is ``'unknown'``.
     * @param filename_overrides 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function addPool(pool: object, x: any, biosample?: string, organism?: string, project?: string, instrument?: string, file?: string, filename_overrides?: boolean, env?: object): any;
   /**
    * close the connection to the spectrum pool
    * 
    * > this function works for the spectrum clustering pool in local file,
    * >  do nothing when running upon based on a cloud service.
    * 
     * @param pool -
   */
   function closePool(pool: object): any;
   /**
    * commit data to the spectrum pool database
    * 
    * > this function works for the spectrum molecular networking pool in a local file,
    * >  do nothing when running upon a cloud service
    * 
     * @param pool -
   */
   function commit(pool: object): object;
   /**
    * generates the guid for the spectrum with unknown annotation
    * 
    * > the conserved guid is generated based on the md5 hashcode of contents:
    * >  
    * >  1. mz(F4):into
    * >  2. mz1(F4)
    * >  3. rt(F2)
    * >  4. biosample
    * >  5. organism
    * >  6. instrument
    * >  7. precursor_type
    * 
     * @param spectral -
     * @param env 
     * + default value Is ``null``.
   */
   function conservedGuid(spectral: any, env?: object): string;
   /**
    * create a new spectrum clustering data pool
    * 
    * 
     * @param link -
     * @param level -
     * 
     * + default value Is ``0.9``.
     * @param split hex, max=15
     * 
     * + default value Is ``9``.
     * @param name 
     * + default value Is ``'no_named'``.
     * @param desc 
     * + default value Is ``'no_information'``.
   */
   function createPool(link: string, level?: number, split?: object, name?: string, desc?: string): object;
   /**
    * get metadata dataframe in a given cluster tree
    * 
    * 
     * @param pool -
     * @param path -
     * 
     * + default value Is ``null``.
     * @return A dataframe object that contains the metadata of each spectrum inside the given 
     *  cluster tree, this includes:
     *  
     *  1. biodeep_id: metabolite unique reference id inside the biodeep database
     *  2. name: the metabolite common name
     *  3. formula: the chemical formula of the current metabolite
     *  4. adducts: the precursor adducts of the metabolite addociated with the spectrum precursor ion
     *  5. mz: the precursor ion m/z
     *  6. rt: the lcms rt in data unit of seconds
     *  7. intensity: the ion intensity value
     *  8. source: the rawdata file source of current spectrum ion comes from
     *  9. biosample: the biological sample source
     *  10. organism: the biological species source
     *  11. project: the public project id, example as the metabolights project id
     *  12. instrument: the instrument name of the spectrum, could be extract from the metabolights project metadata.
   */
   function getClusterInfo(pool: object, path?: string): any;
   /**
    * Infer and make annotation to a specific cluster
    * 
    * > workflow for reference id inference: the alignment should be perfermance
    * >  at first for the cluster spectrum and the reference specturm, and then
    * >  get the reference id list as candidates, then finally use this function
    * >  for the inference analysis.
    * 
     * @param dia -
     * @param cluster_id -
     * @param reference_id the spectrum reference id, if this parameter is missing, then 
     *  the spectrum inference will be based on the reference cluster hits
     *  annotation result
     * 
     * + default value Is ``null``.
     * @param formula 
     * + default value Is ``null``.
     * @param name 
     * + default value Is ``null``.
   */
   function infer(dia: object, cluster_id: string, reference_id?: string, formula?: string, name?: string): object;
   /**
    * Create a spectrum inference protocol workflow
    * 
    * 
     * @param url -
     * @param model_id -
     * @param ms1diff 
     * + default value Is ``'da:0.3'``.
     * @param ms2diff 
     * + default value Is ``'da:0.3'``.
     * @param intocutoff 
     * + default value Is ``0.05``.
   */
   function load_infer(url: string, model_id: string, ms1diff?: string, ms2diff?: string, intocutoff?: number): object;
   /**
    * get model id from the spectrum cluster graph model
    * 
    * 
     * @param pool -
   */
   function model_id(pool: object): string;
   /**
    * open the spectrum pool from a given resource link
    * 
    * 
     * @param link the resource string to the spectrum pool, this resource string could be
     *  a local file or a remote cloud services endpoint
     * @param model_id the model id, this parameter works for open the model in the cloud service
     * 
     * + default value Is ``null``.
     * @param score_overrides WARNING: this optional parameter will overrides the mode score 
     *  level when this parameter has a positive numeric value in 
     *  range ``(0,1]``. it is dangers to overrides the score parameter
     *  in the exists model.
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function openPool(link: string, model_id?: string, score_overrides?: number, env?: object): object;
   /**
    * generate and set conserved guid for each spectrum data
    * 
    * 
     * @param spectral -
     * @param prefix -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A collection of the mzkit @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.PeakMs2`` clr object
     *  which has the lib guid data assigned.
   */
   function set_conservedGuid(spectral: any, prefix?: string, env?: object): any;
}
