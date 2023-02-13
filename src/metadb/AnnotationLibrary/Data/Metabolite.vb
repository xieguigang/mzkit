#Region "Microsoft.VisualBasic::c195d0038309634021eb1ca3621db3e6, mzkit\src\metadb\AnnotationLibrary\Data\Metabolite.vb"

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

    '   Total Lines: 94
    '    Code Lines: 63
    ' Comment Lines: 15
    '   Blank Lines: 16
    '     File Size: 2.98 KB


    ' Class Metabolite
    ' 
    '     Properties: annotation, exactMass, fragments, Id, mz
    '                 precursors, rt, spectrumBlockId, spectrums
    ' 
    ' Class PrecursorData
    ' 
    '     Properties: charge, ion, mz, rt
    ' 
    ' Class Spectrum
    ' 
    '     Properties: annotations, guid, intensity, ionMode, mz
    ' 
    '     Function: getMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' data model of the metabolite ms reference data
''' </summary>
Public Class Metabolite

    Public ReadOnly Property Id As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return annotation.ID
        End Get
    End Property

    Public ReadOnly Property exactMass As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return FormulaScanner.ScanFormula(annotation.formula).ExactMass
        End Get
    End Property

    Public ReadOnly Property rt As Double()
        Get
            Return precursors _
                .Select(Function(p) p.rt) _
                .IteratesALL _
                .ToArray
        End Get
    End Property

    Public ReadOnly Property mz As Double()
        Get
            Return precursors _
                .Select(Function(p) p.mz) _
                .ToArray
        End Get
    End Property

    <MessagePackMember(0)> Public Property annotation As MetaInfo
    <MessagePackMember(1)> Public Property precursors As PrecursorData()
    <MessagePackMember(2)> Public Property spectrums As Spectrum()

    ''' <summary>
    ''' the fragment annotation list
    ''' 
    ''' intensity value in this collection is
    ''' the occupation count of the target 
    ''' annotation
    ''' </summary>
    ''' <returns></returns>
    <MessagePackMember(3)> Public Property fragments As ms2()

    Public Property spectrumBlockId As String

End Class

Public Class PrecursorData

    <MessagePackMember(0)> Public Property ion As String
    <MessagePackMember(1)> Public Property charge As Integer
    <MessagePackMember(2)> Public Property rt As Double()
    ''' <summary>
    ''' the experiment m/z data
    ''' </summary>
    ''' <returns></returns>
    <MessagePackMember(3)> Public Property mz As Double

End Class

Public Class Spectrum

    <MessagePackMember(0)> Public Property guid As String
    <MessagePackMember(1)> Public Property mz As Double()
    <MessagePackMember(2)> Public Property ionMode As Integer
    <MessagePackMember(3)> Public Property intensity As Double()
    <MessagePackMember(4)> Public Property annotations As String()

    Public Function getMatrix() As IEnumerable(Of ms2)
        Return mz _
            .Select(Function(mzi, i)
                        Return New ms2 With {
                            .mz = mzi,
                            .intensity = intensity(i),
                            .Annotation = annotations(i)
                        }
                    End Function)
    End Function

End Class
