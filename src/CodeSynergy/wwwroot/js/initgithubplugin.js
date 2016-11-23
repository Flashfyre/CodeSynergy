var setting = {
    async: {
        enable: true,
        url: getUrl,
        type: "get"
    },
    data: {
        simpleData: {
            enable: true
        }
    },
    view: {
        expandSpeed: ""
    },
    callback: {
        beforeExpand: beforeExpand,
        onAsyncSuccess: onAsyncSuccess,
        onAsyncError: onAsyncError,
        onClick: onNodeClicked,
        onNodeCreated: onNodeCreated
    }
};

var zNodes = [
    { name: "500 nodes", id: "1", count: 1, times: 1, isParent: true },
    { name: "1000 nodes", id: "2", count: 1, times: 1, isParent: true },
    { name: "2000 nodes", id: "3", count: 1, times: 1, isParent: true }
];

zNodes = [];

for (var d = 1; d <= githubRepoData.length; d++) {
    zNodes[d - 1] = { name: githubRepoData[d - 1].name, id: d.toString(), level: 0, count: 1, times: 1, isParent: true };
}

/*var zNodes = [
    { id: 1, pId: 0, name: "ordinary parent", t: "I am ordinary, just click on me", open: true },
    { id: 11, pId: 1, name: "leaf node 1-1", t: "I am ordinary, just click on me" },
    { id: 12, pId: 1, name: "leaf node 1-2", t: "I am ordinary, just click on me" },
    { id: 13, pId: 1, name: "leaf node 1-3", t: "I am ordinary, just click on me" },
    { id: 2, pId: 0, name: "strong parent", t: "You can click on me, but you can not click on the sub-node!", open: true },
    { id: 21, pId: 2, name: "leaf node 2-1", t: "You can't click on me..", click: false },
    { id: 22, pId: 2, name: "leaf node 2-2", t: "You can't click on me..", click: false },
    { id: 23, pId: 2, name: "leaf node 2-3", t: "You can't click on me..", click: false },
    { id: 3, pId: 0, name: "weak parent", t: "You can't click on me, but you can click on the sub-node!", open: true, click: false },
    { id: 31, pId: 3, name: "leaf node 3-1", t: "please click on me.." },
    { id: 32, pId: 3, name: "leaf node 3-2", t: "please click on me.." },
    { id: 33, pId: 3, name: "leaf node 3-3", t: "please click on me.." }
];*/

var log, className = "dark",
startTime = 0, endTime = 0, perCount = 100, perTime = 100;

function getUrl(treeId, treeNode) {
    var url;

    if (treeNode == null)
        return null;

    switch (treeNode.level) {
        case 0:
            url = 'https://api.github.com/repos/' + githubID + '/' + treeNode.name + '/branches';
            break;
        case 1:
            url = 'https://api.github.com/repos/' + githubID + '/' + treeNode.getParentNode().name + '/contents?ref=' + treeNode.name;
            break;
        default:
            var branchNode = treeNode;
            for (var i = 0; i < treeNode.level - 1; i++)
                branchNode = branchNode.getParentNode();
            url = 'https://api.github.com/repos/' + githubID + '/' + branchNode.getParentNode().name + '/contents/' + treeNode.path + '?ref=' + branchNode.name;
            break;
    }

    return url;
}

function beforeExpand(treeId, treeNode) {
    if (!treeNode.isAjaxing) {
        startTime = new Date();
        treeNode.times = 1;
        ajaxGetNodes(treeNode, "refresh");
        return true;
    } else
        return false;
}

function ajaxGetNodes(treeNode, reloadType) {
    var zTree = $.fn.zTree.getZTreeObj("github-repos");
    if (reloadType == "refresh") {
        treeNode.icon = "/lib/zTree/css/codeSynergyStyle/img/loading.gif";
        zTree.updateNode(treeNode);
    }
    zTree.reAsyncChildNodes(treeNode, reloadType, true);
}

function beforeAsync() {
    curAsyncCount++;
}

function onAsyncSuccess(event, treeId, treeNode, msg) {
    var zTree = $.fn.zTree.getZTreeObj("github-repos");
    treeNode.icon = "";
    zTree.updateNode(treeNode);
}

function onAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
    var zTree = $.fn.zTree.getZTreeObj("github-repos");
    curAsyncCount--;
    console.log(errorThrown);
    treeNode.icon = "";
    zTree.updateNode(treeNode);
}

function onNodeCreated(event, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("github-repos");
    if (treeNode.level > 0) {
        treeNode.id = treeNode.getIndex() + 1;
        treeNode.times = 1;
        switch (treeNode.level) {
            case 1:
                treeNode.isParent = true;
                zTree.updateNode(treeNode);
                break;
            default:
                treeNode.isParent = treeNode.type == "dir";
                treeNode.url = null;
                zTree.updateNode(treeNode);
                break;
        }
    }
}

function onNodeClicked(event, treeId, treeNode) {
    if (treeNode.level > 1 && !treeNode.isParent) {
        var zTree = $.fn.zTree.getZTreeObj("github-repos");
        treeNode.icon = "/lib/zTree/css/codeSynergyStyle/img/loading.gif";
        zTree.updateNode(treeNode);
        var branchNode = treeNode;
        for (var i = 0; i < treeNode.level - 1; i++)
            branchNode = branchNode.getParentNode();
        $.ajax({
            type: "GET",
            url: 'https://api.github.com/repos/' + githubID + '/' + branchNode.getParentNode().name + '/contents/' + treeNode.path + '?ref=' + branchNode.name,
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                treeNode.icon = "";
                zTree.updateNode(treeNode);
                githubFileContents = atob(data.content);
            },
            error: function () {
                alert("Failed to load contents for the file '" + treeNode.name + "'.");
                treeNode.icon = "";
                zTree.updateNode(treeNode);
            }
        });
    } else
        githubFileContents = "";
}

$(document).ready(function () {
    $("#github-repos").css("min-height", ((window.innerHeight * 0.8) - 88) + "px").parent().css("overflow", "scroll");
    $.fn.zTree.init($("#github-repos"), setting, zNodes);
});