Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.MassSpectrum.DATA.MetaLib

Namespace MoNA

    Public Class SpectraSection : Inherits MetaInfo

        Public Property xref As xref
        Public Property SpectraInfo As Dictionary(Of String, String)
        Public Property Comment As Dictionary(Of String, String)
        Public Property MassPeaks As DataSet()

    End Class
End Namespace