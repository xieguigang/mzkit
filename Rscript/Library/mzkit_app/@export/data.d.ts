// export R# package module type define for javascript/typescript language
//
// ref=mzkit.data@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * m/z data operator module
 * 
*/
declare namespace data {
   /**
    * get intensity value from the ion scan points
    * 
    * 
     * @param ticks -
     * @param mz 
     * + default value Is ``-1``.
     * @param mzdiff 
     * + default value Is ``'da:0.3'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function intensity(ticks:any, mz?:number, mzdiff?:any, env?:object): number;
   /**
    * Create a library matrix object
    * 
    * 
     * @param matrix for a dataframe object, should contains column data:
     *  mz, into and annotation.
     * @param title -
     * 
     * + default value Is ``'MS Matrix'``.
     * @param parentMz 
     * + default value Is ``-1``.
     * @param centroid 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function libraryMatrix(matrix:any, title?:string, parentMz?:number, centroid?:boolean, env?:object): any;
   /**
     * @param topIons default value Is ``5``.
   */
   function linearMatrix(data:object, topIons?:object): string;
   module make {
      /**
       * makes xcms_id format liked ROI unique id
       * 
       * 
        * @param ROIlist -
        * @param name_chrs just returns the ROI names character?
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function ROI_names(ROIlist:any, name_chrs?:boolean, env?:object): object;
   }
   /**
    * get the size of the target ms peaks
    * 
    * 
     * @param matrix -
     * @param env 
     * + default value Is ``null``.
   */
   function nsize(matrix:any, env?:object): object;
   /**
    * create a new ms2 peaks data object
    * 
    * 
     * @param precursor -
     * @param rt -
     * @param mz -
     * @param into -
     * @param totalIons -
     * 
     * + default value Is ``0``.
     * @param file -
     * 
     * + default value Is ``null``.
     * @param libname 
     * + default value Is ``null``.
     * @param meta -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakMs2(precursor:number, rt:number, mz:number, into:number, totalIons?:number, file?:string, libname?:string, meta?:object, env?:object): object;
   module read {
      /**
      */
      function MsMatrix(file:string): object;
   }
   /**
    * slice a region of ms1 scan data by a given rt window.
    * 
    * 
     * @param ms1 a sequence of ms1 scan data.
     * @param rtmin -
     * @param rtmax -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function rt_slice(ms1:any, rtmin:number, rtmax:number, env?:object): object;
   /**
    * get scan time value from the ion scan points
    * 
    * 
     * @param ticks -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function scan_time(ticks:any, env?:object): number;
   /**
     * @param matrix default value Is ``false``.
     * @param massDiff default value Is ``0.1``.
   */
   function unionPeaks(peaks:object, matrix?:boolean, massDiff?:number): object|object;
   /**
    * get chromatogram data for a specific metabolite with given m/z from the ms1 scans data.
    * 
    * 
     * @param ms1 a sequence data of ms1 scans
     * @param mz target mz value
     * @param tolerance tolerance value in unit ``ppm`` or ``da`` for 
     *  extract ``m/z`` data from the given ms1 ion 
     *  scans.
     * 
     * + default value Is ``'ppm:20'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function XIC(ms1:any, mz:number, tolerance?:any, env?:object): object|object;
   /**
    * grouping of the ms1 scan points by m/z data
    * 
    * 
     * @param ms1 -
     * @param tolerance the m/z diff tolerance value for grouping ms1 scan point 
     *  based on its ``m/z`` value
     * 
     * + default value Is ``'ppm:20'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function XIC_groups(ms1:any, tolerance?:any, env?:object): any;
}
