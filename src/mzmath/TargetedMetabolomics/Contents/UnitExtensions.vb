#Region "Microsoft.VisualBasic::afa150fb8eda119a2d7bd01fb24fad0a, TargetedMetabolomics\Contents\UnitExtensions.vb"

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

    '     Module UnitExtensions
    ' 
    '         Function: ContentVector, ParseContent, ppm2ppb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports r = System.Text.RegularExpressions.Regex

Namespace Content

    <HideModuleName>
    Public Module UnitExtensions

        Public Function ppm2ppb(ppm As Double) As Double
            Return ppm.Unit(ContentUnits.ppm).ScaleTo(ContentUnits.ppb).Value
        End Function

        Const contentPattern$ = NumericPattern & "\s*pp(m|b|t)"

        <Extension>
        Public Function ParseContent(text As String) As UnitValue(Of ContentUnits)
            text = LCase(r.Match(text, contentPattern, RegexICSng).Value)

            Return New UnitValue(Of ContentUnits) With {
                .Unit = r _
                    .Match(text, "pp(m|b|t)", RegexICSng) _
                    .TryParse(Of ContentUnits),
                .Value = Val(text)
            }
        End Function

        <Extension>
        Public Function ContentVector(fileNames As IEnumerable(Of String)) As Dictionary(Of String, Double)
            Dim cv As New Dictionary(Of String, Double)

            For Each file As String In fileNames.Select(AddressOf BaseName)
                cv(file) = ParseContent(file).ScaleTo(ContentUnits.ppb)
            Next

            Return cv
        End Function
    End Module
End Namespace
