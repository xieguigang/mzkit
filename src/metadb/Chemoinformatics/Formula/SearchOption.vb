Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Formula

    Public Class SearchOption

        Public ReadOnly Property candidateElements As List(Of ElementSearchCandiate)
        Public ReadOnly Property ppm As Double
        Public ReadOnly Property chargeRange As IntRange

        Sub New(minCharge As Integer, maxCharge As Integer, Optional ppm As Double = 30)
            Me.candidateElements = New List(Of ElementSearchCandiate)
            Me.ppm = ppm
            Me.chargeRange = New IntRange(minCharge, maxCharge)
        End Sub

        Public Function AddElement(element As String, min As Integer, max As Integer) As SearchOption
            Call New ElementSearchCandiate With {
                .Element = element,
                .MaxCount = max,
                .MinCount = min
            }.DoCall(AddressOf candidateElements.Add)

            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function DefaultMetaboliteProfile() As SearchOption
            Return New SearchOption(-3, 3) _
                .AddElement("C", 1, 20) _
                .AddElement("H", 4, 300) _
                .AddElement("N", 1, 100) _
                .AddElement("O", 2, 100) _
                .AddElement("P", 1, 100)
        End Function

    End Class
End Namespace