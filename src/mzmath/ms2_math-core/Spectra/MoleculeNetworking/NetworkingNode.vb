#Region "Microsoft.VisualBasic::ded47aed874407dc877dca5b6b1754ae, mzkit\src\mzmath\ms2_math-core\Spectra\MoleculeNetworking\NetworkingNode.vb"

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

'   Total Lines: 102
'    Code Lines: 75
' Comment Lines: 14
'   Blank Lines: 13
'     File Size: 4.20 KB


'     Class NetworkingNode
' 
'         Properties: members, mz, referenceId, representation
' 
'         Function: Create, GetXIC, ToString, unionRepresentative
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Spectra.MoleculeNetworking

    Public Class NetworkingNode

        Public Property representation As LibraryMatrix

        Public Property members As PeakMs2()
        Public Property mz As Double

        ''' <summary>
        ''' get the reference <see cref="LibraryMatrix.name"/> from the <see cref="representation"/>
        ''' spectrum object as the reference id of current network node
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property referenceId As String
            Get
                Return representation.name
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return referenceId
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="tolerance">ms2 tolerance</param>
        ''' <returns></returns>
        Public Shared Function Create(parentIon As Double, raw As SpectrumCluster, tolerance As Tolerance, cutoff As LowAbundanceTrimming) As NetworkingNode
            Dim ions As PeakMs2() = raw.cluster _
                .Select(Function(a)
                            Dim maxInto = a.mzInto.Select(Function(x) x.intensity).Max

                            For i As Integer = 0 To a.mzInto.Length - 1
                                a.mzInto(i).intensity = a.mzInto(i).intensity / maxInto
                            Next

                            Return a
                        End Function) _
                .ToArray
            Dim nodeData As LibraryMatrix = unionRepresentative(ions, tolerance, cutoff)

            Return New NetworkingNode With {
                .mz = parentIon,
                .members = ions,
                .representation = nodeData
            }
        End Function

        Private Shared Function unionRepresentative(ions As PeakMs2(), tolerance As Tolerance, cutoff As LowAbundanceTrimming) As LibraryMatrix
            Dim mz As NamedCollection(Of ms2)() = ions _
                .Select(Function(i) i.mzInto) _
                .IteratesALL _
                .GroupBy(Function(a) a.mz, tolerance) _
                .ToArray
            ' Dim files = ions.Select(Function(a) a.file).GroupBy(Function(a) a).OrderByDescending(Function(a) a.Count).First.Key
            ' Dim rt = ions.OrderByDescending(Function(a) a.Ms2Intensity).First.rt
            Dim matrix As ms2() = mz _
                .Select(Function(a)
                            Return New ms2 With {
                                .mz = Val(a.name),
                                .intensity = a.Select(Function(x) x.intensity).Max
                            }
                        End Function) _
                .ToArray _
                .Centroid(tolerance, cutoff) _
                .ToArray

            Return New LibraryMatrix With {
                .centroid = True,
                .ms2 = matrix,
                .name = CreateReferenceId(matrix, topN:=3)
            }
        End Function

        Private Shared Function CreateReferenceId(matrix As ms2(), Optional topN As Integer = 3) As String
            Dim products As ms2() = matrix _
                .OrderByDescending(Function(a) a.intensity) _
                .Take(topN) _
                .ToArray
            Dim uid As String = products _
                .Select(Function(i) $"{i.mz.ToString("F3")}:{(i.intensity * 100).ToString("F0")}") _
                .JoinBy("/")

            Return uid
        End Function
    End Class
End Namespace
