<%@ WebHandler Language="C#" Class="UploadHandler" %>

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing;
using Goheer;

public class UploadHandler : IHttpHandler {
    
    private string CONNECT_STRING;
    private DataTable parameters;

    public void ProcessRequest (HttpContext context) {
        
        int chunk = context.Request["chunk"] != null ? int.Parse(context.Request["chunk"]) : 0;
        string fileName = context.Request["name"] != null ? context.Request["name"] : string.Empty;
        string nop = context.Request["nop"] != null ? context.Request["nop"] : string.Empty;            // if 'nop=true' no database update to strImageDet is done
        string wfid = context.Request["wfid"] != null ? context.Request["wfid"] : string.Empty;
        string fi = context.Request["fi"] != null ? context.Request["fi"] : string.Empty;               // Strone file No. eg: MIS-13-25240
        string rm = context.Request["rm"] != null ? context.Request["rm"] : string.Empty;               // room code
        string schid = context.Request["sc"] != null ? context.Request["sc"] : string.Empty;            // schedule id (only valid for crew)
        string uid = context.Request["uid"] != null ? context.Request["uid"] : string.Empty;            // user id eg: 15355
        if (nop != "true") nop = "false";

        int subproc = 1;
        if (fi.Substring(fi.Length - 1) == "R") subproc = 2;
        
        if ((fileName != "") && (wfid != "") && (fi != "") && (uid != ""))
        {
            HttpPostedFile fileUpload = context.Request.Files[0];

            var uploadPath = ConfigurationSettings.AppSettings["PictureBasePath"] + wfid + "\\";
            if (!System.IO.Directory.Exists(uploadPath))
            {
                System.IO.Directory.CreateDirectory(uploadPath);
            }

            using (var fs = new FileStream(Path.Combine(uploadPath, fileName), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var buffer = new byte[fileUpload.InputStream.Length];
                fileUpload.InputStream.Read(buffer, 0, buffer.Length);

                fs.Write(buffer, 0, buffer.Length);
            }

            try
            {
                string newfileName = fileUpload.FileName;
                int i = newfileName.LastIndexOf("\\");
                if (i > 0) newfileName = newfileName.Substring(i+1);
                
                string file = uploadPath + fileName;
                string newfile = uploadPath + newfileName;
                string chkfile = newfileName;
                
                if (System.IO.File.Exists(file))            // it's arrived, let's rename it and log it 
                {
                    bool unique;
                    int version = 0;
                    string ver = "";
                    string ext = fileName.Substring(fileName.LastIndexOf('.'));
                    do
                    {
                        unique = true;
                        if (System.IO.File.Exists(uploadPath + chkfile))
                        {
                            //unique = false;
                            version++;
                            //ver = "(" + version.ToString() + ")" + ext;
                            //chkfile = newfileName.Replace(ext, ver);
                            nop = "true";
                        }
                    } while (!unique);

                    newfile = uploadPath +chkfile;

                    
                    System.IO.File.Copy(file, newfile, true);
                    
                    System.IO.File.Delete(file);

                    checkphotoorientation(newfile);
                    FixedSize(uploadPath,chkfile,341,209);

                    if (nop == "false")     // if 'nop=true' parameter is passed, no pictures are logged (for testing upload)
                    {
                        CONNECT_STRING = ConfigurationSettings.AppSettings["ConnectionString"];
                        using (SqlConnection conn = new SqlConnection(this.CONNECT_STRING))
                        {
                            int newId = -1;
                            conn.Open();
                            SqlCommand command = conn.CreateCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "ImageSaveUpload";
                            command.Parameters.Clear();
                            SqlParameter fileId = command.Parameters.Add("@fileId", SqlDbType.Int);
                            fileId.Direction = ParameterDirection.Output;
                            command.Parameters.Add("@wfId", wfid);
                            command.Parameters.Add("@fileName", chkfile);
                            command.Parameters.Add("@filePath", uploadPath);
                            command.Parameters.Add("@desc", "Picture for File: " + fi);
                            command.Parameters.Add("@userId", uid);
                            command.Parameters.Add("@viewAdjuster", true);
                            command.Parameters.Add("@viewClient", true);
                            command.Parameters.Add("@viewIC", true);
                            command.ExecuteNonQuery();
                            if (fileId != null)
                            {
                                newId = (int)fileId.Value;
                                SqlCommand command2 = conn.CreateCommand();
                                command2.CommandType = CommandType.StoredProcedure;
                                command2.CommandText = "oUpdateImgExtra";
                                command2.Parameters.Clear();
                                command2.Parameters.Add("@imageId", Convert.ToInt32(newId));
                                command2.Parameters.Add("@locationId", Convert.ToInt32(rm));
                                command2.Parameters.Add("@desc", "");
                                command2.ExecuteNonQuery();                        
                                SqlCommand command3 = conn.CreateCommand();
                                command3.CommandType = CommandType.Text;
                                if (schid == null||schid=="") schid = "0";
                                command3.CommandText = "update StrImagesDet set refid=" + schid + " where keyid=" + newId.ToString();
                                command3.Parameters.Clear();
                                command3.ExecuteNonQuery();
                                SqlCommand command4 = conn.CreateCommand();
                                command4.CommandType = CommandType.StoredProcedure;
                                command4.CommandText = "CreateTasksTablet";
                                command4.Parameters.Clear();
                                command4.Parameters.Add("@wfId", Convert.ToInt32(wfid));
                                command4.Parameters.Add("@SubProcessId", subproc);
                                command4.Parameters.Add("@ActionId", 22052);
                                command4.Parameters.Add("@UserId", Convert.ToInt32(uid));
                                command4.Parameters.Add("@FileId", Convert.ToInt32(newId));
                                command4.ExecuteNonQuery();
                            }
                            else
                            {
                                context.Response.Write("Procedure to Save Uploaded Image Failed.");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                context.Response.Write("Procedure [ImageSaveUpload] Failed: " + e.Message);
            }
        }
        else
        {
            string erm = "Procedure call [ImageSaveUpload] missing arguments ";
            if (fileName == "") erm += "fileName";
            if (wfid == "") erm += " WFID";
            if (fi == "") erm += " fi(FileNo)";
            if (uid == "") erm += " uid(UserId)";
            context.Response.Write(erm);  
        }
        //context.Response.ContentType = "text/plain";
        //context.Response.Write("Success");
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }
    public static void FixedSize(string upPhotoPath,string FileName, int Width, int Height)
    {
        string imgPhotoSrc = upPhotoPath + FileName;
        Image imgPhoto;
        imgPhoto = Image.FromFile(imgPhotoSrc);
        
        int sourceWidth = imgPhoto.Width;
        int sourceHeight = imgPhoto.Height;
        int sourceX = 0;
        int sourceY = 0;
        int destX = 0;
        int destY = 0;

        float nPercent = 0;
        float nPercentW = 0;
        float nPercentH = 0;

        nPercentW = ((float)Width / (float)sourceWidth);
        nPercentH = ((float)Height / (float)sourceHeight);
        if (nPercentH < nPercentW)
        {
            nPercent = nPercentH;
            destX = System.Convert.ToInt16((Width -
                          (sourceWidth * nPercent)) / 2);


            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            bmPhoto.Save(upPhotoPath+"tn_"+FileName, ImageFormat.Jpeg);
        }
        //else
        //{
        //    nPercent = nPercentW;
        //    destY = System.Convert.ToInt16((Height -
        //                  (sourceHeight * nPercent)) / 2);
        //}
    }
    private static void checkphotoorientation(string imagesrc)
    {
        //imagesrc = imagesrc.ToLower().Replace(@"~", "c://inetpub/wwwroot/strwebflow").Trim();
        var bmp = new Bitmap(imagesrc);
        var exif = new Goheer.EXIF.EXIFextractor(ref bmp, "n"); // get source from http://www.codeproject.com/KB/graphics/exifextractor.aspx?fid=207371

        if (exif["Orientation"] != null)
        {
            if (exif["Orientation"].ToString() != "1H\0")
            {
                RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

                if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
                {
                    bmp.RotateFlip(flip);
                    exif.setTag(0x112, "1"); // Optional: reset orientation tag
                    bmp.Save(imagesrc, ImageFormat.Jpeg);
                }
            }
        }
    }
    private static RotateFlipType OrientationToFlipType(string orientation)
    {
        switch (int.Parse(orientation))
        {
            case 1:
                return RotateFlipType.RotateNoneFlipNone;
                break;
            case 2:
                return RotateFlipType.RotateNoneFlipX;
                break;
            case 3:
                return RotateFlipType.Rotate180FlipNone;
                break;
            case 4:
                return RotateFlipType.Rotate180FlipX;
                break;
            case 5:
                return RotateFlipType.Rotate90FlipX;
                break;
            case 6:
                return RotateFlipType.Rotate90FlipNone;
                break;
            case 7:
                return RotateFlipType.Rotate270FlipX;
                break;
            case 8:
                return RotateFlipType.Rotate270FlipNone;
                break;
            default:
                return RotateFlipType.RotateNoneFlipNone;
        }
    }

}