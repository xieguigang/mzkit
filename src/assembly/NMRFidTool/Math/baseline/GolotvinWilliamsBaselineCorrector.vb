#Region "Microsoft.VisualBasic::bc82b3344247bed1da7951292ba9cb2f, assembly\NMRFidTool\Math\baseline\GolotvinWilliamsBaselineCorrector.vb"

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

    '   Total Lines: 236
    '    Code Lines: 162 (68.64%)
    ' Comment Lines: 48 (20.34%)
    '    - Xml Docs: 16.67%
    ' 
    '   Blank Lines: 26 (11.02%)
    '     File Size: 12.17 KB


    '     Class GolotvinWilliamsBaselineCorrector
    ' 
    '         Function: baslineDetection, calculateBaselineModel, calculateSpectralNoise, correctBaseline
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Math

' 
'  Copyright (c) 2013 EMBL, European Bioinformatics Institute.
' 
'  This program is free software: you can redistribute it and/or modify
'  it under the terms of the GNU Lesser General Public License as published by
'  the Free Software Foundation, either version 3 of the License, or
'  (at your option) any later version.
' 
'  This program is distributed in the hope that it will be useful,
'  but WITHOUT ANY WARRANTY; without even the implied warranty of
'  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'  GNU Lesser General Public License for more details.
' 
'  You should have received a copy of the GNU Lesser General Public License
'  along with this program.  If not, see <http://www.gnu.org/licenses/>.
' 

Namespace fidMath.Baseline


    ''' <summary>
    ''' Implementation of a baseline corrector based on the method from Golotvin and Williams (2000)
    ''' 
    ''' @author  Luis F. de Figueiredo
    ''' 
    ''' User: ldpf
    ''' Date: 29/05/2013
    ''' Time: 15:48
    ''' To change this template use File | Settings | File Templates.
    ''' </summary>
    Public Class GolotvinWilliamsBaselineCorrector
        Implements BaselineCorrector


        Public Overridable Function correctBaseline(spectrum As Spectrum) As Spectrum Implements BaselineCorrector.correctBaseline

            Dim baselineIndexes = baslineDetection(spectrum.RealChannelData)

            Dim baselineModel = calculateBaselineModel(baselineIndexes, spectrum.RealChannelData)

            Dim subtractedReal = New Double(spectrum.RealChannelData.Length - 1) {}
            Array.Copy(spectrum.RealChannelData, 0, subtractedReal, 0, spectrum.RealChannelData.Length)

            For i = 0 To spectrum.RealChannelData.Length - 1
                subtractedReal(i) -= baselineModel(i)
            Next

            spectrum.Baseline = baselineIndexes
            spectrum.BaselineModel = baselineModel
            spectrum.RealChannelData = subtractedReal
            Return spectrum 'To change body of implemented methods use File | Settings | File Templates.
        End Function

        Private Function calculateBaselineModel(baselineIndexes As Boolean(), realChannelData As Double()) As Double()
            Dim windowSize = 41
            Dim convolutionWindow = New Double(windowSize - 1) {}
            Dim baselineModel = New Double(realChannelData.Length - 1) {}
            Dim indexes = New Double(1) {}
            indexes(0) = -1
            Dim intesities = New Double(1) {}
            Dim window As IList(Of Double?) = New List(Of Double?)(windowSize)
            Dim smoothValue As Double = 0
            Dim lastVisitedIndex = 0

            ' preload the window list with points at the end of the spectrum
            For i = realChannelData.Length - 1 To 1 Step -1
                If baselineIndexes(i) Then
                    ' is this going to work????
                    window.Insert(0, realChannelData(i) / windowSize)
                    smoothValue += realChannelData(i) / windowSize
                End If
                If window.Count >= windowSize / 2 - 1 Then
                    Exit For
                End If
            Next

            For i = 0 To realChannelData.Length - 1
                ' for each baseline point calculate the smoothed spectrum
                If baselineIndexes(i) Then
                    ' first baseline point considered so the window is not fully loaded
                    If window.Count < windowSize Then
                        window.Add(realChannelData(i) / windowSize)
                        smoothValue += realChannelData(i) / windowSize
                        ' load the window with the right side values
                        For j = i + 1 To realChannelData.Length - 1
                            If baselineIndexes(j) Then
                                lastVisitedIndex = j
                                window.Add(realChannelData(j) / windowSize)
                                smoothValue += realChannelData(j) / windowSize
                            End If
                            If window.Count >= windowSize Then
                                Exit For
                            End If
                        Next
                        If lastVisitedIndex = realChannelData.Length Then
                            Console.Error.WriteLine("Problem loading the window in the baseline correction")
                        End If
                    Else
                        ' just update the window and the smoothvalue
                        smoothValue -= window(0).Value
                        window.RemoveAt(0)
                        While lastVisitedIndex < realChannelData.Length - 1
                            lastVisitedIndex += 1
                            If baselineIndexes(lastVisitedIndex) Then
                                window.Add(realChannelData(lastVisitedIndex) / windowSize)
                                smoothValue += realChannelData(lastVisitedIndex) / windowSize
                            End If
                            If window.Count = windowSize Then
                                Exit While
                            End If
                        End While
                        ' if we were not able to fill in the window with values
                        If window.Count < windowSize Then
                            ' this should only happen if we reach the edge of the spectrum...
                            lastVisitedIndex = -1
                            While lastVisitedIndex < realChannelData.Length
                                lastVisitedIndex += 1
                                If baselineIndexes(lastVisitedIndex) Then
                                    window.Add(realChannelData(lastVisitedIndex) / windowSize)
                                    smoothValue += realChannelData(lastVisitedIndex) / windowSize
                                End If
                                If window.Count = windowSize Then
                                    Exit While
                                End If
                            End While
                        End If
                    End If
                    baselineModel(i) = smoothValue
                    ' check if we are coming from a non-baseline point and do a linear interpolation
                    ' for the non-baseline points in between
                    ' TODO !!!make sure this is not the first point!!!! but the first point would break this...
                    If i > 0 Then
                        If Not baselineIndexes(i - 1) AndAlso indexes(0) > 0 Then
                            indexes(1) = i
                            intesities(1) = baselineModel(i)
                            Dim splineFunction As FitResult = LeastSquares.LinearFit(indexes, intesities)
                            For j As Integer = indexes(0) To indexes(1) - 1
                                baselineModel(j) = splineFunction(j)
                            Next
                        End If
                    End If
                    indexes(0) = i
                    intesities(0) = baselineModel(i)
                End If

            Next
            ' I still need to take care of the cases where edges of the spectrum are not baseline points...
            If Not baselineIndexes(0) OrElse Not baselineIndexes(baselineIndexes.Length - 1) Then
                ' fetch the of the extreme baseline indexes
                For i = baselineModel.Length - 1 To 1 Step -1
                    If baselineIndexes(i) Then
                        indexes(0) = i - baselineModel.Length
                        intesities(0) = baselineModel(i)
                        Exit For
                    End If
                Next
                For i = 0 To baselineModel.Length - 1
                    If baselineIndexes(i) Then
                        indexes(1) = i
                        intesities(1) = baselineModel(i)
                        Exit For
                    End If
                Next
                Dim splineFunction As FitResult = LeastSquares.LinearFit(indexes, intesities)
                For j As Integer = CInt(indexes(0)) + 1 To -1
                    baselineModel(j + baselineModel.Length) = splineFunction(j)
                Next
                For j As Integer = 0 To indexes(1) - 1
                    baselineModel(j) = splineFunction(j)
                Next

            End If

            Return baselineModel 'To change body of created methods use File | Settings | File Templates.
        End Function

        Private Function baslineDetection(realChannelData As Double()) As Boolean()
            Dim rectangleWidth = 60
            Dim factor = 4
            Dim noiseStandardDeviation = calculateSpectralNoise(realChannelData)
            Dim baseline = New Boolean(realChannelData.Length - 1) {}

            For i = 0 To realChannelData.Length - 1
                Dim datapoints = New Double(rectangleWidth + 1 - 1) {}
                'Let's assume that the spectrum is a continuum of points like in Friedrichs, M. (1995)
                If i < rectangleWidth / 2 Then
                    Array.Copy(realChannelData, 0, datapoints, 0, CInt(i + rectangleWidth / 2 + 1))
                    ' take the missing points from the end of the spectra
                    Array.Copy(realChannelData, realChannelData.Length - CInt(rectangleWidth / 2 - i), datapoints, i + CInt(rectangleWidth / 2) + 1, CInt(rectangleWidth - (i + rectangleWidth / 2)))
                ElseIf realChannelData.Length - i < rectangleWidth / 2 + 1 Then
                    'collect the last i points plus the the left side points of i
                    Array.Copy(realChannelData, i - CInt(rectangleWidth / 2), datapoints, 0, realChannelData.Length - i + CInt(rectangleWidth / 2))
                    ' take the missing points from the beginning of the spectra
                    Array.Copy(realChannelData, 0, datapoints, realChannelData.Length - i + CInt(rectangleWidth / 2), CInt(rectangleWidth / 2 - (realChannelData.Length - i) + 1))
                Else
                    Array.Copy(realChannelData, i - CInt(rectangleWidth / 2), datapoints, 0, rectangleWidth + 1)
                End If
                baseline(i) = datapoints.Max - datapoints.Min < factor * noiseStandardDeviation
            Next

            Return baseline 'To change body of created methods use File | Settings | File Templates.
        End Function

        Private Function calculateSpectralNoise(realChannelData As Double()) As Double
            Dim numberOfRegions = 1
            ' define the window size starting with 32 datapoints
            ' and shrinking it until the exact division is achieved
            For i As Integer = 32 To 1 Step -1
                ' check if the datapoints can be exactly divided into i regions
                If realChannelData.Length Mod i = 0 Then
                    numberOfRegions = i
                    Exit For
                End If
            Next
            'set the noise to the maximum intensity
            Dim noiseStandardDeviation As Double = realChannelData.Max

            'determine the standard deviation for each window and record the lowest standard deviation
            Dim idx = 0

            While idx < realChannelData.Length
                Dim region = New Double(realChannelData.Length / numberOfRegions - 1) {}
                Array.Copy(realChannelData, idx, region, 0, CInt(realChannelData.Length / numberOfRegions))
                noiseStandardDeviation = If(noiseStandardDeviation < region.SD, noiseStandardDeviation, region.SD)
                idx += realChannelData.Length / numberOfRegions
            End While
            Return noiseStandardDeviation
        End Function


    End Class

End Namespace
