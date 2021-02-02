#Region "Microsoft.VisualBasic::83a5d7686b59af7c76b081365a839f05, assembly\UnifyReader\Provider.vb"

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

    '     Module Provider
    ' 
    '         Function: GetMsMs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Module Provider

        Public Function GetMsMs(Of Scan)() As Func(Of Scan, ms2())
            Dim reader = MsDataReader(Of Scan).ScanProvider

            Select Case GetType(Scan)
                Case GetType(mzXML.scan)
                    Return CObj(New Func(Of mzXML.scan, ms2())(AddressOf DirectCast(reader, mzXMLScan).GetMsMs))
                Case GetType(mzML.spectrum)
                    Return CObj(New Func(Of mzML.spectrum, ms2())(AddressOf DirectCast(reader, mzMLScan).GetMsMs))
                Case Else
                    Throw New NotImplementedException(GetType(Scan).FullName)
            End Select
        End Function

    End Module
End Namespace
