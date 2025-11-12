#Region "Microsoft.VisualBasic::df40974bcc2d83830fb4801f18602209, assembly\mzPackExtensions\RawStream.vb"

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

    '   Total Lines: 43
    '    Code Lines: 35 (81.40%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (18.60%)
    '     File Size: 1.49 KB


    ' Module RawStream
    ' 
    '     Function: GetChromatogram, LoadFromWiffRaw, LoadFromXRaw
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Language

Public Module RawStream

#If NET48 Then
    <Extension>
    Public Function LoadFromWiffRaw(raw As sciexWiffReader.WiffScanFileReader,
                                    Optional checkNoise As Boolean = True,
                                    Optional println As Action(Of String) = Nothing) As mzPack

        Return New WiffRawStream(raw, checkNoise:=checkNoise).StreamTo(println:=println)
    End Function
#End If

    ''' <summary>
    ''' Load mzpack raw data object from Xcalibur raw data file object
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="println"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadFromXcaliburRaw(raw As MSFileReader, Optional println As Action(Of String) = Nothing) As mzPack
        Return New XRawStream(raw).StreamTo(println:=println)
    End Function

    ''' <summary>
    ''' Load mzpack raw data object from Xcalibur raw data file
    ''' </summary>
    ''' <param name="path">the file path to a given Xcalibur raw data file</param>
    ''' <param name="println"></param>
    ''' <returns></returns>
    Public Function LoadFromXcaliburRaw(path As String, Optional println As Action(Of String) = Nothing) As mzPack
        Dim raw As New MSFileReader(path)
        Dim data As mzPack = LoadFromXcaliburRaw(raw, println)

        Return data
    End Function


    <Extension>
    Public Function GetChromatogram(mzpack As mzPack) As Chromatogram
        Dim rt As New List(Of Double)
        Dim BPC As New List(Of Double)
        Dim TIC As New List(Of Double)

        For Each scan As ScanMS1 In mzpack.MS
            rt += scan.rt
            BPC += scan.BPC
            TIC += scan.TIC
        Next

        Return New Chromatogram With {
            .TIC = TIC.ToArray,
            .BPC = BPC.ToArray,
            .scan_time = rt.ToArray
        }
    End Function
End Module
