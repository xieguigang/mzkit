#Region "Microsoft.VisualBasic::b6b48b099f1ede469dffd02c524f1eef, assembly\ASCII\MSN\PeakTable.vb"

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

    '     Class PeakTable
    ' 
    '         Properties: annotated_compound_id, annotation, annotation_method_deteils_id, comment, id
    '                     intensity, ion_species, isotope_peaks, mass_detected, retention_index
    '                     retention_time
    ' 
    '         Function: GetPeakAnnotation, ParseTable, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Linq
Imports ANSI = Microsoft.VisualBasic.Text.ASCII

Namespace ASCII.MSN

    Public Class PeakTable

        Public Property id As String
        Public Property intensity As Double
        Public Property retention_time As Double
        Public Property retention_index As Double
        Public Property mass_detected As String
        Public Property ion_species As String
        Public Property isotope_peaks As String
        Public Property annotation As String
        Public Property annotation_method_deteils_id As String
        Public Property annotated_compound_id As String
        Public Property comment As String

        Public Overrides Function ToString() As String
            Return $"{id}: {annotation}"
        End Function

        Friend Shared Function ParseTable(file As String) As PeakTable()
            Dim lines As String() = file _
                .IterateAllLines _
                .SkipWhile(Function(l) l.First = "#"c) _
                .Skip(1) _
                .ToArray
            Dim peaks As PeakTable() = lines _
                .Select(Function(l)
                            Return l.Split(ANSI.TAB).DoCall(AddressOf GetPeakAnnotation)
                        End Function) _
                .ToArray

            Return peaks
        End Function

        Private Shared Function GetPeakAnnotation(data As String()) As PeakTable
            Dim i As Pointer(Of String) = data
            Dim peak As New PeakTable With {
                .id = ++i,
                .intensity = Val(++i),
                .retention_time = Val(++i),
                .retention_index = Val(++i),
                .mass_detected = ++i,
                .ion_species = ++i,
                .isotope_peaks = ++i,
                .annotation = ++i,
                .annotation_method_deteils_id = ++i,
                .annotated_compound_id = ++i,
                .comment = ++i
            }

            Return peak
        End Function
    End Class
End Namespace
