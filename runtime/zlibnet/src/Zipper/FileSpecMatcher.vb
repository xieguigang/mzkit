Imports System.Collections.Generic
Imports System.Text
Imports System.IO

Friend Class FileSpecMatcher
	Private Class FileSpec
		Public FileName As String
		Public DirName As String

		Public Sub New(fileSpec__1 As String, recurseFiles As Boolean)
			DirName = Path.GetDirectoryName(fileSpec__1)
			If DirName Is Nothing Then
				Throw New ArgumentException("invalid fileSpec")
			End If

			FileName = Path.GetFileName(fileSpec__1)
			If FileName Is Nothing Then
				Throw New ArgumentException("invalid fileSpec")
			End If

			DirName = DirName.SetEndDirSep().TrimStartDirSep()

			If recurseFiles AndAlso FileName.Length > 0 Then
				DirName += "*"
			End If
		End Sub
	End Class

	Private pFileSpecs As New List(Of FileSpec)()

	''' <summary>
	''' if true, the fileName in spec matches any file in any subdirs of the matched dir.
	''' if false, spec matches one exact dir + file
	''' </summary>
	Public Sub New(specs As List(Of String), recurseFiles As Boolean)
		For Each spec As String In specs
			pFileSpecs.Add(New FileSpec(spec, recurseFiles))
		Next
	End Sub

	Public Function MatchSpecs(entryName As String, entryIsDir As Boolean) As Boolean
		If entryIsDir Then
			entryName = entryName.SetEndDirSep()
		End If

		Dim entryDirName As String = Path.GetDirectoryName(entryName)
		'will create a backslashed name
		Dim entryFileName As String = Path.GetFileName(entryName)

		'trimStart: dont want to make an empty string \
		'also, if someone sends paths that start with \, well get rid of them leading \
		'also, we never want an entry to start with \ (scary!)
		entryDirName = entryDirName.SetEndDirSep().TrimStartDirSep()

		For Each fileSpec As FileSpec In pFileSpecs
			Dim fileMatch As Boolean = entryFileName.WildcardMatch(fileSpec.FileName, True)
			Dim dirMath As Boolean = entryDirName.WildcardMatch(fileSpec.DirName, True)

			If dirMath AndAlso fileMatch Then
				Return True
			End If
		Next

		Return False
	End Function
End Class
