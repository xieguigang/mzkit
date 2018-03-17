
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Text.Levenshtein

''' <summary>
''' Global alignment of two MS/MS matrix.
''' </summary>
Public Module GlobalAlignment

    ReadOnly da3 As DefaultValue(Of Tolerance) = New DAmethod() With {
        .da = 0.3
    }.Interface

    ''' <summary>
    ''' ### shared peak count
    ''' 
    ''' In the matrix ``M``, ``i`` and ``j`` are positions, where ``i`` is the
    ''' horizontal coordinate And ``j`` Is the vertical coordinate. For Each
    ''' cell in the matrix, a score Is calculated (Si,j). If the two compared
    ''' masses are a match, Then ``C`` the cost Is equal To 0. However, If
    ''' the two masses are outside the designated ppm Error window,
    ''' then the cost Is equal to 1.
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="subject"></param>
    ''' <returns></returns>
    Public Function NWGlobalAlign(query As LibraryMatrix, subject As LibraryMatrix, Optional tolerance As Tolerance = Nothing) As GlobalAlign(Of ms2)()
        Dim massEquals As Equals(Of ms2)
        Dim empty As New ms2 With {
            .mz = -1,
            .intensity = -1,
            .quantity = -1
        }

        With tolerance Or da3
            massEquals = Function(q, s)
                             Return .Assert(q.mz, s.mz)
                         End Function
        End With

        Dim nw As New NeedlemanWunsch(Of ms2)(query, subject, massEquals, empty, Function(ms2) ms2.ToString.First)
        Call nw.compute()

        Return nw.PopulateAlignments.ToArray
    End Function
End Module
