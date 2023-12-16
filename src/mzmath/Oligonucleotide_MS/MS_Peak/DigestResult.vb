Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class DigestResult

    Public Property nt As FastaSeq
    Public Property coverage As Double
    Public Property hit_sites As Boolean()
    Public Property matches As Match()
    Public Property mass_hits As MatchedInput()
    Public Property mass As Double()
    Public Property digest As TheoreticalDigestMass()

End Class
