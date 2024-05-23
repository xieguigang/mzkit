#Region "Microsoft.VisualBasic::cbf228eaf6374fe6845a1f76c772cfbb, metadb\Chemoinformatics\Formula\MS\AdductIon.vb"

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


    ' Code Statistics:

    '   Total Lines: 178
    '    Code Lines: 123 (69.10%)
    ' Comment Lines: 25 (14.04%)
    '    - Xml Docs: 96.00%
    ' 
    '   Blank Lines: 30 (16.85%)
    '     File Size: 6.89 KB


    '     Class AdductIon
    ' 
    '         Properties: AdductIonAccurateMass, AdductIonName, AdductIonXmer, ChargeNumber, FormatCheck
    '                     HasAdduct, IonMode, IsFA, IsHac, IsIncluded
    '                     IsRadical, M1Intensity, M2Intensity
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ConvertToExactMass, ConvertToMz, GetAdductIon, GetAdductIonCore, GetStandardAdductIon
    '                   ToString
    '         Class AdductIons
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: GetOrAdd
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Concurrent
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports std = System.Math

Namespace Formula.MS

    ''' <summary>
    ''' A <see cref="MzCalculator"/> liked precursor m/z evaluation model.
    ''' 
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

        Sub New(adducts As MzCalculator)
            Me.AdductIonAccurateMass = std.Abs(adducts.adducts)
            Me.AdductIonName = adducts.ToString
            Me.AdductIonXmer = adducts.M
            Me.ChargeNumber = adducts.charge
            Me.IonMode = Provider.ParseIonMode(adducts.mode)
        End Sub

        Public Property AdductIonAccurateMass As Double

        Public Function ConvertToMz(exactMass As Double) As Double
            Dim precursorMz = (exactMass * AdductIonXmer + AdductIonAccurateMass) / ChargeNumber
            If IonMode = IonModes.Positive Then
                precursorMz -= 0.0005485799 * ChargeNumber
            Else
                precursorMz += 0.0005485799 * ChargeNumber
            End If
            Return precursorMz
        End Function

        Public Function ConvertToExactMass(mz As Double) As Double
            Dim monoIsotopicMass = (mz * ChargeNumber - AdductIonAccurateMass) / AdductIonXmer
            If IonMode = IonModes.Positive Then
                monoIsotopicMass += 0.0005485799 * ChargeNumber
            Else
                monoIsotopicMass -= 0.0005485799 * ChargeNumber
            End If
            Return monoIsotopicMass
        End Function

        Public Property AdductIonXmer As Integer

        ''' <summary>
        ''' the precursor adduct name, example as ``[M+H]+``
        ''' </summary>
        ''' <returns></returns>
        Public Property AdductIonName As String = String.Empty
        Public Property ChargeNumber As Integer
        Public Property IonMode As IonModes
        Public Property FormatCheck As Boolean
        Public Property M1Intensity As Double
        Public Property M2Intensity As Double
        Public Property IsRadical As Boolean
        Public Property IsIncluded As Boolean ' used for applications

        ''' <summary>
        ''' false value for ``[M]+`` or ``[M]-``.
        ''' </summary>
        ''' <returns></returns>
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
        Public Shared Function GetAdductIon(adductName As String) As AdductIon
            Return ADDUCT_IONS.GetOrAdd(adductName)
        End Function

        Private Shared Function GetAdductIonCore(adductName As String) As AdductIon
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

        Public Shared Function GetStandardAdductIon(charge As Integer, ionMode As IonModes) As AdductIon
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

        Public Shared ReadOnly [Default] As New AdductIon()

        Private Shared ReadOnly ADDUCT_IONS As New AdductIons()

        Friend Class AdductIons

            ReadOnly _dictionary As ConcurrentDictionary(Of String, AdductIon)

            Public Sub New()
                _dictionary = New ConcurrentDictionary(Of String, AdductIon)()
                _dictionary.TryAdd([Default].AdductIonName, [Default])
            End Sub

            Public Function GetOrAdd(adduct As String) As AdductIon
                Return _dictionary.GetOrAdd(adduct, New Func(Of String, AdductIon)(AddressOf GetAdductIonCore))
            End Function
        End Class
    End Class

End Namespace
