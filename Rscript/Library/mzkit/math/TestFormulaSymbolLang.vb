Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports SMRUCC.Rsharp.Runtime.Interop

Public Class TestFormulaSymbolLang : Implements ITestSymbolTarget

    Public Function Assert(symbol As Object) As Boolean Implements ITestSymbolTarget.Assert
        If symbol Is Nothing OrElse Not TypeOf symbol Is Formula Then
            Return False
        End If

        Try
            Dim mass = DirectCast(symbol, Formula).ExactMass
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function
End Class
