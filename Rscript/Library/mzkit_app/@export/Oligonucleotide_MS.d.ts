// export R# package module type define for javascript/typescript language
//
//    imports "Oligonucleotide_MS" from "mzDIA";
//
// ref=mzkit.Oligonucleotide_MS@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace Oligonucleotide_MS {
   /**
    * evaluate the exact mass for the gene sequence
    * 
    * 
     * @param fa should be a collection of the gene sequence.
     * @param env 
     * + default value Is ``null``.
   */
   function exact_mass(fa: any, env?: object): any;
}
