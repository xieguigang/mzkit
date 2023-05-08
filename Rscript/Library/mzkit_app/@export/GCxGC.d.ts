// export R# package module type define for javascript/typescript language
//
// ref=mzkit.GCxGC@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Processing GCxGC comprehensive chromatogram data
 * 
*/
declare namespace GCxGC {
   /**
   */
   function TIC2D(TIC:object, modtime:number): object;
   /**
   */
   function TIC1D(matrix:object): object;
   /**
    * extract GCxGC 2d peaks from the mzpack raw data file
    * 
    * > this function will extract the TIC data by default.
    * 
     * @param raw -
     * @param mz target mz value for extract XIC data. NA means extract 
     *  TIC data by default.
     * 
     * + default value Is ``NaN``.
     * @param mzdiff the mz tolerance error for match the intensity data for
     *  extract XIC data if the **`mz`** is not 
     *  ``NA`` value.
     * 
     * + default value Is ``'ppm:30'``.
     * @param env 
     * + default value Is ``null``.
   */
   function extract_2D_peaks(raw:object, mz?:number, mzdiff?:any, env?:object): object;
   module save {
      /**
       * save GCxGC 2D Chromatogram data as a new netcdf file.
       * 
       * 
        * @param TIC -
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function cdf(TIC:object, file:any, env?:object): any;
   }
   module read {
      /**
       * read GCxGC 2D Chromatogram data from a given netcdf file.
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function cdf(file:any, env?:object): object;
   }
}
