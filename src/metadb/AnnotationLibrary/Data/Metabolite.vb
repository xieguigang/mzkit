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