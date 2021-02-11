Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace LinearQuantitative.Data

    Public Module StandardCurveCDF

        <Extension>
        Public Function WriteCDF(linear As StandardCurve) As MemoryStream
            Using ms As New MemoryStream, cdf As New netCDF.CDFWriter(ms)
                Call linear.WriteCDF(cdf)
                Return ms
            End Using
        End Function

        <Extension>
        Private Sub WriteCDF(linear As StandardCurve, cdf As netCDF.CDFWriter)
            Dim name As New attribute With {.name = "name", .type = CDFDataTypes.CHAR, .value = linear.name}
            Dim [IS] As New attribute With {.name = "IS", .type = CDFDataTypes.CHAR, .value = linear.IS.ID}
            Dim IS_name As New attribute With {.name = "IS_name", .type = CDFDataTypes.CHAR, .value = linear.IS.name}
            Dim cIS As New attribute With {.name = "cIS", .type = CDFDataTypes.DOUBLE, .value = linear.IS.CIS}
            Dim R2 As New attribute With {.name = "R2", .type = CDFDataTypes.DOUBLE, .value = linear.linear.R2}
            Dim outliers As New attribute With {.name = "outliers", .type = CDFDataTypes.INT, .value = linear.points.Where(Function(p) Not p.valid).Count}
            Dim weighted As New attribute With {.name = "is_weighted", .type = CDFDataTypes.BOOLEAN, .value = TypeOf linear.linear Is WeightedFit}
            Dim points As New attribute With {.name = "ref_size", .type = CDFDataTypes.INT, .value = linear.points.Length}

            cdf.GlobalAttributes(name, [IS], IS_name, cIS, R2, outliers, weighted, points)

            If TypeOf linear.linear Is WeightedFit Then
                Call DirectCast(linear.linear, WeightedFit).fitLinear(cdf)
            Else
                Call DirectCast(linear.linear, FitResult).fitLinear(cdf)
            End If

            Dim blankSize As New Dimension With {
                .name = "blank_size",
                .size = linear.blankControls.Length
            }

            cdf.AddVariable("blanks", linear.blankControls.SafeQuery.ToArray, blankSize)

            Dim width As New Dimension With {.name = "width", .size = 5 + 3}

            For Each p As ReferencePoint In linear.points
                cdf.AddVariable(p.level, New Double() {p.AIS, p.Ati, p.cIS, p.Cti, p.Px, p.yfit, p.error, p.variant}, width, {
                    New attribute With {.name = "valid", .type = CDFDataTypes.BOOLEAN, .value = p.valid},
                    New attribute With {.name = "ID", .type = CDFDataTypes.CHAR, .value = p.ID},
                    New attribute With {.name = "name", .type = CDFDataTypes.CHAR, .value = p.Name}
                })
            Next
        End Sub

        <Extension>
        Private Sub fitLinear(fit As WeightedFit, cdf As netCDF.CDFWriter)
            Dim formula As Polynomial = fit.Polynomial
            Dim size As New Dimension With {.name = "polynomial_size", .size = formula.Factors.Length}
            Dim sizeofDY As New Dimension With {.name = "dy_size", .size = fit.Residuals.Length}
            Dim sizeofSEC As New Dimension With {.name = "sec_size", .size = fit.CoefficientsStandardError.Length}
            Dim attrs As attribute() = {
                New attribute With {.name = "fisher", .type = CDFDataTypes.DOUBLE, .value = fit.FisherF},
                New attribute With {.name = "R2", .type = CDFDataTypes.DOUBLE, .value = fit.CorrelationCoefficient},
                New attribute With {.name = "SDV", .type = CDFDataTypes.DOUBLE, .value = fit.StandardDeviation}
            }
            Dim vars As Double() = fit.VarianceMatrix.RowIterator.IteratesALL.ToArray
            Dim sizeofVar As New Dimension With {.name = "var_size", .size = vars.Length}

            cdf.AddVariable("polynomial", formula.Factors, size, attrs)
            cdf.AddVariable("DY", fit.Residuals, sizeofDY)
            cdf.AddVariable("SEC", fit.CoefficientsStandardError, sizeofSEC)
            cdf.AddVariable("COVAR", vars, sizeofSEC)
        End Sub

        <Extension>
        Private Sub fitLinear(fit As FitResult, cdf As netCDF.CDFWriter)
            Dim formula As Polynomial = fit.Polynomial
            Dim size As New Dimension With {.name = "polynomial_size", .size = formula.Factors.Length}
            Dim attrs As attribute() = {
                New attribute With {.name = "adjustR_square", .type = CDFDataTypes.DOUBLE, .value = fit.AdjustR_square},
                New attribute With {.name = "factor_size", .type = CDFDataTypes.DOUBLE, .value = fit.FactorSize},
                New attribute With {.name = "intercept", .type = CDFDataTypes.DOUBLE, .value = fit.Intercept},
                New attribute With {.name = "is_poly", .type = CDFDataTypes.DOUBLE, .value = fit.IsPolyFit},
                New attribute With {.name = "RMSE", .type = CDFDataTypes.DOUBLE, .value = fit.RMSE},
                New attribute With {.name = "R2", .type = CDFDataTypes.DOUBLE, .value = fit.R_square},
                New attribute With {.name = "slope", .type = CDFDataTypes.DOUBLE, .value = fit.Slope},
                New attribute With {.name = "SSE", .type = CDFDataTypes.DOUBLE, .value = fit.SSE},
                New attribute With {.name = "SSR", .type = CDFDataTypes.DOUBLE, .value = fit.SSR}
            }

            cdf.AddVariable("polynomial", formula.Factors, size, attrs)
        End Sub
    End Module
End Namespace