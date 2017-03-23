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
'@author Yoni Lerner

Imports MySql.Data
Imports MySql.Data.MySqlClient
Imports System.Data.Common
Imports System.Data.SQLite
Imports System.Data.SqlClient

Public Class SQLHandler
    Public Property Hostname As String
    Public Property Port As Int32
    Public Property DbName As String
    Public Property DbUser As String
    Public Property DbPwd As String
    Public Property DbType As DbTypes
    Public Enum DbTypes
        MySQL
        SQLite
    End Enum

    Private connection

    Public Function Init() As Boolean
        Logger.Log(LogTemplate.SQL_TYPE, LogLevel.info, GetDbTypeString)
        If DbType = DbTypes.SQLite Then
            Console.WriteLine("SQLIte " & Me.DbName)
            connection = New SQLiteConnection("Data Source=" & Me.DbName & ".db")
        Else
            connection = New MySqlConnection
            Dim connectionString As String
            connectionString = "server=" &
                            Me.Hostname & ";port=" &
                            Me.Port.ToString & ";uid = " &
                            Me.DbUser & ";pwd=" &
                            Me.DbPwd & ";database=" &
                            Me.DbName & ";"
            connection.ConnectionString = connectionString
        End If

        Logger.Log(LogTemplate.SQL_CONNECT_TEST, LogLevel.info)

        Try
            connection.Open()
            Logger.Log(LogTemplate.SQL_CONNECT_OK, LogLevel.info)
            Return True
        Catch ex As Exception
            Logger.Log(LogTemplate.SQL_CONNECT_FAIL, LogLevel.critical)
        End Try

        Return False
    End Function

    Public Function GetDbTypeString() As String
        Return [Enum].GetName(GetType(DbTypes), DbType)
    End Function

    Public Function DoQuery(ByVal sql As String, Optional ByVal names As Array = Nothing, Optional ByVal values As Array = Nothing) As DbDataReader
        Dim reader As DbDataReader = Nothing

        Dim query
        If DbType = DbTypes.SQLite Then
            query = New SQLiteCommand(sql)
        Else
            query = New MySqlConnection(sql)
        End If

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

            If Not names Is Nothing AndAlso names.Length <> 0 Then
                query = AddCmdParams(query, names, values)
            End If

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

    Public Function AddCmdParams(ByRef query As Object, ByVal names As Array, ByVal values As Array) As Object
        With query.Parameters
            For i As Integer = 0 To names.Length - 1
                .AddWithValue(names(i), values(i))
            Next
        End With
        Return query
    End Function

    Public Function NonQuery(ByVal sql As String, Optional ByVal names As Array = Nothing, Optional ByVal values As Array = Nothing) As Int32
        SyncLock Me.connection
            Try
                Logger.Log("Query: " & sql, LogLevel.debug)

                If Not Me.connection.State = ConnectionState.Open Then
                    Me.connection.Open()
                End If

                Dim query
                If DbType = DbTypes.SQLite Then
                    query = New SQLiteCommand(sql)
                Else
                    query = New MySqlConnection(sql)
                End If

                If Not names Is Nothing AndAlso names.Length <> 0 Then
                    query = AddCmdParams(query, names, values)
                End If

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
        Logger.Log(LogTemplate.SQL_CLOSE, LogLevel.info)
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
    Private Function CheckForRows(ByVal res) As Boolean
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

    Private Function GetSQLNow() As String
        If DbType = DbTypes.SQLite Then
            Return "DateTime('now')"
        Else
            Return "NOW()"
        End If
    End Function

    Public Function UnbanPlayerByNameMatch(ByVal partialName As String) As String
        Dim sql As String = "
            SELECT `" & Constants.SQL_BANS_TABLE & "`.id, keyhash, username FROM `" & Constants.SQL_PLAYERS_TABLE & "`
            LEFT JOIN `" & Constants.SQL_BANS_TABLE & "` ON player=`" & Constants.SQL_PLAYERS_TABLE & "`.id
            WHERE keyhash IN (SELECT keyhash FROM `" & Constants.SQL_PLAYERS_TABLE & "` WHERE username LIKE @partialName)
            AND `" & Constants.SQL_BANS_TABLE & "`.id NOT NULL;
        "

        Dim bannedPlayerName As String = Nothing
        Dim bannedId As Int32 = -1
        Dim count As Int32 = 0
        Using res = Me.DoQuery(sql, {"@partialName"}, {"%" & partialName & "%"})
            If res.HasRows Then
                While res.Read()
                    If count >= 1 Then
                        Return Nothing
                    End If
                    count += 1
                    bannedId = res("id")
                    bannedPlayerName = res("username")
                End While
            End If
        End Using

        If bannedId = -1 OrElse bannedPlayerName Is Nothing OrElse bannedPlayerName.Length = 0 Then
            Return Nothing
        End If

        sql = "DELETE FROM `" & Constants.SQL_BANS_TABLE & "` WHERE id = @banid"
        Me.NonQuery(sql, {"@banid"}, {bannedId})
        Return bannedPlayerName
    End Function

    Public Function PlayerExists(ByVal player As User) As Boolean
        Dim sql As String = "
            SELECT id FROM `" & Constants.SQL_PLAYERS_TABLE & "`
            WHERE keyhash = @hash AND username = @name
        "
        Dim values As Array = {player.KeyHash, player.UserName}
        Dim names As Array = {"@hash", "@name"}
        Return Me.CheckForRows(Me.DoQuery(sql, names, values))
    End Function

    Public Sub RegisterPlayer(ByVal player As User)
        Dim sql As String = "
            INSERT INTO " & Constants.SQL_PLAYERS_TABLE & "
            (username, keyhash, lastip, lastseen) VALUES
            (@username, @keyhash, @lastip, " & GetSQLNow() & ")
        "
        Dim names As Array = {"@username", "@keyhash", "@lastip"}
        Dim values As Array = {player.UserName, player.KeyHash, player.IPAddress.ToString}
        Me.NonQuery(sql, names, values)
    End Sub

    Public Function FindRegisteredUser(ByVal partialName As String) As User
        Dim sql As String = "
            SELECT keyhash FROM `" & Constants.SQL_USERS_TABLE & "`
            WHERE username LIKE @partialName
            LIMIT 1
        "
        Using res = Me.DoQuery(sql, {"@partialName"}, {"%" & partialName & "%"})
            If res.HasRows Then
                res.Read()
                Dim user As User = New User
                user.IsRegistered = True
                user.KeyHash = res("keyhash")
                GetUserDetails(user)
                Return user
            Else
                Return Nothing
            End If
        End Using
    End Function

    Public Sub GetUserDetails(ByRef player As User)
        Dim sql As String = "
            SELECT * FROM `" & Constants.SQL_USERS_TABLE & "`
            LEFT JOIN `" & Constants.SQL_GROUPS_TABLE & "`
            ON " & "`" & Constants.SQL_GROUPS_TABLE & "`.`id` = `group`
            WHERE `keyhash` = @keyhash
        "
        Dim names As Array = {"@keyhash"}
        Dim values As Array = {player.KeyHash}
        Using res = Me.DoQuery(sql, names, values)
            If res.HasRows Then
                res.Read()
                player.IsRegistered = True
                player.UserId = res("id")
                player.GroupId = res("group")
                player.UserName = res("username")
                If Not IsDBNull("groupname") Then player.GroupName = res("groupname")
            End If
            res.Close()
        End Using
    End Sub

    Public Sub GetPlayerDetails(ByRef player As User)
        Dim sql As String = "
            SELECT * FROM `" & Constants.SQL_PLAYERS_TABLE & "`
            WHERE `keyhash` = @keyhash
        "
        Using res = Me.DoQuery(sql, {"@keyhash"}, {player.KeyHash})
            If res.HasRows Then
                res.Read()
                player.playerId = res("id")
            End If
            res.Close()
        End Using
    End Sub

    Public Function HasPermission(ByVal player As User, ByVal cmd As Command) As Boolean
        Dim sql As String =
          "SELECT `" & Constants.SQL_PERMISSIONS_TABLE & "`.`id` FROM `" &
                                 Constants.SQL_GROUPS_TABLE & "`, `" &
                                 Constants.SQL_PERMISSIONS_TABLE & "`, `" &
                                 Constants.SQL_USERS_TABLE & "` " &
          "WHERE ((`" & Constants.SQL_GROUPS_TABLE & "`.`id` = `" & Constants.SQL_PERMISSIONS_TABLE & "`.`groupid` AND `" &
                       Constants.SQL_USERS_TABLE & "`.`group` = `" & Constants.SQL_GROUPS_TABLE & "`.`id`) or `" &
                       Constants.SQL_USERS_TABLE & "`.`id` = `" & Constants.SQL_PERMISSIONS_TABLE & "`.`userid`) " &
          "AND `" & Constants.SQL_USERS_TABLE & "`.`keyhash` = @keyhash " &
          "AND `" & Constants.SQL_PERMISSIONS_TABLE & "`.`alias` = '" & cmd.Permission & "'"

        Using res = Me.DoQuery(sql, {"@keyhash"}, {player.KeyHash})
            Return Me.CheckForRows(res)
        End Using
    End Function

    Public Function QueryNameList(ByVal u As User, ByVal order As String, ByVal maxCount As Int32, ByVal ipSeek As Boolean, Optional ByVal ipExpression As String = "") As List(Of String)
        'TODO
        Dim sql As String = "SELECT `username` FROM `" & Constants.SQL_PLAYERS_TABLE & "` WHERE "
        If ipSeek Then
            sql &= "`lastip` " & ipExpression
        Else
            sql &= "`keyhash` = '" & Me.EscapeString(u.KeyHash) & "' "
        End If
        sql &= "ORDER BY `id` " & order & " LIMIT " & maxCount.ToString()

        Dim users As New List(Of String)
        Using res = Me.DoQuery(sql)
            While res.Read()
                users.Add(res("username"))
            End While
            res.Close()
        End Using
        Return users
    End Function

    Public Sub InsertBan(ByVal affectedUser As User, ByVal admin As User, ByVal ipBan As Boolean, Optional ByVal duration As Integer = -1)
        Me.GetPlayerDetails(affectedUser)
        Me.GetUserDetails(admin)
        Dim sql As String = "
            INSERT INTO " & Constants.SQL_BANS_TABLE & "
            (player, admin, duration, `type`, `time`) VALUES
            (@playerId, @adminId, @duration, @type, " & GetSQLNow() & ")
        "
        Dim names As Array = {"@playerId", "@adminId", "@duration", "@type"}
        Dim values As Array = {affectedUser.playerId, admin.UserId, duration.ToString, IIf(ipBan, "1", "0").ToString}
        Me.NonQuery(sql, names, values)
    End Sub

    Public Function IsBanned(ByVal player As User) As Boolean
        Dim sql As String = "SELECT " & Constants.SQL_BANS_TABLE & ".id FROM " & Constants.SQL_BANS_TABLE &
        " LEFT JOIN " & Constants.SQL_PLAYERS_TABLE & " on " & Constants.SQL_BANS_TABLE & ".player =" & Constants.SQL_PLAYERS_TABLE & ".id" &
        " WHERE ((`keyhash` = @keyhash AND `type` = 0) " &
        " OR (`lastip` = @lastip AND `type` = 1)) " &
        " AND (`time` + `duration` > " & GetSQLNow() & " OR `duration` < 0)"

        Dim names As Array = {"@keyhash", "@lastip"}
        Dim values As Array = {player.KeyHash, player.IPAddress.ToString()}
        Using r = Me.DoQuery(sql, names, values)
            If r.HasRows Then
                r.Close()
                Return True
            Else
                r.Close()
                Return False
            End If
        End Using
    End Function

    Public Function GetBans(Optional start_at As String = "0") As List(Of List(Of String))
        Dim limit As String = "1000"
        If start_at <> "0" Then
            limit = "10"
        End If
        Dim sql As String = "
            SELECT " & Constants.SQL_BANS_TABLE & ".id, `" & Constants.SQL_USERS_TABLE & "`.username as admin_name, `" & Constants.SQL_BANS_TABLE & "`.`time` as banned_on, duration, type,
            " & Constants.SQL_PLAYERS_TABLE & ".username, " & Constants.SQL_PLAYERS_TABLE & ".keyhash, lastip FROM " & Constants.SQL_BANS_TABLE & "
            LEFT JOIN " & Constants.SQL_PLAYERS_TABLE & " ON player=" & Constants.SQL_PLAYERS_TABLE & ".id
            LEFT JOIN `" & Constants.SQL_USERS_TABLE & "` ON `" & Constants.SQL_USERS_TABLE & "`.id=admin
            LIMIT @start_at, @limit;
        "
        Dim ret As List(Of List(Of String)) = New List(Of List(Of String))
        Using r = Me.DoQuery(sql, {"@start_at", "@limit"}, {start_at, limit})
            If r.HasRows Then
                While r.Read()
                    Dim item As List(Of String) = New List(Of String)
                    item.Add(r("id"))
                    item.Add(r("admin_name"))
                    item.Add(r("banned_on"))
                    item.Add(IIf(r("duration") = "-1", "Permanent", r("duration")))
                    item.Add(IIf(r("type") = "0", "KeyHash", "IP"))
                    item.Add(r("username"))
                    item.Add(r("keyhash"))
                    item.Add(r("lastip"))
                    ret.Add(item)
                End While
            Else
                Return Nothing
            End If
        End Using
        If ret.Count > 0 Then
            Return ret
        Else
            Return Nothing
        End If
    End Function

    Public Sub RunCleanup()
        Logger.Log(LogTemplate.SQL_CLEANUP, LogLevel.info)
        Dim sql As String = "DELETE FROM `" & Constants.SQL_BANS_TABLE & "` WHERE `time` + `duration` < " & GetSQLNow() & " AND `duration` > 0"
        Me.NonQuery(sql)
    End Sub

    Public Function GetGroupId(ByVal name As String) As Int32
        Dim sql As String = "SELECT `id` FROM `" & Constants.SQL_GROUPS_TABLE & "` WHERE `groupname` = @name"
        Using res = Me.DoQuery(sql, {"@name"}, {name})
            If res.HasRows Then
                res.Read()
                Return res("id")
            Else
                Return -1
            End If
        End Using
    End Function

    Public Sub PutGroup(ByVal userid As Int32, ByVal groupId As Int32)
        Dim sql As String = "UPDATE `" & Constants.SQL_USERS_TABLE & "` SET `group` = @groupId WHERE `id` = @userId"
        Dim names As Array = {"@groupId", "@userId"}
        Dim values As Array = {groupId.ToString(), userid.ToString()}
        Me.NonQuery(sql, names, values)
    End Sub

    Public Function FirstUser() As Boolean
        Dim sql As String = "SELECT `id` FROM `" & Constants.SQL_USERS_TABLE & "`"
        Using res = Me.DoQuery(sql)
            Return Not Me.CheckForRows(res)
        End Using
    End Function

    Public Sub RegisterUser(ByVal player As User)
        Dim sql As String = "
            INSERT INTO " & Constants.SQL_USERS_TABLE & "
            (username, keyhash, `group`) VALUES
            (@username, @keyhash, @groupId)
        "
        Dim names As Array = {"@username", "@keyhash", "@groupId"}
        Dim values As Array = {player.UserName, player.KeyHash, player.GroupId.ToString()}
        Me.NonQuery(sql, names, values)
    End Sub

End Class