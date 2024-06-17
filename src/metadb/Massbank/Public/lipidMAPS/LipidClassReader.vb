#Region "Microsoft.VisualBasic::3effe84ae581f8d905823650dd634186, metadb\Massbank\Public\lipidMAPS\LipidClassReader.vb"

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

    '   Total Lines: 70
    '    Code Lines: 39 (55.71%)
    ' Comment Lines: 22 (31.43%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (12.86%)
    '     File Size: 2.58 KB


    '     Class LipidClassReader
    ' 
    '         Properties: lipids
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: EnumerateId, GetClass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models

Namespace LipidMaps

    ''' <summary>
    ''' A helper module for get lipidmaps <see cref="CompoundClass"/> information via a given lipidmaps id
    ''' </summary>
    ''' <remarks>
    ''' the lipidmaps metabolite data in this module is indexed via the lipidmaps id: <see cref="MetaData.LM_ID"/>.
    ''' </remarks>
    Public Class LipidClassReader : Inherits ClassReader

        ''' <summary>
        ''' the lipidmaps database was indexed via the lipidmaps id at here
        ''' </summary>
        ''' <remarks>
        ''' the key is the lipidmaps id <see cref="MetaData.LM_ID"/>
        ''' </remarks>
        ReadOnly index As Dictionary(Of String, MetaData)

        ''' <summary>
        ''' get number of the lipids inside the database
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property lipids As Integer
            Get
                Return index.TryCount
            End Get
        End Property

        Sub New(lipidmaps As IEnumerable(Of MetaData))
            index = lipidmaps _
                .GroupBy(Function(i) i.LM_ID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)
        End Sub

        ''' <summary>
        ''' get lipidmaps class information via a given lipidmaps id 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns>this function may returns nothing if the given <paramref name="id"/>
        ''' is not exists inside the database index.</returns>
        Public Overrides Function GetClass(id As String) As CompoundClass
            If index.ContainsKey(id) Then
                Dim lipid As MetaData = index(id)
                Dim [class] As New CompoundClass With {
                    .kingdom = "Lipids",
                    .super_class = lipid.CATEGORY,
                    .[class] = lipid.MAIN_CLASS,
                    .sub_class = lipid.SUB_CLASS,
                    .molecular_framework = lipid.CLASS_LEVEL4
                }

                Return [class]
            Else
                Return Nothing
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function EnumerateId() As IEnumerable(Of String)
            Return index.Keys
        End Function
    End Class

End Namespace
