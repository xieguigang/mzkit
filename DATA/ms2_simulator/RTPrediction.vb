Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Bootstrapping
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
            .GroupBy(Function(a) a.KEGGAtom) _
            .ToDictionary(Function(a) a.Key,
                          Function(a) CDbl(a.Count))
    End Function

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
    Public Function RtRegression(experimental As IEnumerable(Of (metabolite As KCF, rt#))) As FitResult
        Dim matrix = experimental _
            .Select(Function(m)
                        Return New LMA.FitInput With {
                            .factors = m.metabolite.KCFComposition,
                            .y = m.rt
                        }
                    End Function)
        Dim fit = matrix.NonLinearFit

    End Function
End Module
