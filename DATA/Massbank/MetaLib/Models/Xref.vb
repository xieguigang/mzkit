Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MetaLib.Models

    ''' <summary>
    ''' 对某一个物质在数据库之间的相互引用编号
    ''' </summary>
    Public Class xref

        ''' <summary>
        ''' chebi主编号
        ''' </summary>
        ''' <returns></returns>
        Public Property chebi As String
        Public Property KEGG As String
        ''' <summary>
        ''' The pubchem cid
        ''' </summary>
        ''' <returns></returns>
        Public Property pubchem As String
        Public Property HMDB As String
        Public Property metlin As String
        Public Property Wikipedia As String
        ''' <summary>
        ''' Multiple CAS id may exists
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property CAS As String()
        Public Property InChIkey As String
        Public Property InChI As String
        Public Property SMILES As String

        ''' <summary>
        ''' This function will fill current <see cref="xref"/> object with 
        ''' additional property data from <paramref name="add"/> data object.
        ''' </summary>
        ''' <param name="add"></param>
        ''' <returns></returns>
        Public Function Join(add As xref) As xref
            If IsEmptyXrefId(chebi) Then
                chebi = add.chebi
            End If
            If KEGG.StringEmpty Then
                KEGG = add.KEGG
            End If
            If IsEmptyXrefId(pubchem) Then
                pubchem = add.pubchem
            End If
            If HMDB.StringEmpty Then
                HMDB = add.HMDB
            End If
            If IsEmptyXrefId(metlin) Then
                metlin = add.metlin
            End If
            If Wikipedia.StringEmpty Then
                Wikipedia = add.Wikipedia
            End If
            If CAS.IsNullOrEmpty Then
                CAS = add.CAS.ToArray
            End If
            If InChI.StringEmpty Then
                InChI = add.InChI
                InChIkey = add.InChIkey
            End If
            If SMILES.StringEmpty Then
                SMILES = add.SMILES
            End If

            Return Me
        End Function

        Shared ReadOnly emptySymbols As Index(Of String) = {"null", "na", "n/a", "inf", "nan"}

        Public Shared Function IsEmptyXrefId(id As String) As Boolean
            If id.StringEmpty OrElse id.ToLower Like emptySymbols Then
                Return True
            ElseIf id.Match("\d+").ParseInteger <= 0 Then
                Return True
            End If

            Return False
        End Function

        Public Shared Function IsEmpty(xref As xref, Optional includeStruct As Boolean = False) As Boolean
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
        Public Shared Function IsChEBI(synonym As String) As Boolean
            Return synonym.IsPattern("CHEBI[:]\d+", RegexICSng)
        End Function

        ''' <summary>
        ''' ``XXX-XXX-XXX``
        ''' </summary>
        ''' <param name="synonym"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function IsCASNumber(synonym As String) As Boolean
            Return synonym.IsPattern("\d+([-]\d+)+", RegexICSng)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function IsHMDB(synonym As String) As Boolean
            Return synonym.IsPattern("HMDB\d+", RegexICSng)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function IsKEGG(synonym As String) As Boolean
            Return synonym.IsPattern("C((\d){5})", RegexICSng)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace