Public Class AnnounceHandler

    Private aCSerializer As ConfigSerializer
    Private config As AnnounceConfig
    Private adminIface As Core
    Private currentIndex As Int32 = 0

    Sub New(ByVal adminIface As Core)
        Me.adminIface = adminIface
    End Sub

    Public Function Init() As Boolean
        Me.aCSerializer = New ConfigSerializer(GetType(AnnounceConfig))
        Me.config = Me.aCSerializer.loadFromFile(Constants.CFG_ANNOUNCE, CurDir() & Constants.CFG_DIR)
        If Me.config.AnnounceList.Count > 0 Then
            Me.adminIface.SyncSheduler.PushRepeatingTask(New RepeatingShedulerTask(AddressOf Me.DoAnnounce), Me.config.AnnounceInterval)
        End If
        Return True
    End Function

    Private Sub DoAnnounce()
        If Me.adminIface.PHandler.PlayerList.Count = 0 Then
            Logger.Log(LogTemplate.ANNOUNCE_SKIP, LogLevel.debug)
            Return
        End If

        If Me.currentIndex = Me.config.AnnounceList.Count Then Me.currentIndex = 0
        Dim announce As String = Me.config.AnnounceList(Me.currentIndex)
        If Me.config.EnableVarParser Then
            With Me.adminIface.Config.ServerInfo
                announce = IngameTemplate.Parse(announce,
                           {.ServerName,
                            .MaxPlayers,
                            .Version,
                            .ServerIP,
                            .Heroes,
                            .NextMap,
                            .CurrentMap,
                            .FFEnabled,
                            Me.adminIface.PHandler.PlayerList.Count.ToString()
                            },
                           {"s:name",
                            "s:maxplayers",
                            "s:version",
                            "s:ip",
                            "s:heroes",
                            "s:nextmap",
                            "s:map",
                            "s:ff",
                            "s:players"})
            End With
            announce = IngameTemplate.Parse(announce,
                            {Now.Day.ToString(),
                             Now.Month.ToString(),
                             Now.Year.ToString(),
                             Now.Hour.ToString(),
                             Now.Minute.ToString(),
                             Now.Second.ToString()},
                            {"t:d",
                             "t:c",
                             "t:y",
                             "t:h",
                             "t:m",
                             "t:s"})
            announce = IngameTemplate.Parse(announce, {Constants.PRODUCT_BANNER}, {"banner"})
        End If

        Me.adminIface.RCClient.Say(announce)
        Me.currentIndex += 1
    End Sub

    Public Sub Terminate()
        Me.adminIface = Nothing
        Me.aCSerializer = Nothing
        Me.config = Nothing
    End Sub

End Class
