#Region "Microsoft.VisualBasic::c7acd1fca07ca818b052ada1ce50211e, metadb\Massbank\Public\TMIC\HMDB\Spectra\Extensions.vb"

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

    '   Total Lines: 58
    '    Code Lines: 43
    ' Comment Lines: 7
    '   Blank Lines: 8
    '     File Size: 2.31 KB


    '     Module Extensions
    ' 
    '         Function: ParseIonizationMode, PopulateSpectras
    ' 
    '     Enum IonizationModes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace TMIC.HMDB.Spectra

    Public Module Extensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="repository"></param>
        ''' <returns>
        ''' populate a collection of the tuple data for: [file name, spectral]
        ''' </returns>
        <Extension>
        Public Iterator Function PopulateSpectras(repository As String) As IEnumerable(Of NamedValue(Of SpectraFile))
            For Each file As String In repository.EnumerateFiles("*.xml")
                Dim header = file.IterateAllLines.Take(2).Last
                Dim rootNodeName$ = header.Trim("<"c, ">"c).ToLower
                Dim fileName As String = file.BaseName
                Dim spectra As SpectraFile

                Select Case rootNodeName
                    Case "c-ms" : spectra = file.LoadXml(Of CMS)
                    Case "ms-ms" : spectra = file.LoadXml(Of MSMS)
                    Case "ei-ms" : spectra = file.LoadXml(Of EIMS)
                    Case "nmr-one-d" : spectra = file.LoadXml(Of NMR1D)
                    Case "nmr-two-d" : spectra = file.LoadXml(Of NMR2D)
                    Case Else
                        Throw New NotImplementedException($"{rootNodeName} @ {file}")
                End Select

                Yield New NamedValue(Of SpectraFile)(fileName, spectra)
            Next
        End Function

        Public Function ParseIonizationMode(mode As String) As IonizationModes
            If mode.StringEmpty OrElse mode = vbNullChar OrElse mode = "NA" OrElse mode = "N/A" Then
                Return IonizationModes.NA
            End If

            Select Case Strings.LCase(mode)
                Case "positive", "pos", "+", "1", "+1", "p"
                    Return IonizationModes.Positive
                Case "negative", "neg", "-", "-1", "n"
                    Return IonizationModes.Negative
                Case Else
                    Throw New NotImplementedException(mode)
            End Select
        End Function
    End Module

    Public Enum IonizationModes As Integer
        NA = 0
        Negative = -1
        Positive = 1
    End Enum
End Namespace
