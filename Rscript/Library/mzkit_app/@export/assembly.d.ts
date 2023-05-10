// export R# package module type define for javascript/typescript language
//
// ref=mzkit.Assembly@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The mass spectrum assembly file read/write library module.
 * 
*/
declare namespace assembly {
   module read {
      /**
       * read MSL data files
       * 
       * 
        * @param unit -
        * 
        * + default value Is ``null``.
      */
      function msl(file: string, unit?: object): object;
      /**
      */
      function mgf(file: string): object;
      /**
        * @param parseMs2 default value Is ``true``.
      */
      function msp(file: string, parseMs2?: boolean): any;
   }
   module mgf {
      /**
       * this function ensure that the output result of the any input ion objects is peakms2 data type.
       * 
       * 
        * @param ions a vector of mgf @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF.Ions`` from the ``read.mgf`` function or other data source.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function ion_peaks(ions: any, env?: object): object;
   }
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function xml_seek(file: string, env?: object): any;
   }
   /**
   */
   function seek(file: object, key: string): object;
   /**
   */
   function scan_id(file: object): string;
   /**
   */
   function load_index(file: string): object;
   module write {
      /**
       * write spectra data in mgf file format.
       * 
       * 
        * @param ions -
        * @param file the file path of the mgf file to write spectra data.
        * @param relativeInto write relative intensity value into the mgf file instead of the raw intensity value.
        *  no recommended...
        * 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function mgf(ions: any, file: string, relativeInto?: boolean, env?: object): boolean;
   }
   module file {
      /**
       * get file index string of the given ms2 peak data.
       * 
       * 
        * @param ms2 -
      */
      function index(ms2: object): string;
   }
   module mzxml {
      /**
       * Convert mzxml file as mgf ions.
       * 
       * 
        * @param file -
        * @param relativeInto 
        * + default value Is ``false``.
        * @param onlyMs2 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function mgf(file: string, relativeInto?: boolean, onlyMs2?: boolean, env?: object): object;
   }
   module raw {
      /**
       * get raw scans data from the ``mzXML`` or ``mzMl`` data file
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function scans(file: string, env?: object): object|object;
   }
   /**
    * get polarity data for each ms2 scans
    * 
    * 
     * @param scans -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function polarity(scans: any, env?: object): object;
   module ms1 {
      /**
       * get all ms1 raw scans from the raw files
       * 
       * 
        * @param raw the file path of the raw data files.
        * @param centroid the tolerance value of m/z for convert to centroid mode
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function scans(raw: any, centroid?: any, env?: object): object;
   }
}
