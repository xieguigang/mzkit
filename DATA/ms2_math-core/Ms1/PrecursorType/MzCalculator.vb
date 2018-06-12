Imports sys = System.Math

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

        ''' <summary>
        ''' 返回加和物的m/z数据
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <param name="adduct#"></param>
        ''' <param name="charge%"></param>
        ''' <returns></returns>
        Public Shared Function AdductMass(mass#, adduct#, charge%) As Double
            Return (mass / sys.Abs(charge) + adduct)
        End Function

        ''' <summary>
        ''' 从质谱的MS/MS的前体的m/z结果反推目标分子的mass结果
        ''' </summary>
        ''' <param name="precursorMZ#"></param>
        ''' <param name="M#"></param>
        ''' <param name="charge%"></param>
        ''' <param name="adduct#"></param>
        ''' <returns></returns>
        Public Shared Function ReverseMass(precursorMZ#, M#, charge%, adduct#) As Double
            Return ((precursorMZ - adduct) * sys.Abs(charge) / M)
        End Function
    End Structure
End Namespace