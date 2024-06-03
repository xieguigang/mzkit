// export R# package module type define for javascript/typescript language
//
//    imports "formula" from "mzkit";
//
// ref=mzkit.FormulaTools@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The chemical formulae toolkit
 *  
 *  In chemistry, a chemical formula is a way of presenting information about the chemical proportions of 
 *  atoms that constitute a particular chemical compound or molecule, using chemical element symbols, numbers,
 *  and sometimes also other symbols, such as parentheses, dashes, brackets, commas and plus (+) and minus (−) 
 *  signs. These are limited to a single typographic line of symbols, which may include subscripts and 
 *  superscripts. A chemical formula is not a chemical name since it does not contain any words. Although a 
 *  chemical formula may imply certain simple chemical structures, it is not the same as a full chemical 
 *  structural formula. Chemical formulae can fully specify the structure of only the simplest of molecules 
 *  and chemical substances, and are generally more limited in power than chemical names and structural formulae.
 * 
 * > The simplest types of chemical formulae are called empirical formulae, which use letters and numbers indicating
 * >  the numerical proportions of atoms of each type. Molecular formulae indicate the simple numbers of each type 
 * >  of atom in a molecule, with no information on structure. For example, the empirical formula for glucose is
 * >  CH2O (twice as many hydrogen atoms as carbon and oxygen), while its molecular formula is C6H12O6 (12 hydrogen 
 * >  atoms, six carbon and oxygen atoms).
 * >  
 * >  Sometimes a chemical formula is complicated by being written as a condensed formula (or condensed molecular 
 * >  formula, occasionally called a "semi-structural formula"), which conveys additional information about the
 * >  particular ways in which the atoms are chemically bonded together, either in covalent bonds, ionic bonds, or 
 * >  various combinations of these types. This is possible if the relevant bonding is easy to show in one dimension. 
 * >  An example is the condensed molecular/chemical formula for ethanol, which is CH3−CH2−OH or CH3CH2OH. However, 
 * >  even a condensed chemical formula is necessarily limited in its ability to show complex bonding relationships 
 * >  between atoms, especially atoms that have bonds to four or more different substituents.
 * >  
 * >  Since a chemical formula must be expressed as a single line of chemical element symbols, it often cannot be as
 * >  informative as a true structural formula, which is a graphical representation of the spatial relationship 
 * >  between atoms in chemical compounds (see for example the figure for butane structural and chemical formulae,
 * >  at right). For reasons of structural complexity, a single condensed chemical formula (or semi-structural formula)
 * >  may correspond to different molecules, known as isomers. For example, glucose shares its molecular formula C6H12O6
 * >  with a number of other sugars, including fructose, galactose and mannose. Linear equivalent chemical names exist 
 * >  that can and do specify uniquely any complex structural formula (see chemical nomenclature), but such names must
 * >  use many terms (words), rather than the simple element symbols, numbers, and simple typographical symbols that 
 * >  define a chemical formula.
 * >  
 * >  Chemical formulae may be used in chemical equations to describe chemical reactions and other chemical transformations, 
 * >  such as the dissolving of ionic compounds into solution. While, as noted, chemical formulae do not have the full 
 * >  power of structural formulae to show chemical relationships between atoms, they are sufficient to keep track of 
 * >  numbers of atoms and numbers of electrical charges in chemical reactions, thus balancing chemical equations so that
 * >  these equations can be used in chemical problems involving conservation of atoms, and conservation of electric 
 * >  charge.
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
    * Make the given formula string into canonical format
    * 
    * 
     * @param formula -
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
     * @param env default value Is ``null``.
   */
   function formula_calibration(formula: object, adducts: any, env?: object): any;
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
     * @param adducts -
     * @param massDiff -
     * 
     * + default value Is ``0.1``.
     * @param isotopeFirst -
     * 
     * + default value Is ``true``.
     * @param as_list 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
   */
   function peakAnnotations(library: any, formula: any, adducts: any, massDiff?: number, isotopeFirst?: boolean, as_list?: boolean, env?: object): object;
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
