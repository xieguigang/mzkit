#Region "Microsoft.VisualBasic::060ac8264719eac72c5ed176fcf98d12, G:/mzkit/src/assembly/SpectrumTree//Query/ClusterHit.vb"

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

    '   Total Lines: 83
    '    Code Lines: 39
    ' Comment Lines: 34
    '   Blank Lines: 10
    '     File Size: 2.69 KB


    '     Class ClusterHit
    ' 
    '         Properties: basePeak, ClusterEntropy, ClusterForward, ClusterId, ClusterJaccard
    '                     ClusterReverse, ClusterRt, entropy, forward, Id
    '                     jaccard, queryId, queryIntensity, queryMz, queryRt
    '                     representive, reverse, size, totalScore
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace Query

    ''' <summary>
    ''' the reference spectrum library search result
    ''' </summary>
    Public Class ClusterHit

        ''' <summary>
        ''' the reference id in library
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' this property value usually be the metabolite id from the metabolite annotation database
        ''' </remarks>
        Public Property Id As String
        Public Property representive As SSM2MatrixFragment()
        Public Property forward As Double
        Public Property reverse As Double
        Public Property jaccard As Double
        Public Property entropy As Double

#Region "the query ms1 data"
        ''' <summary>
        ''' the query id from the sample source data
        ''' </summary>
        ''' <returns></returns>
        Public Property queryId As String
        ''' <summary>
        ''' the query ms1 m/z
        ''' </summary>
        ''' <returns></returns>
        Public Property queryMz As Double
        ''' <summary>
        ''' the query ms1 rt
        ''' </summary>
        ''' <returns></returns>
        Public Property queryRt As Double
        ''' <summary>
        ''' the query ms1 intensity value
        ''' </summary>
        ''' <returns></returns>
        Public Property queryIntensity As Double

        ''' <summary>
        ''' the basepeak m/z of the sample spectrum
        ''' </summary>
        ''' <returns></returns>
        Public Property basePeak As Double
#End Region

        Public Property ClusterRt As Double()
        Public Property ClusterForward As Double()
        Public Property ClusterReverse As Double()
        Public Property ClusterJaccard As Double()
        Public Property ClusterEntropy As Double()
        Public Property ClusterId As String()

        ''' <summary>
        ''' hit size
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                Return ClusterId.Length
            End Get
        End Property

        Public ReadOnly Property totalScore As Double
            Get
                Return (forward + reverse + jaccard + entropy) * size
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{Id}, {size} candidate hits[total_score: {totalScore.ToString("F2")}]"
        End Function

    End Class
End Namespace
