Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace MetaLib.CrossReference

    Module XrefSchema

        ReadOnly strings As New Dictionary(Of String, PropertyInfo)
        ReadOnly arrays As New Dictionary(Of String, PropertyInfo)

        Sub New()
            Dim schema As Type = GetType(xref)
            Dim slots As Dictionary(Of String, PropertyInfo) = schema.Schema(PropertyAccess.ReadWrite, PublicProperty, nonIndex:=True)

            For Each prop As PropertyInfo In slots.Values
                If prop.PropertyType Is GetType(String) Then
                    Call strings.Add(prop.Name, prop)
                ElseIf prop.PropertyType Is GetType(String()) Then
                    Call arrays.Add(prop.Name, prop)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Merge two cross reference set and then create a new cross reference set.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function Join(a As xref, b As xref) As xref
            Dim setAB As Dictionary(Of String, String()) = a.PullCollection _
                .JoinIterates(b.PullCollection) _
                .GroupBy(Function(i) i.Name.ToLower) _
                .ToDictionary(Function(i) i.Key,
                              Function(i)
                                  Return i.Select(Function(id) id.Value) _
                                      .Distinct _
                                      .ToArray
                              End Function)

            Dim union As New xref With {
                .CAS = setAB.TryPopOut("cas"),
                .chebi = setAB.TryPopOut("chebi").JoinBy("; "),
                .ChEMBL = setAB.TryPopOut("chembl").JoinBy("; "),
                .ChemIDplus = setAB.TryPopOut("chemidplus").JoinBy("; "),
                .chemspider = setAB.TryPopOut("chemspider").JoinBy("; "),
                .DrugBank = setAB.TryPopOut("drugbank").JoinBy("; "),
                .foodb = setAB.TryPopOut("foodb").JoinBy("; "),
                .HMDB = setAB.TryPopOut("hmdb").JoinBy("; "),
                .InChI = setAB.TryPopOut("inchi").JoinBy("; "),
                .InChIkey = setAB.TryPopOut("inchikey").JoinBy("; "),
                .KEGG = setAB.TryPopOut("kegg").JoinBy("; "),
                .KEGGdrug = setAB.TryPopOut("keggdrug").JoinBy("; "),
                .KNApSAcK = setAB.TryPopOut("knapsack").JoinBy("; "),
                .lipidmaps = setAB.TryPopOut("lipidmaps").JoinBy("; "),
                .MeSH = setAB.TryPopOut("mesh").JoinBy("; "),
                .MetaCyc = setAB.TryPopOut("metacyc").JoinBy("; "),
                .metlin = setAB.TryPopOut("metlin").JoinBy("; "),
                .pubchem = setAB.TryPopOut("pubchem").JoinBy("; "),
                .SMILES = setAB.TryPopOut("smiles").JoinBy("; "),
                .Wikipedia = setAB.TryPopOut("wikipedia").JoinBy("; "),
                .extras = setAB
            }

            Return union
        End Function

        ''' <summary>
        ''' Convert a cross reference set as a database id collection
        ''' </summary>
        ''' <param name="xref"></param>
        ''' <param name="parseList">
        ''' this parameter will treat the xref id as a set of the id 
        ''' collection, where the id set elements is seperated by the 
        ''' ``;`` symbol.
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function PullCollection(xref As xref, Optional parseList As Boolean = False) As IEnumerable(Of NamedValue(Of String))
            For Each slot In strings
                Dim str As String = slot.Value.GetValue(xref)

                If CrossReference.IsEmptyXrefId(str) AndAlso CrossReference.IsEmptyIdString(str) Then
                    Continue For
                End If

                If parseList Then
                    For Each si As String In str.StringSplit(";\s*")
                        Yield New NamedValue(Of String)(slot.Key, si)
                    Next
                Else
                    Yield New NamedValue(Of String)(slot.Key, str)
                End If
            Next

            For Each arr In arrays
                Dim strs As String() = arr.Value.GetValue(xref)

                If Not strs.IsNullOrEmpty Then
                    For Each id As String In strs
                        If CrossReference.IsEmptyXrefId(id) AndAlso CrossReference.IsEmptyIdString(id) Then
                            Continue For
                        End If

                        Yield New NamedValue(Of String)(arr.Key, id)
                    Next
                End If
            Next

            If Not xref.extras.IsNullOrEmpty Then
                For Each extra In xref.extras
                    For Each id As String In extra.Value.SafeQuery
                        Yield New NamedValue(Of String)(extra.Key, id)
                    Next
                Next
            End If
        End Function
    End Module
End Namespace