

Public Class MWCapillaryFlowClass

    ' Molecular Weight Calculator routines with ActiveX Class interfaces: MWCapillaryFlowClass

    ' -------------------------------------------------------------------------------
    ' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
    ' E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com
    ' Website: http://ncrr.pnnl.gov/ or http://www.sysbio.org/resources/staff/
    ' -------------------------------------------------------------------------------
    ' 
    ' Licensed under the Apache License, Version 2.0; you may not use this file except
    ' in compliance with the License.  You may obtain a copy of the License at 
    ' http://www.apache.org/licenses/LICENSE-2.0
    '
    ' Notice: This computer software was prepared by Battelle Memorial Institute, 
    ' hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the 
    ' Department of Energy (DOE).  All rights in the computer software are reserved 
    ' by DOE on behalf of the United States Government and the Contractor as 
    ' provided in the Contract.  NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY 
    ' WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS 
    ' SOFTWARE.  This notice including this sentence must appear on any copies of 
    ' this computer software.

    Public Sub New()
        MyBase.New()
        InitializeClass()
    End Sub

    ' <EnumStatements>
    Public Enum ctCapillaryTypeConstants
        ctOpenTubularCapillary = 0
        ctPackedCapillary
    End Enum

    Public Enum uprUnitsPressureConstants
        uprPsi = 0
        uprPascals
        uprKiloPascals
        uprAtmospheres
        uprBar
        uprTorr
        uprDynesPerSquareCm
    End Enum

    Public Enum ulnUnitsLengthConstants
        ulnM = 0
        ulnCM
        ulnMM
        ulnMicrons
        ulnInches
    End Enum

    Public Enum uviUnitsViscosityConstants
        uviPoise = 0
        uviCentiPoise
    End Enum

    Public Enum ufrUnitsFlowRateConstants
        ufrMLPerMin = 0
        ufrULPerMin
        ufrNLPerMin
    End Enum

    Public Enum ulvUnitsLinearVelocityConstants
        ulvCmPerHr = 0
        ulvMmPerHr
        ulvCmPerMin
        ulvMmPerMin
        ulvCmPerSec
        ulvMmPerSec
    End Enum

    Public Enum utmUnitsTimeConstants
        utmHours = 0
        utmMinutes
        utmSeconds
    End Enum

    Public Enum uvoUnitsVolumeConstants
        uvoML = 0
        uvoUL
        uvoNL
        uvoPL
    End Enum

    Public Enum ucoUnitsConcentrationConstants
        ucoMolar = 0
        ucoMilliMolar
        ucoMicroMolar
        ucoNanoMolar
        ucoPicoMolar
        ucoFemtoMolar
        ucoAttoMolar
        ucoMgPerML
        ucoUgPerML
        ucongperml
        ucoUgPerUL
        ucoNgPerUL
    End Enum

    Public Enum utpUnitsTemperatureConstants
        utpCelsius = 0
        utpKelvin
        utpFahrenheit
    End Enum

    Public Enum umfMassFlowRateConstants
        umfPmolPerMin = 0
        umfFmolPerMin
        umfAmolPerMin
        umfPmolPerSec
        umfFmolPerSec
        umfAmolPerSec
        umfMolesPerMin
    End Enum

    Public Enum umaMolarAmountConstants
        umaMoles = 0
        umaMilliMoles
        umaMicroMoles
        umaNanoMoles
        umaPicoMoles
        umaFemtoMoles
        umaAttoMoles
    End Enum

    Public Enum udcDiffusionCoefficientConstants
        udcCmSquaredPerHr = 0
        udcCmSquaredPerMin
        udcCmSquaredPerSec
    End Enum

    Public Enum acmAutoComputeModeConstants
        acmBackPressure = 0
        acmColumnID
        acmColumnLength
        acmDeadTime
        acmLinearVelociy
        acmVolFlowrate
        acmVolFlowrateUsingDeadTime
    End Enum
    ' </EnumStatements>

    ' <UDT's>
    Private Structure udtCapillaryFlowParametersType
        Dim CapillaryType As ctCapillaryTypeConstants
        Dim BackPressure As Double ' in dynes/cm^2
        Dim ColumnLength As Double ' in cm
        Dim ColumnID As Double ' in cm
        Dim SolventViscosity As Double ' in poise
        Dim ParticleDiameter As Double ' in cm
        Dim VolumetricFlowRate As Double ' in mL/min
        Dim LinearVelocity As Double ' cm/min
        Dim ColumnDeadTime As Double ' in min
        Dim InterparticlePorosity As Double
    End Structure

    Private Structure udtMassRateParametersType
        Dim SampleConcentration As Double ' in Molar
        Dim SampleMass As Double ' in g/mole
        Dim VolumetricFlowRate As Double ' in mL/min
        Dim InjectionTime As Double ' in min
        Dim MassFlowRate As Double ' in Moles/min
        Dim MolesInjected As Double ' in moles
    End Structure

    Private Structure udtExtraColumnBroadeningParametersType
        Dim LinearVelocity As Double ' in cm/min
        Dim DiffusionCoefficient As Double ' in cm^2/sec
        Dim OpenTubeLength As Double ' in cm
        Dim OpenTubeID As Double ' in cm
        Dim InitialPeakWidth As Double ' in sec
        Dim TemporalVariance As Double ' in sec^2
        Dim AdditionalTemporalVariance As Double ' in sec^2
        Dim ResultantPeakWidth As Double ' in sec
    End Structure
    ' </UDT's>

    ' Conversion Factors
    Private Const CM_PER_INCH As Single = 2.54
    Private Const PI As Double = 3.14159265359

    Private mCapillaryFlowParameters As udtCapillaryFlowParametersType
    Private mMassRateParameters As udtMassRateParametersType
    Private mExtraColumnBroadeningParameters As udtExtraColumnBroadeningParametersType

    Private mAutoCompute As Boolean ' When true, automatically compute results whenever any value changes
    Private mAutoComputeMode As acmAutoComputeModeConstants ' The value to compute when mAutoCompute is true

    Private Sub CheckAutoCompute()
        If mAutoCompute Then
            Select Case mAutoComputeMode
                Case acmAutoComputeModeConstants.acmBackPressure : ComputeBackPressure()
                Case acmAutoComputeModeConstants.acmColumnID : ComputeColumnID()
                Case acmAutoComputeModeConstants.acmColumnLength : ComputeColumnLength()
                Case acmAutoComputeModeConstants.acmDeadTime : ComputeDeadTime()
                Case acmAutoComputeModeConstants.acmLinearVelociy : ComputeLinearVelocity()
                Case acmAutoComputeModeConstants.acmVolFlowrateUsingDeadTime : ComputeVolFlowRateUsingDeadTime()
                Case Else
                    ' Includes acmVolFlowRate
                    ComputeVolFlowRate()
            End Select
        End If
    End Sub

    Public Function ComputeBackPressure(Optional ByRef eUnits As uprUnitsPressureConstants = uprUnitsPressureConstants.uprPsi) As Double
        ' Computes the back pressure, stores in .BackPressure, and returns it

        Dim dblBackPressure, dblRadius As Double

        With mCapillaryFlowParameters

            dblRadius = .ColumnID / 2.0#

            If Math.Abs(dblRadius) > Single.Epsilon Then
                If .CapillaryType = ctCapillaryTypeConstants.ctOpenTubularCapillary Then
                    ' Open tubular capillary
                    dblBackPressure = (.VolumetricFlowRate * 8 * .SolventViscosity * .ColumnLength) / (dblRadius ^ 4 * PI * 60) ' Pressure in dynes/cm^2
                Else
                    ' Packed capillary
                    If Math.Abs(.ParticleDiameter) > Single.Epsilon And Math.Abs(.InterparticlePorosity) > Single.Epsilon Then
                        ' Flow rate in mL/sec
                        dblBackPressure = (.VolumetricFlowRate * 180 * .SolventViscosity * .ColumnLength * (1 - .InterparticlePorosity) ^ 2) / (.ParticleDiameter ^ 2 * .InterparticlePorosity ^ 2 * PI * dblRadius ^ 2 * 60) / .InterparticlePorosity
                    Else
                        dblBackPressure = 0
                    End If
                End If
            Else
                dblBackPressure = 0
            End If

            .BackPressure = dblBackPressure
        End With

        ' Compute Dead Time (and Linear Velocity)
        ' Must send false for RecalculateVolFlowRate since we're finding the backpressure, not volumetric flow rate
        ComputeDeadTime(utmUnitsTimeConstants.utmMinutes, False)

        ' Return Back Pressure
        ComputeBackPressure = ConvertPressure(dblBackPressure, uprUnitsPressureConstants.uprDynesPerSquareCm, eUnits)

    End Function

    Public Function ComputeColumnLength(Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnCM) As Double
        ' Computes the column length, stores in .ColumnLength, and returns it

        Dim dblColumnLength, dblRadius As Double

        With mCapillaryFlowParameters

            dblRadius = .ColumnID / 2.0#

            If Math.Abs(.SolventViscosity) > Single.Epsilon And Math.Abs(.VolumetricFlowRate) > Single.Epsilon Then
                If .CapillaryType = ctCapillaryTypeConstants.ctOpenTubularCapillary Then
                    ' Open tubular capillary
                    dblColumnLength = (.BackPressure * dblRadius ^ 4 * PI * 60) / (8 * .SolventViscosity * .VolumetricFlowRate) ' Column length in cm
                Else
                    ' Packed capillary
                    If Math.Abs(.InterparticlePorosity - 1) > Single.Epsilon Then
                        ' Flow rate in mL/sec
                        dblColumnLength = (.BackPressure * .ParticleDiameter ^ 2 * .InterparticlePorosity ^ 2 * PI * dblRadius ^ 2 * 60) * .InterparticlePorosity / (180 * .SolventViscosity * .VolumetricFlowRate * (1 - .InterparticlePorosity) ^ 2)
                    Else
                        dblColumnLength = 0
                    End If
                End If
            Else
                dblColumnLength = 0
            End If

            .ColumnLength = dblColumnLength
        End With

        ' Compute Dead Time (and Linear Velocity)
        ComputeDeadTime(utmUnitsTimeConstants.utmMinutes, True)

        ' Return Column Length
        ComputeColumnLength = ConvertLength(dblColumnLength, ulnUnitsLengthConstants.ulnCM, eUnits)

    End Function

    Public Function ComputeColumnVolume(Optional ByRef eUnits As uvoUnitsVolumeConstants = 0) As Double
        ' Computes the column volume and returns it (does not store it)

        Dim dblColumnVolume, dblRadius As Double

        With mCapillaryFlowParameters

            dblRadius = .ColumnID / 2.0#

            dblColumnVolume = .ColumnLength * PI * dblRadius ^ 2 ' In mL

            If .CapillaryType = ctCapillaryTypeConstants.ctPackedCapillary Then
                dblColumnVolume = dblColumnVolume * .InterparticlePorosity
            End If
        End With

        ComputeColumnVolume = ConvertVolume(dblColumnVolume, uvoUnitsVolumeConstants.uvoML, eUnits)

    End Function

    Public Function ComputeColumnID(Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnMicrons) As Double
        ' Computes the column length, stores in .ColumnLength, and returns it

        Dim dblRadius As Double

        With mCapillaryFlowParameters

            If Math.Abs(.BackPressure) > Single.Epsilon Then
                If .CapillaryType = ctCapillaryTypeConstants.ctOpenTubularCapillary Then
                    ' Open tubular capillary
                    dblRadius = ((.VolumetricFlowRate * 8 * .SolventViscosity * .ColumnLength) / (.BackPressure * PI * 60)) ^ (0.25)
                Else
                    ' Packed capillary
                    If Math.Abs(.ParticleDiameter) > Single.Epsilon And Math.Abs(.InterparticlePorosity - 1) > Single.Epsilon Then
                        ' Flow rate in mL/sec
                        dblRadius = ((.VolumetricFlowRate * 180 * .SolventViscosity * .ColumnLength * (1 - .InterparticlePorosity) ^ 2) / (.BackPressure * .ParticleDiameter ^ 2 * .InterparticlePorosity ^ 2 * PI * 60) / .InterparticlePorosity) ^ 0.5
                    Else
                        dblRadius = 0
                    End If
                End If
            Else
                dblRadius = 0
            End If

            .ColumnID = dblRadius * 2.0#
        End With

        ' Compute Dead Time (and Linear Velocity)
        ComputeDeadTime(utmUnitsTimeConstants.utmMinutes, True)

        ' Return Column ID
        ComputeColumnID = ConvertLength(dblRadius * 2.0#, ulnUnitsLengthConstants.ulnCM, eUnits)

    End Function

    Public Function ComputeDeadTime(Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmMinutes, Optional ByRef blnRecalculateVolFlowRate As Boolean = True) As Double
        ' Computes the column dead time, stores in .ColumnDeadTime, and returns it

        Dim dblDeadTime As Double

        ' Dead time is dependent on Linear Velocity, so compute
        ComputeLinearVelocity(ulvUnitsLinearVelocityConstants.ulvCmPerSec, blnRecalculateVolFlowRate)

        With mCapillaryFlowParameters

            If Math.Abs(.LinearVelocity) > Single.Epsilon Then
                dblDeadTime = .ColumnLength / .LinearVelocity ' Dead time in minutes
            Else
                dblDeadTime = 0
            End If

            .ColumnDeadTime = dblDeadTime
        End With

        ' Return Dead Time
        ComputeDeadTime = ConvertTime(dblDeadTime, utmUnitsTimeConstants.utmMinutes, eUnits)

    End Function

    Public Function ComputeExtraColumnBroadeningResultantPeakWidth(Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmSeconds) As Double
        ComputeExtraColumnBroadeningValues()

        ComputeExtraColumnBroadeningResultantPeakWidth = GetExtraColumnBroadeningResultantPeakWidth(eUnits)
    End Function

    Private Sub ComputeExtraColumnBroadeningValues()
        Dim dblInitialPeakVariance As Double
        Dim dblSumOfVariances As Double

        With mExtraColumnBroadeningParameters
            If Math.Abs(.LinearVelocity) > Single.Epsilon And Math.Abs(.DiffusionCoefficient) > Single.Epsilon Then
                .TemporalVariance = .OpenTubeID ^ 2 * .OpenTubeLength / (96 * .DiffusionCoefficient * .LinearVelocity / 60) ' in sec^2
            Else
                .TemporalVariance = 0
            End If

            dblInitialPeakVariance = (.InitialPeakWidth / 4) ^ 2

            dblSumOfVariances = dblInitialPeakVariance + .TemporalVariance + .AdditionalTemporalVariance

            If dblSumOfVariances >= 0 Then
                ' ResultantPeakWidth at the base = 4 sigma  and  sigma = Sqr(Total_Variance)
                .ResultantPeakWidth = 4 * Math.Sqrt(dblSumOfVariances)
            Else
                .ResultantPeakWidth = 0
            End If
        End With
    End Sub

    Public Function ComputeLinearVelocity(Optional ByRef eUnits As ulvUnitsLinearVelocityConstants = ulvUnitsLinearVelocityConstants.ulvCmPerSec, Optional ByRef blnRecalculateVolFlowRate As Boolean = True) As Double
        ' Computes the Linear velocity, stores in .LinearVelocity, and returns it

        Dim dblLinearVelocity, dblRadius As Double

        If blnRecalculateVolFlowRate Then
            ComputeVolFlowRate(ufrUnitsFlowRateConstants.ufrMLPerMin)
        End If

        With mCapillaryFlowParameters
            dblRadius = .ColumnID / 2.0#
            If Math.Abs(dblRadius) > Single.Epsilon Then
                dblLinearVelocity = .VolumetricFlowRate / (PI * dblRadius ^ 2) ' Units in cm/min

                ' Divide Linear Velocity by epsilon if a packed capillary
                If .CapillaryType = ctCapillaryTypeConstants.ctPackedCapillary And Math.Abs(.InterparticlePorosity) > Single.Epsilon Then
                    dblLinearVelocity = dblLinearVelocity / .InterparticlePorosity
                End If
            Else
                dblLinearVelocity = 0
            End If

            .LinearVelocity = dblLinearVelocity
        End With

        ' Return Linear Velocity
        ComputeLinearVelocity = ConvertLinearVelocity(dblLinearVelocity, ulvUnitsLinearVelocityConstants.ulvCmPerMin, eUnits)

    End Function

    Public Function ComputeMassFlowRate(Optional ByRef eUnits As umfMassFlowRateConstants = umfMassFlowRateConstants.umfFmolPerSec) As Double
        ' Computes the MassFlowRate and Moles Injected, stores in .MassFlowRate and .MolesInjected, and returns MassFlowRate

        ComputeMassRateValues()
        ComputeMassFlowRate = GetMassFlowRate(eUnits)

    End Function

    Public Function ComputeMassRateMolesInjected(Optional ByRef eUnits As umaMolarAmountConstants = umaMolarAmountConstants.umaFemtoMoles) As Double
        ' Computes the MassFlowRate and Moles Injected, stores in .MassFlowRate and .MolesInjected, and returns MassFlowRate

        ComputeMassRateValues()
        ComputeMassRateMolesInjected = GetMassRateMolesInjected(eUnits)

    End Function

    Private Sub ComputeMassRateValues()

        With mMassRateParameters
            .MassFlowRate = .SampleConcentration * .VolumetricFlowRate / 1000 ' Compute mass flowrate in moles/min

            .MolesInjected = .MassFlowRate * .InjectionTime ' Compute moles injected in moles
        End With

    End Sub

    Public Function ComputeOptimumLinearVelocityUsingParticleDiamAndDiffusionCoeff(Optional ByRef eUnits As ulvUnitsLinearVelocityConstants = ulvUnitsLinearVelocityConstants.ulvCmPerSec) As Double
        ' Computes the optimum linear velocity, based on
        ' mCapillaryFlowParameters.ParticleDiameter
        ' and mExtraColumnBroadeningParameters.DiffusionCoefficient

        Dim dblOptimumLinearVelocity As Double

        With mCapillaryFlowParameters
            If Math.Abs(.ParticleDiameter) > Single.Epsilon Then
                dblOptimumLinearVelocity = 3 * mExtraColumnBroadeningParameters.DiffusionCoefficient / .ParticleDiameter

                dblOptimumLinearVelocity = ConvertLinearVelocity(dblOptimumLinearVelocity, ulvUnitsLinearVelocityConstants.ulvCmPerSec, eUnits)
            End If
        End With

        ComputeOptimumLinearVelocityUsingParticleDiamAndDiffusionCoeff = dblOptimumLinearVelocity

    End Function

    Public Function ComputeMeCNViscosity(ByRef dblPercentAcetonitrile As Double, ByRef dblTemperature As Double, Optional ByRef eTemperatureUnits As utpUnitsTemperatureConstants = utpUnitsTemperatureConstants.utpCelsius, Optional ByRef eViscosityUnits As uviUnitsViscosityConstants = uviUnitsViscosityConstants.uviPoise) As Double

        Dim dblPhi As Double ' Fraction Acetonitrile
        Dim dblKelvin As Double
        Dim dblViscosityInCentiPoise As Double

        Try
            dblPhi = dblPercentAcetonitrile / 100.0#
            If dblPhi < 0 Then dblPhi = 0
            If dblPhi > 100 Then dblPhi = 100

            dblKelvin = ConvertTemperature(dblTemperature, eTemperatureUnits, utpUnitsTemperatureConstants.utpKelvin)

            If dblKelvin > 0 Then
                dblViscosityInCentiPoise = Math.Exp(dblPhi * (-3.476 + 726 / dblKelvin) + (1 - dblPhi) * (-5.414 + 1566 / dblKelvin) + dblPhi * (1 - dblPhi) * (-1.762 + 929 / dblKelvin))
            Else
                dblViscosityInCentiPoise = 0
            End If

            Return ConvertViscosity(dblViscosityInCentiPoise, uviUnitsViscosityConstants.uviCentiPoise, eViscosityUnits)
        Catch ex As Exception
            Return 0
        End Try

    End Function


    Public Function ComputeVolFlowRate(Optional ByRef eUnits As ufrUnitsFlowRateConstants = ufrUnitsFlowRateConstants.ufrNLPerMin) As Double
        ' Computes the Volumetric flow rate, stores in .VolumetricFlowRate, and returns it

        Dim dblVolFlowRate, dblRadius As Double

        With mCapillaryFlowParameters

            dblRadius = .ColumnID / 2.0#

            If Math.Abs(.SolventViscosity) > Single.Epsilon And Math.Abs(.ColumnLength) > Single.Epsilon Then
                If .CapillaryType = ctCapillaryTypeConstants.ctOpenTubularCapillary Then
                    ' Open tubular capillary
                    dblVolFlowRate = (.BackPressure * dblRadius ^ 4 * PI) / (8 * .SolventViscosity * .ColumnLength) ' Flow rate in mL/sec
                Else
                    ' Packed capillary
                    If Math.Abs(.InterparticlePorosity - 1) > Single.Epsilon Then
                        ' Flow rate in mL/sec
                        dblVolFlowRate = (.BackPressure * .ParticleDiameter ^ 2 * .InterparticlePorosity ^ 2 * PI * dblRadius ^ 2) * .InterparticlePorosity / (180 * .SolventViscosity * .ColumnLength * (1 - .InterparticlePorosity) ^ 2)
                    Else
                        dblVolFlowRate = 0
                    End If
                End If

                ' Convert dblVolFlowRate to mL/min
                dblVolFlowRate = dblVolFlowRate * 60
            Else
                dblVolFlowRate = 0
            End If

            .VolumetricFlowRate = dblVolFlowRate
        End With

        ' Compute Dead Time (and Linear Velocity)
        ComputeDeadTime(utmUnitsTimeConstants.utmMinutes, False)

        ComputeVolFlowRate = ConvertVolFlowRate(dblVolFlowRate, ufrUnitsFlowRateConstants.ufrMLPerMin, eUnits)
    End Function

    Public Function ComputeVolFlowRateUsingDeadTime(Optional ByRef eUnits As ufrUnitsFlowRateConstants = ufrUnitsFlowRateConstants.ufrNLPerMin, Optional ByRef dblNewBackPressure As Double = 0, Optional ByRef ePressureUnits As uprUnitsPressureConstants = uprUnitsPressureConstants.uprPsi) As Double
        ' Computes the Volumetric flow rate using the dead time, stores in .VolumetricFlowRate, and returns it
        ' This requires modifying the pressure value to give the computed volumetric flow rate

        Dim dblVolFlowRate, dblRadius As Double

        With mCapillaryFlowParameters

            dblRadius = .ColumnID / 2.0#

            ' First find vol flow rate that gives observed dead time
            If Math.Abs(.ColumnDeadTime) > Single.Epsilon Then

                dblVolFlowRate = .ColumnLength * (PI * dblRadius ^ 2) / .ColumnDeadTime ' Vol flow rate in mL/sec

                If .CapillaryType = ctCapillaryTypeConstants.ctPackedCapillary Then
                    ' Packed Capillary
                    dblVolFlowRate = dblVolFlowRate * .InterparticlePorosity
                End If

                ' Store the new value
                .VolumetricFlowRate = dblVolFlowRate

                ' Now find pressure that gives computed dblVolFlowRate
                ' The ComputeBackPressure sub will store the new pressure
                dblNewBackPressure = ComputeBackPressure(ePressureUnits)
            Else
                dblVolFlowRate = 0
                .VolumetricFlowRate = 0
            End If

        End With

        ' Compute Linear Velocity (but not the dead time)
        ComputeLinearVelocity(ulvUnitsLinearVelocityConstants.ulvCmPerSec, False)

        ComputeVolFlowRateUsingDeadTime = ConvertVolFlowRate(dblVolFlowRate, ufrUnitsFlowRateConstants.ufrMLPerMin, eUnits)
    End Function

    ' Duplicated function, in both MWCapillaryFlowClass and MWMoleMassDilutionClass
    Public Function ConvertConcentration(ByRef dblConcentrationIn As Double, ByRef eCurrentUnits As ucoUnitsConcentrationConstants, ByRef eNewUnits As ucoUnitsConcentrationConstants) As Double
        Dim dblValue, dblFactor As Double
        Dim dblSampleMass As Double

        If eCurrentUnits = eNewUnits Then
            Return dblConcentrationIn
        End If

        dblSampleMass = mMassRateParameters.SampleMass

        dblFactor = FactorConcentration(eCurrentUnits, dblSampleMass)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblConcentrationIn * dblFactor
        End If

        dblFactor = FactorConcentration(eNewUnits, dblSampleMass)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    Public Function ConvertDiffusionCoefficient(ByRef dblDiffusionCoefficientIn As Double, ByRef eCurrentUnits As udcDiffusionCoefficientConstants, ByRef eNewUnits As udcDiffusionCoefficientConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblDiffusionCoefficientIn
        End If

        dblFactor = FactorDiffusionCoeff(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblDiffusionCoefficientIn * dblFactor
        End If

        dblFactor = FactorDiffusionCoeff(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            ConvertDiffusionCoefficient = -1
        Else
            ConvertDiffusionCoefficient = dblValue / dblFactor
        End If

    End Function

    Public Function ConvertLength(ByRef dblLengthIn As Double, ByRef eCurrentUnits As ulnUnitsLengthConstants, ByRef eNewUnits As ulnUnitsLengthConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblLengthIn
        End If

        dblFactor = FactorLength(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblLengthIn * dblFactor
        End If

        dblFactor = FactorLength(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    Public Function ConvertLinearVelocity(ByRef dblLinearVelocityIn As Double, ByRef eCurrentUnits As ulvUnitsLinearVelocityConstants, ByRef eNewUnits As ulvUnitsLinearVelocityConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblLinearVelocityIn
        End If

        dblFactor = FactorLinearVelocity(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblLinearVelocityIn * dblFactor
        End If

        dblFactor = FactorLinearVelocity(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            ConvertLinearVelocity = -1
        Else
            ConvertLinearVelocity = dblValue / dblFactor
        End If

    End Function

    Public Function ConvertMassFlowRate(ByRef dblMassFlowRateIn As Double, ByRef eCurrentUnits As umfMassFlowRateConstants, ByRef eNewUnits As umfMassFlowRateConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblMassFlowRateIn
        End If

        dblFactor = FactorMassFlowRate(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblMassFlowRateIn * dblFactor
        End If

        dblFactor = FactorMassFlowRate(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    Public Function ConvertMoles(ByRef dblMolesIn As Double, ByRef eCurrentUnits As umaMolarAmountConstants, ByRef eNewUnits As umaMolarAmountConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblMolesIn
        End If

        dblFactor = FactorMoles(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblMolesIn * dblFactor
        End If

        dblFactor = FactorMoles(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function


    Public Function ConvertPressure(ByRef dblPressureIn As Double, ByRef eCurrentUnits As uprUnitsPressureConstants, ByRef eNewUnits As uprUnitsPressureConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblPressureIn
        End If

        dblFactor = FactorPressure(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblPressureIn * dblFactor
        End If

        dblFactor = FactorPressure(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    Public Function ConvertTemperature(ByRef dblTemperatureIn As Double, ByRef eCurrentUnits As utpUnitsTemperatureConstants, ByRef eNewUnits As utpUnitsTemperatureConstants) As Double
        Dim dblValue As Double

        If eCurrentUnits = eNewUnits Then
            Return dblTemperatureIn
        End If

        ' First convert to Kelvin
        Select Case eCurrentUnits
            Case utpUnitsTemperatureConstants.utpCelsius
                ' K = C + 273
                dblValue = dblTemperatureIn + 273
            Case utpUnitsTemperatureConstants.utpFahrenheit
                ' Convert to Kelvin: C = 5/9*(F-32) and K = C + 273
                dblValue = 5.0# / 9.0# * (dblTemperatureIn - 32) + 273
            Case Else
                ' Includes utpKelvin
                ' Assume already Kelvin
        End Select

        ' We cannot get colder than absolute 0
        If dblValue < 0 Then dblValue = 0

        ' Now convert to the target units
        Select Case eNewUnits
            Case utpUnitsTemperatureConstants.utpCelsius
                ' C = K - 273
                dblValue = dblValue - 273
            Case utpUnitsTemperatureConstants.utpFahrenheit
                ' Convert to Fahrenheit: C = K - 273 and F = (9/5)C + 32
                dblValue = 9.0# / 5.0# * (dblValue - 273) + 32
            Case Else
                ' Includes utpKelvin
                ' Already in Kelvin
        End Select

        Return dblValue

    End Function

    Public Function ConvertTime(ByRef dblTimeIn As Double, ByRef eCurrentUnits As utmUnitsTimeConstants, ByRef eNewUnits As utmUnitsTimeConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblTimeIn
        End If

        dblFactor = FactorTime(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblTimeIn * dblFactor
        End If

        dblFactor = FactorTime(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    Public Function ConvertViscosity(ByRef dblViscosityIn As Double, ByRef eCurrentUnits As uviUnitsViscosityConstants, ByRef eNewUnits As uviUnitsViscosityConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblViscosityIn
        End If

        dblFactor = FactorViscosity(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblViscosityIn * dblFactor
        End If

        dblFactor = FactorViscosity(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    Public Function ConvertVolFlowRate(ByRef dblVolFlowRateIn As Double, ByRef eCurrentUnits As ufrUnitsFlowRateConstants, ByRef eNewUnits As ufrUnitsFlowRateConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblVolFlowRateIn
        End If

        dblFactor = FactorVolFlowRate(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblVolFlowRateIn * dblFactor
        End If

        dblFactor = FactorVolFlowRate(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    Public Function ConvertVolume(ByRef dblVolume As Double, ByRef eCurrentUnits As uvoUnitsVolumeConstants, ByRef eNewUnits As uvoUnitsVolumeConstants) As Double
        Dim dblValue, dblFactor As Double

        If eCurrentUnits = eNewUnits Then
            Return dblVolume
        End If

        dblFactor = FactorVolume(eCurrentUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Then
            Return -1
        Else
            dblValue = dblVolume * dblFactor
        End If

        dblFactor = FactorVolume(eNewUnits)
        If Math.Abs(dblFactor + 1) < Single.Epsilon Or Math.Abs(dblFactor) < Single.Epsilon Then
            Return -1
        Else
            Return dblValue / dblFactor
        End If

    End Function

    ' Multiplication factor for converting from eUnits to M
    ' dblSampleMass is required for mass-based units
    ' Duplicated function, in both MWCapillaryFlowClass and MWMoleMassDilutionClass
    Private Function FactorConcentration(ByRef eUnits As ucoUnitsConcentrationConstants, Optional dblSampleMass As Double = 0) As Double
        Dim dblFactor As Double

        If Math.Abs(dblSampleMass) < Single.Epsilon Then
            dblFactor = -1
        Else
            Select Case eUnits
                Case ucoUnitsConcentrationConstants.ucoMolar : dblFactor = 1.0#
                Case ucoUnitsConcentrationConstants.ucoMilliMolar : dblFactor = 1 / 1000.0#
                Case ucoUnitsConcentrationConstants.ucoMicroMolar : dblFactor = 1 / 1000000.0#
                Case ucoUnitsConcentrationConstants.ucoNanoMolar : dblFactor = 1 / 1000000000.0#
                Case ucoUnitsConcentrationConstants.ucoPicoMolar : dblFactor = 1 / 1000000000000.0#
                Case ucoUnitsConcentrationConstants.ucoFemtoMolar : dblFactor = 1 / 1.0E+15
                Case ucoUnitsConcentrationConstants.ucoAttoMolar : dblFactor = 1 / 1.0E+18
                Case ucoUnitsConcentrationConstants.ucoMgPerML : dblFactor = 1 / dblSampleMass '1/[(1 g / 1000 mg) * (1 / MW) * (1000 mL/L)]
                Case ucoUnitsConcentrationConstants.ucoUgPerML : dblFactor = 1 / (dblSampleMass * 1000.0#) '1/[(1 g / 1000000 ug) * (1 / MW) * (1000 mL/L)]
                Case ucoUnitsConcentrationConstants.ucongperml : dblFactor = 1 / (dblSampleMass * 1000000.0#) '1/[(1 g / 1000000000 ng) * (1 / MW) * (1000 mL/L)]
                Case ucoUnitsConcentrationConstants.ucoUgPerUL : dblFactor = 1 / (dblSampleMass) '1/[(1 g / 1000000 ug) * (1 / MW) * (1000000 uL/L)]
                Case ucoUnitsConcentrationConstants.ucoNgPerUL : dblFactor = 1 / (dblSampleMass * 1000.0#) '1/[(1 g / 1000000000 ng) * (1 / MW) * (1000000 uL/L)]
                Case Else : dblFactor = -1
            End Select
        End If

        FactorConcentration = dblFactor
    End Function

    ' Multiplication factor for converting from eUnits to Cm
    Private Function FactorLength(ByRef eUnits As ulnUnitsLengthConstants) As Double

        Select Case eUnits
            Case ulnUnitsLengthConstants.ulnM : FactorLength = 100.0#
            Case ulnUnitsLengthConstants.ulnCM : FactorLength = 1.0#
            Case ulnUnitsLengthConstants.ulnMM : FactorLength = 1 / 10.0#
            Case ulnUnitsLengthConstants.ulnMicrons : FactorLength = 1 / 10000.0#
            Case ulnUnitsLengthConstants.ulnInches : FactorLength = CM_PER_INCH
            Case Else : FactorLength = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to Cm/Min
    Private Function FactorLinearVelocity(ByRef eUnits As ulvUnitsLinearVelocityConstants) As Double

        Select Case eUnits
            Case ulvUnitsLinearVelocityConstants.ulvCmPerHr : FactorLinearVelocity = 1 / 60.0#
            Case ulvUnitsLinearVelocityConstants.ulvMmPerHr : FactorLinearVelocity = 1 / 60.0# / 10.0#
            Case ulvUnitsLinearVelocityConstants.ulvCmPerMin : FactorLinearVelocity = 1
            Case ulvUnitsLinearVelocityConstants.ulvMmPerMin : FactorLinearVelocity = 1 / 10.0#
            Case ulvUnitsLinearVelocityConstants.ulvCmPerSec : FactorLinearVelocity = 60.0#
            Case ulvUnitsLinearVelocityConstants.ulvMmPerSec : FactorLinearVelocity = 60.0# / 10.0#
            Case Else : FactorLinearVelocity = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to moles/min
    Private Function FactorMassFlowRate(ByRef eUnits As umfMassFlowRateConstants) As Double

        Select Case eUnits
            Case umfMassFlowRateConstants.umfPmolPerMin : FactorMassFlowRate = 1 / 1000000000000.0#
            Case umfMassFlowRateConstants.umfFmolPerMin : FactorMassFlowRate = 1 / 1.0E+15
            Case umfMassFlowRateConstants.umfAmolPerMin : FactorMassFlowRate = 1 / 1.0E+18
            Case umfMassFlowRateConstants.umfPmolPerSec : FactorMassFlowRate = 1 / (1000000000000.0# / 60.0#)
            Case umfMassFlowRateConstants.umfFmolPerSec : FactorMassFlowRate = 1 / (1.0E+15 / 60.0#)
            Case umfMassFlowRateConstants.umfAmolPerSec : FactorMassFlowRate = 1 / (1.0E+18 / 60.0#)
            Case umfMassFlowRateConstants.umfMolesPerMin : FactorMassFlowRate = 1.0#
            Case Else : FactorMassFlowRate = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to moles
    Private Function FactorMoles(ByRef eUnits As umaMolarAmountConstants) As Double

        Select Case eUnits
            Case umaMolarAmountConstants.umaMoles : FactorMoles = 1.0#
            Case umaMolarAmountConstants.umaMilliMoles : FactorMoles = 1 / 1000.0#
            Case umaMolarAmountConstants.umaMicroMoles : FactorMoles = 1 / 1000000.0#
            Case umaMolarAmountConstants.umaNanoMoles : FactorMoles = 1 / 1000000000.0#
            Case umaMolarAmountConstants.umaPicoMoles : FactorMoles = 1 / 1000000000000.0#
            Case umaMolarAmountConstants.umaFemtoMoles : FactorMoles = 1 / 1.0E+15
            Case umaMolarAmountConstants.umaAttoMoles : FactorMoles = 1 / 1.0E+18
            Case Else : FactorMoles = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to dynes per cm^2
    Private Function FactorPressure(ByRef eUnits As uprUnitsPressureConstants) As Double

        Select Case eUnits
            Case uprUnitsPressureConstants.uprPsi : FactorPressure = 68947.57
            Case uprUnitsPressureConstants.uprPascals : FactorPressure = 10.0#
            Case uprUnitsPressureConstants.uprKiloPascals : FactorPressure = 10000.0#
            Case uprUnitsPressureConstants.uprAtmospheres : FactorPressure = 1013250.0#
            Case uprUnitsPressureConstants.uprBar : FactorPressure = 1000000.0#
            Case uprUnitsPressureConstants.uprTorr : FactorPressure = 1333.22
            Case uprUnitsPressureConstants.uprDynesPerSquareCm : FactorPressure = 1
            Case Else : FactorPressure = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to minutes
    Private Function FactorTime(ByRef eUnits As utmUnitsTimeConstants) As Double

        Select Case eUnits
            Case utmUnitsTimeConstants.utmHours : FactorTime = 60.0#
            Case utmUnitsTimeConstants.utmMinutes : FactorTime = 1.0#
            Case utmUnitsTimeConstants.utmSeconds : FactorTime = 1 / 60.0#
            Case Else : FactorTime = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to cm^2/sec
    Private Function FactorDiffusionCoeff(ByRef eUnits As udcDiffusionCoefficientConstants) As Double

        Select Case eUnits
            Case udcDiffusionCoefficientConstants.udcCmSquaredPerHr : FactorDiffusionCoeff = 1 / 3600.0#
            Case udcDiffusionCoefficientConstants.udcCmSquaredPerMin : FactorDiffusionCoeff = 1 / 60.0#
            Case udcDiffusionCoefficientConstants.udcCmSquaredPerSec : FactorDiffusionCoeff = 1.0#
            Case Else : FactorDiffusionCoeff = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to poise
    Private Function FactorViscosity(ByRef eUnits As uviUnitsViscosityConstants) As Double

        Select Case eUnits
            Case uviUnitsViscosityConstants.uviPoise : FactorViscosity = 1.0#
            Case uviUnitsViscosityConstants.uviCentiPoise : FactorViscosity = 1 / 100.0#
            Case Else : FactorViscosity = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to mL/min
    Private Function FactorVolFlowRate(ByRef eUnits As ufrUnitsFlowRateConstants) As Double

        Select Case eUnits
            Case ufrUnitsFlowRateConstants.ufrMLPerMin : FactorVolFlowRate = 1.0#
            Case ufrUnitsFlowRateConstants.ufrULPerMin : FactorVolFlowRate = 1 / 1000.0#
            Case ufrUnitsFlowRateConstants.ufrNLPerMin : FactorVolFlowRate = 1 / 1000000.0#
            Case Else : FactorVolFlowRate = -1
        End Select

    End Function

    ' Multiplication factor for converting from eUnits to mL
    Private Function FactorVolume(ByRef eUnits As uvoUnitsVolumeConstants) As Double

        Select Case eUnits
            Case uvoUnitsVolumeConstants.uvoML : FactorVolume = 1.0#
            Case uvoUnitsVolumeConstants.uvoUL : FactorVolume = 1 / 1000.0#
            Case uvoUnitsVolumeConstants.uvoNL : FactorVolume = 1 / 1000000.0#
            Case uvoUnitsVolumeConstants.uvoPL : FactorVolume = 1 / 1000000000.0#
            Case Else : FactorVolume = -1
        End Select

    End Function

    ' Get Subs
    ' Gets the most recently computed value
    ' If mAutoCompute = False, then must manually call a Compute Sub to recompute the value

    Public Function GetAutoComputeEnabled() As Boolean
        GetAutoComputeEnabled = mAutoCompute
    End Function

    Public Function GetAutoComputeMode() As acmAutoComputeModeConstants
        GetAutoComputeMode = mAutoComputeMode
    End Function

    Public Function GetBackPressure(Optional ByRef eUnits As uprUnitsPressureConstants = uprUnitsPressureConstants.uprPsi) As Double
        GetBackPressure = ConvertPressure(mCapillaryFlowParameters.BackPressure, uprUnitsPressureConstants.uprDynesPerSquareCm, eUnits)
    End Function

    Public Function GetCapillaryType() As ctCapillaryTypeConstants
        GetCapillaryType = mCapillaryFlowParameters.CapillaryType
    End Function

    Public Function GetColumnID(Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnMicrons) As Double
        GetColumnID = ConvertLength(mCapillaryFlowParameters.ColumnID, ulnUnitsLengthConstants.ulnCM, eUnits)
    End Function

    Public Function GetColumnLength(Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnCM) As Double
        GetColumnLength = ConvertLength(mCapillaryFlowParameters.ColumnLength, ulnUnitsLengthConstants.ulnCM, eUnits)
    End Function

    Public Function GetColumnVolume(Optional ByRef eUnits As uvoUnitsVolumeConstants = uvoUnitsVolumeConstants.uvoUL) As Double
        ' Column volume isn't stored; simply re-compute it
        GetColumnVolume = ComputeColumnVolume(eUnits)
    End Function

    Public Function GetDeadTime(Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmMinutes) As Double
        GetDeadTime = ConvertTime(mCapillaryFlowParameters.ColumnDeadTime, utmUnitsTimeConstants.utmMinutes, eUnits)
    End Function

    Public Function GetExtraColumnBroadeningAdditionalVarianceInSquareSeconds() As Double
        GetExtraColumnBroadeningAdditionalVarianceInSquareSeconds = mExtraColumnBroadeningParameters.AdditionalTemporalVariance
    End Function

    Public Function GetExtraColumnBroadeningDiffusionCoefficient(Optional ByRef eUnits As udcDiffusionCoefficientConstants = udcDiffusionCoefficientConstants.udcCmSquaredPerSec) As Double
        GetExtraColumnBroadeningDiffusionCoefficient = ConvertDiffusionCoefficient(mExtraColumnBroadeningParameters.DiffusionCoefficient, udcDiffusionCoefficientConstants.udcCmSquaredPerSec, eUnits)
    End Function

    Public Function GetExtraColumnBroadeningInitialPeakWidthAtBase(Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmSeconds) As Double
        GetExtraColumnBroadeningInitialPeakWidthAtBase = ConvertTime(mExtraColumnBroadeningParameters.InitialPeakWidth, utmUnitsTimeConstants.utmSeconds, eUnits)
    End Function

    Public Function GetExtraColumnBroadeningLinearVelocity(Optional ByRef eUnits As ulvUnitsLinearVelocityConstants = ulvUnitsLinearVelocityConstants.ulvMmPerMin) As Double
        GetExtraColumnBroadeningLinearVelocity = ConvertLinearVelocity(mExtraColumnBroadeningParameters.LinearVelocity, ulvUnitsLinearVelocityConstants.ulvCmPerMin, eUnits)
    End Function

    Public Function GetExtraColumnBroadeningOpenTubeID(Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnMicrons) As Double
        GetExtraColumnBroadeningOpenTubeID = ConvertLength(mExtraColumnBroadeningParameters.OpenTubeID, ulnUnitsLengthConstants.ulnCM, eUnits)
    End Function

    Public Function GetExtraColumnBroadeningOpenTubeLength(Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnCM) As Double
        GetExtraColumnBroadeningOpenTubeLength = ConvertLength(mExtraColumnBroadeningParameters.OpenTubeLength, ulnUnitsLengthConstants.ulnCM, eUnits)
    End Function

    Public Function GetExtraColumnBroadeningResultantPeakWidth(Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmSeconds) As Double
        GetExtraColumnBroadeningResultantPeakWidth = ConvertTime(mExtraColumnBroadeningParameters.ResultantPeakWidth, utmUnitsTimeConstants.utmSeconds, eUnits)
    End Function

    Public Function GetExtraColumnBroadeningTemporalVarianceInSquareSeconds() As Double
        GetExtraColumnBroadeningTemporalVarianceInSquareSeconds = mExtraColumnBroadeningParameters.TemporalVariance
    End Function

    Public Function GetInterparticlePorosity() As Double
        GetInterparticlePorosity = mCapillaryFlowParameters.InterparticlePorosity
    End Function

    Public Function GetLinearVelocity(Optional ByRef eUnits As ulvUnitsLinearVelocityConstants = ulvUnitsLinearVelocityConstants.ulvCmPerSec) As Double
        GetLinearVelocity = ConvertLinearVelocity(mCapillaryFlowParameters.LinearVelocity, ulvUnitsLinearVelocityConstants.ulvCmPerMin, eUnits)
    End Function

    Public Function GetMassRateConcentration(Optional ByRef eUnits As ucoUnitsConcentrationConstants = ucoUnitsConcentrationConstants.ucoMicroMolar) As Double
        GetMassRateConcentration = ConvertConcentration(mMassRateParameters.SampleConcentration, ucoUnitsConcentrationConstants.ucoMolar, eUnits)
    End Function

    Public Function GetMassRateInjectionTime(Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmMinutes) As Double
        GetMassRateInjectionTime = ConvertTime(mMassRateParameters.InjectionTime, utmUnitsTimeConstants.utmMinutes, eUnits)
    End Function

    Public Function GetMassFlowRate(Optional ByRef eUnits As umfMassFlowRateConstants = umfMassFlowRateConstants.umfFmolPerSec) As Double
        GetMassFlowRate = ConvertMassFlowRate(mMassRateParameters.MassFlowRate, umfMassFlowRateConstants.umfMolesPerMin, eUnits)
    End Function

    Public Function GetMassRateMolesInjected(Optional ByRef eUnits As umaMolarAmountConstants = umaMolarAmountConstants.umaFemtoMoles) As Double
        GetMassRateMolesInjected = ConvertMoles(mMassRateParameters.MolesInjected, umaMolarAmountConstants.umaMoles, eUnits)
    End Function

    Public Function GetMassRateSampleMass() As Double
        GetMassRateSampleMass = mMassRateParameters.SampleMass
    End Function

    Public Function GetMassRateVolFlowRate(Optional ByRef eUnits As ufrUnitsFlowRateConstants = ufrUnitsFlowRateConstants.ufrNLPerMin) As Double
        GetMassRateVolFlowRate = ConvertVolFlowRate(mMassRateParameters.VolumetricFlowRate, ufrUnitsFlowRateConstants.ufrMLPerMin, eUnits)
    End Function

    Public Function GetParticleDiameter(Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnMicrons) As Double
        GetParticleDiameter = ConvertLength(mCapillaryFlowParameters.ParticleDiameter, ulnUnitsLengthConstants.ulnCM, eUnits)
    End Function

    Public Function GetSolventViscosity(Optional ByRef eUnits As uviUnitsViscosityConstants = uviUnitsViscosityConstants.uviPoise) As Double
        GetSolventViscosity = ConvertViscosity(mCapillaryFlowParameters.SolventViscosity, uviUnitsViscosityConstants.uviPoise, eUnits)
    End Function

    Public Function GetVolFlowRate(Optional ByRef eUnits As ufrUnitsFlowRateConstants = ufrUnitsFlowRateConstants.ufrNLPerMin) As Double
        GetVolFlowRate = ConvertVolFlowRate(mCapillaryFlowParameters.VolumetricFlowRate, ufrUnitsFlowRateConstants.ufrMLPerMin, eUnits)
    End Function


    ' Set Subs
    ' If mAutoCompute = False, then must manually call a Compute Sub to recompute other values

    Public Sub SetAutoComputeEnabled(ByRef blnAutoCompute As Boolean)
        mAutoCompute = blnAutoCompute
    End Sub

    Public Sub SetAutoComputeMode(ByRef eAutoComputeMode As acmAutoComputeModeConstants)
        If eAutoComputeMode >= acmAutoComputeModeConstants.acmBackPressure And eAutoComputeMode <= acmAutoComputeModeConstants.acmVolFlowrateUsingDeadTime Then
            mAutoComputeMode = eAutoComputeMode
        End If
    End Sub

    Public Sub SetBackPressure(ByRef dblBackPressure As Double, Optional ByRef eUnits As uprUnitsPressureConstants = uprUnitsPressureConstants.uprPsi)
        mCapillaryFlowParameters.BackPressure = ConvertPressure(dblBackPressure, eUnits, uprUnitsPressureConstants.uprDynesPerSquareCm)
        CheckAutoCompute()
    End Sub

    Public Sub SetCapillaryType(ByRef eCapillaryType As ctCapillaryTypeConstants)
        If eCapillaryType >= ctCapillaryTypeConstants.ctOpenTubularCapillary And eCapillaryType <= ctCapillaryTypeConstants.ctPackedCapillary Then
            mCapillaryFlowParameters.CapillaryType = eCapillaryType
        End If
        CheckAutoCompute()
    End Sub

    Public Sub SetColumnID(ByRef dblColumnID As Double, Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnMicrons)
        mCapillaryFlowParameters.ColumnID = ConvertLength(dblColumnID, eUnits, ulnUnitsLengthConstants.ulnCM)
        CheckAutoCompute()
    End Sub

    Public Sub SetColumnLength(ByRef dblColumnLength As Double, Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnCM)
        mCapillaryFlowParameters.ColumnLength = ConvertLength(dblColumnLength, eUnits, ulnUnitsLengthConstants.ulnCM)
        CheckAutoCompute()
    End Sub

    Public Sub SetDeadTime(ByRef dblDeadTime As Double, Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmMinutes)
        mCapillaryFlowParameters.ColumnDeadTime = ConvertTime(dblDeadTime, eUnits, utmUnitsTimeConstants.utmMinutes)
        CheckAutoCompute()
    End Sub

    Public Sub SetExtraColumnBroadeningAdditionalVariance(ByRef dblAdditionalVarianceInSquareSeconds As Double)
        mExtraColumnBroadeningParameters.AdditionalTemporalVariance = dblAdditionalVarianceInSquareSeconds
        ComputeExtraColumnBroadeningValues()
    End Sub

    Public Sub SetExtraColumnBroadeningDiffusionCoefficient(ByRef dblDiffusionCoefficient As Double, Optional ByRef eUnits As udcDiffusionCoefficientConstants = udcDiffusionCoefficientConstants.udcCmSquaredPerSec)
        mExtraColumnBroadeningParameters.DiffusionCoefficient = ConvertDiffusionCoefficient(dblDiffusionCoefficient, eUnits, udcDiffusionCoefficientConstants.udcCmSquaredPerSec)
        ComputeExtraColumnBroadeningValues()
    End Sub

    Public Sub SetExtraColumnBroadeningInitialPeakWidthAtBase(ByRef dblWidth As Double, Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmSeconds)
        mExtraColumnBroadeningParameters.InitialPeakWidth = ConvertTime(dblWidth, eUnits, utmUnitsTimeConstants.utmSeconds)
        ComputeExtraColumnBroadeningValues()
    End Sub

    Public Sub SetExtraColumnBroadeningLinearVelocity(ByRef dblLinearVelocity As Double, Optional ByRef eUnits As ulvUnitsLinearVelocityConstants = ulvUnitsLinearVelocityConstants.ulvMmPerMin)
        mExtraColumnBroadeningParameters.LinearVelocity = ConvertLinearVelocity(dblLinearVelocity, eUnits, ulvUnitsLinearVelocityConstants.ulvCmPerMin)
        ComputeExtraColumnBroadeningValues()
    End Sub

    Public Sub SetExtraColumnBroadeningOpenTubeID(ByRef dblOpenTubeID As Double, Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnMicrons)
        mExtraColumnBroadeningParameters.OpenTubeID = ConvertLength(dblOpenTubeID, eUnits, ulnUnitsLengthConstants.ulnCM)
        ComputeExtraColumnBroadeningValues()
    End Sub

    Public Sub SetExtraColumnBroadeningOpenTubeLength(ByRef dblLength As Double, Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnCM)
        mExtraColumnBroadeningParameters.OpenTubeLength = ConvertLength(dblLength, eUnits, ulnUnitsLengthConstants.ulnCM)
        ComputeExtraColumnBroadeningValues()
    End Sub

    Public Sub SetInterparticlePorosity(ByRef dblPorosity As Double)
        If dblPorosity >= 0 And dblPorosity <= 1 Then
            mCapillaryFlowParameters.InterparticlePorosity = dblPorosity
        End If
        CheckAutoCompute()
    End Sub

    Public Sub SetMassRateConcentration(ByRef dblConcentration As Double, Optional ByRef eUnits As ucoUnitsConcentrationConstants = ucoUnitsConcentrationConstants.ucoMicroMolar)
        mMassRateParameters.SampleConcentration = ConvertConcentration(dblConcentration, eUnits, ucoUnitsConcentrationConstants.ucoMolar)
        ComputeMassRateValues()
    End Sub

    Public Sub SetMassRateInjectionTime(ByRef dblInjectionTime As Double, Optional ByRef eUnits As utmUnitsTimeConstants = utmUnitsTimeConstants.utmMinutes)
        mMassRateParameters.InjectionTime = ConvertTime(dblInjectionTime, eUnits, utmUnitsTimeConstants.utmMinutes)
        ComputeMassRateValues()
    End Sub

    Public Sub SetMassRateSampleMass(ByRef dblMassInGramsPerMole As Double)
        If dblMassInGramsPerMole >= 0 Then
            mMassRateParameters.SampleMass = dblMassInGramsPerMole
        Else
            mMassRateParameters.SampleMass = 0
        End If
        ComputeMassRateValues()
    End Sub

    Public Sub SetMassRateVolFlowRate(ByRef dblVolFlowRate As Double, Optional ByRef eUnits As ufrUnitsFlowRateConstants = ufrUnitsFlowRateConstants.ufrNLPerMin)
        mMassRateParameters.VolumetricFlowRate = ConvertVolFlowRate(dblVolFlowRate, eUnits, ufrUnitsFlowRateConstants.ufrMLPerMin)
        ComputeMassRateValues()
    End Sub

    Public Sub SetParticleDiameter(ByRef dblParticleDiameter As Double, Optional ByRef eUnits As ulnUnitsLengthConstants = ulnUnitsLengthConstants.ulnMicrons)
        mCapillaryFlowParameters.ParticleDiameter = ConvertLength(dblParticleDiameter, eUnits, ulnUnitsLengthConstants.ulnCM)
        CheckAutoCompute()
    End Sub

    Public Sub SetSolventViscosity(ByRef dblSolventViscosity As Double, Optional ByRef eUnits As uviUnitsViscosityConstants = uviUnitsViscosityConstants.uviPoise)
        mCapillaryFlowParameters.SolventViscosity = ConvertViscosity(dblSolventViscosity, eUnits, uviUnitsViscosityConstants.uviPoise)
        CheckAutoCompute()
    End Sub

    Public Sub SetVolFlowRate(ByRef dblVolFlowRate As Double, Optional ByRef eUnits As ufrUnitsFlowRateConstants = ufrUnitsFlowRateConstants.ufrNLPerMin)
        mCapillaryFlowParameters.VolumetricFlowRate = ConvertVolFlowRate(dblVolFlowRate, eUnits, ufrUnitsFlowRateConstants.ufrMLPerMin)
        CheckAutoCompute()
    End Sub

    Private Sub InitializeClass()
        Me.SetAutoComputeEnabled(False)

        Me.SetAutoComputeMode(acmAutoComputeModeConstants.acmVolFlowrate)
        Me.SetCapillaryType(ctCapillaryTypeConstants.ctPackedCapillary)
        Me.SetBackPressure(3000, uprUnitsPressureConstants.uprPsi)
        Me.SetColumnLength(50, ulnUnitsLengthConstants.ulnCM)
        Me.SetColumnID(75, ulnUnitsLengthConstants.ulnMicrons)
        Me.SetSolventViscosity(0.0089, uviUnitsViscosityConstants.uviPoise)
        Me.SetParticleDiameter(5, ulnUnitsLengthConstants.ulnMicrons)
        Me.SetInterparticlePorosity(0.4)

        Me.SetMassRateConcentration(1, ucoUnitsConcentrationConstants.ucoMicroMolar)
        Me.SetMassRateVolFlowRate(600, ufrUnitsFlowRateConstants.ufrNLPerMin)
        Me.SetMassRateInjectionTime(5, utmUnitsTimeConstants.utmMinutes)

        ' Recompute
        ComputeVolFlowRate()
        ComputeMassRateValues()
        ComputeExtraColumnBroadeningValues()

        Me.SetAutoComputeEnabled(True)

    End Sub

End Class