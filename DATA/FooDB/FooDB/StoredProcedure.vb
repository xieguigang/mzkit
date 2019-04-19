#Region "Microsoft.VisualBasic::8742f4fe9100e9442b359d7140b1adfd, FooDB\FooDB\StoredProcedure.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Module StoredProcedure
    ' 
    '         Function: GetAssociatedFoods, UnitConversion
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Expressions
Imports SMRUCC.MassSpectrum.DATA.TMIC.HMDB

Namespace TMIC.FooDB

    ''' <summary>
    ''' The stored procedure for foodb read/write
    ''' </summary>
    Public Module StoredProcedure

        ''' <summary>
        ''' 将单位都统一转换为``mg/100g``
        ''' </summary>
        ''' <param name="value#"></param>
        ''' <param name="unit$"></param>
        ''' <param name="mw">摩尔分子量</param>
        ''' <returns></returns>
        Public Function UnitConversion(value#, unit$, mw#) As Double
            Select Case LCase(unit)
                Case "g"
                    Return value * 1000
                Case "mg"
                    Return value * 1
                Case "mg/100g"
                    Return value * 1
                Case "ppm"
                    Return value * (10 ^ -6)
                Case "um"
                    ' 分子量是165.19，就是1mol有165.19g；
                    ' 一微摩尔的该物质当然等于165.19毫克
                    ' 三微摩尔的该物质相当于3 * 165.19毫克
                    Return value * mw
                Case "ug"
                    Return value * (10 ^ -3)
                Case ""
                    Return 0
                Case Else
                    ' Throw New NotImplementedException(unit)
                    Call $"not_implemented_for: {value} ({unit})".PrintException
                    Return 0
            End Select
        End Function

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
                    .range = {content.orig_min, content.orig_max},
                    .group = food?.food_group
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
End Namespace
