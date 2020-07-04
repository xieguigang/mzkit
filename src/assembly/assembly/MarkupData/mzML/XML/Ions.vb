Imports System.Xml.Serialization

Namespace MarkupData.mzML

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

    Public Class selectedIonList : Inherits List

        <XmlElement>
        Public Property selectedIon As Params()

        Public Function GetIonMz() As Double()
            Return selectedIon _
                .Select(Function(ion)
                            Return ion.cvParams.FirstOrDefault(Function(a) a.name = "selected ion m/z")?.value
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
        End Function

        Public Function GetIonIntensity() As Double()
            Return selectedIon _
                .Select(Function(ion)
                            Return ion.cvParams.FirstOrDefault(Function(a) a.name = "peak intensity")?.value
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
        End Function

    End Class

    Public Class product : Implements IMRMSelector

        Public Property isolationWindow As Params Implements IMRMSelector.isolationWindow
        Public Property activation As Params

    End Class
End Namespace