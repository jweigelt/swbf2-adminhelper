Public Class CmdTeamStats
    Inherits Command

    Public Property TeamStatsStr As String = "Team 1: %p, Team 2: %q"
    Public Property OnSyntaxError As String = "Syntax: #teamstats"

    Sub New()
        Me.CommandAlias = "teamstats"
        Me.Permission = "stats"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        Dim team1 As Int32 = 0
        Dim team2 As Int32 = 0

        For Each p As User In Me.adminIface.PHandler.PlayerList
            Dim kills As Int32 = Me.adminIface.PHandler.PlayerPointsTracker(p.KeyHash)(1)
            If p.TeamName = "Her" Then
                team1 = team1 + kills
            Else
                team2 = team2 + kills
            End If
        Next

        Me.Pm(Me.ParseTemplate(Me.TeamStatsStr, {team1, team2}, {"p", "q"}), player)
        Return True
    End Function
End Class
