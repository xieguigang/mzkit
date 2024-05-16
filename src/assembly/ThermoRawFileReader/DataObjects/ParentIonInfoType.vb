#Region "Microsoft.VisualBasic::826b877700c4cd2d51c8186440c4fd70, assembly\ThermoRawFileReader\DataObjects\ParentIonInfoType.vb"

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

    '   Total Lines: 83
    '    Code Lines: 28
    ' Comment Lines: 43
    '   Blank Lines: 12
    '     File Size: 2.67 KB


    '     Structure ParentIonInfoType
    ' 
    '         Function: ToString
    ' 
    '         Sub: Clear
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData

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
