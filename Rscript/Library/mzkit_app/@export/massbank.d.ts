﻿// export R# package module type define for javascript/typescript language
//
//    imports "massbank" from "mzkit";
//
// ref=mzkit.Massbank@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Metabolite annotation database toolkit
 * 
*/
declare namespace massbank {
   module as {
      /**
       * populate lipidmaps meta data objects from the loaded sdf data stream
       * 
       * 
        * @param sdf a sequence of sdf molecular data which can be read from the ``read.SDF`` function.
        * @param asList 
        * + default value Is ``false``.
        * @param lazy 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function lipidmaps(sdf: any, asList?: boolean, lazy?: boolean, env?: object): object;
   }
   module chebi {
      module secondary2main {
         /**
         */
         function mapping(repository: string): string;
      }
   }
   /**
    * normalized the input id data as canonical chebi id
    * 
    * 
     * @param id -
     * @param env 
     * + default value Is ``null``.
   */
   function chebi_id(id: any, env?: object): any;
   /**
    * extract the chebi annotation data from the chebi ontology data
    * 
    * 
     * @param chebi the chebi ontology data, in clr type: @``T:SMRUCC.genomics.foundation.OBO_Foundry.IO.Models.OBOFile``
   */
   function extract_chebi_compounds(chebi: object): object;
   /**
    * Extract the unique metabolite information from the mona database
    * 
    * 
     * @param mona -
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a tuple list of the @``T:BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaInfo`` data. andalso an attribute with name ``mapping`` is tagged
     *  with the result tuple list that contains mapping from the spectrum id to the metabolite unique 
     *  reference id.
   */
   function extract_mona_metabolites(mona: any, env?: object): object;
   module glycosyl {
      /**
        * @param rules default value Is ``null``.
      */
      function solver(rules?: object): object;
      /**
       * Parse the glycosyl compound name
       * 
       * 
        * @param glycosyl -
        * @param rules -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function tokens(glycosyl: string, rules?: object, env?: object): string;
   }
   module hmdb {
      module secondary2main {
         /**
           * @param env default value Is ``null``.
         */
         function mapping(repository: any, env?: object): any;
      }
   }
   /**
    * check of the mona reference spectrum is positive or not?
    * 
    * 
     * @param spec -
   */
   function is_positive(spec: object): boolean;
   module lipid {
      /**
       * Create lipid class helper for annotation
       * 
       * 
        * @param lipidmaps -
        * @param id 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function class(lipidmaps: any, id?: any, env?: object): object|object;
      /**
      */
      function nameMaps(lipidmaps: object): object;
      /**
       * Create lipid name helper for annotation
       * 
       * 
        * @param lipidmaps -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function names(lipidmaps: any, env?: object): object;
   }
   /**
   */
   function lipid_classprofiles(lipid_class: object): object;
   /**
   */
   function lipid_profiles(categry: object, enrich: object): object;
   /**
    * gets the metabolite id collection from lipidmaps database
    * 
    * 
     * @param lipidmaps A lipidmaps database related dataset object
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function lipidmaps_id(lipidmaps: any, env?: object): any;
   /**
    * load the herb compound information
    * 
    * 
     * @param file -
   */
   function load_HERB_ingredient(file: string): object;
   /**
    * load compounds from herbs database
    * 
    * 
   */
   function load_herbs(repo: string): object;
   /**
    * load herbs species information
    * 
    * 
     * @param file -
   */
   function load_herbs_list(file: string): object;
   /**
    * construct a new metabolite annotation information data
    * 
    * 
     * @param iupac_name 
     * + default value Is ``null``.
     * @param xref the database cross reference links of current metabolite.
     * 
     * + default value Is ``null``.
     * @param synonym 
     * + default value Is ``null``.
     * @param desc 
     * + default value Is ``null``.
     * @param organism_source 
     * + default value Is ``null``.
   */
   function metabo_anno(id: string, formula: string, name: string, iupac_name?: string, xref?: object, synonym?: any, desc?: any, organism_source?: any): object;
   module mona {
      /**
       * Extract the annotation metadata from the MONA comment data
       * 
       * 
        * @param msp A metabolite data which is parse from the MONA msp dataset
      */
      function msp_metadata(msp: object): object;
   }
   /**
    * Extract odors information from the metabolite data
    * 
    * 
     * @param meta -
     * @param env -
   */
   function odors(meta: object, env: object): object;
   module parse {
      /**
      */
      function ChEBI_entity(xml: string): object;
   }
   /**
    * Ranking a set of the given synonym string collection for find common name.
    * 
    * 
     * @param x -
     * @param max_len -
     * 
     * + default value Is ``32``.
     * @param min_len -
     * 
     * + default value Is ``5``.
   */
   function rankingNames(x: any, max_len?: object, min_len?: object): object;
   module read {
      /**
       * read lipidmaps messagepack repository file
       * 
       * 
        * @param file -
        * @param gsea_background and also cast the lipidmaps metabolite metadata to the gsea background model?
        * 
        * + default value Is ``false``.
        * @param category_model 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function lipidmaps(file: any, gsea_background?: boolean, category_model?: boolean, env?: object): object|object|object;
      /**
       * load the lotus natural products metabolite library from a given file
       * 
       * 
        * @param file -
        * @param lazy 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function lotus(file: any, lazy?: boolean, env?: object): object;
      /**
       * read MoNA database file.
       * 
       * 
        * @param rawfile a vector of the mona database file, could be a set of multiple mona database file.
        *  the database reader is switched automatically based on this file path its 
        *  extension name. currently supported data file formats: ``sdf`` and ``msp``.
        * @param skipSpectraInfo 
        * + default value Is ``false``.
        * @param is_gcms Load gcms reference dataset?
        * 
        * + default value Is ``false``.
        * @param lazy Create a lazy data populator or load all data in memory and returns a vector of the spectral data
        * 
        * + default value Is ``true``.
        * @param verbose 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a linq pipeline for populate the spectrum data 
        *  from the MoNA database.
      */
      function MoNA(rawfile: string, skipSpectraInfo?: boolean, is_gcms?: boolean, lazy?: boolean, verbose?: boolean, env?: object): object;
      /**
       * read the csv table of refmet
       * 
       * > the sheet table could be download from page:
       * >  
       * >  > https://www.metabolomicsworkbench.org/databases/refmet/browse.php
       * >  > Reference: RefMet: a reference nomenclature for metabolomics (Nature Methods, 2020)
       * 
        * @param file -
      */
      function RefMet(file: string): object;
      /**
       * read metabolite data in a given sdf data file.
       * 
       * 
        * @param file the file path of the target sdf file
        * @param parseStruct Andalso parse the molecular structure data inside the metabolite annotation data?
        * 
        * + default value Is ``true``.
        * @param lazy 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function SDF(file: string, parseStruct?: boolean, lazy?: boolean, env?: object): object;
   }
   module save {
      /**
       * Save id mapping file in json file format
       * 
       * 
        * @param mapping -
        * @param file ``*.json`` file name.
        * @param envir -
        * 
        * + default value Is ``null``.
      */
      function mapping(mapping: object, file: string, envir?: object): any;
   }
   module secondary2main {
      /**
       * Create SecondaryIDSolver object from mapping file or mapping dictionary object data.
       * 
       * 
        * @param mapping -
        * @param envir 
        * + default value Is ``null``.
      */
      function mapping(mapping: any, envir?: object): object;
   }
   module write {
      /**
       * save lipidmaps data repository.
       * 
       * > save the lipidmaps data object into file in messagepack format
       * 
        * @param lipidmaps A collection of the lipidmaps metabolite @``T:BioNovoGene.BioDeep.Chemistry.LipidMaps.MetaData``
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function lipidmaps(lipidmaps: any, file: any, env?: object): boolean;
      /**
       * write the metabolite annotation data collection as messagepack
       * 
       * 
        * @param metadb should be a collection of the mzkit metabolite annotation model @``T:BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib``.
        * @param file the file to the target messagepack file
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function metalib(metadb: any, file: any, env?: object): boolean;
   }
   /**
   */
   function write_mona(pack: object, spec: object): ;
}
