// export R# package module type define for javascript/typescript language
//
//    imports "GCxGC" from "mzkit";
//
// ref=mzkit.GCxGC@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace GCxGC {
   /**
     * @param env default value Is ``null``.
   */
   function demodulate_2D(rawdata: any, modtime: number, env?: object): object;
   /**
     * @param mz default value Is ``NaN``.
     * @param mzdiff default value Is ``'ppm:30'``.
     * @param env default value Is ``null``.
   */
   function extract_2D_peaks(raw: object, mz?: number, mzdiff?: any, env?: object): object;
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function cdf(file: any, env?: object): object;
   }
   module save {
      /**
        * @param env default value Is ``null``.
      */
      function cdf(TIC: object, file: any, env?: object): boolean;
   }
   /**
   */
   function TIC1D(matrix: object): object;
   /**
   */
   function TIC2D(TIC: object, modtime: number): object;
}
