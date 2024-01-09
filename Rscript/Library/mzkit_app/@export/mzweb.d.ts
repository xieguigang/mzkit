// export R# package module type define for javascript/typescript language
//
//    imports "mzweb" from "mzkit";
//
// ref=mzkit.MzWeb@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * biodeep mzweb data viewer raw data file helper
 * 
*/
declare namespace mzweb {
   module as {
      /**
       * convert assembly file to mzpack format data object
       * 
       * 
        * @param assembly -
        * @param args 1. modtime: [GCxGC] the modulation time of the chromatographic run. 
        *     modulation period in time unit 'seconds'.
        *  2. sample_rate: [GCxGC] the sampling rate of the equipment.
        *     If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
        *     the difference of two adjacent scan time.
        *  3. imzml: [MS-Imaging] the pixel spot scan metadata collection for
        *     read ms data from the ibd file, the corresponding assembly object should
        *     be a @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.ibdReader`` object
        *  4. dims: [MS-Imaging] the canvas dimension size value for the ms-imaging
        *     heatmap rendering
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function mzpack(assembly: any, args?: object, env?: object): object;
   }
   module load {
      /**
       * load chromatogram data from the raw file data
       * 
       * 
        * @param scans the scan data object that reads from the mzXML/mzML/mzPack raw data file
        * @param env -
        * 
        * + default value Is ``null``.
        * @return the chromatogram data wrapper of TIC/BPC
      */
      function chromatogram(scans: any, env?: object): object;
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
      function stream(scans: object, mzErr?: string, env?: object): object;
   }
   /**
    * load the xcms cache dataset
    * 
    * 
     * @param file -
     * @param env -
     * 
     * + default value Is ``null``.
     * @return this function get a set of the spectrum peak ms2 data from the given R dataset
   */
   function loadXcmsRData(file: any, env?: object): object;
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
   function mass_calibration(data: object, mzdiff?: number, env?: object): object;
   /**
    * get a overview ms1 spectrum data from the mzpack raw data
    * 
    * 
     * @param mzpack The mzpack rawdata object
     * @param tolerance The mass tolerance error
     * 
     * + default value Is ``'da:0.001'``.
     * @param cutoff intensity cutoff percentage value for removes the noised liked peaks.
     * 
     * + default value Is ``0.05``.
     * @param ionset A numeric vector for make subset of the ion features 
     *  which is extract from the input mzpack rawdata file
     *  object.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A ms peaks object
   */
   function ms1_peaks(mzpack: object, tolerance?: any, cutoff?: number, ionset?: any, env?: object): object;
   /**
    * get all ms1 scan data points
    * 
    * 
     * @param mzpack -
   */
   function ms1_scans(mzpack: object): object;
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
   function ms2_peaks(mzpack: object, precursorMz?: number, tolerance?: any, tag_source?: boolean, centroid?: boolean, norm?: boolean, filter_empty?: boolean, into_cutoff?: any, env?: object): object;
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
      function mzpack(file: any, env?: object): object;
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
        * @param env 
        * + default value Is ``null``.
      */
      function xml(file: string, prefer?: string, env?: object): object;
   }
   /**
    * write binary format of mzweb stream data
    * 
    * 
     * @param file -
     * @param cache -
   */
   function packBin(file: string, cache: string): ;
   module parse {
      /**
       * Parse the ms scan data from a given raw byte stream data
       * 
       * 
        * @param bytes the raw vector which could be parsed from the HDS file via HDS read data function.
        * @param level specific the ms level to parse the scan data, level could be 1(scan ms1) or 2(scan ms2)
        * 
        * + default value Is ``[1,2]``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function scanMs(bytes: any, level?: any, env?: object): any;
   }
   module read {
      /**
       * read the mzPack data file liked simple msn cached data
       * 
       * 
        * @param file The cache file path or the file binary data buffer object
        * @param env 
        * + default value Is ``null``.
        * @return A vector of the mzkit peakms2 object
      */
      function cache(file: any, env?: object): object;
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
   function setThumbnail(mzpack: object, thumb: any, env?: object): object;
   /**
    * Get TIC from the mzpack layer reader
    * 
    * 
     * @param mzpack -
   */
   function TIC(mzpack: object): object;
   module write {
      /**
       * write binary format of mzweb stream data
       * 
       * 
        * @param file -
        * @param env 
        * + default value Is ``null``.
      */
      function cache(ions: object, file: any, env?: object): boolean;
      /**
        * @param Ms2Only default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function cdf(mzpack: object, file: any, Ms2Only?: boolean, env?: object): any;
      /**
       * write version 2 format of the mzpack by default
       * 
       * 
        * @param mzpack -
        * @param file -
        * @param version 
        * + default value Is ``2``.
        * @param headerSize negative value or zero means auto-evaluated via the different file size
        * 
        * + default value Is ``-1``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mzPack(mzpack: object, file: any, version?: object, headerSize?: object, env?: object): any;
      /**
       * write ASCII text format of mzweb stream
       * 
       * > this method used for create ascii text package data for the biodeep
       * >  web application js code to read the ms rawdata.
       * 
        * @param scans -
        * @param file -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function text_cache(scans: object, file?: any, env?: object): any;
   }
}
