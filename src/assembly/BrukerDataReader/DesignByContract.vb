Imports System
Imports System.Diagnostics

' ReSharper disable UnusedMember.Global
Namespace BrukerDataReader
    <AttributeUsage(AttributeTargets.All)>
    Public Class CoverageExcludeAttribute
        Inherits Attribute
    End Class

    <CoverageExclude>
    Public Module Check
        ''' <summary>
        ''' Precondition check - should run regardless of preprocessor directives.
        ''' </summary>
        Public Sub Require(ByVal assertion As Boolean, ByVal message As String)
            If UseExceptions Then
                If Not assertion Then Throw New PreconditionException(message)
            Else
                Trace.Assert(assertion, "Precondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Precondition check - should run regardless of preprocessor directives.
        ''' </summary>
        Public Sub Require(ByVal assertion As Boolean, ByVal message As String, ByVal inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New PreconditionException(message, inner)
            Else
                Trace.Assert(assertion, "Precondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Precondition check - should run regardless of preprocessor directives.
        ''' </summary>
        Public Sub Require(ByVal assertion As Boolean)
            If UseExceptions Then
                If Not assertion Then Throw New PreconditionException("Precondition failed.")
            Else
                Trace.Assert(assertion, "Precondition failed.")
            End If
        End Sub

        ''' <summary>
        ''' Postcondition check.
        ''' </summary>
        Public Sub Ensure(ByVal assertion As Boolean, ByVal message As String)
            If UseExceptions Then
                If Not assertion Then Throw New PostconditionException(message)
            Else
                Trace.Assert(assertion, "Postcondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Postcondition check.
        ''' </summary>
        Public Sub Ensure(ByVal assertion As Boolean, ByVal message As String, ByVal inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New PostconditionException(message, inner)
            Else
                Trace.Assert(assertion, "Postcondition: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Postcondition check.
        ''' </summary>
        Public Sub Ensure(ByVal assertion As Boolean)
            If UseExceptions Then
                If Not assertion Then Throw New PostconditionException("Postcondition failed.")
            Else
                Trace.Assert(assertion, "Postcondition failed.")
            End If
        End Sub

        ''' <summary>
        ''' Invariant check.
        ''' </summary>
        Public Sub Invariant(ByVal assertion As Boolean, ByVal message As String)
            If UseExceptions Then
                If Not assertion Then Throw New InvariantException(message)
            Else
                Trace.Assert(assertion, "Invariant: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Invariant check.
        ''' </summary>
        Public Sub Invariant(ByVal assertion As Boolean, ByVal message As String, ByVal inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New InvariantException(message, inner)
            Else
                Trace.Assert(assertion, "Invariant: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Invariant check.
        ''' </summary>
        Public Sub Invariant(ByVal assertion As Boolean)
            If UseExceptions Then
                If Not assertion Then Throw New InvariantException("Invariant failed.")
            Else
                Trace.Assert(assertion, "Invariant failed.")
            End If
        End Sub

        ''' <summary>
        ''' Assertion check.
        ''' </summary>
        Public Sub Assert(ByVal assertion As Boolean, ByVal message As String)
            If UseExceptions Then
                If Not assertion Then Throw New AssertionException(message)
            Else
                Trace.Assert(assertion, "Assertion: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Assertion check.
        ''' </summary>
        Public Sub Assert(ByVal assertion As Boolean, ByVal message As String, ByVal inner As Exception)
            If UseExceptions Then
                If Not assertion Then Throw New AssertionException(message, inner)
            Else
                Trace.Assert(assertion, "Assertion: " & message)
            End If
        End Sub

        ''' <summary>
        ''' Assertion check.
        ''' </summary>
        Public Sub Assert(ByVal assertion As Boolean)
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

    ''' <summary>
    ''' Exception raised when a contract is broken.
    ''' Catch this exception type if you wish to differentiate between
    ''' any DesignByContract exception and other runtime exceptions.
    '''
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class DesignByContractException
        Inherits ApplicationException

        Protected Sub New()
        End Sub

        Protected Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Protected Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class

    ''' <summary>
    ''' Exception raised when a precondition fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class PreconditionException
        Inherits DesignByContractException
        ''' <summary>
        ''' Precondition Exception.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Precondition Exception.
        ''' </summary>
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        ''' Precondition Exception.
        ''' </summary>
        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class

    ''' <summary>
    ''' Exception raised when a postcondition fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class PostconditionException
        Inherits DesignByContractException
        ''' <summary>
        ''' Postcondition Exception.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Postcondition Exception.
        ''' </summary>
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        ''' Postcondition Exception.
        ''' </summary>
        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class

    ''' <summary>
    ''' Exception raised when an invariant fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class InvariantException
        Inherits DesignByContractException
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New()
        End Sub
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class

    ''' <summary>
    ''' Exception raised when an assertion fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class AssertionException
        Inherits DesignByContractException
        ''' <summary>
        ''' Assertion Exception.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Assertion Exception.
        ''' </summary>
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        ''' Assertion Exception.
        ''' </summary>
        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class
End Namespace
