// export R# package module type define for javascript/typescript language
//
//    imports "SMILES" from "mzkit";
//
// ref=mzkit.SMILESTool@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace SMILES {
   module as {
      /**
        * @param canonical default value Is ``true``.
      */
      function formula(SMILES: object, canonical?: boolean): object;
      /**
      */
      function graph(smiles: object): object;
   }
   /**
    * get atoms table from the SMILES structure data
    * 
    * 
     * @param SMILES -
   */
   function atoms(SMILES: object): object;
   /**
     * @param kappa default value Is ``2``.
     * @param normalize_size default value Is ``false``.
   */
   function links(SMILES: object, kappa?: number, normalize_size?: boolean): object;
   /**
    * Parse the SMILES molecule structre string
    * 
    * > SMILES denotes a molecular structure as a graph with optional chiral 
    * >  indications. This is essentially the two-dimensional picture chemists
    * >  draw to describe a molecule. SMILES describing only the labeled
    * >  molecular graph (i.e. atoms and bonds, but no chiral or isotopic 
    * >  information) are known as generic SMILES.
    * 
     * @param SMILES -
     * @param strict -
     * 
     * + default value Is ``true``.
     * @return A chemical graph object that could be used for build formula or structure analysis
   */
   function parse(SMILES: string, strict?: boolean): object;
}
