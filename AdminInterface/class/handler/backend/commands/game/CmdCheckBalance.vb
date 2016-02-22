Public Class CmdCheckBalance
    Inherits Command

    Public Property BalanceStr As String = "%t: %c, %s: %n"

    Sub new()
        Me.CommandAlias = "checkbalance"
        Me.Permission = "swap"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim team1 As String = ""
        Dim team2 As String = ""
        Dim team1Count As Int32 = 0
        Dim team2Count As Int32 = 0

        For Each p As User In Me.adminIface.PHandler.PlayerList
            Dim teamVal As String = IIf(p.TeamName Is Nothing, p.TeamId, p.TeamName)
            If team1 = "" Then
                team1 = teamVal
            ElseIf team2 = "" AndAlso p.TeamName <> team1 Then
                team2 = teamVal
            End If

            If teamVal = team1 Then
                team1Count += 1
            ElseIf teamVal = team2 Then
                team2Count += 1
            End If
        Next

        If team1 = "" AndAlso team2 = "" Then
            Me.Pm("Error: " & Me.ParseTemplate(Me.BalanceStr, {team1, team1Count, team2, team2Count}, {"t", "c", "s", "n"}), player)
            Return False
        End If

        Me.Pm(Me.ParseTemplate(Me.BalanceStr, {team1, team1Count, team2, team2Count}, {"t", "c", "s", "n"}), player)

        Return True
    End Function

End Class
