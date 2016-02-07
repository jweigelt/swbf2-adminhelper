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
'Class for handling Players, this also gives a consistent interface to access the players
'even though the playerlist may be retrieved using different methods
Public Class PlayerHandler

    Public Property RconPlayerHandling As Boolean = False
    Public Property Slots As Byte
    Public Property AdminIface As Core
    Public Property AdminList As List(Of User)
    Public Property PlayerList As List(Of User)
    'Private oldPlayerList As List(Of User)

    Public Event PlayerJoined(ByVal sender As Object, ByVal player As User)
    Public Event NewPlayerJoined(ByVal sender As Object, ByVal player As User)
    Public Event OldPlayerJoined(ByVal sender As Object, ByVal player As User)
    Public Event BannedPlayerJoined(ByVal sender As Object, ByVal player As User)

    Sub New(ByVal AdminIface As Core)
        Me.PlayerList = New List(Of User)
        Me.AdminIface = AdminIface
    End Sub

    Public Function Init() As Boolean
        Return True
    End Function

    Public Sub Terminate()
        If Not Me.AdminList Is Nothing Then Me.AdminList.Clear()
        'If Not Me.oldPlayerList Is Nothing Then Me.oldPlayerList.Clear()
        If Not Me.PlayerList Is Nothing Then Me.PlayerList.Clear()

        Me.AdminIface = Nothing
        Me.AdminList = Nothing
        Me.PlayerList = Nothing
        'Me.oldPlayerList = Nothing
    End Sub

    Public Sub UpdatePlayerList()
        Dim plist As List(Of User)

        If Me.RconPlayerHandling Then 'fetch it via network
            Dim PLP As New PlayerlistPacket
            Me.AdminIface.RCClient.SendPacket(PLP)

            If PLP.PacketOK = False Then
                plist = New List(Of User)
            Else
                plist = PLP.PlayerList
            End If
        Else 'fetch it from memory
            plist = New List(Of User)
            For i = 1 To Me.Slots
                Dim player As User
                player = Me.AdminIface.MemReader.ReadPlayer(i)
                If player.KeyHash <> String.Empty Then
                    plist.Add(player)
                End If
            Next
        End If

        If Not Me.PlayerList Is Nothing Then
            'Check if any events have to be triggered
            For Each player As User In plist
                If player.KeyHash <> String.Empty Then
                    Dim u As User = CheckForKeyhash(player, Me.PlayerList)

                    If u Is Nothing Then
                        player.IsBanned = Me.AdminIface.SQL.IsBanned(player)
                        Me.AdminIface.SQL.GetUserDetails(player)
                        Me.OnPlayerJoin(player)
                    Else
                        player.IsBanned = u.IsBanned
                    End If
                    If player.IsBanned Then Me.KickPlayer(player)
                End If
            Next
        End If

        Me.PlayerList = plist
    End Sub

    'You might be able to guess what this one does
    Private Sub KickPlayer(ByVal player As User)
        Me.AdminIface.RCClient.SendRaw("boot " & player.SlotId)
    End Sub

    Private Function CheckForKeyhash(ByVal player As User, ByVal playerList As List(Of User)) As User
        For Each p As User In playerList
            If p.KeyHash = player.KeyHash Then
                Return p
            End If
        Next
        Return Nothing
    End Function

    Private Sub OnPlayerJoin(ByVal player As User)
        'trigger events for a new player
        RaiseEvent PlayerJoined(Me, player)
        If Not Me.AdminIface.SQL.PlayerExists(player) Then
            Me.AdminIface.SQL.RegisterPlayer(player)
            RaiseEvent NewPlayerJoined(Me, player)

            If Me.AdminIface.IGTemplate.EnableOnNewPlayerJoin Then
                Me.AdminIface.SQL.GetPlayerDetails(player)
                Me.AdminIface.RCClient.Say(
                IngameTemplate.Parse(
                    Me.AdminIface.IGTemplate.OnNewPlayerJoin,
                    {player.UserName, player.playerId.ToString()}, {"u", "i"}))
            End If

        ElseIf player.IsBanned Then
            RaiseEvent BannedPlayerJoined(Me, player)
            If Me.AdminIface.IGTemplate.EnableOnBannedPlayerJoin Then
                Me.AdminIface.RCClient.Say(
                IngameTemplate.Parse(
                    Me.AdminIface.IGTemplate.OnBannedPlayerJoin,
                    {player.UserName}, {"u"}))
            End If

        ElseIf player.IsRegistered Then
            If Me.AdminIface.IGTemplate.EnableOnRegisteredPlayerJoin Then
                Me.AdminIface.RCClient.Say(
                IngameTemplate.Parse(
                    Me.AdminIface.IGTemplate.OnRegisteredPlayerJoin,
                    {player.GroupName, player.UserName}, {"g", "u"}))
            End If

        Else

            If Me.AdminIface.IGTemplate.EnableOnRegisteredPlayerJoin Then
                Me.AdminIface.RCClient.Say(
               IngameTemplate.Parse(
                    Me.AdminIface.IGTemplate.OnRegisteredPlayerJoin,
                    {player.UserName}, {"u"}))
            End If
            RaiseEvent OldPlayerJoined(Me, player)
        End If
    End Sub


    'Some methods to fetch a user by giving different properties
    Public Function FetchUserByName(ByVal username As String)
        For Each p As User In Me.PlayerList
            If p.UserName = username Then
                Return p
            End If
        Next
        Return Nothing
    End Function

    Public Function FetchUserByKeyhash(ByVal keyhash As String)
        For Each p As User In Me.PlayerList
            If p.KeyHash = keyhash Then
                Return p
            End If
        Next
        Return Nothing
    End Function

    Public Function FetchUserByNameMatch(ByVal needle As String) As User
        Dim result As User = Nothing
        Dim results As Integer = 0
        For Each p As User In Me.PlayerList
            If p.UserName.ToLower.Contains(needle.ToLower) Then
                Me.AdminIface.SQL.GetUserDetails(p)
                result = p
                results += 1
            End If
        Next
        If results = 1 Then
            Return result
        End If
        Return Nothing
    End Function

    Public Function HasPermission(ByVal player As User, ByVal cmd As Command)
        If player.IsSuperAdmin Then Return True
        Return Me.AdminIface.SQL.HasPermission(player, cmd)
    End Function
End Class