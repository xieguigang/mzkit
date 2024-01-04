// export R# package module type define for javascript/typescript language
//
//    imports "assembly" from "mzkit";
//
// ref=mzkit.Assembly@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace assembly {
   module file {
      /**
      */
      function index(ms2: object): string;
   }
   /**
   */
   function load_index(file: string): object;
   module mgf {
      /**
        * @param lazy default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function ion_peaks(ions: any, lazy?: boolean, env?: object): object;
   }
   module ms1 {
      /**
        * @param centroid default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function scans(raw: any, centroid?: any, env?: object): object;
   }
   module mzxml {
      /**
        * @param relativeInto default value Is ``false``.
        * @param onlyMs2 default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function mgf(file: string, relativeInto?: boolean, onlyMs2?: boolean, env?: object): object;
   }
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function xml_seek(file: string, env?: object): any;
   }
   /**
     * @param env default value Is ``null``.
   */
   function polarity(scans: any, env?: object): object;
   module raw {
      /**
        * @param env default value Is ``null``.
      */
      function scans(file: string, env?: object): object|object;
   }
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function mgf(file: any, env?: object): object;
      /**
        * @param unit default value Is ``null``.
      */
      function msl(file: string, unit?: object): object;
      /**
        * @param parseMs2 default value Is ``true``.
      */
      function msp(file: string, parseMs2?: boolean): any;
   }
   /**
   */
   function scan_id(file: object): string;
   /**
   */
   function seek(file: object, key: string): object;
   module write {
      /**
        * @param relativeInto default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function mgf(ions: any, file: string, relativeInto?: boolean, env?: object): boolean;
   }
}
