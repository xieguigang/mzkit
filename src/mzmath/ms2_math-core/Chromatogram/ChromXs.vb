Namespace Chromatogram

    Public Class ChromXs : Implements IChromXs

        Public Property InnerRT As IChromX
            Get
                Return RT
            End Get
            Set(value As IChromX)
                RT = CType(value, RetentionTime)
            End Set
        End Property

        Public Property InnerRI As IChromX
            Get
                Return RI
            End Get
            Set(value As IChromX)
                RI = CType(value, RetentionIndex)
            End Set
        End Property

        Public Property InnerDrift As IChromX
            Get
                Return Drift
            End Get
            Set(value As IChromX)
                Drift = CType(value, DriftTime)
            End Set
        End Property

        Public Property InnerMz As IChromX
            Get
                Return Mz
            End Get
            Set(value As IChromX)
                Mz = CType(value, MzValue)
            End Set
        End Property


        Public Property RT As RetentionTime Implements IChromXs.RT


        Public Property RI As RetentionIndex Implements IChromXs.RI


        Public Property Drift As DriftTime Implements IChromXs.Drift


        Public Property Mz As MzValue Implements IChromXs.Mz


        Public Property MainType As ChromXType = ChromXType.RT Implements IChromXs.MainType

        Public Sub New(innerRT As IChromX, innerRI As IChromX, innerDrift As IChromX, innerMz As IChromX, mainType As ChromXType)
            Me.InnerRT = innerRT
            Me.InnerRI = innerRI
            Me.InnerDrift = innerDrift
            Me.InnerMz = innerMz
            Me.MainType = mainType
        End Sub

        Public Sub New()
            RT = RetentionTime.Default
            RI = RetentionIndex.Default
            Drift = DriftTime.Default
            Mz = MzValue.Default
            MainType = ChromXType.RT
        End Sub

        Public Sub New(value As Double, Optional type As ChromXType = ChromXType.RT, Optional unit As ChromXUnit = ChromXUnit.Min)
            RT = RetentionTime.Default
            RI = RetentionIndex.Default
            Drift = DriftTime.Default
            Mz = MzValue.Default
            Select Case type
                Case ChromXType.RT
                    RT = New RetentionTime(value, unit)
                Case ChromXType.RI
                    RI = New RetentionIndex(value, unit)
                Case ChromXType.Drift
                    Drift = New DriftTime(value, unit)
                Case ChromXType.Mz
                    Mz = New MzValue(value, unit)
                Case Else
            End Select
            MainType = type
        End Sub

        Public Sub New(chromX As IChromX)
            RT = RetentionTime.Default
            RI = RetentionIndex.Default
            Drift = DriftTime.Default
            Mz = MzValue.Default
            Select Case chromX.GetType
                Case GetType(RetentionTime)
                    Me.RT = chromX

                Case GetType(RetentionIndex)
                    Me.RI = chromX

                Case GetType(DriftTime)
                    Drift = chromX
                Case GetType(MzValue)
                    Me.Mz = chromX
                Case Else
                    ' do nothing
            End Select

            MainType = chromX.Type
        End Sub

        Public Sub New(rt As RetentionTime, ri As RetentionIndex, dt As DriftTime, mz As MzValue, type As ChromXType)
            Me.RT = rt
            Me.RI = ri
            Drift = dt
            Me.Mz = mz
            MainType = type
        End Sub

        Public Sub New(retentionTime As RetentionTime)
            Me.New(retentionTime, RetentionIndex.Default, DriftTime.Default, MzValue.Default, retentionTime.Type)
        End Sub

        Public Sub New(retentionIndex As RetentionIndex)
            Me.New(RetentionTime.Default, retentionIndex, DriftTime.Default, MzValue.Default, retentionIndex.Type)

        End Sub

        Public Sub New(driftTime As DriftTime)
            Me.New(RetentionTime.Default, RetentionIndex.Default, driftTime, MzValue.Default, driftTime.Type)

        End Sub

        Public Sub New(mz As MzValue)
            Me.New(RetentionTime.Default, RetentionIndex.Default, DriftTime.Default, mz, mz.Type)

        End Sub

        Public Function GetRepresentativeXAxis() As IChromX
            Select Case MainType
                Case ChromXType.RT
                    Return RT
                Case ChromXType.RI
                    Return RI
                Case ChromXType.Drift
                    Return Drift
                Case ChromXType.Mz
                    Return Mz
                Case Else
                    Return Nothing
            End Select
        End Function

        Public ReadOnly Property Value As Double
            Get
                Select Case MainType
                    Case ChromXType.RT
                        Return If(RT?.Value, -1)
                    Case ChromXType.RI
                        Return If(RI?.Value, -1)
                    Case ChromXType.Drift
                        Return If(Drift?.Value, -1)
                    Case ChromXType.Mz
                        Return If(Mz?.Value, -1)
                    Case Else
                        Return -1
                End Select
            End Get
        End Property

        Public ReadOnly Property Unit As ChromXUnit
            Get
                Select Case MainType
                    Case ChromXType.RT
                        Return If(RT?.Unit, ChromXUnit.None)
                    Case ChromXType.RI
                        Return If(RI?.Unit, ChromXUnit.None)
                    Case ChromXType.Drift
                        Return If(Drift?.Unit, ChromXUnit.None)
                    Case ChromXType.Mz
                        Return If(Mz?.Unit, ChromXUnit.None)
                    Case Else
                        Return ChromXUnit.None
                End Select
            End Get
        End Property

        Public ReadOnly Property Type As ChromXType
            Get
                Return MainType
            End Get
        End Property

        Public Function GetChromByType(type As ChromXType) As IChromX
            Select Case type
                Case ChromXType.RT
                    Return RT
                Case ChromXType.RI
                    Return RI
                Case ChromXType.Drift
                    Return Drift
                Case ChromXType.Mz
                    Return Mz
                Case Else
                    Throw New ArgumentException(NameOf(type))
            End Select
        End Function

        Public Function HasTimeInfo() As Boolean
            If RT Is Nothing AndAlso RI Is Nothing AndAlso Drift Is Nothing Then Return False
            If RT.Value < 0 AndAlso RI.Value < 0 AndAlso Drift.Value < 0 Then Return False
            Return True
        End Function

        Public Function HasAbsolute() As Boolean
            If RT Is Nothing Then Return False
            If RT.Value < 0 Then Return False
            Return True
        End Function

        Public Function HasRelative() As Boolean
            If RI Is Nothing Then Return False
            If RI.Value < 0 Then Return False
            Return True
        End Function

        Public Function HasDrift() As Boolean
            If Drift Is Nothing Then Return False
            If Drift.Value < 0 Then Return False
            Return True
        End Function

        Public Shared Function Create(Of T As IChromX)(time As T) As ChromXs
            Return ChromXBuilder(Of T).action(time)
        End Function

        Friend NotInheritable Class ChromXBuilder(Of T As IChromX)
            Public Shared ReadOnly action As Func(Of T, ChromXs)

            Shared Sub New()
                If GetType(T) Is GetType(RetentionTime) Then
                    action = Function(chrom) New ChromXs(TryCast(chrom, RetentionTime))
                ElseIf GetType(T) Is GetType(RetentionIndex) Then
                    action = Function(chrom) New ChromXs(TryCast(chrom, RetentionIndex))
                ElseIf GetType(T) Is GetType(DriftTime) Then
                    action = Function(chrom) New ChromXs(TryCast(chrom, DriftTime))
                ElseIf GetType(T) Is GetType(MzValue) Then
                    action = Function(chrom) New ChromXs(TryCast(chrom, MzValue))
                Else
                    action = Function(chrom) New ChromXs(chrom)
                End If
            End Sub
        End Class
    End Class
End Namespace
