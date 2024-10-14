// export R# package module type define for javascript/typescript language
//
//    imports "GCxGC" from "mzkit";
//
// ref=mzkit.GCxGC@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Comprehensive two-dimensional gas chromatography
 *  
 *  Processing GCxGC comprehensive chromatogram data: Comprehensive Two-dimensional gas chromatography, 
 *  or GC×GC is a multidimensional gas chromatography technique that was originally described in 1984 
 *  by J. Calvin Giddings and first successfully implemented in 1991 by Professor Phillips and his 
 *  student Zaiyou Liu.
 * 
 *  GC×GC utilizes two different columns With two different stationary phases. In GC×GC, all Of the 
 *  effluent from the first dimension column Is diverted To the second dimension column via a modulator. 
 *  The modulator quickly traps, Then "injects" the effluent from the first dimension column onto the second 
 *  dimension. This process creates a retention plane Of the 1St dimension separation x 2nd dimension 
 *  separation.
 * 
 *  The Oil And Gas Industry were early adopters Of the technology For the complex oil samples To determine
 *  the many different types Of Hydrocarbons And its isomers. Nowadays In these types Of samples it has been 
 *  reported that over 30000 different compounds could be identified In a crude oil With this Comprehensive 
 *  Chromatography Technology (CCT).
 * 
 *  The CCT evolved from a technology only used In academic R&D laboratories, into a more robust technology 
 *  used In many different industrial labs. Comprehensive Chromatography Is used In forensics, food And flavor, 
 *  environmental, metabolomics, biomarkers And clinical applications. Some Of the most well-established 
 *  research groups In the world that are found In Australia, Italy, the Netherlands, Canada, United States,
 *  And Brazil use this analytical technique.
 * 
*/
declare namespace GCxGC {
   /**
    * Demodulate the 1D rawdata input as 2D data
    * 
    * 
     * @param rawdata -
     * @param modtime -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function demodulate_2D(rawdata: any, modtime: number, env?: object): object;
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
   function extract_2D_peaks(raw: object, mz?: number, mzdiff?: any, env?: object): object;
   module read {
      /**
       * read GCxGC 2D Chromatogram data from a given netcdf file.
       * 
       * 
        * @param file this function used for parse the cdf file format for both mzkit format or LECO format
        * @param env -
        * 
        * + default value Is ``null``.
        * @return A data model for GCxGC 2d chromatogram
      */
      function cdf(file: any, env?: object): object;
   }
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
      function cdf(TIC: object, file: any, env?: object): boolean;
   }
   /**
   */
   function TIC1D(matrix: object): object;
   /**
    * Demodulate the 1D TIC to 2D data
    * 
    * 
     * @param TIC -
     * @param modtime The time required to complete a cycle is called the period of modulation (modulation time)
     *  and is actually the time in between two hot pulses, which typically lasts between 2 and 10 
     *  seconds is related to the time needed for the compounds to eluted in 2D.
   */
   function TIC2D(TIC: object, modtime: number): object;
}
