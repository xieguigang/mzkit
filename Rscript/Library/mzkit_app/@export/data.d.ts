// export R# package module type define for javascript/typescript language
//
//    imports "data" from "mzkit";
//
// ref=mzkit.data@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace data {
   /**
     * @param mz default value Is ``-1``.
     * @param mzdiff default value Is ``'da:0.3'``.
     * @param env default value Is ``null``.
   */
   function intensity(ticks: any, mz?: number, mzdiff?: any, env?: object): number;
   /**
     * @param matrix default value Is ``null``.
     * @param title default value Is ``'MS Matrix'``.
     * @param parentMz default value Is ``-1``.
     * @param centroid default value Is ``false``.
     * @param args default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function libraryMatrix(matrix?: any, title?: string, parentMz?: number, centroid?: boolean, args?: object, env?: object): any;
   /**
     * @param topIons default value Is ``5``.
   */
   function linearMatrix(data: object, topIons?: object): string;
   module make {
      /**
        * @param name_chrs default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function ROI_names(ROIlist: any, name_chrs?: boolean, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function nsize(matrix: any, env?: object): object;
   /**
     * @param totalIons default value Is ``0``.
     * @param file default value Is ``null``.
     * @param libname default value Is ``null``.
     * @param precursor_type default value Is ``null``.
     * @param meta default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function peakMs2(precursor: number, rt: number, mz: number, into: number, totalIons?: number, file?: string, libname?: string, precursor_type?: string, meta?: object, env?: object): object;
   module read {
      /**
      */
      function MsMatrix(file: string): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function rt_slice(ms1: any, rtmin: number, rtmax: number, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function scan_time(ticks: any, env?: object): number;
   /**
     * @param tolerance default value Is ``'da:0.3'``.
     * @param intocutoff default value Is ``0.05``.
     * @param similarity_cutoff default value Is ``0.3``.
     * @param env default value Is ``null``.
   */
   function search(q: any, refer: object, tolerance?: any, intocutoff?: number, similarity_cutoff?: number, env?: object): object;
   /**
     * @param type default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function splash_id(spec: any, type?: object, env?: object): any;
   /**
     * @param matrix default value Is ``false``.
     * @param massDiff default value Is ``0.1``.
   */
   function unionPeaks(peaks: object, matrix?: boolean, massDiff?: number): object|object;
   /**
     * @param tolerance default value Is ``'ppm:20'``.
     * @param env default value Is ``null``.
   */
   function XIC(ms1: any, mz: number, tolerance?: any, env?: object): object|object;
   /**
     * @param tolerance default value Is ``'ppm:20'``.
     * @param env default value Is ``null``.
   */
   function XIC_groups(ms1: any, tolerance?: any, env?: object): any;
}
