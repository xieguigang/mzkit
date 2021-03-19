#Region "Microsoft.VisualBasic::03f6f6b8bc38a3f61b99a956e7efb39e, assembly\mzPack\Stream\Scan.vb"

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

    '     Class ScanMS2
    ' 
    '         Properties: intensity, parentMz, polarity, rt
    ' 
    '     Class ScanMS1
    ' 
    '         Properties: BPC, products, TIC
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math

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

    End Class
End Namespace
