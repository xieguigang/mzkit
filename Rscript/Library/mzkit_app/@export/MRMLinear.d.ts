// export R# package module type define for javascript/typescript language
//
//    imports "MRMLinear" from "mz_quantify";
//
// ref=mzkit.MRMkit@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * MRM Targeted Metabolomics
 * 
*/
declare namespace MRMLinear {
   module as {
      /**
       * Convert any compatibale type as the ion pairs data object for MRM target selected.
       * 
       * 
        * @param mz should be a set of the @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL.MSLIon`` object data.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a collection of the mzkit clr ion pair data object. this MRM ion pair data object could 
        *  be cast to dataframe vai the ``as.data.frame`` conversion.
      */
      function ion_pairs(mz: any, env?: object): object;
   }
   module extract {
      /**
       * Extract ion peaks
       * 
       * 
        * @param mzML the file path to a mzML raw data file.
        * @param ionpairs metabolite targets, value could be a vector of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models.IonPair`` or vector of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models.IsomerismIonPairs`` clr object.
        * @param tolerance 
        * + default value Is ``'ppm:20'``.
        * @param env 
        * + default value Is ``null``.
        * @return vector of the ion pair corresponding xic data
      */
      function ions(mzML: string, ionpairs: any, tolerance?: any, env?: object): object;
      /**
       * Exact ``regions of interested`` based on the given ion pair as targets.
       * 
       * 
        * @param mzML A mzML raw data file
        * @param ionpairs MRM ion pairs
        * @param tolerance 
        * + default value Is ``'ppm:20'``.
        * @param timeWindowSize 
        * + default value Is ``5``.
        * @param baselineQuantile 
        * + default value Is ``0.65``.
        * @param integratorTicks 
        * + default value Is ``5000``.
        * @param peakAreaMethod -
        * 
        * + default value Is ``null``.
        * @param angleThreshold 
        * + default value Is ``5``.
        * @param peakwidth 
        * + default value Is ``'8,30'``.
        * @param rtshift 
        * + default value Is ``null``.
        * @param bsplineDensity 
        * + default value Is ``100``.
        * @param bsplineDegree 
        * + default value Is ``2``.
        * @param sn_threshold 
        * + default value Is ``3``.
        * @param TPAFactors Peak factors
        * 
        * + default value Is ``null``.
        * @param joint_peaks 
        * + default value Is ``true``.
        * @param time_shift_method 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function peakROI(mzML: string, ionpairs: object, tolerance?: string, timeWindowSize?: number, baselineQuantile?: number, integratorTicks?: object, peakAreaMethod?: object, angleThreshold?: number, peakwidth?: any, rtshift?: object, bsplineDensity?: object, bsplineDegree?: object, sn_threshold?: number, TPAFactors?: object, joint_peaks?: boolean, time_shift_method?: boolean, env?: object): object;
   }
   /**
    * Extract all ion pairs information from the given rawdata file
    * 
    * 
     * @param mzML the file path to the given mzML rawdata file
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a collection of the MRM ion pairs data objects that extract from the precursor/product isolation window target ``m/z`` value.
   */
   function extract_ionpairs(mzML: string, env?: object): object;
   /**
    * Extract ion peaks
    * 
    * 
     * @param mzML the file path to a mzML raw data file.
     * @param tolerance 
     * + default value Is ``'da:0.1'``.
     * @param env 
     * + default value Is ``null``.
     * @return vector of the ion pair corresponding xic data
   */
   function extract_mrm(mzML: string, q1: number, q3: number, tolerance?: any, env?: object): object;
   /**
    * MRM-Ion Pair Finder is used to automatically and systematically define MRM transitions from untargeted metabolomics data. 
    *  Our research group first introduced the concept of pseudotargeted metabolomics using the retention time locking 
    *  GC-MS-selected ions monitoring in 2012. The pseudotargeted metabolomics method was extended to LC-MS in 2013. To define
    *  ion pairs automatically and systematically, the in-house software “Mul-tiple Reaction Monitoring-Ion Pair Finder (MRM-Ion 
    *  Pair Finder)” was developed, which made defining of the MRM transitions for untargeted metabolic profiling easier and 
    *  less time consuming. Recently, MRM-Ion Pair Finder was updated to version 2.0. The new version is more convenient, consumes 
    *  less time and is also suitable for negative ion mode. And the function of MRM-Ion Pair Finder is also performed in R so 
    *  that users have more options when using pseudotargeted method.
    * 
    * > https://github.com/zhengfj1994/MRM-Ion_Pair_Finder
    * 
     * @param ms2 -
     * @param ms1 
     * + default value Is ``null``.
     * @param diff_MS2MS1 
     * + default value Is ``0.05``.
     * @param ms2_intensity 
     * + default value Is ``0.05``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function find_untargeted_ionpair(ms2: any, ms1?: object, diff_MS2MS1?: number, ms2_intensity?: number, env?: object): object;
   /**
    * Create the MRM peak finding arguments from a json string
    * 
    * 
     * @param json_str -
   */
   function from_arguments_json(json_str: string): object;
   module isomerism {
      /**
        * @param tolerance default value Is ``'ppm:20'``.
        * @param env default value Is ``null``.
      */
      function ion_pairs(ions: object, tolerance?: string, env?: object): object;
   }
   /**
    * Create linear fitting based on the wiff raw scan data.
    * 
    * > 20200106 checked, test success
    * 
     * @param rawScan The wiff raw scan data which are extract by function: ``wiff.scans``.
     * @param calibrates -
     * @param blankControls 
     * + default value Is ``null``.
     * @param maxDeletions Max number of the reference points that delete automatically by 
     *  the linear modelling program.
     *  
     *  + negative value means auto
     *  + zero means no deletion
     *  + positive means the max allowed point numbers for auto deletion by the program
     * 
     * + default value Is ``1``.
     * @param isWorkCurveMode 
     * + default value Is ``true``.
     * @param args 
     * + default value Is ``null``.
   */
   function linears(rawScan: object, calibrates: object, ISvector: object, blankControls?: object, maxDeletions?: object, isWorkCurveMode?: boolean, args?: object): object;
   module MRM {
      /**
       * Create argument object for run MRM quantification.
       * 
       * 
        * @param tolerance -
        * 
        * + default value Is ``'da:0.3'``.
        * @param timeWindowSize 
        * + default value Is ``5``.
        * @param angleThreshold 
        * + default value Is ``5``.
        * @param baselineQuantile 
        * + default value Is ``0.65``.
        * @param integratorTicks 
        * + default value Is ``5000``.
        * @param peakAreaMethod -
        * 
        * + default value Is ``null``.
        * @param peakwidth -
        * 
        * + default value Is ``'8,30'``.
        * @param TPAFactors -
        * 
        * + default value Is ``null``.
        * @param sn_threshold -
        * 
        * + default value Is ``3``.
        * @param joint_peaks 
        * + default value Is ``true``.
        * @param time_shift_method 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function arguments(tolerance?: any, timeWindowSize?: number, angleThreshold?: number, baselineQuantile?: number, integratorTicks?: object, peakAreaMethod?: object, peakwidth?: any, TPAFactors?: object, sn_threshold?: number, joint_peaks?: boolean, time_shift_method?: boolean, env?: object): object;
      /**
       * Get MRM ions peaks data from a given raw data file
       * 
       * 
        * @param mzML the file path of the mzML raw data file
        * @param ions the ion pairs data list
        * @param rtshifts 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function peak2(mzML: string, ions: object, args: object, rtshifts?: object, env?: object): object;
      /**
       * Extract the peak area data from the given xic data object.
       *  
       *  This function is used to extract the peak area data from the given xic data object, 
       *  which is usually a result of the @``M:mzkit.MRMkit.ExtractIonData(System.String,System.Double,System.Double,System.Object,SMRUCC.Rsharp.Runtime.Environment)`` function.
       * 
       * 
        * @param xic -
        * @param args the arguments for peak finding
        * @param env -
        * 
        * + default value Is ``null``.
        * @return A vector of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.IonTPA`` mzkit clr object, and the ``rtshifts`` tuple
        *  list data is tagged inside this vector attributes data.
      */
      function peakarea(xic: any, args: object, env?: object): object;
      /**
       * Get MRM ions peaks data from a given raw data file
       * 
       * 
        * @param mzML the file path of the mzML raw data file
        * @param ions the ion pairs data list
        * @param peakAreaMethod -
        * 
        * + default value Is ``null``.
        * @param tolerance 
        * + default value Is ``'ppm:20'``.
        * @param timeWindowSize 
        * + default value Is ``5``.
        * @param angleThreshold 
        * + default value Is ``5``.
        * @param baselineQuantile 
        * + default value Is ``0.65``.
        * @param rtshifts -
        * 
        * + default value Is ``null``.
        * @param TPAFactors -
        * 
        * + default value Is ``null``.
        * @param peakwidth -
        * 
        * + default value Is ``'8,30'``.
        * @param sn_threshold -
        * 
        * + default value Is ``3``.
        * @param joint_peaks 
        * + default value Is ``true``.
        * @param time_shift_method 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function peaks(mzML: string, ions: object, peakAreaMethod?: object, tolerance?: string, timeWindowSize?: number, angleThreshold?: number, baselineQuantile?: number, rtshifts?: object, TPAFactors?: object, peakwidth?: any, sn_threshold?: number, joint_peaks?: boolean, time_shift_method?: boolean, env?: object): object;
      /**
        * @param args default value Is ``null``.
      */
      function rt_alignments(cal: any, ions: any, args?: object): object;
   }
   /**
   */
   function R2(lines: object): number;
   module read {
      /**
       * Get ion pair definition data from a given table file.
       * 
       * 
        * @param file A csv file or xlsx Excel data sheet
        * @param sheetName The sheet name in excel tables.
        * 
        * + default value Is ``'Sheet1'``.
      */
      function ion_pairs(file: string, sheetName?: string): object;
      /**
       * Read the definition of internal standards
       * 
       * 
        * @param file A csv file or xlsx file
        * @param sheetName The sheet name that contains data of the IS data if the given file is a xlsx file.
        * 
        * + default value Is ``'Sheet1'``.
        * @param env 
        * + default value Is ``null``.
      */
      function IS(file: string, sheetName?: string, env?: object): object;
      /**
       * Read reference points
       * 
       * 
        * @param sheetName 
        * + default value Is ``'Sheet1'``.
      */
      function reference(file: string, sheetName?: string): object;
   }
   module sample {
      /**
       * Do sample quantify
       * 
       * 
        * @param model -
        * @param file The sample raw file its file path.
        * @param ions -
        * @param peakAreaMethod 
        * + default value Is ``null``.
        * @param tolerance 
        * + default value Is ``'ppm:20'``.
        * @param timeWindowSize 
        * + default value Is ``5``.
        * @param angleThreshold 
        * + default value Is ``5``.
        * @param baselineQuantile 
        * + default value Is ``0.65``.
        * @param peakwidth 
        * + default value Is ``'8,30'``.
        * @param TPAFactors 
        * + default value Is ``null``.
        * @param sn_threshold 
        * + default value Is ``3``.
        * @param joint_peaks 
        * + default value Is ``true``.
        * @param time_shift_method 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function quantify(model: object, file: string, ions: object, peakAreaMethod?: object, tolerance?: string, timeWindowSize?: number, angleThreshold?: number, baselineQuantile?: number, peakwidth?: any, TPAFactors?: object, sn_threshold?: number, joint_peaks?: boolean, time_shift_method?: boolean, env?: object): object;
      /**
       * Do sample quantify
       * 
       * 
        * @param model -
        * @param file The sample raw file its file path.
        * @param ions -
        * @param env 
        * + default value Is ``null``.
      */
      function quantify2(model: object, file: string, ions: object, env?: object): object;
   }
   module wiff {
      /**
       * Create model of the MRM raw files
       * 
       * 
        * @param convertDir A directory data object for read MRM sample raw files. If the parameter value is
        *  a ``list``, then it should contains at least two fields: ``samples`` and ``reference``.
        *  The balnks raw data should be contains in reference files directory.
        * @param patternOfRef File name pattern for filter reference data.
        * 
        * + default value Is ``'.+[-]CAL[-]?\d+'``.
        * @param patternOfBlank File name pattern for filter blank controls.
        * 
        * + default value Is ``'KB[-]?(\d+)?'``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function rawfiles(convertDir: any, patternOfRef?: string, patternOfBlank?: string, env?: object): any;
      /**
       * # Scan the raw file data
       *  
       *  Get the peak area data of the metabolites in each given sample 
       *  data files
       * 
       * 
        * @param wiffConverts A directory that contains the mzML files which are converts from 
        *  the given wiff raw file.
        * @param ions Ion pairs definition data.
        * @param args 
        * + default value Is ``null``.
        * @param rtshifts For the calibration of the linear model reference points used only, 
        *  **DO NOT apply this parameter for the user sample data!**
        * 
        * + default value Is ``null``.
        * @param removesWiffName -
        * 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function scan2(wiffConverts: string, ions: object, args?: object, rtshifts?: object, removesWiffName?: boolean, env?: object): object;
      /**
       * # Scan the raw file data
       *  
       *  Get the peak area data of the metabolites in each given sample 
       *  data files
       * 
       * 
        * @param wiffConverts A directory that contains the mzML files which are converts from 
        *  the given wiff raw file.
        * @param ions Ion pairs definition data.
        * @param peakAreaMethod -
        * 
        * + default value Is ``null``.
        * @param tolerance 
        * + default value Is ``'ppm:20'``.
        * @param angleThreshold 
        * + default value Is ``5``.
        * @param baselineQuantile 
        * + default value Is ``0.65``.
        * @param removesWiffName -
        * 
        * + default value Is ``true``.
        * @param timeWindowSize 
        * + default value Is ``5``.
        * @param rtshifts For the calibration of the linear model reference points used only, 
        *  **DO NOT apply this parameter for the user sample data!**
        * 
        * + default value Is ``null``.
        * @param bsplineDensity 
        * + default value Is ``100``.
        * @param bsplineDegree 
        * + default value Is ``2``.
        * @param resolution 
        * + default value Is ``3000``.
        * @param peakwidth 
        * + default value Is ``'8,30'``.
        * @param TPAFactors -
        * 
        * + default value Is ``null``.
        * @param sn_threshold 
        * + default value Is ``3``.
        * @param joint_peaks 
        * + default value Is ``true``.
        * @param time_shift_method 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function scans(wiffConverts: string, ions: object, peakAreaMethod?: object, tolerance?: string, angleThreshold?: number, baselineQuantile?: number, removesWiffName?: boolean, timeWindowSize?: number, rtshifts?: object, bsplineDensity?: object, bsplineDegree?: object, resolution?: object, peakwidth?: any, TPAFactors?: object, sn_threshold?: number, joint_peaks?: boolean, time_shift_method?: boolean, env?: object): object;
   }
}
