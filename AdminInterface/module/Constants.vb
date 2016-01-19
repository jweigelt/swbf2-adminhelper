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

'Constants
Public Class Constants
    Public Const PRODUCT_NAME As String = "SWBF2-SADS Admin Helper"
    Public Const PRODUCT_VERSION As String = "3.3"
    Public Const PRODUCT_VENDOR As String = "JW 'LeKeks' 2015"
    Public Const PRODUCT_BANNER As String = "SWBF2 Admin Helper v3.3 / LeKeks (Customized by Yoni) 2015"

    Public Const CFG_DIR As String = "/cfg"
    Public Const CFG_CMD_DIR As String = "/cmd"
    Public Const CFG_CORE As String = "/core.xml"
    Public Const CFG_IGAMETEMPLATE As String = "/ingamemessages.xml"
    Public Const CFG_ANNOUNCE As String = "/announce.xml"
    Public Const CFG_DYNCOMMAND As String = "/dyncommand.xml"

    Public Const SQL_GROUPS_TABLE As String = "ai_groups"
    Public Const SQL_PERMISSIONS_TABLE As String = "ai_permissions"
    Public Const SQL_PLAYERS_TABLE As String = "ai_players"
    Public Const SQL_USERS_TABLE As String = "ai_users"
    Public Const SQL_BANS_TABLE As String = "ai_bans"

    Public Const CMD_EXIT As String = "exit"

    Public Const UPDATE_VERSION_URL As String = "http://localhost/ai_update.txt"
    Public Const UPDATE_WGET_URL As String = "http://localhost/ai_update.exe"
    Public Const UPDATE_MAINEXECUTABLE As String = "/AdminInterface.exe"
    Public Const UPDATE_BACKUPFILE As String = "/AdminInterface.exe.bak"
    Public Const UPDATE_UPDATEFILE As String = "/AdminInterface.exe.new"

    Public Const V1_0 As Boolean = False

    Public Const MEM_1_1_CLIENT_BASE As Int32 = &HC22408
    Public Const MEM_1_1_PLAYER_BASE As Int32 = &HC09F68
    Public Const MEM_1_1_ADMINPASSWORD As Int32 = &HC06978

End Class
