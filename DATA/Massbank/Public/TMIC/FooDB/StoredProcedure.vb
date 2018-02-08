Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Oracle.LinuxCompatibility.MySQL
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
            Dim list$
            Dim SQL$ = $"SELECT * FROM foodb.compounds WHERE lower(`public_id`) = lower('{HMDB.foodb_id}') LIMIT 1;"
            Dim compound = mysql.ExecuteScalar(Of mysql.compounds)(SQL)

            If compound Is Nothing Then
                Return
            End If

            Dim contents = mysql.Query(Of mysql.contents)($"SELECT * FROM foodb.contents WHERE `source_id` = {compound.id};")

            If contents.IsNullOrEmpty Then
                Return
            End If

            ' get food informations
            Dim foods As Dictionary(Of Long, mysql.foods)

            list = contents _
                .Select(Function(c) c.food_id) _
                .Distinct _
                .Select(Function(id) $"'{id}'") _
                .JoinBy(", ")
            SQL = $"SELECT * FROM foodb.foods WHERE `id` IN ({list});"
            foods = mysql _
                .Query(Of mysql.foods)(SQL) _
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
                    .unit = content.orig_unit
                }

                out += asso

                Yield asso
            Next

            Dim foodGroup = out.GroupBy(Function(f) f.food_id).ToArray

            For Each food In foodGroup
                Dim range As DoubleRange = food.Select(Function(f) f.content).ToArray

                For Each f In food
                    f.range = range
                Next
            Next
        End Function
    End Module

    Public Class FoodSource

        Public Property foodb_id As String
        Public Property HMDB As String
        Public Property name As String
        Public Property content As Double
        Public Property range As DoubleRange
        Public Property unit As String
        Public Property food_id As String
        Public Property food_name As String
        Public Property food_general_name As String
        Public Property reference As String

        Public Overrides Function ToString() As String
            Return $"{name} @ {food_general_name} ({food_name}) = {content} ({unit})"
        End Function
    End Class
End Namespace