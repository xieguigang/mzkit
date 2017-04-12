Imports System.Collections.Generic
Imports System.Text
Imports System.Collections
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Reflection

Friend Class FixedArray
	Implements IDisposable
	Private pHandle As GCHandle
	Private pArray As Array

	Public Sub New(array As Array)
		pArray = array
		pHandle = GCHandle.Alloc(array, GCHandleType.Pinned)
	End Sub

	Protected Overrides Sub Finalize()
		Try
			pHandle.Free()
		Finally
			MyBase.Finalize()
		End Try
	End Sub

	#Region "IDisposable Members"

	Public Sub Dispose() Implements IDisposable.Dispose
		pHandle.Free()
		GC.SuppressFinalize(Me)
	End Sub

	Public Default ReadOnly Property Item(idx As Integer) As IntPtr
		Get
			Return Marshal.UnsafeAddrOfPinnedArrayElement(pArray, idx)
		End Get
	End Property
	Public Shared Widening Operator CType(fixedArray As FixedArray) As IntPtr
		Return fixedArray(0)
	End Operator
	#End Region
End Class

Module ListHelper

    <System.Runtime.CompilerServices.Extension>
    Public Sub Add(Of T)(list As List(Of T), ParamArray items As T())
        For Each i As T In items
            list.Add(i)
        Next
    End Sub
    <System.Runtime.CompilerServices.Extension>
    Public Sub AddRange(Of T)(list As List(Of T), items As IEnumerable(Of T))
        For Each i As T In items
            list.Add(i)
        Next
    End Sub
End Module


Friend NotInheritable Class BitFlag
    Private Sub New()
    End Sub
    Friend Shared Function IsSet(bits As Integer, flag As Integer) As Boolean
        Return (bits And flag) = flag
    End Function
    Friend Shared Function IsSet(bits As UInteger, flag As UInteger) As Boolean
        Return (bits And flag) = flag
    End Function
End Class

Public Module DllLoader

    <DllImport("kernel32", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Private Function LoadLibrary(lpFileName As String) As IntPtr
    End Function

    ''' <summary>
    ''' http://stackoverflow.com/questions/666799/embedding-unmanaged-dll-into-a-managed-c-sharp-dll
    ''' </summary>
    Public Sub Load()
        ' Get a temporary directory in which we can store the unmanaged DLL, with
        ' this assembly's version number in the path in order to avoid version
        ' conflicts in case two applications are running at once with different versions
        Dim dirName As String = Path.Combine(Path.GetTempPath(), "zlibnet-zlib" & ZLibDll.ZLibDllFileVersion)

        Try
            If Not Directory.Exists(dirName) Then
                Directory.CreateDirectory(dirName)
            End If
        Catch
            ' raced?
            If Not Directory.Exists(dirName) Then
                Throw
            End If
        End Try

        Dim dllName As String = ZLibDll.GetDllName()
        Dim dllFullName As String = Path.Combine(dirName, dllName)

        ' Get the embedded resource stream that holds the Internal DLL in this assembly.
        ' The name looks funny because it must be the default namespace of this project
        ' (MyAssembly.) plus the name of the Properties subdirectory where the
        ' embedded resource resides (Properties.) plus the name of the file.
        If Not File.Exists(dllFullName) Then
            ' Copy the assembly to the temporary file
            Dim tempFile As String = Path.GetTempFileName()
            Using stm As Stream = New MemoryStream(If(ZLibDll.Is64, My.Resources.zlib64, My.Resources.zlib32))
                Using outFile As Stream = File.Create(tempFile)
                    stm.CopyTo(outFile)
                End Using
            End Using

            Try
                File.Move(tempFile, dllFullName)
            Catch
                ' clean up tempfile
                Try
                    File.Delete(tempFile)
                    ' eat
                Catch
                End Try

                ' raced?
                If Not File.Exists(dllFullName) Then
                    Throw
                End If

            End Try
        End If

        ' We must explicitly load the DLL here because the temporary directory is not in the PATH.
        ' Once it is loaded, the DllImport directives that use the DLL will use the one that is already loaded into the process.
        Dim hFile As IntPtr = LoadLibrary(dllFullName)
        If hFile = IntPtr.Zero Then
            Throw New Exception("Can't load " & dllFullName)
        End If
    End Sub
End Module
