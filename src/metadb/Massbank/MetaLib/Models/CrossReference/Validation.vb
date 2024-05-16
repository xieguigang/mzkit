#Region "Microsoft.VisualBasic::0171f5d8ac463052720273f533683b82, metadb\Massbank\MetaLib\Models\CrossReference\Validation.vb"

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

    '   Total Lines: 90
    '    Code Lines: 60
    ' Comment Lines: 18
    '   Blank Lines: 12
    '     File Size: 3.36 KB


    '     Module Validation
    ' 
    '         Function: IsCASNumber, IsChEBI, IsEmpty, IsEmptyIdString, IsEmptyXrefId
    '                   IsHMDB, IsKEGG
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MetaLib.CrossReference

    ''' <summary>
    ''' the xref id string format validation
    ''' </summary>
    Public Module Validation

        ReadOnly emptySymbols As Index(Of String) = {"null", "na", "n/a", "inf", "nan", "-"}

        ''' <summary>
        ''' test for the id with integer part
        ''' </summary>
        ''' <param name="id">example as 'HMDB0000001'</param>
        ''' <returns></returns>
        Public Function IsEmptyXrefId(id As String) As Boolean
            If id.StringEmpty(testEmptyFactor:=True) OrElse id.ToLower Like emptySymbols Then
                Return True
            ElseIf id.Match("\d+").ParseInteger <= 0 Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' test for the id without integer part
        ''' </summary>
        ''' <param name="id">example as 'ATP'</param>
        ''' <returns></returns>
        Public Function IsEmptyIdString(id As String) As Boolean
            Return id.StringEmpty(testEmptyFactor:=True) OrElse id.ToLower Like emptySymbols
        End Function

        Public Function IsEmpty(xref As xref, Optional includeStruct As Boolean = False) As Boolean
            If Not xref.chebi.StringEmpty Then
                Return False
            ElseIf Not xref.KEGG.StringEmpty Then
                Return False
            ElseIf Not xref.pubchem.StringEmpty Then
                Return False
            ElseIf Not xref.HMDB.StringEmpty Then
                Return False
            ElseIf Not xref.metlin.StringEmpty Then
                Return False
            ElseIf Not xref.Wikipedia.StringEmpty Then
                Return False
            ElseIf Not xref.CAS.IsNullOrEmpty Then
                Return False
            ElseIf includeStruct Then
                If Not xref.InChI.StringEmpty Then
                    Return False
                ElseIf Not xref.InChIkey.StringEmpty Then
                    Return False
                ElseIf Not xref.SMILES.StringEmpty Then
                    Return False
                End If
            End If

            Return True
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsChEBI(synonym As String) As Boolean
            Return synonym.IsPattern("CHEBI[:]\d+", RegexICSng)
        End Function

        ''' <summary>
        ''' ``XXX-XXX-XXX``
        ''' </summary>
        ''' <param name="synonym"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsCASNumber(synonym As String) As Boolean
            Return synonym.IsPattern("\d+([-]\d+)+", RegexICSng)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsHMDB(synonym As String) As Boolean
            Return synonym.IsPattern("HMDB\d+", RegexICSng)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsKEGG(synonym As String) As Boolean
            Return synonym.IsPattern("C((\d){5})", RegexICSng)
        End Function
    End Module
End Namespace
