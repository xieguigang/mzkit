Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.Chemistry.Model

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
End Module
