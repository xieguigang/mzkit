#Region "Microsoft.VisualBasic::24afaba7fd9f7df37ab2b2f52738e00c, assembly\Comprehensive\MsImaging\Converter\ConvertSingleScan.vb"

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

    '   Total Lines: 74
    '    Code Lines: 49 (66.22%)
    ' Comment Lines: 13 (17.57%)
    '    - Xml Docs: 92.31%
    ' 
    '   Blank Lines: 12 (16.22%)
    '     File Size: 2.75 KB


    '     Module ConvertSingleScan
    ' 
    '         Function: ConvertToMSI, ConvertToMSIFromMRM, ConvertToMSIInternal
    ' 
    '         Sub: dimensionNotMatched
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Imaging.Math2D

Namespace MsImaging

    Public Module ConvertSingleScan

        <Extension>
        Public Function ConvertToMSI(raw As mzPack, dims As Size) As mzPack
            If raw.Application = FileApplicationClass.MSImaging Then
                ' is already here
                Return raw
            ElseIf raw.Application = FileApplicationClass.LCMSMS Then
                Return ConvertToMSIFromMRM(raw, dims)
            Else
                Return ConvertToMSIInternal(raw, dims)
            End If
        End Function

        ''' <summary>
        ''' MS-Imaging in MRM targetted mode
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="dims"></param>
        ''' <returns></returns>
        Private Function ConvertToMSIFromMRM(raw As mzPack, dims As Size) As mzPack
            raw = ConvertToMSIInternal(raw, dims)
            raw.Application = FileApplicationClass.MSImaging
            raw.metadata("MRM") = "yes"
            Return raw
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Sub dimensionNotMatched(raw As mzPack, dims As Size)
            Call $"the raw data scan size({raw.MS.Length}) is not matched with the given ms-imaging dimension size({dims.ToString} = {dims.Area})!".Warning
        End Sub

        ''' <summary>
        ''' works for scan by lines
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="dims"></param>
        ''' <returns></returns>
        Private Function ConvertToMSIInternal(raw As mzPack, dims As Size) As mzPack
            Dim ms1 As ScanMS1()() = raw.MS.OrderBy(Function(a) a.rt).Split(dims.Width)

            If ms1.Length <> dims.Height Then
                Call dimensionNotMatched(raw, dims)
            End If

            For rowId As Integer = 0 To ms1.Length - 1
                Dim row As ScanMS1() = ms1(rowId)

                For x As Integer = 0 To row.Length - 1
                    If row(x).meta Is Nothing Then
                        row(x).meta = New Dictionary(Of String, String)
                    End If

                    row(x).meta("x") = x + 1
                    row(x).meta("y") = rowId + 1
                Next
            Next

            If raw.metadata Is Nothing Then
                raw.metadata = New Dictionary(Of String, String)
            End If

            raw.Application = FileApplicationClass.MSImaging
            Return raw
        End Function
    End Module
End Namespace
