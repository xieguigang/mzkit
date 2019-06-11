Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.foundation.OBO_Foundry

''' <summary>
''' 因为在这里填充数据是直接进行的,所以structure而言是值类型,只能够修改函数之中的副本,所以在这个模块之中,类型参数都必须要限制为引用类型
''' </summary>
Public Module FillClass

    <Extension>
    Public Iterator Function FillCompoundClass(Of cpd As {Class, INamedValue, ICompoundClass})(
            anno As IEnumerable(Of ClassyfireAnnotation),
            classifyObo As ChemOntClassify,
            compounds As IEnumerable(Of cpd)) As IEnumerable(Of cpd)

        Dim classyfireFiller = anno.ClassyfireFillerLambda(Of cpd)(classifyObo)

        For Each compound As cpd In compounds
            Yield classyfireFiller(compound)
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ClassyfireFillerLambda(Of cpd As {Class, INamedValue, ICompoundClass})(anno As IEnumerable(Of ClassyfireAnnotation), classifyObo As ChemOntClassify) As Func(Of cpd, cpd)
        Return anno.ClassyfireFillerLambda(Of cpd)(classifyObo, Function(c) c.Key)
    End Function

    <Extension>
    Public Function ClassyfireFillerLambda(Of cpd As {Class, ICompoundClass})(
             anno As IEnumerable(Of ClassyfireAnnotation),
             classifyObo As ChemOntClassify,
             getKey As Func(Of cpd, String)) As Func(Of cpd, cpd)

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

        Return Function(compound) As cpd
                   Dim compoundKey As String = getKey(compound)

                   If annotations.ContainsKey(compoundKey) Then
                       Dim classyfire As ClassyfireAnnotation() = annotations(compoundKey)
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

                   Return compound
               End Function
    End Function
End Module
