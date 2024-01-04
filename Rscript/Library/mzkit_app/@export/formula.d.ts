// export R# package module type define for javascript/typescript language
//
//    imports "formula" from "mzkit";
//
// ref=mzkit.FormulaTools@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace formula {
   /**
     * @param ppm default value Is ``5``.
     * @param candidateElements default value Is ``null``.
     * @param env default value Is ``null``.
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
     * @param env default value Is ``null``.
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
      */
      function graph(kcf: object): object;
   }
   module open {
      module descriptor {
         /**
         */
         function db(dbFile: string): object;
      }
   }
   module parse {
      /**
        * @param parseStruct default value Is ``true``.
      */
      function SDF(data: string, parseStruct?: boolean): object;
   }
   /**
     * @param massDiff default value Is ``0.1``.
     * @param isotopeFirst default value Is ``true``.
     * @param as_list default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function peakAnnotations(library: any, formula: any, adducts: any, massDiff?: number, isotopeFirst?: boolean, as_list?: boolean, env?: object): object;
   module read {
      /**
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
     * @param env default value Is ``null``.
   */
   function scan(formula: string, env?: object): object;
   module SDF {
      /**
      */
      function convertKCF(sdfModel: object): object;
   }
}
