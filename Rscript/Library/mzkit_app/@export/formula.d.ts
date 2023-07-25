// export R# package module type define for javascript/typescript language
//
//    imports "formula" from "mzkit";
//
// ref=mzkit.FormulaTools@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The chemical formulae toolkit
 * 
*/
declare namespace formula {
   /**
    * find all of the candidate chemical formulas by a 
    *  specific exact mass value and a specific mass 
    *  tolerance value in ppm
    * 
    * 
     * @param mass the exact mass value
     * @param ppm the mass tolerance value in ppm
     * 
     * + default value Is ``5``.
     * @param candidateElements a list configuration of the formula candidates
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function candidates(mass: number, ppm?: number, candidateElements?: object, env?: object): object;
   /**
   */
   function canonical_formula(formula: object): string;
   module descriptor {
      /**
        * @param env default value Is ``null``.
      */
      function matrix(repo: object, cid: object, env?: object): object;
   }
   /**
    * ### Evaluate formula exact mass
    *  
    *  evaluate exact mass for the given formula strings.
    * 
    * > -1 will be return if the given formula is empty or error/invalid
    * 
     * @param formula a vector of the character formulas.
     * @param env 
     * + default value Is ``null``.
     * @return the result data type is keeps the same as the given data type of the
     *  **`formula`** parameter: this function returns a numeric 
     *  vector if the given **`formula`** is a character vector,
     *  or this function returns a key-value pair tuple list if the given
     *  **`formula`** is a list.
   */
   function eval(formula: any, env?: object): number;
   /**
   */
   function getElementCount(formula: object, element: string): object;
   /**
     * @param prob_threshold default value Is ``0.001``.
     * @param fwhm default value Is ``0.1``.
     * @param pad_left default value Is ``3``.
     * @param pad_right default value Is ``3``.
     * @param interpolate_grid default value Is ``0.005``.
   */
   function isotope_distribution(formula: any, prob_threshold?: number, fwhm?: number, pad_left?: number, pad_right?: number, interpolate_grid?: number): object;
   module KCF {
      /**
       * Create molecular network graph model
       * 
       * 
        * @param kcf The KCF molecule model
      */
      function graph(kcf: object): object;
   }
   module open {
      module descriptor {
         /**
          * open the file handles of the chemical descriptor database.
          * 
          * 
           * @param dbFile A directory path which contains the multiple database file of the 
           *  chemical descriptors.
         */
         function db(dbFile: string): object;
      }
   }
   module parse {
      /**
       * parse a single sdf text block
       * 
       * 
        * @param data -
        * @param parseStruct -
        * 
        * + default value Is ``true``.
      */
      function SDF(data: string, parseStruct?: boolean): object;
   }
   /**
    * do peak annotation for the ms2 fragments
    * 
    * 
     * @param library A ms2 matrix object
     * @param massDiff -
     * 
     * + default value Is ``0.1``.
     * @param isotopeFirst -
     * 
     * + default value Is ``true``.
     * @param adducts -
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function peakAnnotations(library: any, massDiff?: number, isotopeFirst?: boolean, adducts?: object, env?: object): object;
   module read {
      /**
       * Read KCF model data
       * 
       * 
        * @param data The text data or file path
      */
      function KCF(data: string): object;
   }
   /**
     * @param debug default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function registerAnnotations(annotation: object, debug?: boolean, env?: object): any;
   /**
   */
   function removeElement(formula: object, element: string): object;
   /**
    * Get atom composition from a formula string
    * 
    * 
     * @param formula The input formula string text.
     * @param env 
     * + default value Is ``null``.
   */
   function scan(formula: string, env?: object): object;
   module SDF {
      /**
      */
      function convertKCF(sdfModel: object): object;
   }
}
