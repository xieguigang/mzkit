Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

''' <summary>
''' Indexed of target compound by m/z
''' </summary>
Public Structure MassIndexKey

    Dim mz As Double
    Dim precursorType As String

    ''' <summary>
    ''' debug view
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"{precursorType} {mz}"
    End Function

    Friend Shared Function ComparesMass(tolerance As Tolerance) As Comparison(Of MassIndexKey)
        Return Function(x As MassIndexKey, b As MassIndexKey) As Integer
                   If tolerance(x.mz, b.mz) Then
                       Return 0
                   ElseIf x.mz > b.mz Then
                       Return 1
                   Else
                       Return -1
                   End If
               End Function
    End Function

End Structure