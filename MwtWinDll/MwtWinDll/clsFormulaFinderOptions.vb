Public Class clsFormulaFinderOptions

#Region "Contants and Enums"
    Public Enum eSearchMode
        Thorough = 0
        Bounded = 1
    End Enum
#End Region

#Region "Member Variables"
    Private mFindCharge As Boolean
    Private mLimitChargeRange As Boolean
    Private mFindTargetMZ As Boolean
#End Region

#Region "Properties"

    ''' <summary>
    ''' When true, compute the overall charge of each compound
    ''' </summary>
    ''' <remarks></remarks>
    Public Property FindCharge As Boolean
        Get
            Return mFindCharge
        End Get
        Set(value As Boolean)
            mFindCharge = value

            If mFindCharge = False Then
                ' Auto-disable a few options
                mLimitChargeRange = False
                mFindTargetMZ = False
            End If

        End Set
    End Property

    ''' <summary>
    ''' When true, filter the results by ChargeMin and ChargeMax
    ''' </summary>
    ''' <remarks>
    ''' Setting this to True auto-sets FindCharge to true
    ''' Setting this to False auto-sets FindTargetMZ to false</remarks>
    Public Property LimitChargeRange As Boolean
        Get
            Return mLimitChargeRange
        End Get
        Set(value As Boolean)
            mLimitChargeRange = value
            If mLimitChargeRange Then
                FindCharge = True
            Else
                mFindTargetMZ = False
            End If
        End Set
    End Property

    ''' <summary>
    ''' When LimitChargeRange is true, results will be limited to the range ChargeMin to ChargeMax
    ''' </summary>
    ''' <remarks>Negative values are allowed</remarks>
    Public Property ChargeMin As Integer

    ''' <summary>
    ''' When LimitChargeRange is true, results will be limited to the range ChargeMin to ChargeMax
    ''' </summary>
    ''' <remarks>Negative values are allowed</remarks>
    Public Property ChargeMax As Integer

    ''' <summary>
    ''' Set to true to search for a target m/z value instead of a target mass
    ''' </summary>
    ''' <remarks>Setting this to True auto-sets FindCharge and LimitChargeRange to True</remarks>
    Public Property FindTargetMZ As Boolean
        Get
            Return mFindTargetMZ
        End Get
        Set(value As Boolean)
            mFindTargetMZ = value
            If (mFindTargetMZ) Then
                FindCharge = True
                LimitChargeRange = True
            End If
        End Set
    End Property

    Public Property SearchMode As eSearchMode

    Public Property VerifyHydrogens As Boolean

#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        FindCharge = True
        LimitChargeRange = False
        ChargeMin = -4
        ChargeMax = 4
        FindTargetMZ = False
        SearchMode = eSearchMode.Thorough
        VerifyHydrogens = True
    End Sub
End Class
