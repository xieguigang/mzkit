#Region "Microsoft.VisualBasic::114571cf8fe5863d7456ad677c54fe54, src\mzkit\mzkit\settings\PrecursorSearchSettings.vb"

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

    ' Class PrecursorSearchSettings
    ' 
    '     Properties: ppm, precursor_types
    ' 
    ' Enum FormulaSearchProfiles
    ' 
    '     GeneralFlavone
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class FormulaSearchProfile
    ' 
    '     Properties: elements
    ' 
    '     Function: CreateOptions
    ' 
    ' Class ElementRange
    ' 
    '     Properties: max, min
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Public Class PrecursorSearchSettings

    Public Property ppm As Double
    Public Property precursor_types As String()

End Class

Public Enum FormulaSearchProfiles
    <Description("Custom_Profile")> Custom
    <Description("Default_Profile")> [Default]
    <Description("Small_Molecule")> SmallMolecule
    <Description("Natural_Product")> NaturalProduct
    GeneralFlavone
End Enum

Public Class FormulaSearchProfile

    Public Property elements As Dictionary(Of String, ElementRange)

    Public Function CreateOptions() As SearchOption
        Dim opts = New SearchOption(-99999, 99999, 5)

        For Each element In elements
            opts.AddElement(element.Key, element.Value.min, element.Value.max)
        Next

        Return opts
    End Function
End Class

Public Class ElementRange
    Public Property min As Integer
    Public Property max As Integer

End Class
