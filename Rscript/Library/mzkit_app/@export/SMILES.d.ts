// export R# package module type define for javascript/typescript language
//
//    imports "SMILES" from "mzkit";
//
// ref=mzkit.SMILESTool@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * ### Simplified molecular-input line-entry system
 *  
 *  The simplified molecular-input line-entry system (SMILES) is a specification in the 
 *  form of a line notation for describing the structure of chemical species using short
 *  ASCII strings. SMILES strings can be imported by most molecule editors for conversion
 *  back into two-dimensional drawings or three-dimensional models of the molecules.
 * 
 *  The original SMILES specification was initiated In the 1980S. It has since been 
 *  modified And extended. In 2007, an open standard called OpenSMILES was developed In
 *  the open source chemistry community.
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
    * create graph embedding result for a specific molecular strucutre data
    * 
    * 
     * @param SMILES the molecular structure data which is parsed from a given smiles string
     * @param kappa kappa parameter for SGT embedding algorithm
     * 
     * + default value Is ``2``.
     * @param normalize_size -
     * 
     * + default value Is ``false``.
     * @param tabular 
     * + default value Is ``true``.
     * @return a dataframe object that contains the SGT embedding result of a molecular 
     *  strcutre data, contains the data fields:
     *  
     *  1. atom1 the label of the atom group
     *  2. atom2 the label of the another atom group
     *  3. weight the embedding score result of current link
     *  4. vk SGT vk score
     *  5. v0 SGT v0 score
     *  6. vertex a set of the vertex data for generates current graph embedding score data
   */
   function links(SMILES: object, kappa?: number, normalize_size?: boolean, tabular?: boolean): object;
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
   /**
    * evaluate the similarity score between two molecular strcuture
    * 
    * 
     * @param x -
     * @param y -
     * @param kappa -
     * 
     * + default value Is ``2``.
     * @param normalize_size -
     * 
     * + default value Is ``false``.
   */
   function score(x: object, y: object, kappa?: number, normalize_size?: boolean): number;
}
