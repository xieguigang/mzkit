#Region "Microsoft.VisualBasic::8d1b11642e50f97b02511f25091b0eed, G:/mzkit/src/mzmath/MSEngine//MassSearchArguments.vb"

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

    '   Total Lines: 10
    '    Code Lines: 7
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 303 B


    ' Class MassSearchArguments
    ' 
    '     Properties: Adducts, IonMode, Optionals, PPM
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Public Class MassSearchArguments

    Public Property IonMode As IonModes
    Public Property PPM As Integer
    Public Property Adducts As String()
    Public Property Optionals As Dictionary(Of String, String)

End Class
