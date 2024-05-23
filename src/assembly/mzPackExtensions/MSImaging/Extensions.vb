#Region "Microsoft.VisualBasic::09873354fb3b4a342b2fc0f8238ae263, assembly\mzPackExtensions\MSImaging\Extensions.vb"

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

    '   Total Lines: 40
    '    Code Lines: 33 (82.50%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (17.50%)
    '     File Size: 1.50 KB


    ' Module Extensions
    ' 
    '     Function: CheckMatrixBaseIon
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If NET48 Then
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.Math

Public Module Extensions

    Public Function CheckMatrixBaseIon(fileName As String) As (nscans As Integer, ion As Double)
        Dim n As Integer
        Dim allscans As mzPack

        Select Case fileName.ExtensionSuffix.ToLower
            Case "raw"
                Dim Xraw As New MSFileReader(fileName)

                n = Xraw.ScanMax
                allscans = New XRawStream(Xraw).StreamTo
            Case "wiff"
                Dim wiffRaw As New WiffScanFileReader(fileName)
                Dim println As Action(Of String) = AddressOf Console.WriteLine

                allscans = wiffRaw.LoadFromWiffRaw(checkNoise:=False, println:=println)
                n = allscans.size
            Case Else
                Throw New NotImplementedException(fileName.ExtensionSuffix)
        End Select

        Dim basePeak As Double = allscans.MS _
            .Select(Function(a) a.GetMs.OrderByDescending(Function(i) i.intensity).FirstOrDefault) _
            .Where(Function(mzi) Not mzi Is Nothing) _
            .GroupBy(Function(x) x.mz, offsets:=0.3) _
            .OrderByDescending(Function(ni) ni.Length) _
            .First _
            .name _
            .ParseDouble

        Return (n, basePeak)
    End Function
End Module
#End If
