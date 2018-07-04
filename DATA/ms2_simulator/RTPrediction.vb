Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.Chemistry.Model
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.MassSpectrum.Math

''' <summary>
''' Prediciton of the relative LC-MS retention time for measuring ``rt.error``
''' in small compound metabolite identification, based on the KEGG KCF 
''' moleculate strucutre composition non-linear regression model.
''' </summary>
Public Module RTPrediction

    ''' <summary>
    ''' KEGG KCF molecular strucutre model to regression model factors.
    ''' </summary>
    ''' <param name="KCF"></param>
    ''' <returns>Regression model factors</returns>
    <Extension>
    Public Function KCFComposition(KCF As KCF) As Dictionary(Of String, Double)
        Return KCF.Atoms _
            .GroupBy(Function(a)
                         Return a.KEGGAtom.code
                     End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a) CDbl(a.Count))
    End Function

    ReadOnly namedVector As NamedVectorFactory

    Sub New()
        Dim elements = KegAtomType.KEGGAtomTypes _
            .Values _
            .IteratesALL _
            .Where(Function(a)
                       Return a.type = KegAtomType.Types.Carbon OrElse
                              a.type = KegAtomType.Types.Nitrogen OrElse
                              a.type = KegAtomType.Types.Oxygen
                   End Function) _
            .Select(Function(a) a.code) _
            .OrderBy(Function(s) s) _
            .ToArray

        namedVector = New NamedVectorFactory(factors:=elements)
    End Sub

    ''' <summary>
    ''' This algorithm only works for a specific LC-MS experiment condition.
    ''' It works fine in the company's standardized Thermo QE+ LC-MS workflow.
    ''' </summary>
    ''' <param name="experimental">Experimental rt value of a given set of metabolite</param>
    ''' <returns>
    ''' Using this non-linear regression fitted model for predicted of the unknowns
    ''' </returns>
    ''' 
    <Extension>
    Public Function RtRegression(experimental As IEnumerable(Of (metabolite As KCF, rt#))) As MLRFit
        Dim values As (Vector, Double)() = experimental _
            .Select(Function(m)
                        Dim factors = m.metabolite.KCFComposition
                        Dim v = namedVector.AsVector(factors)
                        Return (v, m.rt)
                    End Function) _
            .ToArray
        Dim matrix As New GeneralMatrix(values.Select(Function(m) m.Item1))
        Dim RT As Vector = values.Select(Function(m) m.Item2).AsVector
        Dim fit = MLRFit.LinearFitting(matrix, RT)

        Return fit
    End Function

    ''' <summary>
    ''' 通过扫描标准品库之中的注释信息结合KEGG分子结构信息构建出RT的预测模型
    ''' </summary>
    ''' <param name="metadb">标准品库MS1注释信息</param>
    ''' <param name="kegg">KEGG代谢物数据库</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function ScanMLRModel(metadb As IEnumerable(Of MetaInfo), kegg As CompoundRepository) As MLRFit
        ' 无效数据过滤
        Dim valids = metadb _
            .Where(Function(m)
                       Dim a = Not m.xref!KEGG.StringEmpty AndAlso kegg.Exists(key:=m.xref!KEGG)
                       Dim b = m.rt >= 0
                       Dim c = Not m.rt.IsNaNImaginary

                       Return a AndAlso b AndAlso c
                   End Function) _
            .ToArray
        Dim inputs = valids _
            .Select(Function(meta)
                        Dim compound As Compound = kegg.GetByKey(meta.xref!KEGG).Entity
                        Dim KCF As KCF = IO.LoadKCF(stream:=compound.KCF)

                        Return (KCF, meta.rt)
                    End Function)

        Return inputs.RtRegression
    End Function
End Module
