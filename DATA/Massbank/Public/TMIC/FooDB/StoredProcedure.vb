Imports System.Runtime.CompilerServices
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.MassSpectrum.DATA.TMIC.HMDB

Namespace TMIC.FooDB

    ''' <summary>
    ''' The stored procedure for foodb read/write
    ''' </summary>
    Public Module StoredProcedure

        ' compound <--> food associations
        ' HMDB -> [hmdb_id] compounds [id] -> [source_id] contents [food_id] -> foods

        <Extension>
        Public Iterator Function GetAssociatedFoods(HMDB As metabolite, mysql As MySqli) As IEnumerable(Of FoodSource)
            Dim list$
            Dim SQL$ = $"SELECT * FROM foodb.compounds WHERE lower(`public_id`) = lower('{HMDB.foodb_id}') LIMIT 1;"
            Dim compound = mysql.ExecuteScalar(Of mysql.compounds)(SQL)
            Dim contents = mysql.Query(Of mysql.contents)($"SELECT * FROM foodb.contents WHERE `source_id` = {compound.id};")

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

            For Each content As mysql.contents In contents
                Dim asso As New FoodSource With {
                    .HMDB = HMDB.accession,
                    .content = content.orig_content,
                    .food_id = content.food_id,
                    .food_name = foods(.food_id).name_scientific,
                    .food_general_name = foods(.food_id).name,
                    .name = compound.name,
                    .reference = content.citation,
                    .unit = content.orig_unit
                }

                Yield asso
            Next
        End Function
    End Module

    Public Class FoodSource

        Public Property HMDB As String
        Public Property name As String
        Public Property content As Double
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