﻿#Region "Microsoft.VisualBasic::480197bf8dae2334b2711432b3655de9, mzmath\ms2_math-core\Spectra\Models\Xml\Meta.vb"

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

    '   Total Lines: 47
    '    Code Lines: 36 (76.60%)
    ' Comment Lines: 3 (6.38%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (17.02%)
    '     File Size: 1.45 KB


    '     Class Meta
    ' 
    '         Properties: id
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: Log, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace Spectra.Xml

    ''' <summary>
    ''' [id, mz, rt, intensity]
    ''' </summary>
    Public Class Meta : Inherits ms1_scan
        Implements IMs1Scan

        <XmlText>
        Public Property id As String

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(id As String)
            Me.id = id
        End Sub

        Sub New(mz As Double, rt As Double, into As Double, Optional id As String = Nothing)
            Me.id = If(id, $"{mz.ToString("F4")}@{(rt / 60).ToString("F2")}min, intensity={into.ToString("G3")}")
            Me.mz = mz
            Me.scan_time = rt
            Me.intensity = into
        End Sub

        Public Overrides Function ToString() As String
            Return id
        End Function

        Public Shared Iterator Function Log(data As IEnumerable(Of Meta), Optional base As Double = std.E) As IEnumerable(Of Meta)
            For Each x As Meta In data.SafeQuery
                Yield New Meta With {
                    .id = x.id,
                    .intensity = std.Log(x.intensity + 1, base),
                    .mz = x.mz,
                    .scan_time = x.scan_time
                }
            Next
        End Function
    End Class
End Namespace
