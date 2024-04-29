#Region "Microsoft.VisualBasic::2e034050c18daf7ef900b0e7ab4617b0, E:/mzkit/src/assembly/LoadR.NET5//ChromatogramOverlap.vb"

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

    '   Total Lines: 113
    '    Code Lines: 84
    ' Comment Lines: 8
    '   Blank Lines: 21
    '     File Size: 3.76 KB


    ' Class ChromatogramOverlap
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: (+2 Overloads) getByName, getNames, hasName, (+2 Overloads) setByName, setNames
    ' 
    ' /********************************************************************************/

#End Region

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
