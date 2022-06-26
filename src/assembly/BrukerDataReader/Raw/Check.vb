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