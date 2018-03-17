Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace TMIC.FooDB

    Public Class FoodSource

        Public Property foodb_id As String
        Public Property HMDB As String
        Public Property name As String
        Public Property content As Double
        <Column("range", GetType(parser))>
        Public Property range As DoubleRange
        Public Property unit As String
        Public Property food_id As String
        Public Property group As String
        ''' <summary>
        ''' 学名
        ''' </summary>
        ''' <returns></returns>
        Public Property food_name As String
        ''' <summary>
        ''' 俗名
        ''' </summary>
        ''' <returns></returns>
        Public Property food_general_name As String
        Public Property reference As String

        Public Overrides Function ToString() As String
            Return $"{name} @ {food_general_name} ({food_name}) = {content} ({unit})"
        End Function

        Private Class parser : Implements IParser

            Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
                With DirectCast(obj, DoubleRange)
                    Return $"{ .Min}~{ .Max}"
                End With
            End Function

            Public Function TryParse(cell As String) As Object Implements IParser.TryParse
                Return CType(cell, DoubleRange)
            End Function
        End Class
    End Class
End Namespace