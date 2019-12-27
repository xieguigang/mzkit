#Region "Microsoft.VisualBasic::a58f92c0aed71924e286eddb36bc1733, visual\test\Module2.vb"

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

    ' Module Module2
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.MassSpectrum.Math.Spectra

Module Module2

    Sub Main()
        Dim ref = New LibraryMatrix With {
            .ms2 = {New ms2 With {.mz = 100, .intensity = 1, .quantity = 1}, New ms2 With {.mz = 200, .intensity = 0, .quantity = 0}},
            .Name = "Library"
        }

        Dim A As New LibraryMatrix With {
            .ms2 = {New ms2 With {.mz = 100, .intensity = 0.8, .quantity = 0.8}, New ms2 With {.mz = 200, .intensity = 0.05, .quantity = 0.05}},
            .Name = NameOf(A)
        }

        Dim B As New LibraryMatrix With {
            .ms2 = {New ms2 With {.mz = 100, .intensity = 0.2, .quantity = 0.2}, New ms2 With {.mz = 200, .intensity = 0.98, .quantity = 0.98}},
            .Name = NameOf(B)
        }

        Call SMRUCC.MassSpectrum.Visualization.AlignMirrorPlot(A, ref).Save("./A.png")
        Call SMRUCC.MassSpectrum.Visualization.AlignMirrorPlot(B, ref).Save("./B.png")
    End Sub
End Module
