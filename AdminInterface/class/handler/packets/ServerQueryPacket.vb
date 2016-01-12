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

Public Class ServerQueryPacket
    Inherits TcpPacket

    Public Property ServerInfo As ServerInformation

    Sub New()
        Me.CommandAlias = "status"
    End Sub

    Public Overrides Sub FetchData(ByVal data As String)
        Me.ServerInfo = New ServerInformation()
        Dim rows() As String = Split(data, vbCrLf)
        With Me.ServerInfo
            .ServerName = FetchValue(rows, 0)
            .ServerIP = FetchValue(rows, 1)
            .Version = FetchValue(rows, 2)
            .MaxPlayers = FetchValue(rows, 3)
            .Password = FetchValue(rows, 4)
            .CurrentMap = FetchValue(rows, 5)
            .NextMap = FetchValue(rows, 6)
            .GameMode = FetchValue(rows, 7)
            .Players = FetchValue(rows, 8)
            .Scores = FetchValue(rows, 9)
            .Tickets = FetchValue(rows, 10)
            .FFEnabled = FetchValue(rows, 11)
            .Heroes = FetchValue(rows, 12)
        End With
        Me.PacketOK = True
    End Sub

    Private Function FetchValue(ByVal rows() As String, ByVal index As Int32) As String
        Dim str() As String = Split(rows(index), ": ")
        Return str(1)
    End Function

End Class