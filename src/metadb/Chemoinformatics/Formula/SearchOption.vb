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

        Public Function AdjustPpm(ppm As Double) As SearchOption
            _ppm = ppm
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function DefaultMetaboliteProfile() As SearchOption
            Return New SearchOption(-999999999, 999999999, ppm:=1) _
                .AddElement("C", 1, 30) _
                .AddElement("H", 0, 300) _
                .AddElement("N", 0, 30) _
                .AddElement("O", 0, 30) _
                .AddElement("P", 0, 30) _
                .AddElement("S", 0, 30)
        End Function

    End Class
End Namespace