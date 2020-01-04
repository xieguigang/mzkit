#Region "Microsoft.VisualBasic::3ead28ef0714adf720cb663bc97d8169, src\mzmath\MwtWinDll\MwtWinDll\Formula\FormulaFinderOptions.vb"

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

    ' Class FormulaFinderOptions
    ' 
    ' 
    '     Enum eSearchMode
    ' 
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: ChargeMax, ChargeMin, FindCharge, FindTargetMZ, LimitChargeRange
    '                 SearchMode, VerifyHydrogens
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Public Class FormulaFinderOptions

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
