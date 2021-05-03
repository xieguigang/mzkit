Namespace DataObjects

    ''' <summary>
    ''' Type for storing Parent Ion Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure ParentIonInfoType
        ''' <summary>
        ''' MS Level of the spectrum
        ''' </summary>
        ''' <remarks>1 for MS1 spectra, 2 for MS2, 3 for MS3</remarks>
        Public MSLevel As Integer

        ''' <summary>
        ''' Parent ion m/z
        ''' </summary>
        Public ParentIonMZ As Double

        ''' <summary>
        ''' Collision mode
        ''' </summary>
        ''' <remarks>Examples: cid, etd, hcd, EThcD, ETciD</remarks>
        Public CollisionMode As String

        ''' <summary>
        ''' Secondary collision mode
        ''' </summary>
        ''' <remarks>
        ''' For example, for filter string: ITMS + c NSI r d sa Full ms2 1143.72@etd120.55@cid20.00 [120.00-2000.00]
        ''' CollisionMode = ETciD
        ''' CollisionMode2 = cid
        ''' </remarks>
        Public CollisionMode2 As String

        ''' <summary>
        ''' Collision energy
        ''' </summary>
        Public CollisionEnergy As Single

        ''' <summary>
        ''' Secondary collision energy
        ''' </summary>
        ''' <remarks>
        ''' For example, for filter string: ITMS + c NSI r d sa Full ms2 1143.72@etd120.55@cid20.00 [120.00-2000.00]
        ''' CollisionEnergy = 120.55
        ''' CollisionEnergy2 = 20.0
        ''' </remarks>
        Public CollisionEnergy2 As Single

        ''' <summary>
        ''' Activation type
        ''' </summary>
        ''' <remarks>Examples: CID, ETD, or HCD</remarks>
        Public ActivationType As ActivationMethods

        ''' <summary>
        ''' Clear the data
        ''' </summary>
        Public Sub Clear()
            MSLevel = 1
            ParentIonMZ = 0
            CollisionMode = String.Empty
            CollisionMode2 = String.Empty
            CollisionEnergy = 0
            CollisionEnergy2 = 0
            ActivationType = ActivationMethods.Unknown
        End Sub

        ''' <summary>
        ''' Return a simple summary of the object
        ''' </summary>
        Public Overrides Function ToString() As String
            If String.IsNullOrWhiteSpace(CollisionMode) Then
                Return "ms" & MSLevel & " " & ParentIonMZ.ToString("0.0#")
            End If

            Return "ms" & MSLevel & " " & ParentIonMZ.ToString("0.0#") & "@" & CollisionMode & CollisionEnergy.ToString("0.00")
        End Function
    End Structure

End Namespace