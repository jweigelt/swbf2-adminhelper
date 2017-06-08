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
'
'class for processing user command input
Public Class CommandHandler

    Public Property Commands As List(Of Command)
    Public Property CommandPrefix As String = "!"
    Public Property AdminIface As Core

    Sub New(ByVal adminIface As Core)
        Me.AdminIface = adminIface
        Me.Commands = New List(Of Command)
    End Sub
    Public Sub RCClient_newChat(ByVal sender As Object, ByVal playerName As String, ByVal chatText As String)
        If chatText.StartsWith(Me.CommandPrefix) Then
            Me.HandleCommand(Mid(chatText, 2), playerName)
        End If
    End Sub

    Public Sub HandleCommand(ByVal commandStr As String, ByVal playerName As String)
        Dim player As User = Me.AdminIface.PHandler.FetchUserByName(playerName)
        If player Is Nothing Then Return
        HandleCommand(commandStr, player)
    End Sub

    Public Sub HandleCommand(ByVal commandStr As String, ByVal player As User)
        If player Is Nothing Then Return
        Dim commandAlias As String = Split(commandStr, " ")(0)

        For Each cmd As Command In Me.Commands
            If player.IsSuperAdmin Then
                cmd.BySuperAdmin = True
            Else
                cmd.BySuperAdmin = False
            End If
            If cmd.CommandAlias.Contains(commandAlias) Then
                If Me.AdminIface.PHandler.HasPermission(player, cmd) Or cmd.IsPublic Then
                    Logger.Log(LogTemplate.CMD_EXECUTED, LogLevel.info, player.UserName, commandStr)
                    Me.AdminIface.SyncSheduler.PushTask(New SchedulerTask(AddressOf Me.CommandProxy, {cmd, commandStr, player}))
                    Return
                Else
                    Logger.Log(LogTemplate.CMD_NO_PERMISSION, LogLevel.info, player.UserName, commandStr)
                    Return
                End If
                Return
            End If
        Next
    End Sub

    Private Sub CommandProxy(ByVal params() As Object)
        Dim cmd As Command = DirectCast(params(0), Command)
        cmd.Execute(params(1), params(2))
    End Sub

    Public Sub RegisterDynCommand(ByVal c As DynCommand)
        c.adminIface = Me.AdminIface
        Me.Commands.Add(c)
    End Sub

    Public Sub RegisterCommand(ByVal cmd As Type, ByVal cfg As String)
        Dim CS As New ConfigSerializer(cmd)
        Dim c As Command = CS.loadFromFile("/" & cfg & ".xml", CurDir() & Constants.CFG_DIR & Constants.CFG_CMD_DIR)
        c.adminIface = Me.AdminIface
        Me.Commands.Add(c)
    End Sub

End Class
