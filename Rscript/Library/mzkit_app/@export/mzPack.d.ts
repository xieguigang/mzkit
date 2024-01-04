// export R# package module type define for javascript/typescript language
//
//    imports "mzPack" from "mzkit";
//
// ref=mzkit.MzPackAccess@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace mzPack {
   /**
     * @param env default value Is ``null``.
   */
   function convertTo_mzXML(mzpack: object, file: any, env?: object): boolean;
   /**
     * @param env default value Is ``null``.
   */
   function getSampleTags(mzpack: any, env?: object): string;
   /**
     * @param env default value Is ``null``.
   */
   function ls(mzpack: any, env?: object): string;
   /**
   */
   function metadata(mzpack: object, index: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function mzpack(file: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function mzwork(file: any, env?: object): object;
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function mzwork(mzwork: string, env?: object): object;
   }
   /**
     * @param scan_id1 default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function pack_ms1(ms2: any, scan_id1?: string, env?: object): object;
   /**
     * @param timeWindow default value Is ``1``.
     * @param pack_singleCells default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function packData(data: any, timeWindow?: number, pack_singleCells?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function packStream(data: object, file: any, env?: object): any;
   /**
     * @param single default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function readFileCache(mzwork: object, fileName: string, single?: boolean, env?: object): object;
   /**
     * @param cut default value Is ``2``.
     * @param env default value Is ``null``.
   */
   function removeSciexNoise(raw: any, cut?: object, env?: object): any;
   /**
   */
   function scaninfo(mzpack: object, index: string): object;
   /**
   */
   function split_samples(mzpack: string): any;
}
