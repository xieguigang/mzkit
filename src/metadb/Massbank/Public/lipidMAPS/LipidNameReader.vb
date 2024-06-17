#Region "Microsoft.VisualBasic::ed751481ce4a514519401b7226df47af, metadb\Massbank\Public\lipidMAPS\LipidNameReader.vb"

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

    '   Total Lines: 35
    '    Code Lines: 21 (60.00%)
    ' Comment Lines: 8 (22.86%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (17.14%)
    '     File Size: 1.10 KB


    '     Class LipidNameReader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models

Namespace LipidMaps

    Public Class LipidNameReader : Inherits CompoundNameReader

        ''' <summary>
        ''' index by lipidmaps id
        ''' </summary>
        ReadOnly index As Dictionary(Of String, MetaData)

        Sub New(lipidmaps As IEnumerable(Of MetaData))
            index = lipidmaps _
                .GroupBy(Function(i) i.LM_ID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)
        End Sub

        ''' <summary>
        ''' get lipid <see cref="MetaData.ABBREVIATION"/> name by its id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Overrides Function GetName(id As String) As String
            If index.ContainsKey(id) Then
                Return index(id).ABBREVIATION
            Else
                Return Nothing
            End If
        End Function
    End Class

End Namespace
