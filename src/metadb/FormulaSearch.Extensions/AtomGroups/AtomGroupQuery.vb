Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.My
Imports stdNum = System.Math

Namespace AtomGroups

    Public Class AtomGroupQuery

        ReadOnly mass As Double
        ReadOnly da As Double
        ReadOnly adducts As MzCalculator()

        Sub New(mass As Double,
                Optional da As Double = 0.1,
                Optional adducts As MzCalculator() = Nothing)

            Me.mass = mass
            Me.da = da
            Me.adducts = adducts
        End Sub

        ''' <summary>
        ''' populate all candidate hits list by a specific mass tolerance hits
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function FilterByMass() As IEnumerable(Of FragmentAnnotationHolder)
            For Each group As FragmentAnnotationHolder In SingletonList(Of FragmentAnnotationHolder).ForEach
                ' returns the first value if matched with mass delta
                If stdNum.Abs(group.exactMass - mass) <= da Then
                    Yield group
                ElseIf Not adducts Is Nothing Then
                    ' test on adducts
                    For Each type As MzCalculator In adducts
                        Dim mz As Double = type.CalcMZ(group.exactMass)

                        If stdNum.Abs(mz - mass) <= da Then
                            Yield New FragmentAnnotationHolder(MassGroup.CreateAdducts(group, adducts:=type))
                        End If
                    Next
                End If
            Next
        End Function

        ''' <summary>
        ''' match on first hit
        ''' </summary>
        ''' <returns></returns>
        Public Function GetByMass() As FragmentAnnotationHolder
            For Each group As FragmentAnnotationHolder In SingletonList(Of FragmentAnnotationHolder).ForEach
                ' returns the first value if matched with mass delta
                If stdNum.Abs(group.exactMass - mass) <= da Then
                    Return group
                ElseIf Not adducts Is Nothing Then
                    ' test on adducts
                    For Each type As MzCalculator In adducts
                        Dim mz As Double = type.CalcMZ(group.exactMass)

                        If stdNum.Abs(mz - mass) <= da Then
                            Return New FragmentAnnotationHolder(MassGroup.CreateAdducts(group, adducts:=type))
                        End If
                    Next
                End If
            Next

            Return Nothing
        End Function
    End Class
End Namespace