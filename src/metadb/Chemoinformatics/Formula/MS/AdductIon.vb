Imports System.Collections.Concurrent
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
''' <summary>
''' This is the storage of adduct ion information.
''' </summary>
Public Class AdductIon
    ''' <summary>
    ''' Initializes a new instance of the AdductIon class.
    ''' <para>
    ''' This constructor is preserved for use with MessagePack for C#, and direct usage is deprecated. If used as a default value,
    ''' consider using the AdductIon.Default property. If you know the AdductIonName, consider using the AdductIon.GetAdductIon method.
    ''' </para>
    ''' </summary>
    Public Sub New()
    End Sub

    Public Property AdductIonAccurateMass As Double

    Public Function ConvertToMz(ByVal exactMass As Double) As Double
        Dim precursorMz = (exactMass * AdductIonXmer + AdductIonAccurateMass) / ChargeNumber
        If IonMode = IonModes.Positive Then
            precursorMz -= 0.0005485799 * ChargeNumber
        Else
            precursorMz += 0.0005485799 * ChargeNumber
        End If
        Return precursorMz
    End Function

    Public Function ConvertToExactMass(ByVal mz As Double) As Double
        Dim monoIsotopicMass = (mz * ChargeNumber - AdductIonAccurateMass) / AdductIonXmer
        If IonMode = IonModes.Positive Then
            monoIsotopicMass += 0.0005485799 * ChargeNumber
        Else
            monoIsotopicMass -= 0.0005485799 * ChargeNumber
        End If
        Return monoIsotopicMass
    End Function

    Public Property AdductIonXmer As Integer
    Public Property AdductIonName As String = String.Empty
    Public Property ChargeNumber As Integer
    Public Property IonMode As IonModes
    Public Property FormatCheck As Boolean
    Public Property M1Intensity As Double
    Public Property M2Intensity As Double
    Public Property IsRadical As Boolean
    Public Property IsIncluded As Boolean ' used for applications

    Public ReadOnly Property HasAdduct As Boolean
        Get
            Return Not String.IsNullOrEmpty(AdductIonName)
        End Get
    End Property

    Public ReadOnly Property IsFA As Boolean
        Get
            Return Equals(AdductIonName, "[M+HCOO]-") OrElse Equals(AdductIonName, "[M+FA-H]-")
        End Get
    End Property

    Public ReadOnly Property IsHac As Boolean
        Get
            Return Equals(AdductIonName, "[M+CH3COO]-") OrElse Equals(AdductIonName, "[M+Hac-H]-")
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return AdductIonName
    End Function

    ''' <summary>
    ''' This method returns the AdductIon class variable from the adduct string.
    ''' </summary>
    ''' <param name="adductName">Add the formula string such as "C6H12O6"</param>
    ''' <returns>AdductIon</returns>
    Public Shared Function GetAdductIon(ByVal adductName As String) As AdductIon
        Return ADDUCT_IONS.GetOrAdd(adductName)
    End Function

    Private Shared Function GetAdductIonCore(ByVal adductName As String) As AdductIon
        Dim adduct As AdductIon = New AdductIon() With {
            .AdductIonName = adductName
        }

        If Not IonTypeFormatChecker(adductName) Then
            adduct.FormatCheck = False
            Return adduct
        End If

        Dim chargeNum = GetChargeNumber(adductName)
        If chargeNum = -1 Then
            adduct.FormatCheck = False
            Return adduct
        End If

        Dim adductIonXmer = GetAdductIonXmer(adductName)
        Dim ionType = GetIonType(adductName)
        Dim isRadical = GetRadicalInfo(adductName)

        Dim accurateMass As Double = Nothing, m1Intensity As Double = Nothing, m2Intensity As Double = Nothing
        Dim tr As (accurateMass As Double, m1Intensity As Double, m2Intensity As Double) = CalculateAccurateMassAndIsotopeRatio(adduct.AdductIonName)
        adduct.AdductIonAccurateMass += accurateMass
        adduct.M1Intensity += m1Intensity
        adduct.M2Intensity += m2Intensity

        adduct.AdductIonXmer = adductIonXmer
        adduct.ChargeNumber = chargeNum
        adduct.FormatCheck = True
        adduct.IonMode = ionType
        adduct.IsRadical = isRadical

        Return adduct
    End Function

    Public Shared Function GetStandardAdductIon(ByVal charge As Integer, ByVal ionMode As IonModes) As AdductIon
        Select Case ionMode
            Case IonModes.Positive
                If charge >= 2 Then
                    Return GetAdductIon($"[M+{charge}H]{charge}+")
                Else
                    Return GetAdductIon("[M+H]+")
                End If
            Case IonModes.Negative
                If charge >= 2 Then
                    Return GetAdductIon($"[M-{charge}H]{charge}-")
                Else
                    Return GetAdductIon("[M-H]-")
                End If

            Case Else
                Return [Default]
        End Select
    End Function

    Public Shared ReadOnly [Default] As AdductIon = New AdductIon()

    Private Shared ReadOnly ADDUCT_IONS As AdductIons = New AdductIons()

    Friend Class AdductIons
        Private ReadOnly _dictionary As ConcurrentDictionary(Of String, AdductIon)
        Public Sub New()
            _dictionary = New ConcurrentDictionary(Of String, AdductIon)()
            _dictionary.TryAdd([Default].AdductIonName, [Default])
        End Sub

        Public Function GetOrAdd(ByVal adduct As String) As AdductIon
            Return _dictionary.GetOrAdd(adduct, New Func(Of String, AdductIon)(AddressOf GetAdductIonCore))
        End Function
    End Class
End Class

