#Region "Microsoft.VisualBasic::c5018a585caede4d9ba8b38ee4d5e66b, FormulaSearch.Extensions\AtomGroups\AtomGroupHandler.vb"

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

    ' Class AtomGroupHandler
    ' 
    '     Function: FindDelta, GetByMass, loadGroup
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports stdnum = System.Math

Public Class AtomGroupHandler

    Shared ReadOnly alkyl As Dictionary(Of String, Formula) = loadGroup(Of Alkyl)()
    Shared ReadOnly ketones As Dictionary(Of String, Formula) = loadGroup(Of Ketones)()
    Shared ReadOnly amines As Dictionary(Of String, Formula) = loadGroup(Of Amines)()
    Shared ReadOnly alkenyl As Dictionary(Of String, Formula) = loadGroup(Of Alkenyl)()
    Shared ReadOnly others As Dictionary(Of String, Formula) = loadGroup(Of Others)()

    Private Shared Function loadGroup(Of T As Class)() As Dictionary(Of String, Formula)
        Return DataFramework.Schema(Of T)(
            flag:=PropertyAccess.Readable,
            nonIndex:=True,
            binds:=BindingFlags.Static Or BindingFlags.Public
        ) _
        .ToDictionary(Function(p) p.Key,
                        Function(p)
                            Return DirectCast(p.Value.GetValue(Nothing, Nothing), Formula)
                        End Function)
    End Function

    Public Shared Function GetByMass(mass As Double) As NamedValue(Of Formula)
        Static all_groups As List(Of KeyValuePair(Of String, Formula)) = New List(Of KeyValuePair(Of String, Formula)) + alkyl + ketones + amines + alkenyl + others

        For Each group In all_groups
            If stdnum.Abs(group.Value.ExactMass - mass) <= 0.00001 Then
                Return New NamedValue(Of Formula)(group.Key, group.Value)
            End If
        Next

        Return Nothing
    End Function

    Public Shared Function FindDelta(mz1 As Double, mz2 As Double, Optional ByRef delta As Integer = 0) As NamedValue(Of Formula)
        Dim d As Double = mz1 - mz2
        Dim dmass As Double = stdnum.Abs(d)

        If dmass <= 0.01 Then
            Return Nothing
        End If

        Dim group = GetByMass(dmass)

        delta = stdnum.Sign(d)

        Return group
    End Function
End Class

