Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MSP

Module Module1

    Sub Main()






        Dim testMsp = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\HMDB00008.msp").ToArray


        Dim lysoPC = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\custom-lysoPC+Hpos.msp")

        Dim meta = lysoPC.First.Comments.LipidBlastParser

        Call meta.DictionaryTable(Function(x)
                                      If x Is Nothing Then
                                          Return False
                                      ElseIf {GetType(Integer), GetType(Double), GetType(Long)}.IndexOf(x.GetType) > -1 Then
                                          If Val(x) = 0R Then
                                              Return False
                                          Else
                                              Return True
                                          End If
                                      Else
                                          Return True
                                      End If
                                  End Function).GetJson.__DEBUG_ECHO


        Pause()
    End Sub

End Module
