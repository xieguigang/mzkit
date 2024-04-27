#Region "Microsoft.VisualBasic::2a578ecaeb8e9210bbace575dc93658a, G:/mzkit/Rscript/Library/mzkit_app/src/mzkit//assembly/NMR.vb"

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


    ' Code Statistics:

    '   Total Lines: 64
    '    Code Lines: 37
    ' Comment Lines: 18
    '   Blank Lines: 9
    '     File Size: 2.10 KB


    ' Module NMRTool
    ' 
    '     Function: acquisition, FourierTransform, GetMatrix, readSmall, spectrumData
    '               spectrumList
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.nmrML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.NMRFidTool
Imports BioNovoGene.Analytical.NMRFidTool.fidMath.FFT
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("NMR")>
Module NMRTool

    <ExportAPI("read.nmrML")>
    Public Function readSmall(file As String) As nmrML.XML
        Return file.LoadXml(Of nmrML.XML)
    End Function

    ''' <summary>
    ''' get all acquisition data in the raw data file
    ''' </summary>
    ''' <param name="nmrML"></param>
    ''' <returns></returns>
    <ExportAPI("acquisition")>
    Public Function acquisition(nmrML As nmrML.XML) As acquisition()
        Return nmrML.acquisition
    End Function

    ''' <summary>
    ''' Read Free Induction Decay data matrix
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <ExportAPI("FID")>
    Public Function GetMatrix(data As acquisition) As Fid
        Return Fid.Create(data)
    End Function

    <ExportAPI("nmr_dft")>
    Public Function FourierTransform(fidData As Fid) As Spectrum
        Return New FastFourierTransform1D(New Spectrum(fidData)).computeFFT
    End Function

    <ExportAPI("spectrumList")>
    Public Function spectrumList(nmrML As nmrML.XML) As spectrumList()
        Return nmrML.spectrumList
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="spectrum"></param>
    ''' <param name="nmrML"></param>
    ''' <returns>
    ''' a matrix of [ppm => intensity]
    ''' </returns>
    <ExportAPI("spectrum")>
    Public Function spectrumData(spectrum As spectrumList, nmrML As nmrML.XML) As LibraryMatrix
        Dim data = spectrum.spectrum1D(Scan0)
        Dim sw = nmrML.acquisition.First.acquisition1D.SW
        Dim matrix = data.ParseMatrix(SW:=sw)

        Return matrix
    End Function

End Module
