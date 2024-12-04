
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace PackLib

    ''' <summary>
    ''' an abstract model of the reference spectrum database.
    ''' </summary>
    Public Interface IReferencePack : Inherits IDisposable

        ''' <summary>
        ''' Write a reference spectrum to file
        ''' </summary>
        ''' <param name="uid"></param>
        ''' <param name="formula"></param>
        ''' <param name="spectrum"></param>
        Sub Push(uid As String, formula As String, spectrum As PeakMs2)

    End Interface

    ''' <summary>
    ''' A database pack file in mgf ascii text file format
    ''' </summary>
    Public Class MgfPack : Implements IDisposable, IReferencePack

        Dim disposedValue As Boolean
        Dim text As StreamWriter

        Sub New(s As Stream)
            text = New StreamWriter(s)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Push(uid As String, formula As String, spectrum As PeakMs2) Implements IReferencePack.Push
            Call MgfWriter.WriteAsciiMgf(spectrum.MgfIon, text)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call text.Flush()
                    Call text.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace