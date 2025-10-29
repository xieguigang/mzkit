﻿// export R# package module type define for javascript/typescript language
//
//    imports "math" from "mzkit";
//
// ref=mzkit.MzMath@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * mass spectrometry data math toolkit
 * 
*/
declare namespace math {
   /**
    * #### Converts profiles peak data to peak data in centroid mode.
    *  
    *  profile and centroid in Mass Spectrometry?
    *  
    *  1. Profile means the continuous wave form in a mass spectrum.
    *  
    *    + Number of data points Is large.
    *    
    *  2. Centroid means the peaks in a profile data Is changed to bars.
    *  
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
     * @param aggregate default is get the max intensity value.
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
     * @return Peaks data in centroid mode or a new m/z vector in centroid.
   */
   function centroid(ions: any, tolerance?: any, intoCutoff?: number, parallel?: boolean, aggregate?: object, env?: object): object|object|number;
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
       * pairwise alignment of the spectrum peak set
       * 
       * 
        * @param query a spectrum set of the sample query input.
        * @param ref a spectrum set of the reference library
        * @param tolerance the ion m/z mass tolerance value for make the peak alignment
        * 
        * + default value Is ``'da:0.3'``.
        * @param intocutoff spectrum peak cutoff by relative intensity
        * 
        * + default value Is ``0.05``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml.AlignmentOutput`` from the pairwise alignment between the query and reference.
      */
      function pairwise(query: any, ref: any, tolerance?: any, intocutoff?: number, env?: object): object;
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
     * @param mz a single ion m/z value.
     * @param mode the ion polarity mode ``+/-`` for evaluate all kind of 
     *  precursor type under the specific polarity mode, or a vector of the 
     *  @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType.MzCalculator`` precursor type model which is generates from 
     *  the ``math::precursor_types`` function.
     * 
     * + default value Is ``'+'``.
     * @param env 
     * + default value Is ``null``.
     * @return a collection of the exact mass evaluation result, could be cast
     *  to dataframe via ``as.data.frame`` function.
   */
   function exact_mass(mz: number, mode?: any, env?: object): object;
   /**
    * find precursor adducts type for a given mass and the corresponding precursor mz
    * 
    * 
     * @param mass the exact mass value
     * @param mz the m/z value from the ion, usually be the mz value from the xcms peaktable.
     * @param libtype the ion mode polarity value of the adducts for matches, value could be an integer value [1,-1].
     *  and also this parameter value could be a set of the precursor adducts type character vector 
     *  for do the data matches job.
     * 
     * + default value Is ``1``.
     * @param da -
     * 
     * + default value Is ``0.3``.
     * @param safe 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
     * @return a matched adducts result, for no matched data, a details error message will be returns.
     *  generally, the result tuple list contains the slot data:
     *  
     *  1. precursor_type: the result adducts type that could be used for matches the given mass and mz value
     *  2. error: the mass error in unit delta dalton between the given mz and the theoretical m/z value that evaluated from the given mass and the matched adducts type.
     *  3. theoretical: the theoretical m/z value that evaluated from the given mass and the matched adducts type.
     *  4. ppm: the mass ppm error between the given mz and the theoretical m/z value that evaluated from the given mass and the matched adducts type.
     *  5. message: usually be the error message.
   */
   function find_precursor(mass: number, mz: number, libtype?: any, da?: number, safe?: boolean, env?: object): object;
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
   function jaccard(query: number, ref: number, tolerance?: any, env?: object): number;
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
   function jaccardSet(query: number, ref: number, tolerance?: any, env?: object): object;
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
     * @param unsafe 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
   */
   function mz(mass: number, mode?: any, unsafe?: boolean, env?: object): object|number;
   /**
    * Create a peak index
    * 
    * 
     * @param mz A numeric vector of the peak m/z vector
     * @param win_size 
     * + default value Is ``1``.
   */
   function mz_index(mz: any, win_size?: number): object;
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
     * @param types a character vector of the precursor type symbols, example as ``[M+H]+``, etc.
     * @param unsafe this parameter indicates that how the function handling of the string parser error when the given string value is empty:
     *  
     *  1. for unsafe, an exception will be throw
     *  2. for unsafe is false, corresponding null value will be generated.
     * 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a collection of the ion precursor adducts object.
   */
   function precursor_types(types: any, unsafe?: boolean, env?: object): object;
   /**
    * evaluate of the adduct annotation ranking score
    * 
    * 
     * @param formula -
     * @param adducts -
     * @param max_score 
     * + default value Is ``10``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A ranking score numeric vector
   */
   function rank_adducts(formula: any, adducts: any, max_score?: number, env?: object): number;
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
   function sequenceOrder(scans: any, mzwidth?: any, rtwidth?: number, env?: object): object;
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
   function spectral_entropy(x: object, ref?: object, tolerance?: any, intocutoff?: number, env?: object): number;
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
        * @param score_aggregate A @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.ScoreAggregates`` method, should be a function in clr delegate 
        *  liked: ``@``T:System.Func`3````.
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
    * convert a single spectrum alignment details as dataframe
    * 
    * 
     * @param align -
     * @param args -
     * @param env -
   */
   function summary_result(align: object, args: object, env: object): any;
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
     * @return the value clr type of this function is determine based on 
     *  the **`method`** parameter value.
   */
   function tolerance(threshold: number, method?: any, env?: object): object|object;
   /**
   */
   function toMS(isotope: object): object;
   /**
    * makes xcms_id format liked ROI unique id
    * 
    * > the dimension size of the ion m/z vector and the corresponding scan time vector should be equals.
    * 
     * @param mz a numeric vector of the ion m/z value
     * @param rt the corresponding scan time rt vector.
     * @param prefix 
     * + default value Is ``''``.
     * @param env 
     * + default value Is ``null``.
     * @return a character vector of the generated unique id based on the given m/z and rt ROI features.
   */
   function xcms_id(mz: number, rt: number, prefix?: string, env?: object): string;
}
