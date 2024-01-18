// export R# package module type define for javascript/typescript language
//
//    imports "taxonomy" from "mzDIA";
//
// ref=mzkit.Taxonomy@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for spectrum taxonomy
 * 
*/
declare namespace taxonomy {
   /**
    * get spectrum clusters
    * 
    * 
     * @param tree -
   */
   function clusters(tree: object): any;
   /**
    * create taxonomy tree for multiple sample data in parallel
    * 
    * 
     * @param x a collection of the spectrum sample data, should be a tuple list 
     *  object that contains multiple sample data to build tree. each slot value in this 
     *  tuple list should be a vector of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.PeakMs2`` spectrum data.
     * @param mzdiff -
     * 
     * + default value Is ``0.3``.
     * @param intocutoff -
     * 
     * + default value Is ``0.05``.
     * @param equals -
     * 
     * + default value Is ``0.85``.
     * @param interval -
     * 
     * + default value Is ``0.1``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a union spectrum taxonomy tree object
   */
   function parallel_tree(x: object, mzdiff?: number, intocutoff?: number, equals?: number, interval?: number, env?: object): object;
   /**
    * create a spectrum taxonomy tree object
    * 
    * 
     * @param mzdiff 
     * + default value Is ``0.3``.
     * @param intocutoff 
     * + default value Is ``0.05``.
     * @param equals 
     * + default value Is ``0.85``.
     * @param interval 
     * + default value Is ``0.1``.
   */
   function tree(mzdiff?: number, intocutoff?: number, equals?: number, interval?: number): object;
   /**
   */
   function vocabulary(tree: object): object;
}
