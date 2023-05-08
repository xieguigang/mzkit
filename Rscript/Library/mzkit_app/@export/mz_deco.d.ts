// export R# package module type define for javascript/typescript language
//
// ref=mzkit.mz_deco@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace mz_deco {
   module read {
      /**
      */
      function xcms_peaks(file:string): object;
   }
   /**
   */
   function peak_subset(peaktable:object, sampleNames:string): object;
}
