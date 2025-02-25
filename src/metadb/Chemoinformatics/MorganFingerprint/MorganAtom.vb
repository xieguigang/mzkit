Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models

Namespace MorganFingerprint

    Public Class MorganAtom : Inherits Atom

        Public Property Index As Integer
        Public Property Code As ULong

        Sub New(base As Atom)
            Atom = base.Atom
            Coordination = base.Coordination

            If TypeOf base Is MorganAtom Then
                Index = DirectCast(base, MorganAtom).Index
                Code = DirectCast(base, MorganAtom).Code
            End If
        End Sub

    End Class
End Namespace