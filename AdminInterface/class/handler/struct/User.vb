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

Public Class User
    Public Property SlotId As Int32 = 0
    Public Property UserName As String = String.Empty
    Public Property KeyHash As String = String.Empty
    Public Property IPAddress As Net.IPAddress
    Public Property TeamId As Byte
    Public Property Ping As Int32
    Public Property Points As Int32
    Public Property TeamName As String
    Public Property Kills As Int32
    Public Property Deaths As Int32
    Public Property IsBanned As Boolean

    Public Property IsRegistered As Boolean = False
    Public Property GroupId As Int32 = -1
    Public Property GroupName As String = "-"
    Public Property UserId As Int32 = -1
    Public Property playerId As Int32 = -1
End Class
