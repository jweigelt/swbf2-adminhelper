Public Class CmdCommands
    Inherits Command

    Public Property OnSyntaxError As String = "Syntax: #commands"
    Public Property UnlistedCommands As List(Of String) = New List(Of String) From {
        "shutdown", "adminpls", "test", "hi", "assault"
    }

    Sub New()
        Me.CommandAlias = "commands"
        Me.Permission = "commands"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        Dim strs As List(Of String) = New List(Of String)
        strs.Add(String.Empty)

        Dim counter As Integer = 0

        Me.adminIface.CHandler.Commands.ForEach(
            Sub(ByVal command As Command)
                If Not Me.adminIface.PHandler.HasPermission(player, command) And Not command.IsPublic Then
                    Return
                End If
                If Me.UnlistedCommands.Contains(command.CommandAlias) Then
                    Return
                End If
                If strs(counter).Length + command.CommandAlias.Length + 3 > 100 Then
                    strs.Add(String.Empty)
                    counter += 1
                End If
                strs(counter) = strs(counter) & Me.adminIface.Config.CommandPrefix & command.CommandAlias & ", "
            End Sub
        )
        strs(counter) = strs(counter).Substring(0, strs(counter).Length - 2)

        strs.ForEach(
            Sub(ByVal str As String)
                Me.Pm(str, player)
            End Sub
        )

        Return True
    End Function
End Class
