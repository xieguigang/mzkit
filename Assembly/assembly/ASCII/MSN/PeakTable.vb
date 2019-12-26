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
            Dim lines As String() = file.IterateAllLines.SkipWhile(Function(l) l.First = "#"c).Skip(1).ToArray
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