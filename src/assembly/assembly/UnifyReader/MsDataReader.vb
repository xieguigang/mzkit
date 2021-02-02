#Region "Microsoft.VisualBasic::7087c2147a11370d9497bf9d38792dc7, assembly\UnifyReader\MsDataReader.vb"

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

    '     Class MsDataReader
    ' 
    '         Function: ScanProvider
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public MustInherit Class MsDataReader(Of Scan)

        Public MustOverride Function GetScanTime(scan As Scan) As Double
        Public MustOverride Function GetScanId(scan As Scan) As String
        Public MustOverride Function IsEmpty(scan As Scan) As Boolean

        ''' <summary>
        ''' get ms1 or ms2 data
        ''' </summary>
        ''' <param name="scan"></param>
        ''' <returns></returns>
        Public MustOverride Function GetMsMs(scan As Scan) As ms2()
        Public MustOverride Function GetMsLevel(scan As Scan) As Integer
        Public MustOverride Function GetBPC(scan As Scan) As Double
        Public MustOverride Function GetTIC(scan As Scan) As Double
        Public MustOverride Function GetParentMz(scan As Scan) As Double
        Public MustOverride Function GetPolarity(scan As Scan) As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>
        ''' <see cref="MsDataReader(Of Scan)"/>
        ''' </returns>
        Public Shared Function ScanProvider() As Object
            Select Case GetType(Scan)
                Case GetType(mzXML.scan) : Return New mzXMLScan
                Case GetType(mzML.spectrum) : Return New mzMLScan
                Case GetType(imzML.ScanReader) : Return New imzMLScan
                Case Else
                    Throw New NotImplementedException(GetType(Scan).ToString)
            End Select
        End Function

    End Class
End Namespace
