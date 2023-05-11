// export R# package module type define for javascript/typescript language
//
// ref=mzkit.NMRTool@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=mzkit.plotNMR@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

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
   /**
     * @param size default value Is ``'3600,2400'``.
     * @param padding default value Is ``'padding: 200px 400px 300px 100px'``.
     * @param env default value Is ``null``.
   */
   function plot_nmr(nmr: object, size?: any, padding?: any, env?: object): any;
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
