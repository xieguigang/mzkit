#Region "Microsoft.VisualBasic::f00f0f31ddb332fe034d3056e50cac48, E:/mzkit/src/assembly/BrukerDataReader//Raw/Check.vb"

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

    '   Total Lines: 157
    '    Code Lines: 97
    ' Comment Lines: 44
    '   Blank Lines: 16
    '     File Size: 5.54 KB


    '     Module Check
    ' 
    '         Properties: UseAssertions, UseExceptions
    ' 
    '         Sub: (+3 Overloads) Assert, (+3 Overloads) Ensure, (+3 Overloads) Invariant, (+3 Overloads) Require
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.Diagnostics

Namespace Raw

    <CoverageExclude>
    Public Module Check
        ''' <summary>
        ''' Precondition check - should run regardless of preprocessor directives.
        ''' </summary>
        Public Sub Require(assertion As Boolean, message As String)
            If UseExceptions Then
                If Not assertion Then Throw New PreconditionException(message)
            Else
                Trace.Assert(assertion, "Precondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Precondition check - should run regardless of preprocessor directives.
        ''' </summary>
        Public Sub Require(assertion As Boolean, message As String, inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New PreconditionException(message, inner)
            Else
                Trace.Assert(assertion, "Precondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Precondition check - should run regardless of preprocessor directives.
        ''' </summary>
        Public Sub Require(assertion As Boolean)
            If UseExceptions Then
                If Not assertion Then Throw New PreconditionException("Precondition failed.")
            Else
                Trace.Assert(assertion, "Precondition failed.")
            End If
        End Sub

        ''' <summary>
        ''' Postcondition check.
        ''' </summary>
        Public Sub Ensure(assertion As Boolean, message As String)
            If UseExceptions Then
                If Not assertion Then Throw New PostconditionException(message)
            Else
                Trace.Assert(assertion, "Postcondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Postcondition check.
        ''' </summary>
        Public Sub Ensure(assertion As Boolean, message As String, inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New PostconditionException(message, inner)
            Else
                Trace.Assert(assertion, "Postcondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Postcondition check.
        ''' </summary>
        Public Sub Ensure(assertion As Boolean)
            If UseExceptions Then
                If Not assertion Then Throw New PostconditionException("Postcondition failed.")
            Else
                Trace.Assert(assertion, "Postcondition failed.")
            End If
        End Sub

        ''' <summary>
        ''' Invariant check.
        ''' </summary>
        Public Sub Invariant(assertion As Boolean, message As String)
            If UseExceptions Then
                If Not assertion Then Throw New InvariantException(message)
            Else
                Trace.Assert(assertion, "Invariant: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Invariant check.
        ''' </summary>
        Public Sub Invariant(assertion As Boolean, message As String, inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New InvariantException(message, inner)
            Else
                Trace.Assert(assertion, "Invariant: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Invariant check.
        ''' </summary>
        Public Sub Invariant(assertion As Boolean)
            If UseExceptions Then
                If Not assertion Then Throw New InvariantException("Invariant failed.")
            Else
                Trace.Assert(assertion, "Invariant failed.")
            End If
        End Sub

        ''' <summary>
        ''' Assertion check.
        ''' </summary>
        Public Sub Assert(assertion As Boolean, message As String)
            If UseExceptions Then
                If Not assertion Then Throw New AssertionException(message)
            Else
                Trace.Assert(assertion, "Assertion: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Assertion check.
        ''' </summary>
        Public Sub Assert(assertion As Boolean, message As String, inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New AssertionException(message, inner)
            Else
                Trace.Assert(assertion, "Assertion: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Assertion check.
        ''' </summary>
        Public Sub Assert(assertion As Boolean)
            If UseExceptions Then
                If Not assertion Then Throw New AssertionException("Assertion failed.")
            Else
                Trace.Assert(assertion, "Assertion failed.")
            End If
        End Sub

        ''' <summary>
        ''' Set this if you wish to use Trace Assert statements
        ''' instead of exception handling.
        ''' (The Check class uses exception handling by default.)
        ''' </summary>
        Public Property UseAssertions As Boolean = False

        ''' <summary>
        ''' Is exception handling being used?
        ''' </summary>
        Private ReadOnly Property UseExceptions As Boolean
            Get
                Return Not UseAssertions
            End Get
        End Property
    End Module

End Namespace
