Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.foundation.OBO_Foundry

Public Module FillClass

    <Extension>
    Public Iterator Function FillCompoundClass(Of cpd As {INamedValue, ICompoundClass})(anno As IEnumerable(Of ClassyfireAnnotation),
                                                                                        classifyObo As ChemOntClassify,
                                                                                        compounds As IEnumerable(Of cpd)) As IEnumerable(Of cpd)
        Dim annotations = anno _
            .GroupBy(Function(a) a.CompoundID) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)
        Dim kingdom As Index(Of String) = classifyObo.kingdom.TermIndex
        Dim super_class
        Dim [class]
        Dim sub_class
        Dim molecular_framework

    End Function


End Module
