// export R# package module type define for javascript/typescript language
//
// ref=mzkit.MzWeb@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * biodeep mzweb data viewer raw data file helper
 * 
*/
declare namespace mzweb {
   /**
     * @param env default value Is ``null``.
   */
   function loadXcmsRData(file:any, env?:object): object;
   module open {
      /**
       * open a raw data files in common raw data format and then returns 
       *  in a unify mzpack data format.
       * 
       * 
        * @param file the ``*.mzXML``/``*.mzML``/``*.mzPack``/``*.raw`` raw data file
        * @param env 
        * + default value Is ``null``.
      */
      function mzpack(file:any, env?:object): object;
   }
   /**
   */
   function TIC(mzpack:object): object;
   module load {
      /**
       * load chromatogram data from the raw file data
       * 
       * 
        * @param scans the scan data object that reads from the mzXML/mzML/mzPack raw data file
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function chromatogram(scans:any, env?:object): object;
      /**
       * load the unify mzweb scan stream data from the mzml/mzxml raw scan data stream.
       * 
       * 
        * @param scans -
        * @param mzErr 
        * + default value Is ``'da:0.1'``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function stream(scans:object, mzErr?:string, env?:object): object;
   }
   module write {
      /**
       * write ASCII text format of mzweb stream
       * 
       * 
        * @param scans -
        * @param file -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function text_cache(scans:object, file?:any, env?:object): any;
      /**
       * write binary format of mzweb stream data
       * 
       * 
        * @param file -
      */
      function cache(ions:object, file:string): boolean;
      /**
       * write version 2 format of the mzpack by default
       * 
       * 
        * @param mzpack -
        * @param file -
        * @param version 
        * + default value Is ``2``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mzPack(mzpack:object, file:any, version?:object, env?:object): any;
      /**
        * @param Ms2Only default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function cdf(mzpack:object, file:any, Ms2Only?:boolean, env?:object): any;
   }
   /**
    * write binary format of mzweb stream data
    * 
    * 
     * @param file -
     * @param cache -
   */
   function packBin(file:string, cache:string): ;
   module read {
      /**
       * read the mzPack data file liked simple msn cached data
       * 
       * 
        * @param file -
      */
      function cache(file:string): object;
   }
   module open_mzpack {
      /**
       * open mzpack data from a raw data file in xml file format.
       * 
       * 
        * @param file -
        * @param prefer the prefer file format used when the given **`file`** its extension
        *  suffix name is ``XML``. value of this parameter could be imzml/mzml/mzxml
        * 
        * + default value Is ``null``.
      */
      function xml(file:string, prefer?:string): object;
   }
   /**
    * set thumbnail image to the raw data file
    * 
    * > the parameter value of the **`thumb`** lambda
    * >  function will be **`mzpack`** parameter value
    * >  input
    * 
     * @param mzpack -
     * @param thumb Thumbnail image object data can be a gdi+ image or 
     *  bitmap or a gdi+ canvas object in type @``T:Microsoft.VisualBasic.Imaging.Driver.ImageData``.
     *  And also this parameter could be a lambda function that
     *  could be used for invoke and generates the image data.
     * @param env 
     * + default value Is ``null``.
     * @return returns a modified mzpack data object with Thumbnail 
     *  property data has been updated.
   */
   function setThumbnail(mzpack:object, thumb:any, env?:object): object;
   /**
    * get all ms1 scan data points
    * 
    * 
     * @param mzpack -
   */
   function ms1_scans(mzpack:object): object;
   /**
    * get a overview ms1 spectrum data from the mzpack raw data
    * 
    * 
     * @param mzpack -
     * @param tolerance -
     * 
     * + default value Is ``'da:0.001'``.
     * @param cutoff -
     * 
     * + default value Is ``0.05``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function ms1_peaks(mzpack:object, tolerance?:any, cutoff?:number, env?:object): object;
   /**
    * extract ms2 peaks data from the mzpack data object
    * 
    * 
     * @param mzpack -
     * @param precursorMz if the precursor m/z data is assign by this parameter
     *  value, then this function will extract the ms2 xic data
     *  only
     * 
     * + default value Is ``NaN``.
     * @param tolerance ppm toleracne error for extract ms2 xic data.
     * 
     * + default value Is ``'ppm:30'``.
     * @param tag_source 
     * + default value Is ``true``.
     * @param centroid and also convert the data to centroid mode?
     * 
     * + default value Is ``false``.
     * @param norm 
     * + default value Is ``false``.
     * @param filter_empty 
     * + default value Is ``true``.
     * @param into_cutoff 
     * + default value Is ``0``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function ms2_peaks(mzpack:object, precursorMz?:number, tolerance?:any, tag_source?:boolean, centroid?:boolean, norm?:boolean, filter_empty?:boolean, into_cutoff?:any, env?:object): object;
   module as {
      /**
       * convert assembly file to mzpack format data object
       * 
       * 
        * @param assembly -
        * @param modtime [GCxGC]
        *  the modulation time of the chromatographic run. 
        *  modulation period in time unit 'seconds'.
        * 
        * + default value Is ``-1``.
        * @param sample_rate [GCxGC]
        *  the sampling rate of the equipment.
        *  If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
        *  the difference of two adjacent scan time.
        * 
        * + default value Is ``NaN``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mzpack(assembly:any, modtime?:number, sample_rate?:number, env?:object): object;
   }
   /**
    * do mass calibration
    * 
    * 
     * @param data -
     * @param mzdiff mass tolerance in delta dalton
     * 
     * + default value Is ``0.1``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function mass_calibration(data:object, mzdiff?:number, env?:object): object;
}
