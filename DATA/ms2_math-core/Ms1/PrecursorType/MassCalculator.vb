Imports System.Runtime.CompilerServices

Public Module MassCalculator

    <Extension>
    Public Function GetMass(composition As Dictionary(Of String, Integer)) As Double

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ParseComposition(formula As String) As Dictionary(Of String, Integer)

    End Function
End Module
