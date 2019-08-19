Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
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

            For Each term In compound
                With chemOntClassify.GetLineages(term.ChemOntID)
                    For Each line In .Where(Function(node) node.value.Length >= 6)
                        lineages(line.description) = line
                    Next
                End With
            Next

            For Each classy In lineages.Values
                Yield New ClassyfireInfoTable With {
                    .CompoundID = compound.Key,
                    .kingdom = classy(1).name,
                    .super_class = classy(2).name,
                    .[class] = classy(3).name,
                    .sub_class = classy(4).name,
                    .molecular_framework = classy(5).name
                }
            Next
        Next
    End Function
End Class
