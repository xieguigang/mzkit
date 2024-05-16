#Region "Microsoft.VisualBasic::3db6528367d17ad63178462c340b52be, assembly\NMRFidTool\Math\phasing\Export.vb"

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

    '   Total Lines: 14
    '    Code Lines: 12
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 670 B


    '     Module Export
    ' 
    '         Function: phasecorrection
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace fidMath.Phasing

    Public Module Export

        Public Function phasecorrection(spectrumRaw As Double(), teta0 As Double, teta1 As Double, pivot As Integer) As Double()
            Dim spectrum = New Double(spectrumRaw.Length / 2 - 1) {}
            Console.WriteLine(teta0 + teta1 * (0 - pivot) / spectrum.Length)
            For i = 0 To spectrum.Length - 1
                spectrum(i) = spectrumRaw(i * 2) * Math.Cos(teta0 + teta1 * (i - pivot) / spectrum.Length) + spectrumRaw(i * 2 + 1) * Math.Sin(teta0 + teta1 * (i - pivot) / spectrum.Length)
            Next
            Return spectrum
        End Function
    End Module
End Namespace
