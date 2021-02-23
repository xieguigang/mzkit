Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.mzML.IonTargeted

    Public Class precursor : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params
        Public Property selectedIonList As selectedIonList

        Shared ReadOnly methods As New Dictionary(Of String, String) From {
            {"beam-type collision-induced dissociation", "CID"}
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