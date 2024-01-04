// export R# package module type define for javascript/typescript language
//
//    imports "spectrumPool" from "mzDIA";
//
// ref=mzkit.MolecularSpectrumPool@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace spectrumPool {
   /**
     * @param biosample default value Is ``'unknown'``.
     * @param organism default value Is ``'unknown'``.
     * @param project default value Is ``'unknown'``.
     * @param instrument default value Is ``'unknown'``.
     * @param file default value Is ``'unknown'``.
     * @param filename_overrides default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function addPool(pool: object, x: any, biosample?: string, organism?: string, project?: string, instrument?: string, file?: string, filename_overrides?: boolean, env?: object): any;
   /**
   */
   function closePool(pool: object): any;
   /**
   */
   function commit(pool: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function conservedGuid(spectral: any, env?: object): string;
   /**
     * @param level default value Is ``0.9``.
     * @param split default value Is ``9``.
     * @param name default value Is ``'no_named'``.
     * @param desc default value Is ``'no_information'``.
   */
   function createPool(link: string, level?: number, split?: object, name?: string, desc?: string): object;
   /**
     * @param path default value Is ``null``.
   */
   function getClusterInfo(pool: object, path?: string): any;
   /**
     * @param reference_id default value Is ``null``.
     * @param formula default value Is ``null``.
     * @param name default value Is ``null``.
   */
   function infer(dia: object, cluster_id: string, reference_id?: string, formula?: string, name?: string): object;
   /**
     * @param ms1diff default value Is ``'da:0.3'``.
     * @param ms2diff default value Is ``'da:0.3'``.
     * @param intocutoff default value Is ``0.05``.
   */
   function load_infer(url: string, model_id: string, ms1diff?: string, ms2diff?: string, intocutoff?: number): object;
   /**
   */
   function model_id(pool: object): string;
   /**
     * @param model_id default value Is ``null``.
     * @param score_overrides default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function openPool(link: string, model_id?: string, score_overrides?: number, env?: object): object;
   /**
     * @param prefix default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function set_conservedGuid(spectral: any, prefix?: string, env?: object): any;
}
