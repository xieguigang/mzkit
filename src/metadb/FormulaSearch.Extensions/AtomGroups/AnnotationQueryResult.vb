Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Namespace AtomGroups

    Public Class AnnotationQueryResult

        Public Property Annotation As FragmentAnnotationHolder
        Public Property delta As Double

        Sub New(annotation As FragmentAnnotationHolder, delta As Double)
            Me.delta = delta
            Me.Annotation = annotation
        End Sub

        Public Shared Function TestValid(annotation As FragmentAnnotationHolder) As Boolean
            Dim formula = annotation.base

            If TypeOf formula Is Formula Then
                Dim f As Formula = formula

                If f.CountsByElement.Count = 1 Then
                    For Each atom As String In {"O", "N", "P", "S", "H"}
                        If f.Elements(Scan0) = atom Then
                            Return f(atom) < 2
                        End If
                    Next

                    Return True
                Else
                    Return True
                End If
            Else
                ' no data to run test
                ' assuming is correct data
                Return True
            End If
        End Function

        Public Shared Function TestValid(annotation As FragmentAnnotationHolder, molecule As Formula) As Boolean
            Dim formula = annotation.base

            If TypeOf formula Is Formula Then
                Dim f As Formula = formula

                For Each atom As String In f.Elements
                    If molecule(atom) = 0 Then
                        ' missing in metabolite
                        Return False
                    End If
                Next

                If f.CountsByElement.Count = 1 Then
                    For Each atom As String In {"O", "N", "P", "S", "H"}
                        If f.Elements(Scan0) = atom Then
                            Return f(atom) < 2
                        End If
                    Next

                    Return True
                Else
                    Return True
                End If
            Else
                ' no data to run test
                ' assuming is correct data
                Return True
            End If
        End Function
    End Class
End Namespace