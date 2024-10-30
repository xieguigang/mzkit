// export R# package module type define for javascript/typescript language
//
//    imports "InChI" from "mzkit";
//
// ref=mzkit.InChITool@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * InChI (International Chemical Identifier) string parser
 * 
*/
declare namespace InChI {
   /**
   */
   function get_formula(inchi: object): object;
   /**
   */
   function get_struct(inchi: object): object;
   /**
   */
   function inchikey(inchi: object): object;
   /**
    * parse the inchi string
    * 
    * > Parsing an InChI (International Chemical Identifier) string can be a complex task due to the detailed and structured format of the InChI. 
    * >  However, the InChI format is designed to be readable by both humans and machines, and it can be broken down into several layers that 
    * >  represent different aspects of the chemical substance.
    * >  
    * >  Here's a basic guide on how to manually parse an InChI string:
    * >  
    * >  1. **InChI Version**: The InChI string starts with the version number, followed by a slash. For example, `InChI=1S/`.
    * >  2. **Formula Layer**: This is the chemical formula of the compound. It starts after the version and is enclosed in parentheses. For example, 
    * >     `InChI=1S/C6H12O6/` indicates that the compound is composed of 6 carbon, 12 hydrogen, and 6 oxygen atoms.
    * >  3. **Main Layer**: This layer describes the connectivity of atoms in the molecule and is followed by a slash. It can include:
    * >     - Atom connectivity (`c`)
    * >     - Hydrogen atoms (`h`)
    * >     - Charge (`q`)
    * >     - Stereochemistry (`b`, `t`, `m`)
    * >  4. **Stereochemical Layer**: This layer provides information about the stereochemistry of the compound if applicable. It is also followed by a slash.
    * >  5. **Isotopic Layer**: If the compound contains isotopes, this layer will provide that information, followed by a slash.
    * >  6. **Fixed-H Layer**: This layer indicates the positions of non-exchangeable hydrogen atoms and is followed by a slash.
    * >  7. **Reconnected Layer**: In cases where the molecule can be disconnected into multiple subunits, this layer will describe the reconnected
    * >     form of the molecule.
    * >  8. **Charge Layer**: This layer indicates the net electric charge of the compound.
    * >  9. **Auxiliary Information**: Sometimes additional information is provided in parentheses at the end of the InChI string.
    * >  
    * >  Here's an example of an InChI string broken down into its components:
    * >  
    * >  ```
    * >  InChI=1S/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-1/h1-11H/t2-6+/m1/s1
    * >  ```
    * >  
    * >  - `InChI=1S/` indicates the InChI version.
    * >  - `C6H12O6/` is the formula layer.
    * >  - `c7-1-2-3(8)4(9)5(10)6(11)` is the main layer with atom connectivity.
    * >  - `h1-11H` indicates the presence of hydrogen atoms.
    * >  - `t2-6+` indicates the stereochemistry.
    * >  - `/m1/s1` provides additional stereochemical information.
    * 
     * @param inchi -
   */
   function parse(inchi: string): object;
}
