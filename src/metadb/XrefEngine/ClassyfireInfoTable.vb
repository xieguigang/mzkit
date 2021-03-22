#Region "Microsoft.VisualBasic::b0dabec85da3edfd6cbed0d967e2623b, src\metadb\XrefEngine\ClassyfireInfoTable.vb"

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

    ' Class ClassyfireInfoTable
    ' 
    '     Properties: [class], CompoundID, kingdom, molecular_framework, sub_class
    '                 super_class
    ' 
    '     Function: PopulateMolecules, Unique
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.foundation.OBO_Foundry.Tree
Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models

Public Class ClassyfireInfoTable : Implements ICompoundClass

    ''' <summary>
    ''' Compound id in given database.
    ''' </summary>
    ''' <returns></returns>
    Public Property CompoundID As String
    Public Property kingdom As String Implements ICompoundClass.kingdom
    Public Property super_class As String Implements ICompoundClass.super_class
    Public Property [class] As String Implements ICompoundClass.class
    Public Property sub_class As String Implements ICompoundClass.sub_class
    Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

    Public Shared Iterator Function PopulateMolecules(anno As IEnumerable(Of ClassyfireAnnotation), chemOntClassify As ChemOntClassify) As IEnumerable(Of ClassyfireInfoTable)
        Dim lineages As New Dictionary(Of String, NamedCollection(Of GenericTree))

        For Each compound In anno.GroupBy(Function(a) a.CompoundID)
            lineages.Clear()

            For Each term As ClassyfireAnnotation In compound _
                .Where(Function(row)
                           ' 20190820 classyfire的数据库文件格式存在一些错误
                           ' 所以会需要在这里进行一下判断, 否则会出现无法找到键名称的问题
                           Return row.ChemOntID.IsPattern("CHEMONTID[:]\d+")
                       End Function)

                With chemOntClassify.GetLineages(term.ChemOntID)
                    For Each line In .Where(Function(node) node.value.Length >= 6)
                        lineages(line.description) = line
                    Next
                End With
            Next

            For Each classy As NamedCollection(Of GenericTree) In lineages.Values
                Yield New ClassyfireInfoTable With {
                    .CompoundID = compound.Key,
                    .kingdom = classy(1).name,
                    .super_class = classy(2).name,
                    .[class] = classy(3).name,
                    .sub_class = classy(4).name,
                    .molecular_framework = classy.Last.name
                }
            Next
        Next
    End Function

    Public Shared Iterator Function Unique(table As IEnumerable(Of ClassyfireInfoTable)) As IEnumerable(Of ClassyfireInfoTable)
        Dim classyfire = table.ToArray
        Dim kingdoms = classyfire.Select(Function(c) c.kingdom).TokenCount
        Dim super_classs = classyfire.Select(Function(c) c.super_class).TokenCount
        Dim classs = classyfire.Select(Function(c) c.class).TokenCount
        Dim sub_class = classyfire.Select(Function(c) c.sub_class).TokenCount
        Dim molecular_framework = classyfire.Select(Function(c) c.molecular_framework).TokenCount
        Dim groupBy As Map(Of Func(Of ClassyfireInfoTable, String), Dictionary(Of String, Integer))() = {
            New Map(Of Func(Of ClassyfireInfoTable, String), Dictionary(Of String, Integer))(Function(c) c.kingdom, kingdoms),
            New Map(Of Func(Of ClassyfireInfoTable, String), Dictionary(Of String, Integer))(Function(c) c.super_class, super_classs),
            New Map(Of Func(Of ClassyfireInfoTable, String), Dictionary(Of String, Integer))(Function(c) c.class, classs),
            New Map(Of Func(Of ClassyfireInfoTable, String), Dictionary(Of String, Integer))(Function(c) c.sub_class, sub_class),
            New Map(Of Func(Of ClassyfireInfoTable, String), Dictionary(Of String, Integer))(Function(c) c.molecular_framework, molecular_framework)
        }
        Dim uniqueMax As ClassyfireInfoTable() = Nothing

        For Each compound In classyfire.GroupBy(Function(c) c.CompoundID)
            If compound.Count = 1 Then
                Yield compound.First
            Else
                Dim doYield As Boolean = False

                For Each method In groupBy
                    uniqueMax = compound _
                        .GroupBy(method.Key) _
                        .OrderByDescending(Function(g) method.Maps(g.Key)) _
                        .First _
                        .ToArray

                    If uniqueMax.Length = 1 Then
                        Yield uniqueMax.First
                        doYield = True
                        Exit For
                    End If
                Next

                If Not doYield Then
                    Yield uniqueMax.First
                End If
            End If
        Next
    End Function
End Class
