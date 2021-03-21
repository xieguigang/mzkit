#Region "Microsoft.VisualBasic::3f622f94849c89371a6e5e70277de00f, src\assembly\assembly\mzPack\ActivationMethods.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Enum ActivationMethods
    ' 
    '     CID, ECD, HCD, IRMPD, NA
    '     SID
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' the most common ion activation techniques employed in tandem mass spectrometry
''' </summary>
Public Enum ActivationMethods As Byte
    NA

    ''' <summary>
    ''' In collision-induced dissociation (CID), activation of the selected ions 
    ''' occurs by collision(s) with neutral gas molecules in a collision cell. 
    ''' This experiment can be done at high (keV) collision energies, using tandem 
    ''' sector and time-of-flight instruments, or at low (eV range) energies, in 
    ''' tandem quadrupole and ion trapping instruments.
    ''' </summary>
    CID
    HCD
    ''' <summary>
    ''' Collisional activation upon impact of precursor ions on solid surfaces, 
    ''' surface-induced dissociation (SID), is gaining importance as an alternative 
    ''' to gas targets and has been implemented in several different types of mass 
    ''' spectrometers.
    ''' </summary>
    SID
    ''' <summary>
    ''' unique fragmentation mechanisms of multiply-charged species can be studied 
    ''' by electron-capture dissociation (ECD). The ECD technique has been recognized 
    ''' as an efficient means to study non-covalent interactions and to gain 
    ''' sequence information in proteomics applications.
    ''' </summary>
    ECD
    ''' <summary>
    ''' Trapping instruments, such as quadrupole ion traps and Fourier transform ion 
    ''' cyclotron resonance instruments, are particularly useful for the photoactivation 
    ''' of ions, specifically for fragmentation of precursor ions by infrared 
    ''' multiphoton dissociation (IRMPD). IRMPD is a non-selective activation method 
    ''' and usually yields rich fragmentation spectra. 
    ''' </summary>
    IRMPD

End Enum

