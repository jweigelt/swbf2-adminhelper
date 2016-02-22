Public Class CmdDumpBans
    Inherits Command

    Public Property DumpFile As String = CurDir() & "\bans.csv"
    Public Property OnSuccess As String = "Banlist has been dumped at " & Me.DumpFile

    Sub New()
        Me.CommandAlias = "dumpbans"
        Me.Permission = "superadmin"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim bans As String = ""
        bans &= "ban_id,admin_name,time,duration,type,username,keyhash,ip" & vbCrLf
        Dim banList As List(Of List(Of String)) = Me.adminIface.SQL.GetBans()
        If banList Is Nothing OrElse banList.Count = 0 Then
            IO.File.WriteAllText(Me.DumpFile, bans)
            Return True
        End If
        For Each ban As List(Of String) In banList
            bans &= String.Format("{0},{1},{2},{3},{4},{5},{6},{7}", ban.ToArray)
            bans &= vbCrLf
        Next
        IO.File.WriteAllText(Me.DumpFile, bans)
        'IO.File.AppendAllText(Me.DumpFile, bans & vbCrLf)

        Me.Pm(Me.OnSuccess, player)

        Return True
    End Function

End Class
