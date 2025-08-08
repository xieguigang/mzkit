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
     * @param spectrum a set of the mzkit supported spectrum object.
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
     * @return a set of the decomposed spectrum object
   */
   function dia_nmf(spectrum: any, n: object, maxItrs?: object, tolerance?: number, eps?: number, env?: object): object;
   /**
     * @param adducts default value Is ``["[M+H]+","[M+Na]+","[M+K]+","[M+NH4]+","[M-H]-","[M+Acetate]-","[M+HCOO]-"]``.
   */
   function peptide_lib(len: object, adducts?: any): object;
}
