#Region "Microsoft.VisualBasic::8befed8b85ee07eb257431aaa98738cd, src\metadb\Massbank\Public\lipidMAPS\MetaTable.vb"

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

    '     Module MetaTable
    ' 
    '         Function: ProjectVectors
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace LipidMaps

    Public Module MetaTable

        Friend ReadOnly properties As Dictionary(Of String, PropertyInfo) = DataFramework.Schema(Of MetaData)(PropertyAccess.Writeable, True)

        <Extension>
        Public Iterator Function ProjectVectors(lipidmaps As MetaData()) As IEnumerable(Of NamedValue(Of Array))
            For Each [property] As PropertyInfo In properties.Values
                Dim values As Array = Array.CreateInstance([property].PropertyType, lipidmaps.Length)

                For i As Integer = 0 To lipidmaps.Length - 1
                    values.SetValue([property].GetValue(lipidmaps(i)), i)
                Next

                Yield New NamedValue(Of Array) With {
                    .Name = [property].Name,
                    .Value = values
                }
            Next
        End Function

    End Module
End Namespace
