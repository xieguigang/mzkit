#Region "Microsoft.VisualBasic::b8f80580cb8ccfedfac4c6a08549dd1c, src\assembly\assembly\MarkupData\mzML\XML\IonTargeted\precursor.vb"

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

    '     Class precursor
    ' 
    '         Properties: activation, isolationWindow, selectedIonList
    ' 
    '         Function: GetActivationMethod, GetCollisionEnergy, GetMz
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.mzML.IonTargeted

    Public Class precursor : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params
        Public Property selectedIonList As selectedIonList

        Shared ReadOnly methods As New Dictionary(Of String, String) From {
            {"beam-type collision-induced dissociation", "HCD"},
            {"collision-induced dissociation", "CID"}
        }

        Public Function GetActivationMethod() As String
            For Each par In activation.cvParams
                If methods.ContainsKey(par.name) Then
                    Return methods(par.name)
                End If
            Next

            Throw New NotImplementedException
        End Function

        Public Function GetCollisionEnergy() As Double
            Dim energy As String = activation.cvParams.FirstOrDefault(Function(a) a.name = "collision energy")?.value
            Return Val(energy)
        End Function

        Public Function GetMz() As Double
            Return isolationWindow.cvParams.KeyItem("isolation window target m/z").value
        End Function
    End Class
End Namespace
