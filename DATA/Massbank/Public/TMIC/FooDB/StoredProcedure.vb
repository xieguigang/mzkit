Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Expressions
Imports SMRUCC.MassSpectrum.DATA.TMIC.HMDB

Namespace TMIC.FooDB

    ''' <summary>
    ''' The stored procedure for foodb read/write
    ''' </summary>
    Public Module StoredProcedure

        ' compound <--> food associations
        ' HMDB -> [hmdb_id] compounds [id] -> [source_id] contents [food_id] -> foods

        '<Extension>
        'Public Iterator Function GetAssociatedFoods(HMDB$, mysql As MySqli) As IEnumerable(Of FoodSource)

        'End Function

        <Extension>
        Public Iterator Function GetAssociatedFoods(HMDB As metabolite, mysql As MySqli) As IEnumerable(Of FoodSource)
            Dim compound = New Table(Of mysql.compounds)(mysql) _
                .Where($"lower(`public_id`) = lower('{HMDB.foodb_id}')") _
                .Find

            If compound Is Nothing Then
                Return
            End If

            Dim contents = New Table(Of mysql.contents)(mysql) _
                .Where($"`source_id` = {compound.id}") _
                .SelectALL

            If contents.IsNullOrEmpty Then
                Return
            End If

            ' get food informations
            Dim list$ = contents _
                .Select(Function(c) c.food_id) _
                .Distinct _
                .Select(Function(id) $"'{id}'") _
                .JoinBy(", ")
            Dim foods As Dictionary(Of Long, mysql.foods) = New Table(Of mysql.foods)(mysql) _
                .Where($"`id` IN ({list})") _
                .SelectALL _
                .ToDictionary(Function(food) food.id)

            Dim out As New List(Of FoodSource)

            For Each content As mysql.contents In contents
                Dim food As mysql.foods = foods.TryGetValue(content.food_id)
                Dim asso As New FoodSource With {
                    .HMDB = HMDB.accession,
                    .foodb_id = compound.public_id,
                    .content = content.orig_content,
                    .food_id = content.food_id,
                    .food_name = food?.name_scientific,
                    .food_general_name = food?.name,
                    .name = compound.name,
                    .reference = content.citation,
                    .unit = content.orig_unit,
                    .range = {content.orig_min, content.orig_max}
                }

                If asso.content = 0R Then
                    With asso.range
                        asso.content = { .Min, .Max}.Average
                    End With
                End If

                ' 如果min = max，也会出现length为零的情况，在这判断一下min是否为零就好了
                If asso.range.Length = 0R AndAlso asso.range.Min = 0R Then
                    asso.range = {asso.content, asso.content}
                End If

                out += asso

                Yield asso
            Next
        End Function
    End Module

    Public Class FoodSource

        Public Property foodb_id As String
        Public Property HMDB As String
        Public Property name As String
        Public Property content As Double
        <Column("range", GetType(parser))>
        Public Property range As DoubleRange
        Public Property unit As String
        Public Property food_id As String
        Public Property food_name As String
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