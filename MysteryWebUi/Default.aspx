<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MysteryWebUi.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base href="<%=ResolveUrl("~/") %>" />

    <title>Mystery!</title>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <%--thanks aps.net you make me have ResolveUrl here or you will rewrite wrong those urls--%>
    <link href="<%=ResolveUrl("~/Content/bootstrap.min.css") %>" rel="stylesheet" />

    <link href="<%=ResolveUrl("~/Content/font-awesome.min.css") %>" rel="stylesheet" />
    <link href="<%=ResolveUrl("~/Content/sb-admin-2.css") %>" rel="stylesheet" />
    <link href="<%=ResolveUrl("~/Content/bootstrap-fileinput/css/fileinput.css") %>" rel="stylesheet" />
    <link href="<%=ResolveUrl("~/Content/metisMenu.min.css") %>" rel="stylesheet" />

    <link href="<%=ResolveUrl("~/MysteryWebContent/Shared/Mystery.css") %>" rel="stylesheet" />
    
    <script src="scripts/jquery-3.1.1.min.js"></script>
    <script src="scripts/bootstrap.min.js"></script>
    <script src="scripts/metisMenu.min.js"></script>

    <!--those should change to the min in prod-->
    <script src="scripts/angular.js"></script>
    <script src="scripts/angular-route.js"></script>
    <script src="scripts/angular-cookies.js"></script>
    <script src="scripts/angular-sanitize.js"></script>
    <script src="scripts/angular-translate.js"></script>
    <script src="scripts/angular-ui/ui-bootstrap.js"></script>
    <script src="scripts/angular-ui/ui-bootstrap-tpls.js"></script>
    <script src="scripts/angular-ui/ui-utils.js"></script>

    <script src="scripts/smart-table.js"></script>
    <script src="scripts/fileinput.js"></script>
    <script src="scripts/RecursionHelper.js"></script>


    <script>
        var app = angular.module("Mystery", [
    'pascalprecht.translate',
    'ngSanitize',
    'ui.bootstrap',
    'ui.keypress',
    'ngRoute',
    'ngCookies',
    'smart-table',
    'RecursionHelper',
        ]);
    </script>

    <%--time for mystery--%>
    <script src="MysteryWebContent/Shared/Mystery.js"></script>
    <%--//run time generated scripts--%>
    <script src="Dictionary"></script>
    <script src="Routes"></script>


    <script src="MysteryWebContent/Shared/MysteryAngular.js"></script>
    <script src="MysteryWebContent/Shared/MysteryDownloader.js"></script>
    <script src="MysteryWebContent/Shared/MysteryInstance.js"></script>
    <script src="MysteryWebContent/Shared/LoginService.js"></script>
    <script src="MysteryWebContent/Shared/MysteryFormService.js"></script>
    
    <script src="MysteryWebContent/Shared/NavBar/NavBar.js"></script>
    <script src="MysteryWebContent/Shared/Start/Start.js"></script>
    <script src="MysteryWebContent/Shared/MysteryServiceViewController.js"></script>
    <script src="MysteryWebContent/MysteryContent/View.js"></script>
    <script src="MysteryWebContent/MysteryContent/Properties/PropertyValueDirectives.js"></script>
    <script src="MysteryWebContent/MysteryContent/Properties/DatePropertyController.js"></script>
    <script src="MysteryWebContent/MysteryContent/Properties/SingleReferenceController.js"></script>
    <script src="MysteryWebContent/MysteryContent/Properties/MultiReferenceController.js"></script>
    <script src="MysteryWebContent/MysteryContent/Properties/SingleFileController.js"></script>
    <script src="MysteryWebContent/MysteryContent/Properties/TextPropertyController.js"></script>
    <script src="MysteryWebContent/MysteryContent/ContentTypeListRouteController.js"></script>
    <script src="MysteryWebContent/MysteryContent/List.js"></script>
    <script src="MysteryWebContent/MysteryContent/TypeButton.js"></script>
    <script src="MysteryWebContent/MysteryContent/ContentActionMenu.js"></script>
    <script src="MysteryWebContent/MysteryContent/Actions/DeleteContent.js"></script>

    <script src="MysteryWebContent/Login/LoginDialogController.js"></script>

    <script src="ModulesJs"></script>


</head>
<body role="document" ng-app="Mystery" class="container-fluid">
    <%--//if we have a link to the "folder" we need to add a / after it
    //basically we need to access http://mysite/ instead of http://mysite--%>
    <script>

        function absoluteUrl(url) {
            var urlParsingNode = document.createElement("a");
            urlParsingNode.setAttribute("href", url);
            return urlParsingNode.href;
        }

        function stripQuery(url) {
            return url.replace(/\?[^?]*/, '');
        }

        var baseElement = document.getElementsByTagName('base')[0];
        var baseUrl = absoluteUrl(baseElement ? baseElement.getAttribute('href') : '/');
        var currentUrl = stripQuery(location.href);

        //console.log(baseUrl, currentUrl);

        if (currentUrl + '/' === baseUrl) {
            var newUrl = location.href.replace(currentUrl, baseUrl);
            //console.log('do redirect to', newUrl);
            location.href = newUrl;
        }

    </script>

    <ng-view></ng-view>

</body>
</html>
