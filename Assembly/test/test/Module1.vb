Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MSP

Module Module1

    Sub Main()

        Dim testMsp = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\HMDB00008.msp").ToArray


        Dim lysoPC = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\custom-lysoPC+Hpos.msp")

        Dim meta = lysoPC.First.Comments.LipidBlastParser

        Call meta.GetJson.__DEBUG_ECHO

        Pause()
    End Sub

End Module
