// export R# package module type define for javascript/typescript language
//
//    imports "spectrumTree" from "mzkit";
//
// ref=mzkit.ReferenceTreePkg@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace spectrumTree {
   /**
     * @param ignore_error default value Is ``false``.
     * @param args default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function addBucket(tree: any, x: any, ignore_error?: boolean, args?: object, env?: object): any;
   /**
     * @param nspec default value Is ``5``.
     * @param xrefDb default value Is ``null``.
     * @param test default value Is ``-1``.
     * @param env default value Is ``null``.
   */
   function compress(spectrumLib: object, file: any, metadb: object, nspec?: object, xrefDb?: string, test?: object, env?: object): any;
   /**
   */
   function dotcutoff(search: object, cutoff: number): object;
   /**
     * @param n default value Is ``30``.
     * @param rtmax default value Is ``840``.
     * @param source_name default value Is ``'get_testSample'``.
   */
   function get_testSample(packlib: object, n?: object, rtmax?: number, source_name?: string): object;
   /**
     * @param rt default value Is ``null``.
     * @param cutoff default value Is ``0.1``.
     * @param filter_complex_adducts default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function jaccardSet(libname: string, mz: number, mzset: string, rt?: number, cutoff?: number, filter_complex_adducts?: boolean, env?: object): object;
   /**
     * @param type default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function new(file: any, type?: object, env?: object): object|object|object;
   /**
     * @param dotcutoff default value Is ``0.6``.
     * @param adducts default value Is ``["[M]+","[M+H]+"]``.
     * @param target_uuid default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function open(file: any, dotcutoff?: number, adducts?: any, target_uuid?: any, env?: object): object|object;
   /**
     * @param maxdepth default value Is ``1024``.
     * @param treeSearch default value Is ``false``.
     * @param top_hits default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function query(tree: object, x: any, maxdepth?: object, treeSearch?: boolean, top_hits?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function readpack(file: any, env?: object): object;
}
