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

Public Class ServerInformation
    Public Property ServerName As String = String.Empty
    Public Property ServerIP As String = String.Empty
    Public Property Version As String = String.Empty
    Public Property MaxPlayers As String = String.Empty
    Public Property Password As String = String.Empty
    Public Property CurrentMap As String = String.Empty
    Public Property NextMap As String = String.Empty
    Public Property GameMode As String = String.Empty
    Public Property Players As String = String.Empty
    Public Property Scores As String = String.Empty
    Public Property Tickets As String = String.Empty
    Public Property FFEnabled As String = String.Empty
    Public Property Heroes As String = String.Empty
End Class
