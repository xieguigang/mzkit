Imports Microsoft.VisualBasic.Text

Namespace IUPAC.InChILayers

    ''' <summary>
    ''' ``AAAAAAAAAAAAAA-BBBBBBBBFV-P``
    ''' </summary>
    Public Class InChIKey

        Public ReadOnly Property MolecularSkeleton As String
            Get

            End Get
        End Property

        Public ReadOnly Property StereochemistryIsotopes As String
            Get

            End Get
        End Property

        Public ReadOnly Property NumberOfProtons As String
            Get
                Return Chr(ASCII.N + inchi.Charge.Proton)
            End Get
        End Property

        Public ReadOnly Property InChIVersion As String
            Get
                Return Chr(ASCII.A + inchi.Version)
            End Get
        End Property

        Public ReadOnly Property IsStandard As String
            Get
                If inchi.IsStandard Then
                    Return "S"
                Else
                    Return "N"
                End If
            End Get
        End Property

        ReadOnly inchi As InChI

        Sub New(inchi As InChI)
            Me.inchi = inchi
        End Sub

        Public Overrides Function ToString() As String
            Return $"{MolecularSkeleton}-{StereochemistryIsotopes}{IsStandard}{InChIVersion}-{NumberOfProtons}"
        End Function
    End Class
End Namespace