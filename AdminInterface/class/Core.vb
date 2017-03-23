'This file is part of SWBF2 SADS-Administation Helper.
'
'SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'SWBF2 SADS-Administation Helper is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with SWBF2 SADS-Administation Helper.  If not, see <http://www.gnu.org/licenses/>.

'Core class
Public Class Core
    Public Property RCClient As RconClient
    Public Property CCSerializer As ConfigSerializer
    Public Property SyncSheduler As Sheduler
    Public Property PHandler As PlayerHandler
    Public Property CHandler As CommandHandler
    Public Property AHandler As AnnounceHandler

    Public Property DCSerializer As ConfigSerializer
    Public Property DCConfig As DyncommandConfig

    Public Property SQL As SQLHandler
    Public Property IGTSerializer As ConfigSerializer
    Public Property IGTemplate As IngameTemplate
    Public Property MemReader As ProcessMemoryReader

    Public Property Config As CoreConfig

    Public Sub Run()
        Me.PreInit()
        If Not (Me.Init()) Then Console.ReadLine()
        Me.PostInit()
    End Sub

    Private Sub PreInit()
        'Me.PerformUpdate()
        Logger.Log(LogTemplate.CORE_PREINIT, LogLevel.debug)
        Me.RCClient = New RconClient()
        Me.CCSerializer = New ConfigSerializer(GetType(CoreConfig))
        Me.IGTSerializer = New ConfigSerializer(GetType(IngameTemplate))
        Me.DCSerializer = New ConfigSerializer(GetType(DyncommandConfig))
        Me.SyncSheduler = New Sheduler()
        Me.SQL = New SQLHandler()
        Me.PHandler = New PlayerHandler(Me)
        Me.CHandler = New CommandHandler(Me)
        Me.MemReader = New ProcessMemoryReader()
        Me.AHandler = New AnnounceHandler(Me)
    End Sub

    Private Function Init() As Boolean
        Logger.Log(LogTemplate.CORE_INIT, LogLevel.debug)
        Me.Config = Me.CCSerializer.loadFromFile(Constants.CFG_CORE, CurDir() & Constants.CFG_DIR)
        Logger.MinLevel = Me.Config.MinLogLevel
        Logger.LogFile = CurDir() & "/" & Me.Config.LogFile
        Logger.LogToFile = Me.Config.LogToFile
        Logger.ExitOnError = Me.Config.ExitOnError
        Me.IGTemplate = Me.IGTSerializer.loadFromFile(Constants.CFG_IGAMETEMPLATE, CurDir() & Constants.CFG_DIR)
        Me.DCConfig = Me.DCSerializer.loadFromFile(Constants.CFG_DYNCOMMAND, CurDir() & Constants.CFG_DIR)

        With Me.SQL
            If Me.Config.UseMySQL Then
                .Hostname = Me.Config.MySQLHostname
                .Port = Me.Config.MySQLPort
                .DbUser = Me.Config.MySQLUser
                .DbPwd = Me.Config.MySQLPassword
                .DbName = Me.Config.SQLDatabase
                .DbType = SQLHandler.DbTypes.MySQL
            Else
                .DbName = Me.Config.SQLDatabase
                .DbType = SQLHandler.DbTypes.SQLite
            End If

            If Not .Init() Then Return False
            .RunCleanup()
        End With

        'MemoryReader
        If Me.Config.MemReaderRequired Then
            MemReader.ApplicationPath = Me.Config.ApplicationPath
            If Not MemReader.Init Then Return False
            If Me.Config.LoginAutoFetch Then
                Me.Config.RconPassword = MemReader.ReadAdminPassword()
                Logger.Log(LogTemplate.PROC_FETCH_PWD, LogLevel.info, Me.Config.RconPassword)
                If String.IsNullOrEmpty(Me.Config.RconPassword) Then
                    Logger.Log(LogTemplate.PROC_SET_PWD, LogLevel.info)
                    Me.Config.RconPassword = RandStr()
                    Me.MemReader.SetAdminPassword(Me.Config.RconPassword)
                End If
            End If
        End If

        'RCON-Client
        With Me.RCClient
            .ServerIP = Me.Config.RconHostname
            .ServerPort = Me.Config.RconPort
            .ServerPwd = Me.Config.RconPassword
            AddHandler .GameEnded, AddressOf Me.Server_GameEnded
            If Not .init() Then Return False
        End With

        If Me.Config.LoginDisable Then
            Logger.Log(LogTemplate.PROC_RM_PWD, LogLevel.info)
            Me.MemReader.DisableIngameLogin()
        End If

        'Query the server to get some more info (next map, slots etc.)
        Logger.Log(LogTemplate.CORE_QUERY_INFO, LogLevel.info)
        Dim queryPacket As New ServerQueryPacket
        Me.RCClient.SendPacket(queryPacket)
        If Not queryPacket.PacketOK Then Return False

        Me.Config.ServerInfo = queryPacket.ServerInfo
        With queryPacket
            Logger.Log(LogTemplate.CORE_QUERY_INFO_OK, LogLevel.info, { .ServerInfo.ServerName,
                                                                       .ServerInfo.Version,
                                                                       .ServerInfo.MaxPlayers})
        End With

        'Playerhandler
        With Me.PHandler
            .RconPlayerHandling = Me.Config.RconPlayerHandling
            .Slots = Me.Config.ServerInfo.MaxPlayers
            If Not .Init() Then Return False
            Me.SyncSheduler.PushRepeatingTask(New RepeatingShedulerTask(AddressOf .UpdatePlayerList), Me.Config.ListRefreshDelay)
        End With

        'CommandHandler
        With Me.CHandler
            .CommandPrefix = Me.Config.CommandPrefix
            .RegisterCommand(GetType(CmdNextmap), "nextmap")
            .RegisterCommand(GetType(CmdInfo), "info")
            .RegisterCommand(GetType(CmdKick), "kick")
            .RegisterCommand(GetType(CmdBan), "ban")
            .RegisterCommand(GetType(CmdUnban), "unban")
            .RegisterCommand(GetType(CmdIpBan), "ipban")
            .RegisterCommand(GetType(CmdTempIpBan), "tempipban")
            .RegisterCommand(GetType(CmdDumpBans), "dumpbans")
            .RegisterCommand(GetType(CmdListBans), "listbans")
            .RegisterCommand(GetType(CmdEndgame), "endgame")
            .RegisterCommand(GetType(CmdBots), "bots")
            .RegisterCommand(GetType(CmdSwap), "swap")
            .RegisterCommand(GetType(CmdCheckBalance), "checkbalance")
            .RegisterCommand(GetType(CmdTempBan), "tempban")
            .RegisterCommand(GetType(CmdTest), "test")
            .RegisterCommand(GetType(CmdNameQuery), "nquery")
            .RegisterCommand(GetType(CmdShutdown), "shutdown")
            .RegisterCommand(GetType(CmdPutGroup), "putgroup")
            .RegisterCommand(GetType(CmdFirst), "first")
            .RegisterCommand(GetType(CmdSetNextMap), "setnextmap")
            .RegisterCommand(GetType(CmdStats), "stats")
            .RegisterCommand(GetType(CmdTeamStats), "teamstats")
            .RegisterCommand(GetType(CmdTimer), "timer")
            .RegisterCommand(GetType(CmdRestart), "restart")
            .RegisterCommand(GetType(CmdScore), "score")
            .RegisterCommand(GetType(CmdCommands), "commands")
            .RegisterCommand(GetType(CmdMods), "mods")
            If Me.DCConfig.EnableDyncommandHandler Then
                For Each c As DynCommand In Me.DCConfig.Commands
                    Logger.Log(LogTemplate.CMD_DYN_REG, LogLevel.info, c.CommandAlias)
                    .RegisterDynCommand(c)
                Next
            End If
            AddHandler Me.RCClient.NewChat, AddressOf .RCClient_newChat
        End With

        'AnnounceHandler
        If Not Me.AHandler.Init Then Return False
        Me.SyncSheduler.Init()

        Logger.Log(LogTemplate.CORE_IDLE, LogLevel.info)

        Dim running As Boolean = True
        While running
            Dim cmd As String = Console.ReadLine
            If cmd.Length > 1 Then
                If cmd(0) = Config.CommandPrefix Then
                    CHandler.HandleCommand(Mid(cmd, 2), User.GetSuperAdmin)
                ElseIf cmd(0) = "/" Then
                    Dim p As New GenericCommandPacket
                    p.CommandAlias = Mid(cmd, 2)
                    Me.SyncSheduler.PushTask(New ShedulerTask(AddressOf Me.SendUserPacket, p))
                Else
                    Select Case cmd
                        Case Constants.CMD_EXIT
                            running = False
                    End Select
                End If
            End If
        End While

        Logger.Log(LogTemplate.CORE_SHUTDOWN, LogLevel.debug)
        Return True
    End Function

    Public Sub Shutdown()
        End
    End Sub

    Private Sub SendUserPacket(ByVal p As GenericCommandPacket)
        Me.RCClient.SendPacket(p)
        Logger.Log(vbCrLf & p.Response, LogLevel.info)
    End Sub

    Private Sub PostInit()
        Logger.Log(LogTemplate.CORE_POSTINIT, LogLevel.debug)
        If Not Me.RCClient Is Nothing Then Me.RCClient.Terminate()
        If Not Me.PHandler Is Nothing Then Me.PHandler.Terminate()
        If Not Me.SQL Is Nothing Then Me.SQL.Terminate()
        If Not Me.MemReader Is Nothing Then Me.MemReader.Terminate()

        Me.DCSerializer = Nothing
        Me.DCConfig = Nothing
        Me.RCClient = Nothing
        Me.PHandler = Nothing
        Me.Config = Nothing
        Me.CCSerializer = Nothing
        Me.PHandler = Nothing
        Me.MemReader = Nothing
        GC.Collect()
    End Sub

    Private Sub PerformUpdate()
        Logger.Log(LogTemplate.CORE_UPDATE_CHECK, LogLevel.info)
        Dim wc As New Net.WebClient
        Try
            Dim ver As String = wc.DownloadString(Constants.UPDATE_VERSION_URL)
            ver = Mid(ver, 1, ver.Length - 1)
            If ver <> Constants.PRODUCT_VERSION Then
                Logger.Log(LogTemplate.CORE_UPDATE_PULL, LogLevel.info, ver)

                If IO.File.Exists(CurDir() & Constants.UPDATE_UPDATEFILE) Then
                    IO.File.Delete(CurDir() & Constants.UPDATE_UPDATEFILE)
                End If

                If IO.File.Exists(CurDir() & Constants.UPDATE_BACKUPFILE) Then
                    IO.File.Delete(CurDir() & Constants.UPDATE_BACKUPFILE)
                End If

                wc.DownloadFile(Constants.UPDATE_WGET_URL, CurDir() & Constants.UPDATE_UPDATEFILE)
                IO.File.Move(CurDir() & Constants.UPDATE_MAINEXECUTABLE, CurDir() & Constants.UPDATE_BACKUPFILE)
                IO.File.Move(CurDir() & Constants.UPDATE_UPDATEFILE, CurDir() & Constants.UPDATE_MAINEXECUTABLE)
                Logger.Log(LogTemplate.CORE_UPDATE_DONE, LogLevel.info)

                Process.Start(CurDir() & Constants.UPDATE_MAINEXECUTABLE)
                End
            Else
                Logger.Log(LogTemplate.CORE_UPDATE_NO, LogLevel.info)
            End If
        Catch ex As Exception
            Logger.Log(LogTemplate.CORE_UPDATE_FAIL, LogLevel.failure, ex.ToString())
        End Try

    End Sub

    Private Sub Server_GameEnded(ByVal sender As Object)
        Dim t As New ShedulerTask(AddressOf Me.QueryServerInfo)
        Me.SyncSheduler.PushTask(t)
        Me.PHandler.PlayerPointsTracker.Clear()

        'For Each kvp As KeyValuePair(Of String, List(Of Int32)) In Me.PHandler.PlayerPointsTracker
        'Me.PHandler.PlayerPointsTracker(kvp.Key) = New List(Of Int32) From {0, 0}
        'Next
    End Sub

    Private Sub QueryServerInfo()
        Threading.Thread.Sleep(5000)
        Dim queryPacket As New ServerQueryPacket
        Me.RCClient.SendPacket(queryPacket)
        If Not queryPacket.PacketOK Then Return
        Me.Config.ServerInfo = queryPacket.ServerInfo
    End Sub

    Private Function RandStr() As String
        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
        Dim r As New Random
        Dim sb As New Text.StringBuilder
        For i As Integer = 1 To 8
            Dim idx As Integer = r.Next(0, 35)
            sb.Append(s.Substring(idx, 1))
        Next
        Return sb.ToString()
    End Function

End Class
