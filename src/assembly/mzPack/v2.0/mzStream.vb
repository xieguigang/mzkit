Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports asciiA = Microsoft.VisualBasic.Text.ASCII

''' <summary>
''' mzPack format in HDS stream file
''' </summary>
Public Class mzStream : Implements IDisposable

    ReadOnly pack As StreamPack

    Dim disposedValue As Boolean

    Public ReadOnly Property Application As FileApplicationClass

    Sub New(filepath As String)
        Call Me.New(filepath.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
    End Sub

    Sub New(stream As Stream)
        pack = New StreamPack(stream)
        Application = safeParseClassType()
    End Sub

    ''' <summary>
    ''' read all data into memory(memory load = max)
    ''' </summary>
    ''' <returns></returns>
    Public Function ReadModel() As mzPack
        Return New mzPack With {
            .Application = Application
        }
    End Function

    Private Function safeParseClassType() As FileApplicationClass
        Return Strings _
            .Trim(pack.ReadText(".etc/app.cls")) _
            .Trim(asciiA.TAB, asciiA.CR, asciiA.LF) _
            .DoCall(Function(str)
                        If str = "" Then
                            Return FileApplicationClass.LCMS
                        Else
                            Return DirectCast([Enum].Parse(GetType(FileApplicationClass), str), FileApplicationClass)
                        End If
                    End Function)
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call pack.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
