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
'"Dynamic" command (defined by the user)
'this class is used to wrap the command so it can be managed to the commandhandler
Public Class DynCommand
    Inherits Command

    Public Property EnableVarParser As Boolean = True
    Private _commandAction As String = String.Empty
    Public Property CommandAction As String
        Get
            Return Me._commandAction
        End Get
        Set(value As String)
            'replace \t s which were used to align the actions in the XML-file
            Me._commandAction = Replace(value, vbTab, String.Empty)
        End Set
    End Property

    Public Overrides Function Execute(commandStr As String, player As User) As Boolean

        Dim rows() As String = Split(Me.CommandAction, vbCrLf)
        For Each row As String In rows

            'commands with more then 100 chars might crash the server -> trim them
            If row.Length > 100 Then
                row = Mid(row, 1, 100)
                Logger.Log(LogTemplate.CMD_DYN_CUT, LogLevel.info, row)
            End If

            'Dynamic variables
            'Parsing text takes a bit of processing time so it can be disabled
            If Me.EnableVarParser Then
                row = Me.ParseTemplate(row,
                                 {player.UserName,
                                  player.SlotId.ToString(),
                                  player.GroupName.ToString(),
                                  player.Kills.ToString(),
                                  player.Ping.ToString(),
                                  player.TeamName.ToString()},
                                 {"p:name", "p:id", "p:group", "p:kills", "p:ping", "p:team"})

                'Serverinfo (queried after Map-reloads)
                With Me.adminIface.Config.ServerInfo
                    row = Me.ParseTemplate(row,
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

                'Just the current time
                row = Me.ParseTemplate(row,
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
            End If

            Me.adminIface.RCClient.SendRaw(row)
        Next
        Return True
    End Function
End Class