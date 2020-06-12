an update version of https://omics.pnl.gov/software/thermo-raw-file-reader

The Thermo Raw File Reader is a .NET DLL that demonstrates how to read 
Thermo-Finnigan .Raw files using Thermo's MS File Reader, which is 
available on Thermo's website at http://sjsupport.thermofinnigan.com/public/detail.asp?id=703

The Thermo Raw File Reader DLL provides several methods for parsing the information returned by MSFileReader, including:
- Determining the parent ion m/z and fragmentation mode in a given scan filter
- Determining the Ionization mode from a given scan filter
- Extracting MRM masses listed in a given scan filter
- Reporting the number of spectra in the .Raw file
- Returning details on a specific spectrum
- Obtaining the raw m/z and intensity values for a given spectrum

The Test_ThermoRawFileReader folder contains a .NET command-line application 
that illustrates how to interface with ThermoRawFileReaderDLL.dll

Prior to using ThermoRawFileReaderDLL.dll you must either download and install the MSFileReader,
or use batch file registerFiles.bat in the lib folder to register MSFileReader.XRawfile2.dll

-------------------------------------------------------------------------------
Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)

E-mail: matthew.monroe@pnl.gov or matt@alchemistmatt.com
Website: http://ncrr.pnl.gov/ or http://www.sysbio.org/resources/staff/
-------------------------------------------------------------------------------

Licensed under the Apache License, Version 2.0; you may not use this file except 
in compliance with the License.  You may obtain a copy of the License at 
http://www.apache.org/licenses/LICENSE-2.0

All publications that result from the use of this software should include 
the following acknowledgment statement:
 Portions of this research were supported by the W.R. Wiley Environmental 
 Molecular Science Laboratory, a national scientific user facility sponsored 
 by the U.S. Department of Energy's Office of Biological and Environmental 
 Research and located at PNNL.  PNNL is operated by Battelle Memorial Institute 
 for the U.S. Department of Energy under contract DE-AC05-76RL0 1830.

Notice: This computer software was prepared by Battelle Memorial Institute, 
hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the 
Department of Energy (DOE).  All rights in the computer software are reserved 
by DOE on behalf of the United States Government and the Contractor as 
provided in the Contract.  NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY 
WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS 
SOFTWARE.  This notice including this sentence must appear on any copies of 
this computer software.
