// export R# package module type define for javascript/typescript language
//
//    imports "xcms" from "mz_quantify";
//
// ref=mzkit.xcms@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the xcms interop and data handler
 * 
*/
declare namespace xcms {
   /**
    * cast the xcms find peaks result raw dataframe to mzkit peak feature data
    * 
    * 
     * @param x A dataframe result of the xcms ``findPeaks``. data could be generated via ``as.data.frame(findPeaks(data));``.
     *  this raw data frame output contains data fields:
     *  
     *  1. mz, mzmin, mzmax
     *  2. rt, rtmin, rtmax
     *  3. into, intf
     *  4. maxo, maxf
     *  5. i
     *  6. sn
     * @param sample_name 
     * + default value Is ``null``.
   */
   function cast_findpeaks_raw(x: object, sample_name?: string): object;
   /**
    * Parse the input file as the mzkit peakset object
    * 
    * 
     * @param file -
     * @param group_features This function returns a xcms sample peaks collection for directly mapping of the input tabular file data. 
     *  set this parameter to value true, will returns a tuple list object that contains the mzkit
     *  peak feature groups, which is group by the sample names.
     * 
     * + default value Is ``false``.
     * @param deli 
     * + default value Is ``','``.
     * @param normalizeID 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function parse_xcms_samples(file: any, group_features?: boolean, deli?: string, normalizeID?: boolean, env?: object): object|object;
   /**
    * set annotation to the ion features
    * 
    * 
     * @param peaktable -
     * @param id should be a character vector of the ion reference id
     * @param annotation should be a collection of the metabolite annotation model @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations.MetID``, 
     *  size of this collection should be equals to the size of the given id vector.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function set_annotations(peaktable: object, id: any, annotation: any, env?: object): object;
}
