#Region "Microsoft.VisualBasic::c0e77fffc4054a7d27ff8ba9ce6e35fa, G:/mzkit/src/mzmath/TargetedMetabolomics//LinearQuantitative/LinearPack/StandardCurveCDF.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 126
    '    Code Lines: 108
    ' Comment Lines: 0
    '   Blank Lines: 18
    '     File Size: 7.24 KB


    '     Module StandardCurveCDF
    ' 
    '         Function: WriteCDF
    ' 
    '         Sub: (+2 Overloads) fitLinear, WriteCDF
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.DataStorage
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace LinearQuantitative.Data

    Public Module StandardCurveCDF

        <Extension>
        Public Function WriteCDF(linear As StandardCurve) As MemoryStream
            Using ms As New MemoryStream
                Dim cdf As New netCDF.CDFWriter(ms)

                Call linear.WriteCDF(cdf)
                Call cdf.Save()
                Call cdf.Flush()
                Call ms.Flush()
                Call ms.Seek(Scan0, SeekOrigin.Begin)

                Return ms
            End Using
        End Function

        <Extension>
        Private Sub WriteCDF(linear As StandardCurve, cdf As netCDF.CDFWriter)
            Dim name As New attribute With {.name = "name", .type = CDFDataTypes.NC_CHAR, .value = linear.name}
            Dim [IS] As New attribute With {.name = "IS", .type = CDFDataTypes.NC_CHAR, .value = linear.IS.ID}
            Dim IS_name As New attribute With {.name = "IS_name", .type = CDFDataTypes.NC_CHAR, .value = linear.IS.name}
            Dim cIS As New attribute With {.name = "cIS", .type = CDFDataTypes.NC_DOUBLE, .value = linear.IS.CIS}
            Dim R2 As New attribute With {.name = "R2", .type = CDFDataTypes.NC_DOUBLE, .value = If(linear.linear Is Nothing, 0.0, linear.linear.R2)}
            Dim outliers As New attribute With {.name = "outliers", .type = CDFDataTypes.NC_INT, .value = linear.points.SafeQuery.Where(Function(p) Not p.valid).Count}
            Dim weighted As New attribute With {.name = "is_weighted", .type = CDFDataTypes.BOOLEAN, .value = TypeOf linear.linear Is WeightedFit}
            Dim points As New attribute With {.name = "ref_size", .type = CDFDataTypes.NC_INT, .value = linear.points.TryCount}

            cdf.GlobalAttributes(name, [IS], IS_name, cIS, R2, outliers, weighted, points)

            If Not linear.linear Is Nothing Then
                If TypeOf linear.linear Is WeightedFit Then
                    Call DirectCast(linear.linear, WeightedFit).fitLinear(cdf)
                Else
                    Call DirectCast(linear.linear, FitResult).fitLinear(cdf)
                End If
            End If

            Dim blankSize As New Dimension With {
                .name = "blank_size",
                .size = linear.blankControls.SafeQuery.Count
            }

            cdf.AddVariable("blanks", CType(linear.blankControls.SafeQuery.ToArray, doubles), blankSize)

            Dim levelNames As chars = linear.points _
                .SafeQuery _
                .Select(Function(p) p.level) _
                .Distinct _
                .GetJson
            Dim levelNameSize As New Dimension With {.name = "levelNames", .size = levelNames.Length}

            cdf.AddVariable("levelNames", levelNames, levelNameSize)

            Dim width As New Dimension With {.name = "width", .size = 5 + 3}

            For Each p As ReferencePoint In linear.points.SafeQuery
                Call cdf.AddVariable(p.level, CType(New Double() {p.AIS, p.Ati, p.cIS, p.Cti, p.Px, p.yfit, p.error, p.variant}, doubles), width, {
                    New attribute With {.name = "valid", .type = CDFDataTypes.BOOLEAN, .value = p.valid},
                    New attribute With {.name = "ID", .type = CDFDataTypes.NC_CHAR, .value = p.ID},
                    New attribute With {.name = "name", .type = CDFDataTypes.NC_CHAR, .value = p.Name}
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
                New attribute With {.name = "fisher", .type = CDFDataTypes.NC_DOUBLE, .value = fit.FisherF},
                New attribute With {.name = "R2", .type = CDFDataTypes.NC_DOUBLE, .value = fit.CorrelationCoefficient},
                New attribute With {.name = "SDV", .type = CDFDataTypes.NC_DOUBLE, .value = fit.StandardDeviation}
            }
            Dim matrix As Double()() = fit.VarianceMatrix.RowIterator.ToArray
            Dim vars As Double() = matrix.IteratesALL.ToArray
            Dim sizeofVar As New Dimension With {.name = "var_size", .size = vars.Length}
            Dim matrixDim As attribute() = {
                New attribute With {.name = "dim1", .type = CDFDataTypes.NC_INT, .value = matrix.Length},
                New attribute With {.name = "dim2", .type = CDFDataTypes.NC_INT, .value = matrix(Scan0).Length}
            }

            cdf.AddVariable("polynomial", CType(formula.Factors, doubles), size, attrs)
            cdf.AddVariable("DY", CType(fit.Residuals, doubles), sizeofDY)
            cdf.AddVector("SEC", fit.CoefficientsStandardError, sizeofSEC)
            cdf.AddVector("COVAR", vars, sizeofVar, matrixDim)
        End Sub

        <Extension>
        Private Sub fitLinear(fit As FitResult, cdf As netCDF.CDFWriter)
            Dim formula As Polynomial = fit.Polynomial
            Dim size As New Dimension With {.name = "polynomial_size", .size = formula.Factors.Length}
            Dim attrs As attribute() = {
                New attribute With {.name = "adjustR_square", .type = CDFDataTypes.NC_DOUBLE, .value = fit.AdjustR_square},
                New attribute With {.name = "factor_size", .type = CDFDataTypes.NC_DOUBLE, .value = fit.FactorSize},
                New attribute With {.name = "intercept", .type = CDFDataTypes.NC_DOUBLE, .value = fit.Intercept},
                New attribute With {.name = "is_poly", .type = CDFDataTypes.NC_DOUBLE, .value = fit.IsPolyFit},
                New attribute With {.name = "RMSE", .type = CDFDataTypes.NC_DOUBLE, .value = fit.RMSE},
                New attribute With {.name = "R2", .type = CDFDataTypes.NC_DOUBLE, .value = fit.R_square},
                New attribute With {.name = "slope", .type = CDFDataTypes.NC_DOUBLE, .value = fit.Slope},
                New attribute With {.name = "SSE", .type = CDFDataTypes.NC_DOUBLE, .value = fit.SSE},
                New attribute With {.name = "SSR", .type = CDFDataTypes.NC_DOUBLE, .value = fit.SSR}
            }

            cdf.AddVector("polynomial", formula.Factors, size, attrs)
        End Sub
    End Module
End Namespace
