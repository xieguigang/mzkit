// export R# package module type define for javascript/typescript language
//
// ref=mzkit.NMRTool@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace NMR {
   /**
    * get all acquisition data in the raw data file
    * 
    * 
     * @param nmrML -
   */
   function acquisition(nmrML: object): object;
   /**
    * Read Free Induction Decay data matrix
    * 
    * 
     * @param data -
   */
   function FID(data: object): object;
   /**
   */
   function nmr_dft(fidData: object): object;
   module read {
      /**
      */
      function nmrML(file: string): object;
   }
   /**
    * 
    * 
     * @param spectrum -
     * @param nmrML -
     * @return a matrix of [ppm => intensity]
   */
   function spectrum(spectrum: object, nmrML: object): object;
   /**
   */
   function spectrumList(nmrML: object): object;
}
