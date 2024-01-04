// export R# package module type define for javascript/typescript language
//
//    imports "NMR" from "mzkit";
//    imports "NMR" from "mzplot";
//
// ref=mzkit.NMRTool@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=mzkit.plotNMR@mzplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace NMR {
   /**
   */
   function acquisition(nmrML: object): object;
   /**
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
   */
   function spectrum(spectrum: object, nmrML: object): object;
   /**
   */
   function spectrumList(nmrML: object): object;
}
