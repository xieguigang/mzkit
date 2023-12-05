Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MetaLib.CrossReference

    ''' <summary>
    ''' the xref id string format validation
    ''' </summary>
    Public Module Validation

        ReadOnly emptySymbols As Index(Of String) = {"null", "na", "n/a", "inf", "nan"}

        Public Function IsEmptyXrefId(id As String) As Boolean
            If id.StringEmpty OrElse id.ToLower Like emptySymbols Then
                Return True
            ElseIf id.Match("\d+").ParseInteger <= 0 Then
                Return True
            End If

            Return False
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