#Region "Microsoft.VisualBasic::f73e74eca3400bfb156ccbe0398e6103, G:/mzkit/src/mzmath/mz_deco//ChromatogramModels/RtShift.vb"

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

    '   Total Lines: 38
    '    Code Lines: 18
    ' Comment Lines: 15
    '   Blank Lines: 5
    '     File Size: 987 B


    ' Class RtShift
    ' 
    '     Properties: refer_rt, RI, sample, sample_rt, shift
    '                 xcms_id
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' the rt shift result
''' </summary>
Public Class RtShift

    ''' <summary>
    ''' the sample name
    ''' </summary>
    ''' <returns></returns>
    Public Property sample As String
    Public Property xcms_id As String
    ''' <summary>
    ''' the reference rt
    ''' </summary>
    ''' <returns></returns>
    Public Property refer_rt As Double
    Public Property sample_rt As Double
    Public Property RI As Double

    ''' <summary>
    ''' <see cref="sample_rt"/> - <see cref="refer_rt"/>. 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property shift As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return sample_rt - refer_rt
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class
