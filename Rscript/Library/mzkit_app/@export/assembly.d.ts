// export R# package module type define for javascript/typescript language
//
//    imports "assembly" from "mzkit";
//
// ref=mzkit.Assembly@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The mass spectrum assembly file read/write library module.
 * 
 * > #### Mass spectrometry data format
 * >  
 * >  Mass spectrometry is a scientific technique for measuring the mass-to-charge ratio of ions.
 * >  It is often coupled to chromatographic techniques such as gas- or liquid chromatography and 
 * >  has found widespread adoption in the fields of analytical chemistry and biochemistry where 
 * >  it can be used to identify and characterize small molecules and proteins (proteomics). The 
 * >  large volume of data produced in a typical mass spectrometry experiment requires that computers 
 * >  be used for data storage and processing. Over the years, different manufacturers of mass 
 * >  spectrometers have developed various proprietary data formats for handling such data which 
 * >  makes it difficult for academic scientists to directly manipulate their data. To address this 
 * >  limitation, several open, XML-based data formats have recently been developed by the Trans-Proteomic
 * >  Pipeline at the Institute for Systems Biology to facilitate data manipulation and innovation 
 * >  in the public sector.
*/
declare namespace assembly {
   module file {
      /**
       * get file index string of the given ms2 peak data.
       * 
       * 
        * @param ms2 -
      */
      function index(ms2: object): string;
   }
   /**
   */
   function load_index(file: string): object;
   module mgf {
      /**
       * this function ensure that the output result of the any input ion objects is peakms2 data type.
       * 
       * 
        * @param ions a vector of mgf @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF.Ions`` from the ``read.mgf`` function or other data source.
        * @param lazy 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function ion_peaks(ions: any, lazy?: boolean, env?: object): object;
   }
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
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function xml_seek(file: string, env?: object): object;
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
   module read {
      /**
       * Read the spectrum data inside a mgf ASCII data file.
       * 
       * 
        * @param file the file path to the target mgf data file
        * @param env 
        * + default value Is ``null``.
      */
      function mgf(file: any, env?: object): object;
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
       * 
       * 
        * @param file -
        * @param parseMs2 -
        * 
        * + default value Is ``true``.
      */
      function msp(file: string, parseMs2?: boolean): object;
   }
   /**
    * get all scan id from the ms xml file
    * 
    * 
     * @param file -
     * @return A character vector of the scan id for read ms data
   */
   function scan_id(file: object): string;
   /**
   */
   function seek(file: object, key: string): object;
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
}
