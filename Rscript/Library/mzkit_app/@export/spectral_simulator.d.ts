// export R# package module type define for javascript/typescript language
//
//    imports "spectral_simulator" from "mzDIA";
//
// ref=mzkit.ms2_simulator@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace spectral_simulator {
   /**
    * build molecule documents for do embedding
    * 
    * 
     * @param mols -
     * @param id 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function buildDocuments(mols: any, id?: any, env?: object): any;
   module embedded {
      /**
       * make the molecular graph embedding to vector
       * 
       * 
        * @param mol -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function graph2vector(mol: any, env?: object): any;
   }
   /**
    * parse the smiles structure string as molecular network graph
    * 
    * 
     * @param mol -
     * @param id tag the id data with the corresponding smiles graph data, this character id vector
     *  length should be equals to the given molecules vector size.
     * 
     * + default value Is ``null``.
     * @param name 
     * + default value Is ``null``.
     * @param digest_formula 
     * + default value Is ``true``.
     * @param verbose -
     * 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
   */
   function molecular_graph(mol: any, id?: any, name?: any, digest_formula?: boolean, verbose?: boolean, env?: object): object;
   module read {
      /**
      */
      function kcf(file: string): object;
   }
}
