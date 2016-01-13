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

'SQLite-Connector-Wrapper - just a boring Database-interface
'Check Threadsafe
Imports MySql.Data
Imports MySql.Data.MySqlClient

Public Class SQLiteHandler
    Public Property Hostname As String
    Public Property Port As Int32
    Public Property DbName As String
    Public Property DbUser As String
    Public Property DbPwd As String

    'Private connection As MySqlConnection
    Private connection As SQLite.SQLiteConnection

    Public Function Init() As Boolean
        'connection = New MySqlConnection
        connection = New SQLite.SQLiteConnection
        Dim connectionString As String = String.Empty

        connectionString = "Data Source=adminmod.db"

        connection.ConnectionString = connectionString

        Logger.Log(LogTemplate.MYSQL_CONNECT_TEST, LogLevel.info)

        Try
            connection.Open()
            Logger.Log(LogTemplate.MYSQL_CONNECT_OK, LogLevel.info)
            Return True
        Catch ex As Exception
            Logger.Log(LogTemplate.MYSQL_CONNECT_FAIL, LogLevel.critical)
        End Try
        Return False
    End Function

    Dim reader As SQLite.SQLiteDataReader = Nothing
    Public Function DoQuery(ByVal sql As String) As SQLite.SQLiteDataReader
        Dim query As SQLite.SQLiteCommand = Nothing

        Try
            Logger.Log("Query: " & sql, LogLevel.debug)
            If Not Me.connection.State = ConnectionState.Open Then
                Me.connection.Open()
            End If

            If Not reader Is Nothing Then
                While Not reader.IsClosed = True
                    Threading.Thread.Sleep(10)
                End While
            End If

            query = New SQLite.SQLiteCommand(sql)
            query.Connection = Me.connection
            query.Prepare()

            reader = query.ExecuteReader()

            Return reader
        Catch ex As Exception
            If Not reader Is Nothing Then
                If reader.IsClosed = False Then reader.Close()
            End If
            Logger.Log("Failed to execute Query " & sql & vbCrLf & ex.ToString, LogLevel.failure)
        End Try
        Return Nothing
    End Function
    Public Function NonQuery(ByVal sql As String) As Int32
        SyncLock Me.connection
            Dim query As SQLite.SQLiteCommand = Nothing
            Try
                Logger.Log("Query: " & sql, LogLevel.debug)

                If Not Me.connection.State = ConnectionState.Open Then
                    Me.connection.Open()
                End If

                query = New SQLite.SQLiteCommand(sql)
                query.Connection = Me.connection
                query.Prepare()
                Return query.ExecuteNonQuery()
            Catch ex As Exception
                Logger.Log("Failed to execute Query " & sql & vbCrLf & ex.ToString, LogLevel.failure)
                Return False
            End Try
        End SyncLock
    End Function
    Public Function EscapeString(ByVal sql As String) 'Die bösen Sachen filtern 
        Return MySqlHelper.EscapeString(sql)
    End Function
    Public Sub Terminate()
        Logger.Log(LogTemplate.MYSQL_CLOSE, LogLevel.info)
        If Me.connection.State = ConnectionState.Open Then
            Me.connection.Close()
        End If
        connection = Nothing
    End Sub
    Private Function GetUnixTimestamp(ByVal time As DateTime) As Int64
        Return (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
    End Function
    Private Function GetDateTime(ByVal timestamp As Int64) As DateTime
        Dim dt As New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        dt.AddSeconds(timestamp).ToLocalTime()
        Return dt
    End Function
    Private Function CheckForRows(ByVal res As SQLite.SQLiteDataReader) As Boolean
        If Not res Is Nothing Then
            res.Read()
            If res.HasRows Then
                res.Close()
                Return True
            End If
            res.Close()
        End If
        Return False
    End Function

    Public Function PlayerExists(ByVal player As User) As Boolean
        Dim sql As String =
            "select `id` from `" & Constants.MYSQL_PLAYERS_TABLE & "` " &
            "where `keyhash` = '" & player.KeyHash & "' and `username` = '" & EscapeString(player.UserName) & "'"
        Return Me.CheckForRows(Me.DoQuery(sql))
    End Function

    Public Sub RegisterPlayer(ByVal player As User)
        Dim sql As String =
            "insert into `" & Constants.MYSQL_PLAYERS_TABLE & "` set " &
             "`username`='" & EscapeString(player.UserName) & "', " &
             "`keyhash`='" & EscapeString(player.KeyHash) & "', " &
             "`lastip`='" & EscapeString(player.IPAddress.ToString) & "', " &
             "`lastseen` = NOW()"
        Me.NonQuery(sql)
    End Sub

    Public Sub GetUserDetails(ByRef player As User)
        Dim sql As String = "select * from `" & Constants.MYSQL_USERS_TABLE & "` " &
                            "left join `" & Constants.MYSQL_GROUPS_TABLE & "` on " & "`" & Constants.MYSQL_GROUPS_TABLE & "`.`id` = " &
                            "`group` where `keyhash` = '" & player.KeyHash & "'"

        Using res As SQLite.SQLiteDataReader = Me.DoQuery(sql)
            If res.HasRows Then
                res.Read()
                player.IsRegistered = True
                player.UserId = res("id")
                player.GroupId = res("group")
                If Not IsDBNull("groupname") Then player.GroupName = res("groupname")
            End If
            res.Close()
        End Using
    End Sub

    Public Sub GetPlayerDetails(ByRef player As User)
        Dim sql As String = "select * from `" & Constants.MYSQL_PLAYERS_TABLE & "` where `keyhash` = '" & player.KeyHash & "'"

        Using res As SQLite.SQLiteDataReader = Me.DoQuery(sql)
            If res.HasRows Then
                res.Read()
                player.playerId = res("id")
            End If
            res.Close()
        End Using
    End Sub

    Public Function HasPermission(ByVal player As User, ByVal cmd As Command) As Boolean
        Dim sql As String =
          "select `" & Constants.MYSQL_PERMISSIONS_TABLE & "`.`id` from `" &
                                 Constants.MYSQL_GROUPS_TABLE & "`, `" &
                                 Constants.MYSQL_PERMISSIONS_TABLE & "`, `" &
                                 Constants.MYSQL_USERS_TABLE & "` " &
          "where ((`" & Constants.MYSQL_GROUPS_TABLE & "`.`id` = `" & Constants.MYSQL_PERMISSIONS_TABLE & "`.`groupid` and `" &
                       Constants.MYSQL_USERS_TABLE & "`.`group` = `" & Constants.MYSQL_GROUPS_TABLE & "`.`id`) or `" &
                       Constants.MYSQL_USERS_TABLE & "`.`id` = `" & Constants.MYSQL_PERMISSIONS_TABLE & "`.`userid`) " &
          "and `" & Constants.MYSQL_USERS_TABLE & "`.`keyhash` = '" & EscapeString(player.KeyHash) & "' " &
          "and `" & Constants.MYSQL_PERMISSIONS_TABLE & "`.`alias` = '" & cmd.Permission & "'"
        Using res As SQLite.SQLiteDataReader = Me.DoQuery(sql)
            Return Me.CheckForRows(res)
        End Using
    End Function

    Public Function QueryNameList(ByVal u As User, ByVal order As String, ByVal maxCount As Int32, ByVal ipSeek As Boolean, Optional ByVal ipExpression As String = "") As List(Of String)
        Dim sql As String = "select `username` from `" & Constants.MYSQL_PLAYERS_TABLE & "` where "
        If ipSeek Then
            sql &= "`lastip` " & ipExpression
        Else
            sql &= "`keyhash` = '" & Me.EscapeString(u.KeyHash) & "' "
        End If
        sql &= "order by `id` " & order & " limit " & maxCount.ToString()

        Dim users As New List(Of String)
        Using res As SQLite.SQLiteDataReader = Me.DoQuery(sql)
            While res.Read()
                users.Add(res("username"))
            End While
            res.Close()
        End Using
        Return users
    End Function

    Public Sub InsertBan(ByVal affectedUser As User, ByVal admin As User, ByVal ipBan As Boolean, Optional ByVal duration As Int16 = -1)
        Me.GetPlayerDetails(affectedUser)
        Me.GetUserDetails(admin)
        Dim sql As String = "insert into `" & Constants.MYSQL_BANS_TABLE & "` set " &
            "`player` = " & affectedUser.playerId &
            ", `admin` = " & admin.UserId &
            ", `duration` = " & duration.ToString &
            ", type = " & IIf(ipBan, "1", "0").ToString &
            ", time = strftime('%s', 'now')"
        Me.NonQuery(sql)
    End Sub

    Public Function IsBanned(ByVal player As User) As Boolean
        Dim sql As String = "select `username` from `" & Constants.MYSQL_PLAYERS_TABLE & "` " &
        "right join `" & Constants.MYSQL_BANS_TABLE & "` on " & "`" & Constants.MYSQL_PLAYERS_TABLE & "`.`id` = `player`" &
        " where ((`keyhash` = '" & Me.EscapeString(player.KeyHash) & "' and `type` = 0) " &
        " or (`lastip` = '" & player.IPAddress.ToString() & "' and `type` = 1)) " &
        " and (`time` + `duration` > strftime('%s', 'now') or `duration` < 0)"

        Using r As SQLite.SQLiteDataReader = Me.DoQuery(sql)
            If r.HasRows Then
                r.Close()
                Return True
            Else
                r.Close()
                Return False
            End If
        End Using
    End Function

    Public Sub RunCleanup()
        Logger.Log(LogTemplate.MYSQL_CLEANUP, LogLevel.info)
        Dim sql As String = "delete from `" & Constants.MYSQL_BANS_TABLE & "` where `time` + `duration` < strftime('%s', 'now') and `duration` > 0"
        Me.NonQuery(sql)
    End Sub

    Public Function GetGroupId(ByVal name As String) As Int32
        name = EscapeString(name)
        Dim sql As String = "select `id` from `" & Constants.MYSQL_GROUPS_TABLE & "` where `groupname` = '" & name & "'"
        Using res As SQLite.SQLiteDataReader = Me.DoQuery(sql)
            If res.HasRows Then
                res.Read()
                Return res("id")
            Else
                Return -1
            End If
        End Using
    End Function

    Public Sub PutGroup(ByVal userid As Int32, ByVal groupId As Int32)
        Dim sql As String = "update `" & Constants.MYSQL_USERS_TABLE & "` set `group` = " & groupId.ToString() & " where `id` = " & userid.ToString()
        Me.NonQuery(sql)
    End Sub

    Public Function FirstUser() As Boolean
        Dim sql As String = "select `id` from `" & Constants.MYSQL_USERS_TABLE & "`"
        Using res As SQLite.SQLiteDataReader = Me.DoQuery(sql)
            Return Not Me.CheckForRows(res)
        End Using
    End Function

    Public Sub RegisterUser(ByVal player As User)
        Dim sql As String =
            "insert into `" & Constants.MYSQL_USERS_TABLE & "` set " &
             "`username`='" & EscapeString(player.UserName) & "', " &
             "`keyhash`='" & EscapeString(player.KeyHash) & "', " &
             "`group` = " & player.GroupId
        Me.NonQuery(sql)
    End Sub

End Class