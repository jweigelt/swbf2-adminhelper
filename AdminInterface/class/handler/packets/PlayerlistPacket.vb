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

Public Class PlayerlistPacket
    Inherits TcpPacket

    Public Property PlayerList As List(Of User)

    Sub New()
        Me.CommandAlias = "players"
    End Sub

    Public Overrides Sub FetchData(ByVal data As String)
        '1  "LeKeks"          Rep 0   0   0   51  192.168.178.35  ae48aab730214004b1f024d54757fccd
        'id name------------- t-- v-- v-- v-- p-- ip------------- key-----------------------------
        '123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
        Me.PlayerList = New List(Of User)

        Dim rows() As String = Split(data, vbCrLf)
        For Each r As String In rows
            'If r.Length <> 90 Then Continue For
            Dim u As New User
            With u
                .SlotId = Int32.Parse(RTrim(Mid(r, 1, 2)))
                .UserName = RTrim(Mid(r, 4, 17))
                .UserName = Mid(.UserName, 2, .UserName.Length - 2)
                .TeamName = RTrim(Mid(r, 22, 3))
                .Points = Int32.Parse(RTrim(Mid(r, 26, 3)))
                .Kills = Int32.Parse(RTrim(Mid(r, 30, 3)))
                .Deaths = Int32.Parse(RTrim(Mid(r, 34, 3)))
                .Ping = Int32.Parse(RTrim(Mid(r, 38, 4)))
                .IPAddress = Net.IPAddress.Parse(RTrim(Mid(r, 42, 15)))
                .KeyHash = RTrim(Mid(r, 58, 32))
            End With
            Me.PlayerList.Add(u)
        Next
        Me.PacketOK = True
    End Sub

End Class
