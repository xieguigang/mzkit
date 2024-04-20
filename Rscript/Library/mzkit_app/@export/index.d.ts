// export R# source type define for javascript/typescript language
//
// package_source=mzkit

declare namespace mzkit {
   module _ {
      /**
        * @param type default value Is ``["genes", "disease", "compounds"]``.
      */
      function extract_pubmed_evidence(evidence: any, u: any, v: any, type?: any): object;
      /**
        * @param type default value Is ``["genes", "disease", "compounds"]``.
      */
      function graph_table(nodes: any, type?: any): object;
      /**
      */
      function minPos(mat: any): object;
      /**
      */
      function onLoad(): object;
      /**
        * @param type default value Is ``["genes", "disease", "compounds"]``.
      */
      function term_maps(x: any, type?: any): object;
   }
   /**
     * @param peak.width default value Is ``[3, 90]``.
   */
   function __deconv_gcms_single(file: any, peak.width?: any): object;
   /**
   */
   function __ms1_xic_bins_single(path: any, mzdiff: any, outputdir: any): object;
   /**
   */
   function ANOVAGroup(data: any, sampleinfo: any): object;
   /**
   */
   function convertToMzPack(file: string): object;
   /**
     * @param export default value Is ``./``.
     * @param peak.width default value Is ``[3, 90]``.
     * @param n_threads default value Is ``16``.
   */
   function deconv_gcms(rawdata: any, export?: any, peak.width?: any, n_threads?: any): object;
   /**
   */
   function GCMS_contentTable(mslIons: any, calfiles: string): object;
   /**
     * @param output_dir default value Is ``./``.
   */
   function GCMS_linearReport(sim: any, ions: any, quantify: any, calfiles: string, output_dir?: any): object;
   /**
     * @param peakwidth default value Is ``[5, 13]``.
     * @param rtshift default value Is ``30``.
     * @param maxDeletions default value Is ``2``.
   */
   function GCMS_linears(contentTable: any, mslIons: any, calfiles: string, peakwidth?: any, rtshift?: any, maxDeletions?: any): object;
   /**
   */
   function GCMS_quantify(linears: any, sim: any, sampleData: any): object;
   /**
     * @param top_n default value Is ``5``.
     * @param mzdiff default value Is ``0.3``.
     * @param intocutoff default value Is ``0.05``.
     * @param equals default value Is ``0.85``.
   */
   function get_representives(ions: any, top_n?: any, mzdiff?: any, intocutoff?: any, equals?: any): object;
   /**
   */
   function getDataValues(section: any): object;
   /**
   */
   function getQuery(fileName: any): object;
   /**
     * @param unit default value Is ``Minute``.
   */
   function ionPairsFromMsl(ions: any, unit?: any): object;
   /**
     * @param precursors default value Is ``["[M]+", "[M+H]+", "[M+H-H2O]+"]``.
     * @param mzdiff default value Is ``ppm:20``.
     * @param repofile default value Is ``KEGG_compounds.msgpack``.
     * @param strict default value Is ``false``.
   */
   function kegg_compounds(precursors?: any, mzdiff?: any, repofile?: any, strict?: any): object;
   /**
     * @param cache default value Is ``./graph_kb``.
   */
   function knowledge_graph(cid: any, cache?: any): object;
   /**
     * @param repofile default value Is ``Call "system.file"("data/LIPIDMAPS.msgpack", "package" <- "mzkit")``.
     * @param gsea default value Is ``false``.
     * @param category default value Is ``false``.
   */
   function lipidmaps_repo(repofile?: any, gsea?: any, category?: any): object;
   /**
     * @param lazy default value Is ``false``.
   */
   function load_LMSD(filepath: any, lazy?: any): object;
   /**
   */
   function loadTree(files: string): object;
   /**
     * @param topics default value Is ``null``.
   */
   function mesh_model(topics?: any): object;
   /**
   */
   function ms1_mz_bins(files: any): object;
   /**
     * @param mzdiff default value Is ``0.005``.
     * @param peak.width default value Is ``[3, 90]``.
   */
   function ms1_peaktable(files: any, mzbins: any, mzdiff?: any, peak.width?: any): object;
   /**
     * @param mzdiff default value Is ``0.005``.
     * @param outputdir default value Is ``./XIC/``.
     * @param n_threads default value Is ``32``.
   */
   function ms1_xic_bins(files: any, mzdiff?: any, outputdir?: any, n_threads?: any): object;
   /**
     * @param factor default value Is ``null``.
   */
   function normData(mat: any, factor?: any): object;
   /**
     * @param output_dir default value Is ``./``.
   */
   function output_datatables(quantify: any, linears: any, output_dir?: any): object;
   /**
   */
   function parseDescriptors(descriptors: any): object;
   /**
   */
   function parseNames(names: any): object;
   /**
   */
   function parsePubchemMeta(document: any): object;
   /**
   */
   function parseXref(refs: any): object;
   /**
     * @param mslIons default value Is ``null``.
     * @param output_dir default value Is ``./linears``.
   */
   function plotLinears(linears: any, mslIons?: any, output_dir?: any): object;
   /**
     * @param process default value Is ``null``.
     * @param extensionCache default value Is ``./.cache/extdata/``.
   */
   function pubchem_graphjson(dataXml: any, process?: any, extensionCache?: any): object;
   /**
   */
   function pubchem_meta(term: any): object;
   module run {
      /**
        * @param outputdir default value Is ``./``.
        * @param mzdiff default value Is ``0.005``.
        * @param peak.width default value Is ``[3, 90]``.
      */
      function Deconvolution(rawdata: any, outputdir?: any, mzdiff?: any, peak.width?: any): object;
   }
   /**
   */
   function tolerance(kind: string, mzdiff: number): object;
}
