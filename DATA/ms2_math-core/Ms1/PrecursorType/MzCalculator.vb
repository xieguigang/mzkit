Namespace Ms1.PrecursorType

    Public Structure MzCalculator

        Dim Name$
        Dim charge%
        Dim M%
        Dim adducts#

        Sub New(type$, charge%, M#, adducts#)
            Me.Name = type
            Me.charge = charge
            Me.M = M
            Me.adducts = adducts
        End Sub

        Public Function CalcMass(precursorMZ#) As Double
            Return (ReverseMass(precursorMZ, M, charge, adducts))
        End Function

        Public Function CalcPrecursorMZ(mass#) As Double
            Return (AdductMass(mass, adducts, charge))
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Structure
End Namespace