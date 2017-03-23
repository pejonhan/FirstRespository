﻿<%@ Page Language="C#" AutoEventWireup="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Please wait...</title>
    <link rel="stylesheet" type="text/css" href="//cdn.bootcss.com/font-awesome/4.6.3/css/font-awesome.min.css" />
    <link rel="stylesheet" type="text/css" href="/content/css/animate.css"/>
    <link rel="stylesheet" type="text/css" href="resources/themes/bootstrap/easyui.css"/>
    <link rel="stylesheet" type="text/css" href="resources/themes/bootstrap/app.css"/>
    <script type="text/javascript" src="/Scripts/jquery-2.1.4.min.js"></script>
    <script type="text/javascript" src="resources/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="http://cdn.bootcss.com/vue/1.0.26/vue.min.js"></script>
</head>
<body class="easyui-layout">
    <div id="app-header" data-options="region:'north'" style="height:40px; color:#fefefe; border:0; overflow:hidden;">
        <div style="float:left; width:150px;">
            <h1 class="app-logo" data-init="config['system.appname']"></h1>
        </div>
        <ul style="float:left;">
            <li><a href="/" target="_blank" title="查看前台主页"><i class="fa fa-home"></i> 回首页</a></li>
            <li><a href="javascript:void(0);" id="fullscreen" title="全屏" onclick="$(document).toggleFullScreen();" style="display:none;"><i class="fa fa-expand"></i></a></li>
        </ul>
        <ul style="float:right;">
            <li><a href="javascript:opentab('user/profile','个人中心');" title="个人信息"><i class="fa fa-user"></i> <label data-init="user['UserName']">&nbsp;</label>, <label>欢迎回来！</label></a></li>
            <li><a href="javascript:void(0);" id="clearCache" title="更新缓存"><i class="fa fa-refresh"></i> 缓存更新</a></li>
            <li><a href="/account/logoff?returnUrl=<%:Request.Url.PathAndQuery %>" title="安全退出"><i class="fa fa-sign-out"></i> 退出登录</a></li>
        </ul>
    </div>
    <div data-options="region:'west'" title="功能导航" style="width:150px;">
        <div id="nav" class="easyui-panel" style="height: 100%; border: 0"
             data-options="href:'app/navigation/accordion.cshtml'">
        </div>
    </div>
    <div data-options="region:'center'">
        <div id="mainTabs" class="easyui-tabs" data-options="fit:true,border:false">
            <div title="起始页" data-options="href:'app/portal/layout.cshtml'" style="padding:10px; overflow-x:hidden;"></div>
            <div title="日程安排" data-options="href:'app/user/calendar.cshtml'"></div>
        </div>
    </div>
    <div data-options="region:'south'" style="height:30px; padding:5px 10px;">
        <div style="float: left;">
            <span id="app-status">Ready</span>
        </div>
        <div style="float:right;">
            当前版本：<label>v<%:App.Core.AppConsts.AppVersion %></label>
        </div>
    </div>
    <div class="ajax-loadding"><i class="fa fa-spinner fa-pulse"></i> 正在处理，请稍候......</div>
    <div id="dlg-main"></div>
    <div id="mainTabMenus" class="easyui-menu" style="width:150px;">  
        <div name="close">关闭</div>  
        <div name="All">全部关闭</div>
        <div name="Other">除此之外全部关闭</div>  
    </div> 
    <script type="text/javascript" src="/libs/jquery.fullscreen-min.js"></script>
    <script type="text/javascript" src="/api/init"></script>
    <script type="text/javascript" src="App/app.js"></script>
    <script type="text/javascript" src="Resources/locale/easyui-lang-zh_CN.js"></script>
    <script type="text/javascript" src="Resources/ux.js"></script>
</body>
</html>



