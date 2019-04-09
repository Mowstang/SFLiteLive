<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="True"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
		<title>Bulk Photo Uploader</title>
		<LINK href="CSS_JS/styles.css" rel="stylesheet">
        <script src="../Scripts/jquery-2.2.3.js" type="text/javascript"></script>
        <!-- Load the queue CSS -->
        <link type="text/css" rel="Stylesheet" href="js/jquery.plupload.queue/css/jquery.plupload.queue.css" />
</head>
<body id="top">

        <!-- Load plupload and required runtimes, and finally the jQuery queue widget -->
        <script type="text/javascript" src="js/plupload.full.js"></script>
        <script type="text/javascript" src="js/jquery.plupload.queue/jquery.plupload.queue.js"></script>

        <script type="text/javascript">

            function post_result(res, data) {
                $.ajax({
                    type: 'POST',
                    url: 'UploadHandler.ashx?stat=' + res,
                    data: {
                        Msg: data
                    },
                    async: false,
                    success: function (response) {
                    }
                });
            };

            var IPCid;
            var urlParams;
            (window.onpopstate = function () {
                var match,
                pl = /\+/g,  // Regex for replacing addition symbol with a space
                search = /([^&=]+)=?([^&]*)/g,
                decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
                query = window.location.search.substring(1);
                urlParams = {};
                while (match = search.exec(query))
                    urlParams[decode(match[1])] = decode(match[2]);
            })();

            IPCid  = urlParams["IPC"];

            $(document).ready(function () {
                // Hook upload to Uploader div

                $("#uploader").pluploadQueue({
                    runtimes: 'html5,silverlight',
                    url: 'UploadHandler.ashx',
                    multipart_params: {
                        'sa': urlParams["sa"],
                        'nop': urlParams["nop"],
                        'wfid': urlParams["wfid"],
                        'fi': urlParams["fi"],
                        'sc': urlParams["sc"],
                        'rm': urlParams["rm"],
                        'uid': urlParams["uid"],
                        'stat': urlParams[""]
                    },
                    max_file_size: '10mb',
                    chunk_size: '1mb',
                    unique_names: true,

                    // Resize images on clientside if we can
                    resize: { width: 960, height: 540, quality: 85 },

                    // Specify what files to browse for
//                    filters: [
//                    { title: "Image files", extensions: "jpg,jpeg,gif,png" }
//                    ],

                    // Silverlight settings
                    silverlight_xap_url: 'js/plupload.silverlight.xap',
                    multiple_queues: true
                });

                // get uploader instance so we can hook events
                var uploader = $("#uploader").pluploadQueue();

                uploader.bind('Error', function (up, e) {
                    erm = 'Upload Error ' + e.code + ', Message: ' + e.message + (e.file ? ', File: ' + e.file.name : '');
                    if (urlParams["sa"] == '0') {
                        post_result('Failure', erm);
                    } else {
                        alert(erm);
                    }
                });

                uploader.bind("UploadComplete", function (up, files) {
                    if (urlParams["sa"] == '0') {
                        parent.pickUpParam('Success');
                    } else {
                        alert('Upload Success');
                    }
                });

            });


       </script>
        <div style="position:relative; top:-11px; left:-9px">
 			<form>
                <table id="tblMain" width="99%">
                    <tr>
                        <td>
				            <div id="uploader" style="height: 600px;">
                            You needd to have HTML5 or Silverlight capabilities</br>
                            Check with your System Administrator
                            </div>
                        </td>
                    </tr>
                </table>

			</form>
        </div>
     </body>
</html>
