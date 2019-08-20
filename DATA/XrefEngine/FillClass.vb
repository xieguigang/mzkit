Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models

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

    ''' <summary>
    ''' 构建出一个信息填充表达式, 即分子的注释信息传递进入函数, 然后函数尝试填充<see cref="ICompoundClass"/>物质结构分类信息后返回原始对象的引用
    ''' </summary>
    ''' <typeparam name="cpd"></typeparam>
    ''' <param name="anno"></param>
    ''' <param name="classifyObo"></param>
    ''' <param name="getKey"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ClassyfireFillerLambda(Of cpd As {Class, ICompoundClass})(
             anno As IEnumerable(Of ClassyfireAnnotation),
             classifyObo As ChemOntClassify,
             getKey As Func(Of cpd, String)) As Func(Of cpd, cpd)

        Dim annotations = ClassyfireInfoTable.PopulateMolecules(anno, classifyObo) _
            .DoCall(Function(mols) ClassyfireInfoTable.Unique(mols)) _
            .ToDictionary(Function(c) c.CompoundID)

        Return Function(compound) As cpd
                   Dim compoundKey As String = getKey(compound)

                   If annotations.ContainsKey(compoundKey) Then
                       Dim classyfire As ClassyfireInfoTable = annotations(compoundKey)

                       With compound
                           .class = classyfire.class
                           .kingdom = classyfire.kingdom
                           .molecular_framework = classyfire.molecular_framework
                           .sub_class = classyfire.sub_class
                           .super_class = classyfire.super_class
                       End With
                   End If

                   Return compound
               End Function
    End Function
End Module
