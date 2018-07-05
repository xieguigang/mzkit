Namespace ASCII.MSP

    Public Class UnionReader

        ReadOnly meta As MetaData

        Public ReadOnly Property collision_energy As String
            Get
                Return meta.Read_collision_energy
            End Get
        End Property

        Public ReadOnly Property CAS As String
            Get
                Return meta.Read_CAS
            End Get
        End Property

        Public ReadOnly Property precursor_type As String
            Get
                Return meta.Read_precursor_type
            End Get
        End Property

        ''' <summary>
        ''' 单位已经统一为秒
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property retention_time As Double
            Get
                Return meta.read_retention_time
            End Get
        End Property

        Public ReadOnly Property instrument_type As String
            Get
                Return meta.Read_instrument_type
            End Get
        End Property

        Public ReadOnly Property exact_mass As Double
            Get
                Return meta.Read_exact_mass
            End Get
        End Property

        Sub New(meta As MetaData)
            Me.meta = meta
        End Sub

        Public Overrides Function ToString() As String
            Return meta.name
        End Function
    End Class
End Namespace