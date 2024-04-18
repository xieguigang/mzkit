Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.Object

Public Class ChromatogramOverlap : Inherits ChromatogramOverlapList
    Implements RNames, RNameIndex

    Sub New()
    End Sub

    Sub New(list_set As Dictionary(Of String, Chromatogram))
        overlaps = New Dictionary(Of String, Chromatogram)(list_set)
    End Sub

    Public Function setNames(names() As String, envir As Environment) As Object Implements RNames.setNames
        Dim renames As New Dictionary(Of String, Chromatogram)
        Dim oldNames As String() = overlaps.Keys.ToArray

        For i As Integer = 0 To names.Length - 1
            renames(names(i)) = overlaps(oldNames(i))
        Next

        overlaps = renames

        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function hasName(name As String) As Boolean Implements RNames.hasName
        Return overlaps.ContainsKey(name)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getNames() As String() Implements IReflector.getNames
        Return overlaps.Keys.ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns>
    ''' <see cref="Chromatogram"/>
    ''' </returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getByName(name As String) As Object Implements RNameIndex.getByName
        Return overlaps.TryGetValue(name)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getByName(names() As String) As Object Implements RNameIndex.getByName
        Return New List With {
            .slots = names _
                .ToDictionary(Function(name) name,
                              Function(name)
                                  Return getByName(name)
                              End Function)
        }
    End Function

    Public Function setByName(name As String, value As Object, envir As Environment) As Object Implements RNameIndex.setByName
        If value Is Nothing Then
            Call overlaps.Remove(name)
            Return Nothing
        End If
        If Not TypeOf value Is Chromatogram Then
            Return message.InCompatibleType(GetType(Chromatogram), value.GetType, envir)
        Else
            overlaps(name) = value
        End If

        Return value
    End Function

    Public Function setByName(names() As String, value As Array, envir As Environment) As Object Implements RNameIndex.setByName
        Dim result As Object

        If value.IsNullOrEmpty Then
            For Each name As String In names
                Call overlaps.Remove(name)
            Next

            Return Nothing
        ElseIf value.Length = 1 Then
            Dim scalar = value.GetValue(0)

            For Each name As String In names
                result = setByName(name, scalar, envir)

                If Program.isException(result) Then
                    Return result
                End If
            Next

            Return scalar
        Else
            For i As Integer = 0 To names.Length - 1
                result = setByName(names(i), value.GetValue(i), envir)

                If Program.isException(result) Then
                    Return result
                End If
            Next

            Return value
        End If
    End Function
End Class
