// export R# package module type define for javascript/typescript language
//
//    imports "annotation" from "mzkit";
//
// ref=mzkit.library@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace annotation {
   module assert {
      /**
        * @param ion_mode default value Is ``'+'``.
        * @param env default value Is ``null``.
      */
      function adducts(formula: string, adducts: any, ion_mode?: any, env?: object): object;
   }
   /**
     * @param da default value Is ``0.1``.
     * @param rt_win default value Is ``5``.
     * @param env default value Is ``null``.
   */
   function checkInSourceFragments(ms1: any, ms2: any, da?: number, rt_win?: number, env?: object): boolean;
   module make {
      /**
        * @param synonym default value Is ``null``.
        * @param xref default value Is ``null``.
      */
      function annotation(id: string, formula: string, name: string, synonym?: string, xref?: object): object;
   }
   /**
     * @param mzdiff default value Is ``'da:0.3'``.
     * @param env default value Is ``null``.
   */
   function populateIonData(raw: object, mzdiff?: any, env?: object): object;
   /**
     * @param chebi default value Is ``null``.
     * @param KEGG default value Is ``null``.
     * @param KEGGdrug default value Is ``null``.
     * @param pubchem default value Is ``null``.
     * @param HMDB default value Is ``null``.
     * @param metlin default value Is ``null``.
     * @param DrugBank default value Is ``null``.
     * @param ChEMBL default value Is ``null``.
     * @param chemspider default value Is ``null``.
     * @param foodb default value Is ``null``.
     * @param Wikipedia default value Is ``null``.
     * @param lipidmaps default value Is ``null``.
     * @param MeSH default value Is ``null``.
     * @param ChemIDplus default value Is ``null``.
     * @param MetaCyc default value Is ``null``.
     * @param KNApSAcK default value Is ``null``.
     * @param CAS default value Is ``null``.
     * @param InChIkey default value Is ``null``.
     * @param InChI default value Is ``null``.
     * @param SMILES default value Is ``null``.
     * @param extras default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function xref(chebi?: any, KEGG?: any, KEGGdrug?: any, pubchem?: any, HMDB?: any, metlin?: any, DrugBank?: any, ChEMBL?: any, chemspider?: any, foodb?: any, Wikipedia?: any, lipidmaps?: any, MeSH?: any, ChemIDplus?: any, MetaCyc?: any, KNApSAcK?: any, CAS?: any, InChIkey?: any, InChI?: any, SMILES?: any, extras?: object, env?: object): object;
}
