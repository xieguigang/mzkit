// export R# package module type define for javascript/typescript language
//
//    imports "DIA" from "mzDIA";
//
// ref=mzkit.DIASpectrumAnnotations@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the package tools for the DIA spectrum data annotation
 * 
*/
declare namespace DIA {
   /**
    * make the spectrum set decompose into multiple spectrum groups via the NMF method
    * 
    * 
     * @param spectrum -
     * @param n the number of the target spectrum to decomposed, 
     *  this number should be query from the DDA experiment 
     *  database.
     * @param maxItrs 
     * + default value Is ``1000``.
     * @param tolerance 
     * + default value Is ``0.001``.
     * @param eps 
     * + default value Is ``0.0001``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function dia_nmf(spectrum: any, n: object, maxItrs?: object, tolerance?: number, eps?: number, env?: object): any;
}
