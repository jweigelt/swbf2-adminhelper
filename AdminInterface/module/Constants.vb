'Constants
Public Class Constants
    Public Const PRODUCT_NAME As String = "SWBF2-SADS Admin Helper"
    Public Const PRODUCT_VERSION As String = "3.0"
    Public Const PRODUCT_VENDOR As String = "JW 'LeKeks' 2015"
    Public Const PRODUCT_BANNER As String = "SWBF2 Admin Helper v3.0 / LeKeks 2015"

    Public Const CFG_DIR As String = "/cfg"
    Public Const CFG_CMD_DIR As String = "/cmd"
    Public Const CFG_CORE As String = "/core.xml"
    Public Const CFG_IGAMETEMPLATE As String = "/ingamemessages.xml"
    Public Const CFG_ANNOUNCE As String = "/announce.xml"
    Public Const CFG_DYNCOMMAND As String = "/dyncommand.xml"

    Public Const MYSQL_GROUPS_TABLE As String = "ai_groups"
    Public Const MYSQL_PERMISSIONS_TABLE As String = "ai_permissions"
    Public Const MYSQL_PLAYERS_TABLE As String = "ai_players"
    Public Const MYSQL_USERS_TABLE As String = "ai_users"
    Public Const MYSQL_BANS_TABLE As String = "ai_bans"

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
