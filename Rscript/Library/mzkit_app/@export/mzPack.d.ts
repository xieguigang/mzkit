// export R# package module type define for javascript/typescript language
//
// ref=mzkit.MzPackAccess@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * raw data accessor for the mzpack data object
 * 
*/
declare namespace mzPack {
   /**
    * method for write mzpack data object as a mzML file
    * 
    * 
     * @param mzpack -
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function convertTo_mzXML(mzpack:object, file:any, env?:object): boolean;
   /**
   */
   function getSampleTags(mzpack:string): string;
   /**
    * show all ms1 scan id in a mzpack data object or 
    *  show all raw data file names in a mzwork data 
    *  package.
    * 
    * 
     * @param mzpack -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function ls(mzpack:any, env?:object): string;
   /**
    * get metadata list from a specific ms1 scan
    * 
    * 
     * @param mzpack -
     * @param index the scan id of the target ms1 scan data
   */
   function metadata(mzpack:object, index:string): object;
   /**
    * open a mzpack data object reader, not read all data into memory in one time.
    * 
    * > a in-memory reader wrapper will be created if the given file object 
    * >  is a in-memory mzpack object itself
    * 
     * @param file the file path for the mzpack file or the mzpack data object it self
     * @param env -
     * 
     * + default value Is ``null``.
     * @return the ms scan data can be load into memory in lazy 
     *  require by a given scan id of the target ms1 scan
   */
   function mzpack(file:any, env?:object): object;
   /**
    * open a mzwork package file
    * 
    * 
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function mzwork(file:any, env?:object): object;
   module open {
      /**
       * open mzwork file and then populate all of the mzpack raw data file
       * 
       * 
        * @param mzwork -
        * @param env 
        * + default value Is ``null``.
        * @return a collection of mzpack raw data objects
      */
      function mzwork(mzwork:string, env?:object): object;
   }
   /**
    * pack mzkit ms2 peaks data as a mzpack data object
    * 
    * 
     * @param data -
     * @param timeWindow -
     * 
     * + default value Is ``1``.
     * @param pack_singleCells 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function packData(data:any, timeWindow?:number, pack_singleCells?:boolean, env?:object): object;
   /**
    * write mzPack in v2 format
    * 
    * 
     * @param data -
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function packStream(data:object, file:any, env?:object): any;
   /**
    * read mzpack data from the mzwork package by a 
    *  given raw data file name as reference id
    * 
    * 
     * @param mzwork -
     * @param fileName -
     * @param single 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function readFileCache(mzwork:object, fileName:string, single?:boolean, env?:object): object;
   /**
     * @param env default value Is ``null``.
   */
   function removeSciexNoise(raw:any, env?:object): any;
   /**
   */
   function scaninfo(mzpack:object, index:string): object;
   /**
   */
   function split_samples(mzpack:string): any;
}
