#Region "Microsoft.VisualBasic::19b86ded23bb6cbbbf8eb960212009ad, mzmath\MoleculeNetworking\SpectrumPool\Utils.vb"

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

    '   Total Lines: 48
    '    Code Lines: 30 (62.50%)
    ' Comment Lines: 13 (27.08%)
    '    - Xml Docs: 92.31%
    ' 
    '   Blank Lines: 5 (10.42%)
    '     File Size: 1.71 KB


    '     Class Utils
    ' 
    '         Function: ConservedGuid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace PoolData

    Public Class Utils

        Public Const unknown As String = NameOf(unknown)

        ''' <summary>
        ''' The conserved guid is generated via the md5 hashcode of contents:
        ''' 
        ''' 1. mz(F4):into
        ''' 2. mz1(F4)
        ''' 3. rt(F2)
        ''' 4. biosample
        ''' 5. organism
        ''' 6. instrument
        ''' 7. precursor_type
        ''' </summary>
        ''' <param name="spectral"></param>
        ''' <returns></returns>
        Public Shared Function ConservedGuid(spectral As PeakMs2) As String
            Dim desc As ms2() = spectral.mzInto _
                .OrderByDescending(Function(mzi) mzi.intensity) _
                .ToArray
            Dim peaks As String() = desc _
                .Select(Function(m) m.mz.ToString("F4") & ":" & m.intensity) _
                .ToArray
            Dim mz1 As String = spectral.mz.ToString("F4")
            Dim rt As String = spectral.rt.ToString("F2")
            Dim meta As String() = {
                spectral.meta.TryGetValue("biosample", unknown),
                spectral.meta.TryGetValue("organism", unknown),
                spectral.meta.TryGetValue("instrument", unknown),
                spectral.precursor_type
            }
            Dim hashcode As String = peaks _
                .JoinIterates(mz1) _
                .JoinIterates(rt) _
                .JoinIterates(meta) _
                .JoinBy(spectral.mzInto.Length) _
                .MD5

            Return hashcode
        End Function
    End Class
End Namespace
