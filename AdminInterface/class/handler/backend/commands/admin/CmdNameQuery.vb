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
Public Class CmdNameQuery
    Inherits Command

    Public Property OnQueryPM As String = "[PM] %r rows matching %e found: %u"
    Public Property OnQuery As String = " %r rows matching %e found: %u"
    ' Public Property OnNoPermission As String = "You can't Query %p (no permission)."
    Public Property OnSyntaxError As String = "Syntax: !nquery <user> [-v <public|pm>] [-o <asc|desc>] [-c <max rows>] [-m <key|ip>] [-s <chars>]"
    Public Property OnNoPlayerMatch As String = "No player matching %e could be found!"
    Public Property DefaultOrder As String = "asc"
    Public Property DefaultVisibility As String = "pm"
    Public Property DefaultMaxCount As Int32 = 20
    Public Property DefaultIPChars As Byte = 15
    Public Property DefaultQueryMode As String = "key"
    Public Property OptionOrder As String = "-o"
    Public Property OptionVisibility As String = "-v"
    Public Property OptionMaxCount As String = "-c"
    Public Property OptionQueryMethod As String = "-m"
    Public Property OptionIPChars As String = "-s"

    Sub New()
        Me.CommandAlias = "nquery"
        Me.Permission = "nquery"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length < 2 Then 'Kein Suchstring
            Me.Say(Me.OnSyntaxError)
            Return False
        End If

        Dim affectedUser As User = Me.adminIface.PHandler.FetchUserByNameMatch(params(1))
        If affectedUser Is Nothing Then 'Kein User gefunden
            Me.Say(Me.ParseTemplate(Me.OnNoPlayerMatch, {params(1)}, {"e"}))
            Return False
        End If

        Dim pm As Boolean = (Me.DefaultVisibility = "pm")
        Dim ipQuery As Boolean = (Me.DefaultQueryMode = "ip")
        Dim order As String = Me.DefaultOrder
        Dim maxCount As Int32 = Me.DefaultMaxCount
        Dim ipChars As Byte = Me.DefaultIPChars
        Dim ipExpression As String = " = '" & affectedUser.IPAddress.ToString() & "'"

        For i = 2 To params.Length - 1
            If i + 1 > params.Length - 1 Then
                Me.Say(Me.OnSyntaxError)
                Return False
            End If

            Select Case params(i)
                Case Me.OptionQueryMethod
                    If params(i + 1) = "ip" Then
                        ipQuery = True
                    ElseIf params(i + 1) = "key" Then
                        ipQuery = False
                    Else
                        Me.Say(Me.OnSyntaxError)
                        Return False
                    End If
                Case Me.OptionMaxCount
                    If Not Int32.TryParse(params(i + 1), maxCount) Then
                        Me.Say(Me.OnSyntaxError)
                        Return False
                    End If
                Case Me.OptionOrder
                    If params(i + 1) = "asc" Or params(i + 1) = "desc" Then
                        order = params(i + 1)
                    Else
                        Me.Say(Me.OnSyntaxError)
                        Return False
                    End If
                Case Me.OptionIPChars
                    If Not Byte.TryParse(params(i + 1), ipChars) Or ipChars > 15 Then
                        Me.Say(Me.OnSyntaxError)
                        Return False
                    End If
                    ipExpression = "like '" & Mid(affectedUser.IPAddress.ToString(), 1, ipChars) & "%' "
                Case Me.OptionVisibility
                    If params(i + 1) = "pm" Then
                        pm = True
                    ElseIf params(i + 1) = "public" Then
                        pm = False
                    Else
                        Me.Say(Me.OnSyntaxError)
                        Return False
                    End If
                Case Else
                    Me.Say(Me.OnSyntaxError)
                    Return False
            End Select
            i += 1
        Next

        Dim users As List(Of String) = Me.adminIface.MySQL.QueryNameList(affectedUser, order, maxCount, ipQuery, ipExpression)

        Dim ofStr As String = String.Empty
        For Each uname As String In users
            ofStr &= uname & ","
        Next
        If ofStr.Length > 1 Then ofStr = Mid(ofStr, 1, ofStr.Length - 1)
        If pm Then
            Me.Pm(Me.ParseTemplate(Me.OnQueryPM, {params(1), ofStr, users.Count}, {"e", "u", "r"}), player)
        Else
            Me.Say(Me.ParseTemplate(Me.OnQuery, {params(1), ofStr, users.Count}, {"e", "u", "r"}))
        End If

        Return True
    End Function

End Class