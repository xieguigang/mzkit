#Region "Microsoft.VisualBasic::7531cfba36cb24fa42bc6584f761f259, mzmath\MoleculeNetworking\SpectrumGrid\SpectrumLine.vb"

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

    '   Total Lines: 23
    '    Code Lines: 15 (65.22%)
    ' Comment Lines: 3 (13.04%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (21.74%)
    '     File Size: 712 B


    ' Class SpectrumLine
    ' 
    '     Properties: cluster, intensity, mz, rt
    ' 
    '     Function: SetRT, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' a spectrum grid line in the grid networking
''' </summary>
Public Class SpectrumLine

    ''' <summary>
    ''' the spectrum data objects across multiple rawdata files in current cluster
    ''' </summary>
    ''' <returns></returns>
    Public Property cluster As PeakMs2()
    ''' <summary>
    ''' the corresponding intensity vector of current cluster data
    ''' </summary>
    ''' <returns></returns>
    Public Property intensity As Double()
    ''' <summary>
    ''' precursor ion of current spectrum cluster
    ''' </summary>
    ''' <returns></returns>
    Public Property rt As Double
    Public Property mz As Double

    ''' <summary>
    ''' gets the cluster size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        Get
            Return cluster.TryCount
        End Get
    End Property

    Friend Function SetRT(rt As Double) As SpectrumLine
        _rt = rt
        Return Me
    End Function

    Friend Function hashKey() As String
        Return $"{mz.ToString("F3")}@{rt.ToString("F0")}s"
    End Function

    Public Overrides Function ToString() As String
        Return $"{mz.ToString("F3")}@{(rt / 60).ToString("F2")}min, {cluster.Length} files: {cluster.Select(Function(s) s.file).GetJson}"
    End Function

End Class
