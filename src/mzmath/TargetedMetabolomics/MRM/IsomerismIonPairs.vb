Imports Microsoft.VisualBasic.Linq

Namespace MRM.Models

    Public Class IsomerismIonPairs : Implements IEnumerable(Of IonPair)

        Public Property ions As IonPair()
        Public Property target As IonPair

        ''' <summary>
        ''' Get the chromatogram overlaps index by rt/ri order
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property index As Integer
            Get
                If ions.IsNullOrEmpty Then
                    Return Scan0
                Else
                    Dim vec As IonPair() = Me.ToArray

                    For i As Integer = 0 To vec.Length - 1
                        If vec(i).accession = target.accession Then
                            Return i
                        End If
                    Next

                    Throw New InvalidProgramException
                End If
            End Get
        End Property

        Public ReadOnly Property hasIsomerism As Boolean
            Get
                Return Not ions.IsNullOrEmpty
            End Get
        End Property

        Friend Function groupKey() As String
            Return Me.Select(Function(i) i.accession).JoinBy("|->|")
        End Function

        Public Overrides Function ToString() As String
            If hasIsomerism Then
                Return $"[{index}, rt:{target.rt} (sec)] {target}"
            Else
                Return target.ToString
            End If
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of IonPair) Implements IEnumerable(Of IonPair).GetEnumerator
            For Each i In ions.Join(target).OrderBy(Function(ion) ion.rt)
                Yield i
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace