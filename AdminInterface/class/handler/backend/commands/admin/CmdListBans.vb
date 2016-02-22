Public Class CmdListBans
    Inherits Command

    Public Property OnSyntaxError As String = "Syntax: #listbans [<start_number>=0] (This will list bans starting from #<start>)"
    Public Property OnNoBans As String = "There are no banned players."

    Sub New()
        Me.CommandAlias = "listbans"
        Me.Permission = "ban"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")
        If params.Length > 2 Then
            Me.Pm(OnSyntaxError, player)
        End If

        Dim start As Int32 = 0
        If params.Length = 2 Then
            If Not Int32.TryParse(params(1), start) Then
                Me.Say(Me.OnSyntaxError)
                Return False
            End If
        End If

        Dim banList As List(Of List(Of String)) = Me.adminIface.SQL.GetBans(start)
        If banList Is Nothing OrElse banList.Count = 0 Then
            Me.Pm(OnNoBans, player)
            Return True
        End If

        Dim banStr As String = ""
        For Each ban As List(Of String) In banList
            banStr &= ban(1)
            banStr &= ", "
        Next
        banStr = banStr.Substring(0, banStr.Length - 2)

        Me.Pm(banStr, player)

        Return True
    End Function

End Class
