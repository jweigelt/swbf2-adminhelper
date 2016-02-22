Public Class CmdCheckBalance
    Inherits Command

    Public Property BalanceStr As String = "%t: %c, %s: %n"

    Sub new()
        Me.CommandAlias = "checkbalance"
        Me.Permission = "swap"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim team1 As String
        Dim team2 As String
        Dim team1Count As Int32 = 0
        Dim team2Count As Int32 = 0

        For Each p As User in Me.adminIface.PHandler.PlayerList
            If team1 Is Nothing Then
                team1 = p.TeamName
            ElseIf team2 Is Nothing AndAlso p.TeamName <> team1 Then
                team2 = p.TeamName
            End If

            If p.TeamName = team1 Then
                team1Count += 1
            ElseIf p.TeamName = team2 Then
                team2Count += 1
            End If
        Next

        If team1 Is Nothing OrElse team2 Is Nothing Then
            Return False
        End If

        Me.Pm(Me.ParseTemplate(Me.BalanceStr, {team1, team1Count, team2, team2Count}, {"t", "c", "s", "n"}), player)

        Return True
    End Function

End Class
