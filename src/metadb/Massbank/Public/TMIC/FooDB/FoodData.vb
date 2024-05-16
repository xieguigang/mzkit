#Region "Microsoft.VisualBasic::890953495284f69892ead036606d384a, metadb\Massbank\Public\TMIC\FooDB\FoodData.vb"

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


    ' Code Statistics:

    '   Total Lines: 31
    '    Code Lines: 26
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1003 B


    '     Class FoodData
    ' 
    '         Properties: compoundFlavors, compounds, contents
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace TMIC.FooDB

    Public Class FoodData : Inherits Food

        Public Property contents As Content()
        Public Property compoundFlavors As Dictionary(Of String, String())
        Public Property compounds As Compound()

        Sub New(food As Food)
            With Me
                .category = food.category
                .description = food.description
                .food_group = food.food_group
                .food_subgroup = food.food_subgroup
                .food_type = food.food_type
                .id = food.id
                .itis_id = food.itis_id
                .legacy_id = food.legacy_id
                .name = food.name
                .name_scientific = food.name_scientific
                .ncbi_taxonomy_id = food.ncbi_taxonomy_id
                .public_id = food.public_id
                .wikipedia_id = food.wikipedia_id
            End With
        End Sub

        Sub New()
        End Sub

    End Class
End Namespace
