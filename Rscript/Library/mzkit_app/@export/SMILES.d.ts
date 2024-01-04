// export R# package module type define for javascript/typescript language
//
//    imports "SMILES" from "mzkit";
//
// ref=mzkit.SMILESTool@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
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
   */
   function atoms(SMILES: object): object;
   /**
     * @param kappa default value Is ``2``.
     * @param normalize_size default value Is ``false``.
   */
   function links(SMILES: object, kappa?: number, normalize_size?: boolean): object;
   /**
     * @param strict default value Is ``true``.
   */
   function parse(SMILES: string, strict?: boolean): object;
}
