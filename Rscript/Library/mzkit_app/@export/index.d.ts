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
        * @param quietly default value Is ``false``.
      */
      function onLoad(quietly?: any): object;
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
     * @param mzdiff default value Is ``0.005``.
     * @param outputdir default value Is ``./``.
   */
   function __ms1_xic_bins_single(path: any, mzdiff?: any, outputdir?: any): object;
   /**
     * @param peaks_dir default value Is ``./peaks``.
     * @param mzdiff default value Is ``0.01``.
   */
   function align_peaktable(peaks_dir?: any, mzdiff?: any): object;
   /**
   */
   function ANOVAGroup(data: any, sampleinfo: any): object;
   /**
   */
   function convertToMzPack(file: string): object;
   /**
     * @param export_dir default value Is ``./``.
     * @param peak.width default value Is ``[3, 90]``.
     * @param n_threads default value Is ``16``.
   */
   function deconv_gcms(rawdata: any, export_dir?: any, peak.width?: any, n_threads?: any): object;
   /**
     * @param mzbins default value Is ``null``.
     * @param args default value Is ``Call "list"("peak_method" <- "CentWave",
     *       "snr_threshold" <- 3,
     *       "window_half_width" <- 5,
     *       "min_peak_width" <- 3,
     *       "max_peak_width" <- 30,
     *       "min_peak_height" <- 0,
     *       "centWave_min_scale" <- 1,
     *       "centWave_max_scale" <- 20,
     *       "centWave_scale_step" <- 1,
     *       "centWave_max_gap" <- 2,
     *       "matched_filter_sigma" <- 3,
     *       "matched_filter_truncate_width" <- 4,
     *       "derivative_smooth_window" <- 3,
     *       "derivative_threshold_factor" <- 0.01,
     *       "noise_segment_count" <- 20,
     *       "peak_merge_distance" <- 1,
     *       "area_method" <- "BaselineCorrected",
     *       "baseline_method" <- "Linear",
     *       "baseline_percentile" <- 10,
     *       "local_minimum_boundary_points" <- 5,
     *       "gaussian_max_iterations" <- 100,
     *       "gaussian_convergence" <- 1E-06,
     *       "recalculate_snr" <- TRUE)``.
     * @param simple default value Is ``true``.
     * @param tmp_out default value Is ``./``.
   */
   function deconv_xicfile(path: any, mzbins?: any, args?: any, simple?: any, tmp_out?: any): object;
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
     * @param export_dir default value Is ``./``.
     * @param overlaps_size default value Is ``[2900, 1600]``.
     * @param overlaps_layout default value Is ``padding:5% 5% 10% 12%;``.
     * @param file_color default value Is ``blue``.
   */
   function make_chromatogram_exports(rawdir: any, sampleinfo: any, group_name: any, export_dir?: any, overlaps_size?: any, overlaps_layout?: any, file_color?: any): object;
   /**
     * @param max_rtwin default value Is ``15``.
     * @param mzdiff default value Is ``0.01``.
   */
   function make_peak_alignment(peakfiles: any, max_rtwin?: any, mzdiff?: any): object;
   /**
     * @param topics default value Is ``null``.
   */
   function mesh_model(topics?: any): object;
   /**
   */
   function MRM_dataReport(xic: any, tpa: any): object;
   /**
     * @param mzdiff default value Is ``0.001``.
   */
   function ms1_mz_bins(files: any, mzdiff?: any): object;
   /**
     * @param mzdiff default value Is ``0.01``.
     * @param args default value Is ``Call "list"("peak_method" <- "CentWave",
     *       "snr_threshold" <- 3,
     *       "window_half_width" <- 5,
     *       "min_peak_width" <- 3,
     *       "max_peak_width" <- 30,
     *       "min_peak_height" <- 0,
     *       "centWave_min_scale" <- 1,
     *       "centWave_max_scale" <- 20,
     *       "centWave_scale_step" <- 1,
     *       "centWave_max_gap" <- 2,
     *       "matched_filter_sigma" <- 3,
     *       "matched_filter_truncate_width" <- 4,
     *       "derivative_smooth_window" <- 3,
     *       "derivative_threshold_factor" <- 0.01,
     *       "noise_segment_count" <- 20,
     *       "peak_merge_distance" <- 1,
     *       "area_method" <- "BaselineCorrected",
     *       "baseline_method" <- "Linear",
     *       "baseline_percentile" <- 10,
     *       "local_minimum_boundary_points" <- 5,
     *       "gaussian_max_iterations" <- 100,
     *       "gaussian_convergence" <- 1E-06,
     *       "recalculate_snr" <- TRUE)``.
     * @param simple default value Is ``true``.
     * @param n_threads default value Is ``32``.
     * @param tmp_out default value Is ``./tmp``.
   */
   function ms1_peaktable(files: any, mzbins: any, mzdiff?: any, args?: any, simple?: any, n_threads?: any, tmp_out?: any): object;
   /**
     * @param mzdiff default value Is ``0.005``.
     * @param outputdir default value Is ``./XIC/``.
     * @param n_threads default value Is ``32``.
   */
   function ms1_xic_bins(files: any, mzdiff?: any, outputdir?: any, n_threads?: any): object;
   /**
     * @param libtype default value Is ``[1, -1]``.
     * @param kegg default value Is ``Call "list"("compounds" <- Call GCModeller::"kegg_compounds"("rawList" <- TRUE, "reference_set" <- FALSE), "pathways" <- Call GCModeller::"kegg_maps"())``.
   */
   function mummichog_anno(peaks: any, sampleinfo: any, libtype?: any, kegg?: any): object;
   /**
   */
   function mz_bin_features(mzbins: any): object;
   /**
     * @param output_dir default value Is ``./``.
   */
   function output_datatables(quantify: any, linears: any, output_dir?: any): object;
   /**
     * @param tag default value Is ``null``.
   */
   function pack_singleCells(rawdata: any, tag?: any): object;
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
     * @param sampleinfo default value Is ``null``.
     * @param factor default value Is ``100000000``.
     * @param missing default value Is ``0.5``.
   */
   function preprocessing_expression(x: any, sampleinfo?: any, factor?: any, missing?: any): object;
   /**
     * @param process default value Is ``null``.
     * @param extensionCache default value Is ``./.cache/extdata/``.
   */
   function pubchem_graphjson(dataXml: any, process?: any, extensionCache?: any): object;
   /**
   */
   function pubchem_meta(term: any): object;
   /**
   */
   function pugview_repo(repo_dir: any): object;
   module read {
      /**
      */
      function cfmid_3_EI(file: any): object;
   }
   module run {
      /**
        * @param outputdir default value Is ``./``.
        * @param mzdiff default value Is ``0.01``.
        * @param xic_mzdiff default value Is ``0.005``.
        * @param peak.width default value Is ``[3, 30]``.
        * @param n_threads default value Is ``32``.
        * @param top_n default value Is ``20000``.
        * @param args default value Is ``Call "list"("peak_method" <- "CentWave",
        *       "snr_threshold" <- 3,
        *       "window_half_width" <- 5,
        *       "min_peak_width" <- 3,
        *       "max_peak_width" <- 30,
        *       "min_peak_height" <- 0,
        *       "centWave_min_scale" <- 1,
        *       "centWave_max_scale" <- 20,
        *       "centWave_scale_step" <- 1,
        *       "centWave_max_gap" <- 2,
        *       "matched_filter_sigma" <- 3,
        *       "matched_filter_truncate_width" <- 4,
        *       "derivative_smooth_window" <- 3,
        *       "derivative_threshold_factor" <- 0.01,
        *       "noise_segment_count" <- 20,
        *       "peak_merge_distance" <- 1,
        *       "area_method" <- "BaselineCorrected",
        *       "baseline_method" <- "Linear",
        *       "baseline_percentile" <- 10,
        *       "local_minimum_boundary_points" <- 5,
        *       "gaussian_max_iterations" <- 100,
        *       "gaussian_convergence" <- 1E-06,
        *       "recalculate_snr" <- TRUE)``.
        * @param filename default value Is ``peaktable.csv``.
      */
      function Deconvolution(rawdata: any, outputdir?: any, mzdiff?: any, xic_mzdiff?: any, peak.width?: any, n_threads?: any, top_n?: any, args?: any, filename?: any): object;
   }
   /**
     * @param kind default value Is ``ppm``.
     * @param mzdiff default value Is ``20``.
   */
   function tolerance(kind?: any, mzdiff?: any): object;
}
