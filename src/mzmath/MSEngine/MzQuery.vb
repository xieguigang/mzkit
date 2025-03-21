﻿#Region "Microsoft.VisualBasic::405483f4bb51715037137fbdcc32d7ed, mzmath\MSEngine\MzQuery.vb"

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

    '   Total Lines: 86
    '    Code Lines: 50 (58.14%)
    ' Comment Lines: 26 (30.23%)
    '    - Xml Docs: 96.15%
    ' 
    '   Blank Lines: 10 (11.63%)
    '     File Size: 2.77 KB


    ' Class MzQuery
    ' 
    '     Properties: isEmpty, mz_ref, ppm, score
    ' 
    '     Function: Clone, IsNullOrEmpty, ReferenceKey, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.Language.Default

<Assembly: InternalsVisibleTo("mzkit")>

''' <summary>
''' query result of a ms1 m/z ion
''' </summary>
Public Class MzQuery : Inherits MetID
    Implements IsEmpty

    ''' <summary>
    ''' the evaluated theoretical m/z value based 
    ''' on the precursor type and formula string 
    ''' data.
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property mz_ref As Double
    <XmlAttribute> Public Property ppm As Double
    ''' <summary>
    ''' used in MSJointConnection peak list annotation.
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property score As Double

    Friend ReadOnly Property isEmpty As Boolean Implements Language.Default.IsEmpty.IsEmpty
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return mz = 0.0 AndAlso
                ppm = 0.0 AndAlso
                score = 0.0 AndAlso
                precursor_type.StringEmpty AndAlso
                unique_id.StringEmpty AndAlso
                name.StringEmpty
        End Get
    End Property

    ''' <summary>
    ''' make value copy
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Clone() As MzQuery
        Return New MzQuery With {
            .unique_id = unique_id,
            .mz = mz,
            .ppm = ppm,
            .precursor_type = precursor_type,
            .score = score,
            .name = name,
            .mz_ref = mz_ref
        }
    End Function

    ''' <summary>
    ''' makes a unique reference key of current mz query result
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ReferenceKey(query As MzQuery, Optional format As String = "F4") As String
        Return $"{query.mz.ToString(format)}|{query.unique_id}"
    End Function

    ''' <summary>
    ''' unique_id precursor_type, m/z xxx.xxxx; score=xxx
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim prefix As String = $"{unique_id} {precursor_type}, m/z {mz.ToString("F4")}"

        If score > 0 Then
            Return $"{prefix}; score={score}"
        Else
            Return prefix
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function IsNullOrEmpty(hit As MzQuery) As Boolean
        Return hit Is Nothing OrElse hit.isEmpty
    End Function

End Class
