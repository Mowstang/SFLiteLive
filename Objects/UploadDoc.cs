using System;
using System.Data;
using System.Data.SqlClient;
using Doxess.Data;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Drawing;
using System.Drawing.Imaging;



namespace SmartflowLite
{
    /// <summary>
    /// Summary description for UploadDoc.
    /// </summary>
    /// 
    public class UploadDoc
    {

        #region Static Methods

        public static int GetFileId(int wfId, int fileTypeId)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@wfId", wfId);
            dba.dxAddParameter("@fileTypeId", fileTypeId);
            DataTable data = dba.dxGetSPData("DocGetFileIdByWfIdFileType");
            if (data.Rows.Count == 0) return -1;
            return (int)data.Rows[0]["DocId"];
        }

        public static DataTable GetTask510Files(int wfid, int subprocessid)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@WfId", wfid);
            dba.dxAddParameter("@SubProcessId", subprocessid);

            return dba.dxGetSPData("DocGetTask510Docs");
        }

        public static DataTable GetUploadFiles(string fileNo)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@fileNo", fileNo);
            return dba.dxGetSPData("DocGetUploadDocs");
        }
        public static DataTable GetUploadFilesIns(int InsAccId)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@InsAccId", InsAccId);
            return dba.dxGetSPData("DocGetUploadDocsIns");
        }

        public static DataTable GetUploadFileTypes()
        {
            DataAccess dba = new DataAccess();
            return dba.dxGetTable("Select FileId, [Desc] FileType, ProcessId, usedby From StrFiles Order By FileId");
        }

        public static string GetCustomerAuthorizationFile(int detId)
        {
            DataAccess dba = new DataAccess();
            //string sql = "Select FilePath + FileName From StrFilesDet Where FileId = 6 And wfId = " + wfId;  //Or
            //string sql = "Select FilePath + FileName From StrFilesDet Where FileId = 1 And wfId = " + wfId; //?
            string sql = "Select FilePath + FileName from strFilesDet where KeyId = (select SIndexId from wfDet where detId =" + detId + ")";

            string file = dba.dxGetTextData(sql);
            if (file.Equals(""))
                return "";
            else
            {
                string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
                if (!strDBfilePath.EndsWith(@"\"))
                    strDBfilePath += @"\";
                file = strDBfilePath + file;
                if (File.Exists(file))
                    return file;
                else
                    return "";
            }
        }
        public static string GetChequeRequistion(int CrId)
        {
            DataAccess dba = new DataAccess();
            //string sql = "Select FilePath + FileName From StrFilesDet Where FileId = 6 And wfId = " + wfId;  //Or
            //string sql = "Select FilePath + FileName From StrFilesDet Where FileId = 1 And wfId = " + wfId; //?
            string sql = "Select FilePath + FileName from strFilesDet where KeyId = (select fileid from apinvoices where crno=" + CrId + ")";

            string file = dba.dxGetTextData(sql);
            if (file.Equals(""))
                return "";
            else
            {
                string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
                if (!strDBfilePath.EndsWith(@"\"))
                    strDBfilePath += @"\";
                file = strDBfilePath + file;
                if (File.Exists(file))
                    return file;
                else
                    return "";
            }
        }
        public static string GetCustomerCCFile(int detId)
        {
            DataAccess dba = new DataAccess();
            //string sql = "Select FilePath + FileName From StrFilesDet Where FileId = 6 And wfId = " + wfId;  //Or
            //string sql = "Select FilePath + FileName From StrFilesDet Where FileId = 1 And wfId = " + wfId; //?
            string sql = "Select FilePath + FileName from strFilesDet where KeyId = (select convert(int,Note) from wfdet where detid=" + detId + ")";

            string file = dba.dxGetTextData(sql);
            if (file.Equals(""))
                return "";
            else
            {
                string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
                if (!strDBfilePath.EndsWith(@"\"))
                    strDBfilePath += @"\";
                file = strDBfilePath + file;
                if (File.Exists(file))
                    return file;
                else
                    return "";
            }
        }

        public static int DeleteFile(SqlCommand command, int detId)
        {
            string sql = "delete from strFilesDet where KeyId = (select SIndexId from wfDet where detId = " + detId + ")";
            try
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static string GetFileName(string fileName)
        {
            fileName = fileName.Replace("/", @"\");
            int lastPos = fileName.LastIndexOf(@"\");
            return fileName.Substring(lastPos + 1);
        }

        #endregion

        #region Attributes
        private int _fileId = -1;
        public int FileId
        {
            get { return this._fileId; }
        }

        private int _mastId;     // fileType
        private int _detCount;   // count of files of mastId 
        private string _fileName = "";
        private string _filePath = "";
        private bool _hasFile = false;
        private int _wfId = 0;
        private System.Web.HttpPostedFile _uploadFile;
        private string _fileDesc;
        private int _processId;
        private string _fileNo = "";
        private bool _canSave = false;
        private int _uploadbyId;

        #endregion

        public bool hasFile
        {
            get { return this._hasFile; }
            set { this._hasFile = value; }
        }
        public UploadDoc(string fileNo)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@fileNo", fileNo);
            object obj = dba.dxGetDataObject("Select wfId From wfMast Where ProcessRef=@fileNo");
            if ((obj != System.DBNull.Value) && (obj != null))
            {
                this._wfId = (int)obj;
                this._fileNo = fileNo;
            }
        }

        public UploadDoc(string InsAccid,bool overwrite)
        {
            this._wfId = Convert.ToInt32(InsAccid);
            this._fileNo = "-9999";

        }
        public UploadDoc(int fileId)
        {
            this._fileId = fileId;
            this.Init();
        }
        public UploadDoc(int fileId, string InsAccId)
        {
            this._fileId = fileId;
            this._fileNo = "-9999";
            this.Init();
        }
        private void Init()
        {
            DataAccess dba = new DataAccess();
            DataTable dta = new DataTable();

            string sql;
            //			sql = "Select Count(*) From StrFilesDet D Join StrFiles M On M.FileId = D.FileId Where M.FileId in (Select FileId From StrFilesDet Where KeyId =@fileId)";
            //			dba.dxAddParameter("@fileId", this._fileId );
            //			this._detCount = (int)dba.dxGetDataObject(sql);
            //			if (this._detCount == 0) return;
            if (this._fileNo != "-9999")
            {
                sql = "Select FileId, FileName, FilePath From StrFilesDet Where KeyId = @fileId";
            }
            else
            {
                sql = "Select FileId, FileName, FilePath From InsFilesDet Where KeyId = @fileId";
            }
            dba.dxAddParameter("@fileId", this._fileId);
            dta = dba.dxGetTable(sql);

            if (dta.Rows.Count > 0)
            {
                DataRow row = dta.Rows[0];
                this._mastId = (int)row["FileId"];
                this._fileName = row["FileName"].ToString();
                this._filePath = row["FilePath"].ToString();
                this._hasFile = true;
            }
        }

        public string FilePath
        {
            get { return this._filePath; }
        }

        public string FileName
        {
            get { return this._fileName; }
        }

        public string getDBFilePath
        {
            get
            {
                DataAccess dba = new DataAccess();
                return dba.dxGetSPString("wfGetFilesRootPath");
            }
        }

        public string FileFullPath
        {
            get
            {
                string strDBfilePath = this.getDBFilePath;
                if (!strDBfilePath.EndsWith(@"\"))
                    strDBfilePath += @"\";
                return strDBfilePath + this._filePath + this._fileName;
            }
        }

        public int DeleteFile(ref string message)
        {
            if (!this._hasFile)
            {
                message = "No file for the file ID, " + this._fileId + ".";
                return 0;  // no file	
            }
            DataAccess dba = new DataAccess();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                SqlCommand command = conn.CreateCommand();
                if (this._fileNo != "-9999")
                {
                    command.CommandText = "Delete From StrFilesDet Where KeyId = @fileId";
                }
                else
                {
                    command.CommandText = "Delete From insfilesdet where keyid=@fileId";
                }

                command.Parameters.AddWithValue("@fileId", this._fileId);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    message = ex.Message;

                    return -1;
                }
                catch (Exception ex)
                {
                    message = ex.Message;

                    return -1;
                }
                finally
                {
                    if (command != null) command.Dispose();
                }

                try
                {
                    string fullPath = this.FileFullPath; //this._filePath + this._fileName;
                    System.IO.File.Delete(fullPath);
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    message = ex.Message;
                    return 1;
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    message = ex.Message;
                    return 1;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return 1;
                }
                if (FileFullPath.Contains(".pdf"))
                {
                    try
                    {
                        string fullPath = this.FileFullPath.Replace(".pdf", ".jpg"); //this._filePath + this._fileName;
                        System.IO.File.Delete(fullPath);
                    }
                    catch (System.IO.DirectoryNotFoundException ex)
                    {
                        message = ex.Message;
                        return 1;
                    }
                    catch (System.IO.FileNotFoundException ex)
                    {
                        message = ex.Message;
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return 1;
                    }
                }
                return 1;   // success
            }
        }

        public int SaveFile(System.Web.HttpPostedFile uploadFile, int fileType, string fileDesc, int userId, bool overwrite)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            this._uploadFile = uploadFile;
            this._uploadbyId = userId;
            this._fileDesc = fileDesc;
            this._mastId = fileType;
            this.SetFilePath();
            this.SetFileName();
            return this.SaveFile(overwrite);
        }

        //private int SaveFile(System.Web.HttpPostedFile uploadFile, string fileDesc, int mastId, bool overwrite)
        //{
        //    // return value: 0 OK, -1: error, 1: file exsiting
        //    this._uploadFile = uploadFile;
        //    this._fileDesc = fileDesc;
        //    this._mastId = mastId;
        //    this.SetFilePath();
        //    this.SetFileName();
        //    return this.SaveFile(overwrite);
        //}

        //public static string GetFilePath(int FileId)
        //{
        //    DataAccess dba = new DataAccess();
        //    string sql = "Select FilePath + FileName from strFilesDet where KeyId = " + FileId;

        //    string file = dba.dxGetTextData(sql);
        //    if (file.Equals(""))
        //    {
        //        return "";
        //    }
        //    else
        //    {
        //        string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
        //        if (!strDBfilePath.EndsWith(@"\"))
        //        {
        //            strDBfilePath += @"\";
        //        }

        //        file = strDBfilePath + file;

        //        if (File.Exists(file))
        //        {
        //            return file;
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //}

        private int SaveFile(bool overwrite)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            DataAccess dba = new DataAccess();
            string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
            if (!System.IO.Directory.Exists(strDBfilePath + this._filePath))
            {
                System.IO.Directory.CreateDirectory(strDBfilePath + this._filePath);
            }
            int versionNo = 0;
            int lastPos = this._fileName.LastIndexOf(@".");
            string suffix = this._fileName.Substring(lastPos);
            string prefix = this._fileName.Substring(0, lastPos);
            string file = strDBfilePath + this._filePath + this._fileName;
            if (System.IO.File.Exists(file))
            {

                    //SP --> inmplement versioning of all files if the upload filename exists already; ie create xxx(1),xxx(2)... xxx(n).
                    do
                    {
                        versionNo++;
                        file = strDBfilePath + this._filePath + prefix + "(" + versionNo + ")" + suffix;
                        if (!System.IO.File.Exists(file))
                        {
                            if (this.SaveFile(file))
                            {
                                tryPDFconversion(file, suffix);
                                _fileName = prefix + "(" + versionNo + ")" + suffix;
                                if (this.SaveFileInfo().Equals(""))
                                {
                                    return 0;
                                }
                                else
                                {
                                    return -1;
                                }
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    } while (versionNo > 0);
                    //SP <---

            }

            if (this.SaveFile(strDBfilePath + this._filePath + this._fileName))
            {
                string msg = tryPDFconversion(file, suffix);
                if (msg == "")
                {
                    if (this.SaveFileInfo().Equals(""))
                    {
                        return 0;
                    }
                    else
                    {
                        return -2;
                    }
                }
                else
                {
                    return -3; // PDF Conversion Failed
                }

            }
            else
            {
                return -1; //File Save Failed
            }
        }

        private string tryPDFconversion(string source, string suffix)
        {
            string destination = source.Replace(suffix, ".pdf");
            if (suffix.ToLower() == ".pdf") return "";
            if ((suffix == ".doc") || (suffix == ".docx"))
            {
                ConvertToPDF(source, suffix);
                return "";
            }
            else
            {
                //ReorientImg(source, suffix);
                //checkphotoorientation(source);
                int wid = 580; int hig = 750;
                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }
                try
                {
                    PdfDocument doc = new PdfDocument();
                    doc.Pages.Add(new PdfPage());
                    XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
                    XImage img = XImage.FromFile(source);

                    // xgr.DrawImage(img, 0, 0);
                    xgr.DrawImage(img, 10, 10, wid, hig);
                    doc.Save(destination);
                    doc.Close();
                    img.Dispose();

                    // succeeded in creating a pdf - change the filename & delete the other
                    this._fileName = this._fileName.Replace(suffix, ".pdf");
                    
                    //source = GetFileName(this._uploadFile.FileName.Replace(" ", "").Replace("#", "").Replace("&", ""));
                    File.Delete(source);
                    return "";
                }
                catch (Exception ex)
                {
                    return "Failed to Convert Image to PDF"; ;   // can't create a pdf so leave the original upload.
                }
            }
        }
        static void ConvertToPDF(string inputFileName, string suffix)
        {
            // This doesn't do anything with .doc/.dox files yet
        }

        //private static void checkphotoorientation(string imagesrc)
        //{
        //    //imagesrc = imagesrc.ToLower().Replace(@"~", "c://inetpub/wwwroot/strwebflow").Trim();
        //    var bmp = new Bitmap(imagesrc);
        //    var exif = new Goheer.EXIF.EXIFextractor(ref bmp, "n"); // get source from http://www.codeproject.com/KB/graphics/exifextractor.aspx?fid=207371

        //    if (exif["Orientation"] != null)
        //    {
        //        RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

        //        if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
        //        {
        //            bmp.RotateFlip(flip);
        //            exif.setTag(0x112, "1"); // Optional: reset orientation tag
        //            bmp.Save(imagesrc, ImageFormat.Jpeg);
        //        }
        //    }
        //}
        //private static RotateFlipType OrientationToFlipType(string orientation)
        //{
        //    switch (int.Parse(orientation))
        //    {
        //        case 1:
        //            return RotateFlipType.RotateNoneFlipNone;
        //            break;
        //        case 2:
        //            return RotateFlipType.RotateNoneFlipX;
        //            break;
        //        case 3:
        //            return RotateFlipType.Rotate180FlipNone;
        //            break;
        //        case 4:
        //            return RotateFlipType.Rotate180FlipX;
        //            break;
        //        case 5:
        //            return RotateFlipType.Rotate90FlipX;
        //            break;
        //        case 6:
        //            return RotateFlipType.Rotate90FlipNone;
        //            break;
        //        case 7:
        //            return RotateFlipType.Rotate270FlipX;
        //            break;
        //        case 8:
        //            return RotateFlipType.Rotate270FlipNone;
        //            break;
        //        default:
        //            return RotateFlipType.RotateNoneFlipNone;
        //    }
        //}
        static void ReorientImg(string sourcepath, string suffix)
        {
            Image img = Image.FromFile(sourcepath);
            string target = sourcepath.Replace(suffix, "._rot");
            foreach (var prop in img.PropertyItems)
            {
                if (prop.Id == 0x0112) //value of EXIF rotation property
                {
                    int orientationValue = img.GetPropertyItem(prop.Id).Value[0];
                    RotateFlipType rotateFlipType = GetOrientationToFlipType(orientationValue);
                    if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
                    {
                        img.RotateFlip(rotateFlipType);
                        img.Save(target);
                        File.Copy(target, sourcepath, true);
                    }
                    break;
                }
            }
        }
        private static RotateFlipType GetOrientationToFlipType(int orientationValue)
        {
            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            switch (orientationValue)
            {
                case 1:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    rotateFlipType = RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    rotateFlipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    rotateFlipType = RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    rotateFlipType = RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    rotateFlipType = RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
            }
            return rotateFlipType;
        }


        public int SaveUploadedSplitInvoice(string uploadFilePath, string fileName, int fileType, string fileDesc, int userId, bool overwrite)
        {
            // return value:  -1: error, 1: file exsiting, >1: OK (returns fileId)			
            SaveFileSI(fileName, fileType, fileDesc, userId);
            int result = this.SaveUploadedSplitInvoice(overwrite, uploadFilePath);
            if (result == 0) result = this._fileId;
            return result;
        }
        public int SaveFileSplitInvoice(System.Web.HttpPostedFile uploadFile, string fileName, int fileType, string fileDesc, int userId, bool overwrite)
        {
            // return value: 0 OK, -1: error, 1: file exsiting			
            SaveFileSI(fileName, fileType, fileDesc, userId);
            return this.SaveFileSplitInvoice(overwrite, uploadFile);
        }
        public void SaveFileSI(string fileName, int fileType, string fileDesc, int userId)
        {
            this._uploadbyId = userId;
            this._fileDesc = fileDesc;
            this._mastId = fileType;
            this.SetFilePath();                         // sets subdirectory portion of name from wfId
            this.SetFileNameSplitInvoice(fileName);
        }
        
        private int SaveFileSplitInvoice(bool overwrite, System.Web.HttpPostedFile uploadFile)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            DataAccess dba = new DataAccess();
            string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
            if (!System.IO.Directory.Exists(strDBfilePath + this._filePath)) 
            {
                System.IO.Directory.CreateDirectory(strDBfilePath + this._filePath);
            }
            string file = strDBfilePath + this._filePath + this._fileName;    
            if (System.IO.File.Exists(file)) 
            {
                if (overwrite)
                {
                    if (this.SaveFileSplitInvoice(strDBfilePath + this._filePath + this._fileName, uploadFile)) { return 0; }
                        else { return -1; }
                }
                else return 1;
            }

            if (this.SaveFileSplitInvoice(strDBfilePath + this._filePath + this._fileName, uploadFile))
            {
                if (this.SaveFileInfo().Equals("")) { return 0; }
                    else return -1;
            }
            else return -1;
        }
        private int SaveUploadedSplitInvoice(bool overwrite, string uploadFilePath)
        // version for using the already uploaded temporary file
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            if (uploadFilePath != "reference")
            {
                DataAccess dba = new DataAccess();
                string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
                if (!System.IO.Directory.Exists(strDBfilePath + this._filePath))
                {
                    System.IO.Directory.CreateDirectory(strDBfilePath + this._filePath);
                }
                string file = strDBfilePath + this._filePath + this._fileName;
                if (System.IO.File.Exists(file))
                {
                    if (overwrite) File.Delete(strDBfilePath + this._filePath + this._fileName);
                    else return 1;
                }
                File.Copy(uploadFilePath, strDBfilePath + this._filePath + this._fileName);
            }
            if (this.SaveFileInfo().Equals("")) { return 0; }
                else return -1;
        }

        private bool SaveFileSplitInvoice(string fileName, System.Web.HttpPostedFile uploadFile)
        {
            try
            {
                if (uploadFile.ContentLength > 0) uploadFile.SaveAs(fileName); 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool SaveFile(string fileName)
        {
            try
            {
                this._uploadFile.SaveAs(fileName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string SaveFileInfo()
        {
            DataAccess dba = new DataAccess();
            string message = "";
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                SqlCommand command = conn.CreateCommand();
                command.Transaction = trans;
                this._fileId = this.SaveFileDetInfo(ref message, command);
                if (!message.Equals("") || this._fileId <= 0)
                {
                    trans.Rollback();
                    return message;
                }
                trans.Commit();
                return "";
            }
        }

        public string AddFile(string filepath, string filename, int wfid, int fileType, string fileDesc, int userId, bool overwrite)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            this._wfId = wfid;
            this._uploadbyId = userId;
            this._fileDesc = fileDesc;
            this._mastId = fileType;
            this._filePath = wfid.ToString() + "\\"; // filepath;
            this._fileName = filename;

            DataAccess dba = new DataAccess();
            int fileId = 0;
            string message = "";
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                SqlCommand command = conn.CreateCommand();
                command.Transaction = trans;
                this._fileId = this.SaveFileDetInfo(ref message, command);
                if (!message.Equals("") || this._fileId <= 0)
                {
                    trans.Rollback();
                    return message;
                }
                trans.Commit();
                return "";
            }
        }

        private int SaveFileDetInfo(ref string message, SqlCommand command)
        {
            if (this._fileNo == "-9999")
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocSaveUploadFileIns";
                command.Parameters.Clear();
                SqlParameter fileId = command.Parameters.Add("@fileId", SqlDbType.Int);
                fileId.Direction = ParameterDirection.InputOutput;
                fileId.Value = this._mastId;
                command.Parameters.Add("@wfId", this._wfId);
                command.Parameters.Add("@fileName", this._fileName);
                command.Parameters.Add("@filePath", this._filePath);
                command.Parameters.Add("@desc", this._fileDesc);
                command.Parameters.Add("@userId", this._uploadbyId);
                //@userId
                try
                {
                    command.ExecuteNonQuery();
                    if (fileId != null)
                    {
                        return (int)fileId.Value;
                    }
                    else
                    {
                        message = "Fail to insert data to FileDetail.";
                        return -1;
                    }
                }
                catch (SqlException ex)
                {
                    message = ex.Message;
                    return -1;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return -1;
                }
            }
            else
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocSaveUploadFile";
                command.Parameters.Clear();
                SqlParameter fileId = command.Parameters.Add("@fileId", SqlDbType.Int);
                fileId.Direction = ParameterDirection.InputOutput;
                fileId.Value = this._mastId;
                command.Parameters.Add("@wfId", this._wfId);
                command.Parameters.Add("@fileName", this._fileName);
                command.Parameters.Add("@filePath", this._filePath);
                command.Parameters.Add("@desc", this._fileDesc);
                command.Parameters.Add("@userId", this._uploadbyId);

                //@userId
                try
                {
                    command.ExecuteNonQuery();
                    if (fileId != null)
                    {
                        return (int)fileId.Value;
                    }
                    else
                    {
                        message = "Fail to insert data to FileDetail.";
                        return -1;
                    }
                }
                catch (SqlException ex)
                {
                    message = ex.Message;
                    return -1;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return -1;
                }
            }
        }

        private void SetFilePath()
        {
            //			DataAccess dba = new DataAccess();
            //			this._filePath = dba.dxGetSPString("wfGetFilesRootPath");
            //			if (!this._filePath.EndsWith(@"\"))
            //			{
            //
            //				this._filePath += @"\";
            //			}
            if (this._wfId > 0)
            {
                this._filePath += this._wfId + @"\";
            }
        }

        private void SetFileName()
        {
            this._fileName = this._uploadFile.FileName.Replace(" ", "").Replace("#", "").Replace("&", "");
            this._fileName = GetFileName(this._fileName);
            string[] tokens = this._fileName.Split(".".ToCharArray());
            if (tokens.Length > 2)
            {
                string ext = tokens[tokens.Length - 1];
                this._fileName = this._fileName.Replace(".", "");
                int len = this._fileName.Length - ext.Length;
                this._fileName = this._fileName.Substring(0, len) + "." + ext;
            }
        }

        private void SetFileNameSplitInvoice(string fileName)
        {
            this._fileName = fileName.Replace(" ", "").Replace("#", "").Replace("&", "");
            this._fileName = GetFileName(this._fileName);
            string[] tokens = this._fileName.Split(".".ToCharArray());
            if (tokens.Length > 2)
            {
                string ext = tokens[tokens.Length - 1];
                this._fileName = this._fileName.Replace(".", "");
                int len = this._fileName.Length - ext.Length;
                this._fileName = this._fileName.Substring(0, len) + "." + ext;
            }
        }


        #region ESX Files

        private void SetFileNameESX()
        {
            this._fileName = this._uploadFile.FileName.Replace("#", "").Replace("&", "");
            this._fileName = GetFileName(this._fileName);
            string[] tokens = this._fileName.Split(".".ToCharArray());
            if (tokens.Length > 2)
            {
                string ext = tokens[tokens.Length - 1];
                this._fileName = this._fileName.Replace(".", "");
                int len = this._fileName.Length - ext.Length;
                this._fileName = this._fileName.Substring(0, len) + "." + ext;
            }
        }

        private void SetFileNameESX(string fileName)
        {
            this._fileName = fileName.Replace("#", "").Replace("&", "");
            this._fileName = GetFileName(this._fileName);
            string[] tokens = this._fileName.Split(".".ToCharArray());
            if (tokens.Length > 2)
            {
                string ext = tokens[tokens.Length - 1];
                this._fileName = this._fileName.Replace(".", "");
                int len = this._fileName.Length - ext.Length;
                this._fileName = this._fileName.Substring(0, len) + "." + ext;
            }
        }

        private int SaveFileESX(bool overwrite)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            DataAccess dba = new DataAccess();
            string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
            if (!System.IO.Directory.Exists(strDBfilePath + this._filePath))
            {
                System.IO.Directory.CreateDirectory(strDBfilePath + this._filePath);
            }
            string file = strDBfilePath + this._filePath + this._fileName;
            if (System.IO.File.Exists(file))
            {
                if (overwrite)
                {
                    if (this.SaveFileESX(strDBfilePath + this._filePath + this._fileName))
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return 1;
                }
            }

            if (this.SaveFileESX(strDBfilePath + this._filePath + this._fileName))
            {
                if (this.SaveFileInfo().Equals(""))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        public int SaveFileESX(System.Web.HttpPostedFile uploadFile, string fileName, int fileType, string fileDesc, int userId, bool overwrite)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            this._uploadbyId = userId;
            this._fileDesc = fileDesc;
            this._mastId = fileType;
            this.SetFilePath();
            this.SetFileNameESX(fileName);
            return this.SaveFileESX(overwrite, uploadFile);
        }

        public int SaveFileESX(System.Web.HttpPostedFile uploadFile, int fileType, string fileDesc, int userId, bool overwrite)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            this._uploadFile = uploadFile;
            this._uploadbyId = userId;
            this._fileDesc = fileDesc;
            this._mastId = fileType;
            this.SetFilePath();
            this.SetFileNameESX();
            return this.SaveFileESX(overwrite);
        }

        private int SaveFileESX(bool overwrite, System.Web.HttpPostedFile uploadFile)
        {
            // return value: 0 OK, -1: error, 1: file exsiting
            DataAccess dba = new DataAccess();
            string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
            if (!System.IO.Directory.Exists(strDBfilePath + this._filePath))
            {
                System.IO.Directory.CreateDirectory(strDBfilePath + this._filePath);
            }
            string file = strDBfilePath + this._filePath + this._fileName;
            if (System.IO.File.Exists(file))
            {
                if (overwrite)
                {
                    if (this.SaveFileESX(strDBfilePath + this._filePath + this._fileName, uploadFile))
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return 1;
                }
            }

            if (this.SaveFileESX(strDBfilePath + this._filePath + this._fileName, uploadFile))
            {
                if (this.SaveFileInfo().Equals(""))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        private bool SaveFileESX(string fileName)
        {
            try
            {
                this._uploadFile.SaveAs(fileName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool SaveFileESX(string fileName, System.Web.HttpPostedFile uploadFile)
        {
            try
            {
                uploadFile.SaveAs(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}