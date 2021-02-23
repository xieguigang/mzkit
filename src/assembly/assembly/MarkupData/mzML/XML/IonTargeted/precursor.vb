Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

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

    End Class
End Namespace