Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.foundation.OBO_Foundry

Public Module FillClass

    <Extension>
    Public Iterator Function FillCompoundClass(Of cpd As {Class, INamedValue, ICompoundClass})(
            anno As IEnumerable(Of ClassyfireAnnotation),
            classifyObo As ChemOntClassify,
            compounds As IEnumerable(Of cpd)) As IEnumerable(Of cpd)

        Dim annotations = anno _
            .GroupBy(Function(a) a.CompoundID) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)
        Dim kingdom As Index(Of String) = classifyObo.kingdom.TermIndex
        Dim super_class As Index(Of String) = classifyObo.superClass.TermIndex
        Dim [class] As Index(Of String) = classifyObo.class.TermIndex
        Dim sub_class As Index(Of String) = classifyObo.subClass.TermIndex
        Dim molecular_framework = classifyObo.molecularFramework.TermIndex

        For Each compound As cpd In compounds
            If annotations.ContainsKey(compound.Key) Then
                Dim classyfire As ClassyfireAnnotation() = annotations(compound.Key)
                Dim getByLevel = Function(level As Index(Of String)) As String
                                     Return classyfire _
                                         .FirstOrDefault(Function(a) a.ChemOntID Like level) _
                                        ?.ParentName
                                 End Function

                With compound
                    .class = getByLevel([class])
                    .kingdom = getByLevel(kingdom)
                    .molecular_framework = getByLevel(molecular_framework)
                    .sub_class = getByLevel(sub_class)
                    .super_class = getByLevel(super_class)
                End With
            End If

            Yield compound
        Next
    End Function
End Module
