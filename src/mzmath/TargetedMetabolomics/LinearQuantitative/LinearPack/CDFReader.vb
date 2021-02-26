#Region "Microsoft.VisualBasic::96bb43437c934b7ea8560e1efc998db3, TargetedMetabolomics\LinearQuantitative\LinearPack\CDFReader.vb"

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

    '     Module CDFReader
    ' 
    '         Function: Load, parseIS, parseLinear, parseLinearFit, parseLinears
    '                   parsePeaks, parseReference, parseWeightedFit
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Serialization.JSON
Imports any = Microsoft.VisualBasic.Scripting

Namespace LinearQuantitative.Data

    Module CDFReader

        Public Function Load(file As Stream) As LinearPack
            Dim pack As New LinearPack

            Using cdf As New netCDFReader(file)
                pack.title = cdf!title
                pack.time = any.ToString(cdf!time).DoCall(AddressOf Date.Parse)
                pack.reference = cdf.parseReference
                pack.peakSamples = cdf.parsePeaks.ToArray
                pack.linears = cdf.parseLinears.ToArray
                pack.IS = cdf.parseIS
            End Using

            Return pack
        End Function

        <Extension>
        Private Function parseIS(cdf As netCDFReader) As [IS]()
            Dim ISbstr As String = cdf.getDataVariable("IS").chars
            Dim list As BElement() = DirectCast(BencodeDecoder.Decode(ISbstr)(Scan0), BList).ToArray

            Return list _
                .Select(Function(b) DirectCast(b, BDictionary)) _
                .Select(Function(b)
                            Return New [IS] With {
                                .CIS = Val(DirectCast(b!CIS, BString).Value),
                                .ID = DirectCast(b!ID, BString).Value,
                                .name = DirectCast(b!name, BString).Value
                            }
                        End Function) _
                .ToArray
        End Function

        <Extension>
        Private Iterator Function parseLinears(cdf As netCDFReader) As IEnumerable(Of StandardCurve)
            Dim ionNames As String() = cdf.getDataVariable("linears").chars.LoadJSON(Of String())

            For Each ionName As String In ionNames
                Using ms As MemoryStream = DirectCast(cdf.getDataVariable(ionName).genericValue, Byte()).DoCall(Function(bytes) New MemoryStream(bytes))
                    Yield ms.parseLinear
                End Using
            Next
        End Function

        <Extension>
        Private Function parseLinear(ms As MemoryStream) As StandardCurve
            Using cdf As New netCDFReader(ms)
                Dim name As String = cdf!name
                Dim IS$ = cdf!IS
                Dim IS_name As String = cdf!IS_name
                Dim cIS As Double = cdf!cIS
                Dim isWeighted As Boolean = cdf!is_weighted
                Dim blanks As Double() = cdf.getDataVariable("blanks").numerics
                Dim points As New List(Of ReferencePoint)
                Dim levelNames As String() = cdf.getDataVariable("levelNames").chars.LoadJSON(Of String())
                Dim p As Double()
                Dim var As variable

                For Each lvKey As String In levelNames
                    var = cdf.getDataVariableEntry(lvKey)
                    p = cdf.getDataVariable(var).numerics
                    points += New ReferencePoint With {
                        .level = lvKey,
                        .ID = var.FindAttribute("ID").value,
                        .Name = var.FindAttribute("name").value,
                        .AIS = p(0),
                        .Ati = p(1),
                        .cIS = p(2),
                        .Cti = p(3),
                        .valid = var.FindAttribute("valid").getObjectValue,
                        .yfit = p(5)
                    }
                Next

                Return New StandardCurve With {
                    .name = name,
                    .[IS] = New [IS] With {
                        .ID = [IS],
                        .name = IS_name,
                        .CIS = cIS
                    },
                    .blankControls = blanks,
                    .points = points.ToArray,
                    .linear = If(isWeighted, cdf.parseWeightedFit, cdf.parseLinearFit)
                }
            End Using
        End Function

        <Extension>
        Private Function parseLinearFit(cdf As netCDFReader) As IFitted
            Dim polynomial As variable = cdf.getDataVariableEntry("polynomial")

            Return New FitResult With {
                .RMSE = polynomial.FindAttribute("RMSE").getObjectValue,
                .SSE = polynomial.FindAttribute("SSE").getObjectValue,
                .SSR = polynomial.FindAttribute("SSR").getObjectValue,
                .Polynomial = New Polynomial With {
                    .Factors = cdf.getDataVariable(polynomial).numerics
                }
            }
        End Function

        <Extension>
        Private Function parseWeightedFit(cdf As netCDFReader) As IFitted
            Dim polynomial As variable = cdf.getDataVariableEntry("polynomial")
            Dim rows As Double() = cdf.getDataVariable("COVAR").numerics
            Dim dim1 As Integer = cdf.getDataVariableEntry("COVAR").FindAttribute("dim1").getObjectValue
            Dim dim2 As Integer = cdf.getDataVariableEntry("COVAR").FindAttribute("dim2").getObjectValue
            Dim matrix As Double(,) = rows.Split(dim2).ToMatrix

            Return New WeightedFit With {
                .CoefficientsStandardError = cdf.getDataVariable("SEC").numerics,
                .Residuals = cdf.getDataVariable("DY").numerics,
                .CorrelationCoefficient = polynomial.FindAttribute("R2").getObjectValue,
                .FisherF = polynomial.FindAttribute("fisher").getObjectValue,
                .StandardDeviation = polynomial.FindAttribute("SDV").getObjectValue,
                .Polynomial = New Polynomial With {
                    .Factors = cdf.getDataVariable(polynomial).numerics
                },
                .VarianceMatrix = matrix
            }
        End Function

        <Extension>
        Private Iterator Function parsePeaks(cdf As netCDFReader) As IEnumerable(Of TargetPeakPoint)
            Dim peakNames As String() = cdf.getDataVariable("peaks").chars.LoadJSON(Of String())

            For Each name As String In peakNames
                Dim var As variable = cdf.getDataVariableEntry(name)
                Dim data = cdf.getDataVariable(var).numerics
                Dim time As Double() = data.Take(data.Length \ 2).ToArray
                Dim into As Double() = data.Skip(time.Length).ToArray
                Dim tickSeq As ChromatogramTick() = time _
                    .Select(Function(ti, i)
                                Return New ChromatogramTick With {
                                    .Time = ti,
                                    .Intensity = into(i)
                                }
                            End Function) _
                    .ToArray

                Yield New TargetPeakPoint With {
                    .Name = var.FindAttribute("name").value,
                    .SampleName = var.FindAttribute("sample_name").value,
                    .ChromatogramSummary = var.FindAttribute("summary").value _
                        .Split("|"c) _
                        .Select(Function(str)
                                    Dim t As Double() = str _
                                        .Split(":"c) _
                                        .Select(AddressOf Val) _
                                        .ToArray

                                    Return New Quantile With {
                                        .Percentage = t(0),
                                        .Quantile = t(1)
                                    }
                                End Function) _
                        .ToArray,
                    .Peak = New ROIPeak With {
                        .base = var.FindAttribute("base").getObjectValue,
                        .peakHeight = var.FindAttribute("maxinto").getObjectValue,
                        .window = New DoubleRange(
                            min:=var.FindAttribute("rtmin").getObjectValue,
                            max:=var.FindAttribute("rtmax").getObjectValue
                        ),
                        .ticks = tickSeq
                    }
                }
            Next
        End Function

        <Extension>
        Private Function parseReference(cdf As netCDFReader) As Dictionary(Of String, SampleContentLevels)
            Dim allSampleNames As String() = cdf.getDataVariable("sampleNames").chars.LoadJSON(Of String())
            Dim levelIons As variable() = cdf.variables _
                .Where(Function(xi) xi.name.StartsWith("levels\")) _
                .ToArray
            Dim list As New Dictionary(Of String, SampleContentLevels)

            For Each ion As variable In levelIons
                Dim ionName As String = ion.name.Replace("levels\", "")
                Dim levels As New Dictionary(Of String, Double)
                Dim vals As Double() = cdf.getDataVariable(ion).numerics
                Dim directMap As Boolean = ion _
                    .FindAttribute("directMap") _
                    .getObjectValue

                For i As Integer = 0 To allSampleNames.Length - 1
                    levels(allSampleNames(i)) = vals(i)
                Next

                list(ionName) = New SampleContentLevels(levels, directMap)
            Next

            Return list
        End Function
    End Module
End Namespace
