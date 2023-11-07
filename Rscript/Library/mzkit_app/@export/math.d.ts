// export R# package module type define for javascript/typescript language
//
//    imports "math" from "mzkit";
//    imports "math" from "mz_quantify";
//
// ref=mzkit.MzMath@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=mzkit.QuantifyMath@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * mass spectrometry data math toolkit
 * 
 * 
*/
declare namespace math {
   /**
    * #### Converts profiles peak data to peak data in centroid mode.
    *  
    *  profile and centroid in Mass Spectrometry?
    *  
    *  1. Profile means the continuous wave form in a mass spectrum.
    *    + Number of data points Is large.
    *  2. Centroid means the peaks in a profile data Is changed to bars.
    *    + location of the bar Is center of the profile peak.
    *    + height of the bar Is area of the profile peak.
    * 
    * 
     * @param ions value of this parameter could be 
     *  
     *  + a collection of peakMs2 data 
     *  + a library matrix data 
     *  + or a dataframe object which should contains at least ``mz`` and ``intensity`` columns.
     *  + or just a m/z vector
     *  + also could be a mzpack data object
     * @param tolerance 
     * + default value Is ``'da:0.1'``.
     * @param intoCutoff 
     * + default value Is ``0.05``.
     * @param parallel 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
     * @return Peaks data in centroid mode or a new m/z vector in centroid.
   */
   function centroid(ions: any, tolerance?: any, intoCutoff?: number, parallel?: boolean, env?: object): object|object|number;
   /**
    * Create a chromatogram data from a dataframe object
    * 
    * 
     * @param x Should be a dataframe object that contains 
     *  the required data field for construct the chromatogram data.
     * 
     * + default value Is ``null``.
     * @param time the column name for get the rt field vector data
     * 
     * + default value Is ``'Time'``.
     * @param into the column name for get the signal intensity field vector data
     * 
     * + default value Is ``'Intensity'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function chromatogram(x?: any, time?: any, into?: any, env?: object): object;
   module cluster {
      /**
       * get all nodes from the spectrum tree cluster result
       * 
       * 
        * @param tree the spectra molecule networking tree
      */
      function nodes(tree: object): object;
   }
   module cosine {
      /**
        * @param tolerance default value Is ``'da:0.3'``.
        * @param intocutoff default value Is ``0.05``.
        * @param env default value Is ``null``.
      */
      function pairwise(query: any, ref: any, tolerance?: any, intocutoff?: number, env?: object): any;
   }
   /**
    * returns all precursor types for a given libtype
    * 
    * 
     * @param ionMode -
   */
   function defaultPrecursors(ionMode: string): object;
   /**
    * evaluate all exact mass for all known precursor type.
    * 
    * 
     * @param mz -
     * @param mode -
     * 
     * + default value Is ``'+'``.
   */
   function exact_mass(mz: number, mode?: any): object;
   /**
    * Extract an intensity vector based on a given peak index
    * 
    * 
     * @param ms -
     * @param mzSet A peak index object, which could be generated based 
     *  on a given set of the peak m/z vector via the 
     *  function ``mz_index``.
     * @param env 
     * + default value Is ``null``.
     * @return the returns value of this function is based on the input **`ms`** data:
     *  
     *  1. for a single msdata object, then this function just returns a intensity numeric vector
     *  2. for a collection of the msdata object, then this function will returns a
     *     dataframe object that each row is the element corresponding in the input msdata
     *     collection and each column is the m/z peak intensity value across the input
     *     msdata collection.
   */
   function intensity_vec(ms: any, mzSet: object, env?: object): number;
   module ions {
      /**
       * data pre-processing helper, make the spectra ion data unique
       * 
       * 
        * @param ions -
        * @param eq 
        * + default value Is ``0.85``.
        * @param gt 
        * + default value Is ``0.6``.
        * @param mzwidth 
        * + default value Is ``'da:0.1'``.
        * @param tolerance 
        * + default value Is ``'da:0.3'``.
        * @param precursor 
        * + default value Is ``'ppm:20'``.
        * @param rtwidth 
        * + default value Is ``5``.
        * @param trim 
        * + default value Is ``'0.05'``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function unique(ions: any, eq?: number, gt?: number, mzwidth?: string, tolerance?: string, precursor?: string, rtwidth?: number, trim?: string, env?: object): any;
   }
   /**
     * @param tolerance default value Is ``'da:0.3'``.
     * @param env default value Is ``null``.
   */
   function jaccard(query: number, ref: number, tolerance?: any, env?: object): any;
   /**
    * search spectrum via the jaccard index method
    * 
    * 
     * @param query -
     * @param ref -
     * @param tolerance -
     * 
     * + default value Is ``'da:0.3'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function jaccardSet(query: number, ref: number, tolerance?: any, env?: object): any;
   /**
    * evaluate all m/z for all known precursor type.
    * 
    * 
     * @param mass the target exact mass value
     * @param mode this parameter could be two type of data:
     *  
     *  1. character of value ``+`` or ``-``, means evaluate all m/z for all known precursor types in given ion mode
     *  2. character of value in precursor type format means calculate mz for the target precursor type
     *  3. mzcalculator type means calculate mz for the traget precursor type
     *  4. a list of the mz calculator object and a list of corresponding mz value will be evaluated.
     * 
     * + default value Is ``'+'``.
     * @param env 
     * + default value Is ``null``.
   */
   function mz(mass: number, mode?: any, env?: object): object|number;
   /**
    * Create a peak index
    * 
    * 
     * @param mz A numeric vector of the peak m/z vector
   */
   function mz_index(mz: number): object;
   /**
    * normalized the peak intensity data, do [0,1] scaled.
    * 
    * 
     * @param msdata Should be a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.LibraryMatrix`` object.
     * @param sum the intensity normalization method to be used in this function, 
     *  set this parameter value to TRUE means use the total ion 
     *  normalization method, and the default value FALSE means used 
     *  the max intensity value for normalize the intensity value 
     *  to a relative percentage value.
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.LibraryMatrix`` 
     *  object with the intensity value for each ms2 peak normalized.
   */
   function norm_msdata(msdata: any, sum?: boolean, env?: object): object;
   /**
    * ### Peak finding
    *  
    *  Extract the peak ROI data from the chromatogram data
    * 
    * 
     * @param chromatogram -
     * @param baselineQuantile -
     * 
     * + default value Is ``0.65``.
     * @param angleThreshold -
     * 
     * + default value Is ``5``.
     * @param peakwidth -
     * 
     * + default value Is ``'8,30'``.
     * @param sn_threshold -
     * 
     * + default value Is ``3``.
     * @param joint 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakROI(chromatogram: any, baselineQuantile?: number, angleThreshold?: number, peakwidth?: any, sn_threshold?: number, joint?: boolean, env?: object): object;
   /**
    * calculate ppm value between two mass vector
    * 
    * 
     * @param a mass a
     * @param b mass b
     * @param env 
     * + default value Is ``null``.
   */
   function ppm(a: any, b: any, env?: object): number;
   /**
    * create precursor type calculator
    * 
    * 
     * @param types -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function precursor_types(types: any, env?: object): any;
   /**
    * Do resample of the chromatogram data
    * 
    * 
     * @param TIC -
     * @param dt -
     * 
     * + default value Is ``1``.
     * @param aggregate 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function resample(TIC: object, dt?: number, aggregate?: object, env?: object): any;
   /**
    * reorder scan points into a sequence for downstream data analysis
    * 
    * 
     * @param scans -
     * @param mzwidth -
     * 
     * + default value Is ``'da:0.1'``.
     * @param rtwidth -
     * 
     * + default value Is ``60``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function sequenceOrder(scans: any, mzwidth?: any, rtwidth?: number, env?: object): any;
   /**
    * Search spectra with entropy similarity
    * 
    * > Li, Y., Kind, T., Folz, J. et al. Spectral entropy outperforms MS/MS dot product
    * >  similarity for small-molecule compound identification. Nat Methods 18, 1524–1531
    * >  (2021). https://doi.org/10.1038/s41592-021-01331-z
    * 
     * @param x target spectral data that used for calculate the entropy value
     * @param ref if this parameter is not nothing, then the spectral similarity score will
     *  be evaluated from this function.
     * 
     * + default value Is ``null``.
     * @param tolerance the mass tolerance error to make the spectrum data centroid
     *  
     *  To calculate spectral entropy, the spectrum need to be centroid first. When you are
     *  focusing on fragment ion's information, the precursor ion may need to be removed 
     *  from the spectrum before calculating spectral entropy. If isotope peak exitsted on
     *  the MS/MS spectrum, the isotope peak should be removed fist as the isotope peak does
     *  not contain useful information for identifing molecule.
     * 
     * + default value Is ``'da:0.3'``.
     * @param intocutoff the percentage relative intensity value that used for removes the noise ion fragment peaks
     * 
     * + default value Is ``0.05``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function spectral_entropy(x: object, ref?: object, tolerance?: any, intocutoff?: number, env?: object): any;
   module spectrum {
      /**
       * create a delegate function pointer that apply for compares spectrums theirs similarity.
       * 
       * 
        * @param tolerance -
        * 
        * + default value Is ``'da:0.1'``.
        * @param equals_score -
        * 
        * + default value Is ``0.85``.
        * @param gt_score -
        * 
        * + default value Is ``0.6``.
        * @param score_aggregate ``@``T:System.Func`3````
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function compares(tolerance?: any, equals_score?: number, gt_score?: number, score_aggregate?: object, env?: object): object;
   }
   module spectrum_tree {
      /**
       * create spectrum tree cluster based on the spectrum to spectrum similarity comparison.
       * 
       * 
        * @param ms2list a vector of spectrum peaks data
        * @param compares a delegate function pointer that could be generated by ``spectrum.compares`` api.
        * 
        * + default value Is ``null``.
        * @param tolerance the mz tolerance threshold value
        * 
        * + default value Is ``'da:0.1'``.
        * @param intocutoff intensity cutoff of the spectrum matrix its product ``m/z`` fragments.
        * 
        * + default value Is ``0.05``.
        * @param showReport show progress report?
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function cluster(ms2list: any, compares?: object, tolerance?: any, intocutoff?: number, showReport?: boolean, env?: object): object;
   }
   /**
    * Create tolerance object
    * 
    * 
     * @param threshold -
     * @param method -
     * 
     * + default value Is ``["ppm","da"]``.
     * @param env 
     * + default value Is ``null``.
   */
   function tolerance(threshold: number, method?: any, env?: object): any;
   /**
   */
   function toMS(isotope: object): object;
   /**
    * makes xcms_id format liked ROI unique id
    * 
    * 
     * @param mz -
     * @param rt -
   */
   function xcms_id(mz: number, rt: number): string;
}
