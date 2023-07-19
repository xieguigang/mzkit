Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PackLib

    ''' <summary>
    ''' A metabolite its spectrum data index
    ''' </summary>
    ''' <remarks>
    ''' A tree node of a specific metabolite reference to multiple spectrum data
    ''' </remarks>
    Public Class MassIndex

        ''' <summary>
        ''' the unique reference of current metabolite spectrum cluster
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        Public Property exactMass As Double
        Public Property formula As String
        ''' <summary>
        ''' the pointer to the spectrum data in the library file
        ''' </summary>
        ''' <returns></returns>
        Public Property spectrum As New List(Of Integer)

        ''' <summary>
        ''' the number of the spectrum that associated with current
        ''' metabolite <see cref="name"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                Return spectrum.Count
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace