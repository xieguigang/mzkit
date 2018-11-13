#Region "Microsoft.VisualBasic::2094732b04a0529208fd4c5068f157e6, Massbank\Public\TMIC\HMDB\Spectra\Extensions.vb"

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

        <Extension>
        Public Iterator Function PopulateSpectras(repository As String) As IEnumerable(Of NamedValue(Of SpectraFile))
            For Each file As String In repository.EnumerateFiles("*.xml")
                Dim header = file.IterateAllLines _
                                 .Take(2) _
                                 .Last
                Dim rootNodeName$ = header _
                    .Match("[<]\S+") _
                    .Trim("<"c) _
                    .ToLower

                Select Case rootNodeName
                    Case "c-ms" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of CMS))
                    Case "ms-ms" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of MSMS))
                    Case "ei-ms" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of EIMS))
                    Case "nmr-one-d" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of NMR1D))
                    Case "nmr-two-d" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of NMR2D))
                    Case Else
                        Throw New NotImplementedException($"{rootNodeName} @ {file}")
                End Select
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

