#Region "Microsoft.VisualBasic::03d0eb583e7ea4fb5ba219afd52a7df6, G:/mzkit/src/assembly/ThermoRawFileReader//DataObjects/RawLabelData.vb"

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
    '    Code Lines: 19
    ' Comment Lines: 12
    '   Blank Lines: 9
    '     File Size: 1.22 KB


    '     Class RawLabelData
    ' 
    '         Properties: MaxIntensity, MSData, MsLevel, ScanNumber, ScanTime
    ' 
    '         Function: [Select], ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace DataObjects

    <CLSCompliant(True)>
    Public Class RawLabelData

        ''' <summary>
        ''' Scan number
        ''' </summary>
        Public Property ScanNumber As Integer

        ''' <summary>
        ''' Acquisition time (in minutes)
        ''' </summary>
        Public Property ScanTime As Double

        Public Property MsLevel As Integer

        ''' <summary>
        ''' Label data (if FTMS), otherwise peak data
        ''' </summary>
        Public Property MSData As List(Of FTLabelInfoType)

        ''' <summary>
        ''' Maximum intensity of the peaks in this scan
        ''' </summary>
        Public Property MaxIntensity As Double

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function [Select](Of T)(project As Func(Of FTLabelInfoType, Integer, T)) As IEnumerable(Of T)
            Return MSData.Select(project)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{ScanNumber}] RT: {ScanTime} = {MaxIntensity}"
        End Function
    End Class
End Namespace
