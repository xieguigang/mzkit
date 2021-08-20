Imports ServiceHub

Module ServiceHub

    Dim services As Process

    Public Sub Start()
        services = Global.ServiceHub.Protocols.StartServer
    End Sub

End Module
