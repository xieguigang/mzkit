#Region "Microsoft.VisualBasic::a9837f67dc6d0805d743079770f81553, mzkit\src\assembly\assembly\mzPack\Stream\ScanMS2.vb"

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

    '   Total Lines: 30
    '    Code Lines: 22
    ' Comment Lines: 3
    '   Blank Lines: 5
    '     File Size: 994.00 B


    '     Class ScanMS2
    ' 
    '         Properties: activationMethod, centroided, charge, collisionEnergy, intensity
    '                     parentMz, polarity, rt
    ' 
    '         Function: GetMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace mzData.mzWebCache

    ''' <summary>
    ''' MS/MS scan
    ''' </summary>
    Public Class ScanMS2 : Inherits MSScan
        Implements IMs1

        Public Property parentMz As Double Implements IMs1.mz
        Public Overrides Property rt As Double Implements IRetentionTime.rt
        Public Property intensity As Double
        Public Property polarity As Integer
        Public Property charge As Integer
        Public Property activationMethod As ActivationMethods
        Public Property collisionEnergy As Double
        Public Property centroided As Boolean

        Public Function GetMatrix() As LibraryMatrix
            Return New LibraryMatrix With {
                .centroid = centroided,
                .ms2 = GetMs.ToArray,
                .name = scan_id
            }
        End Function

    End Class
End Namespace
