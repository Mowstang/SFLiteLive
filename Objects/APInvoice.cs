using System;
using System.Data;
using System.Data.SqlClient;
using Doxess.Data;
using WorkFlow.Objects.BLL;

namespace SmartFlowLite
{
	/// <summary>
	/// Summary description for APInvoice.
	/// </summary>
	public class APInvoice
	{
		public enum InvoiceStatus : int
		{WaitingForApprovel=1, Pending, Completed}
		public enum ApAction : int {New_Blank_Invoice, New_Project_Invoice, Edit_Invoice, View_Invoice}

		#region Static Methods
		
		public static int GetBatchId()
		{
			DataAccess dba = new DataAccess();
			return dba.dxGetIntData("select Max(IsNull(BatchId, 0)) + 1 from APInvoices");
		}
        public static int GetSubProcessIdFromInvoiceId(int InvoiceId)
        {

            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@InvoiceId", InvoiceId);

            return dba.dxGetIntData("Select Subprocessid from apinvoices where InvoiceId=@InvoiceId");

        }
		public static void SetVerified(SqlCommand command, int invId, int userid, ref string message)
		{
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Clear();
			command.CommandText = "APInvoiceSetVerified";
			command.Parameters.AddWithValue("@invId", invId);
			command.Parameters.AddWithValue("@userid", userid);
			try
			{
				command.ExecuteNonQuery();
				message =  string.Empty;
			}
			catch(SqlException ex)
			{
				message =  ex.Message;
			}
			catch(Exception ex)
			{
				message =  ex.Message;
			}
		}
        public static void SetVerified(SqlCommand command, int invId, int userid, int detid, ref string message)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            command.CommandText = "APInvoiceSetVerifiedWithDetid";
            command.Parameters.AddWithValue("@invId", invId);
            command.Parameters.AddWithValue("@userid", userid);
            command.Parameters.AddWithValue("@detid", detid);
            try
            {
                command.ExecuteNonQuery();
                message = string.Empty;
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
        public static void SetVerified(SqlCommand command, int invId, int userid, DateTime verifieddate, ref string message)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            command.CommandText = "APInvoiceSetVerifiedWithDate";
            command.Parameters.AddWithValue("@invId", invId);
            command.Parameters.AddWithValue("@userid", userid);
            command.Parameters.AddWithValue("@VerifiedDate", verifieddate);

            try
            {
                command.ExecuteNonQuery();
                message = string.Empty;
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        public static void SetVerified(int invId, int userid, ref string message)
        {
            DateTime verifieddate = DateTime.Now;
            DataAccess dba = new DataAccess();

            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                SqlCommand command = conn.CreateCommand();
                command.Transaction = trans;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.CommandText = "APInvoiceSetVerifiedWithDate";
                command.Parameters.AddWithValue("@invId", invId);
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@VerifiedDate", verifieddate);

                try
                {
                    command.ExecuteNonQuery();
                    message = string.Empty;
                }
                catch (SqlException ex)
                {
                    message = ex.Message;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }

                trans.Commit();
            }
        }
        public static int APSetVerified(int invoiceId, int userid)
        {
            DataAccess sqlCom = new DataAccess();

            sqlCom.dxAddParameter("@InvoiceId", invoiceId);
            sqlCom.dxAddParameter("@userid", userid);
            return sqlCom.dxExecuteSP("APSetVerified");
        }
        public static void SetApproved(int invId, int userid, ref string message)
        {
            DateTime verifieddate = DateTime.Now;
            DataAccess dba = new DataAccess();

            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                SqlCommand command = conn.CreateCommand();
                command.Transaction = trans;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.CommandText = "APInvoiceSetApprovedWithDate";
                command.Parameters.AddWithValue("@invId", invId);
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@VerifiedDate", verifieddate);

                try
                {
                    command.ExecuteNonQuery();
                    message = string.Empty;
                }
                catch (SqlException ex)
                {
                    message = ex.Message;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }

                trans.Commit();
            }
        }
		public static DataTable GetAPInvoiceCurrData(DateTime StartDate, DateTime EndDate, int BrId)
		{
			DataAccess dba = new DataAccess();
			dba.dxClearParameters();
			dba.dxAddParameter("@StartDate", StartDate);
			dba.dxAddParameter("@EndDate", EndDate);
			dba.dxAddParameter("@BrId", BrId);
			return dba.dxGetSPData("GetAPInvoiceCurrent");			
		}

		public static DataTable GetAPInvoiceOutData(DateTime StartDate, int BrId)
		{
			DataAccess dba = new DataAccess();
			dba.dxClearParameters();
			dba.dxAddParameter("@StartDate", StartDate);			
			dba.dxAddParameter("@BrId", BrId);
			return dba.dxGetSPData("GetAPInvoiceOutStanding");			
		}

		public static bool HasInvoiceNo(string invoiceNo, int VendorId)
		{
			string sql = "Select Count(*) From APInvoices Where InvoiceNo = '" + invoiceNo.Replace("'", "''") + "'" +
				" and VendorId = " + VendorId;
			DataAccess dba = new DataAccess();
			//dba.dxAddParameter("@InvoiceAccNo", invoiceNo);
			return dba.dxGetIntData(sql) > 0;
		}

		public static bool HasInvoiceNo(string invoiceNo, int invId, int batchId, int VendorId)
		{
			string sql = "Select Count(*) From APInvoices Where InvoiceId != " + invId + " and " +
				"InvoiceNo = '" + invoiceNo.Replace("'", "''") + "' and (batchId = 0 or BatchId !=" + batchId + ")" + 
				" and VendorId = " + VendorId;
			DataAccess dba = new DataAccess();
			//dba.dxAddParameter("@InvoiceAccNo", invoiceNo);
			return dba.dxGetIntData(sql) > 0;

		}

		public static DataTable GetIncomingListByWfId(int wfId)
		{
			DataAccess dba = new DataAccess();
			dba.dxAddParameter("@wfId", wfId);
			return dba.dxGetSPData("InvoiceIncomingListByWfId");
		}

//		private static DataTable SetActiveList(DataTable data)
//		{
//			DataTable rev = data.Clone();
//			DataRow newRow = null;
//			foreach(DataRow row in data.Rows)
//			{
//				if(row["JobType"].ToString().Trim().Equals("ER"))
//				{
//					newRow = rev.NewRow();
//					newRow.ItemArray = row.ItemArray;
//					newRow["JobType"] = "E";					
//					rev.Rows.Add(newRow);
//					newRow = rev.NewRow();
//					newRow.ItemArray = row.ItemArray;
//					newRow["JobType"] = "R";					
//					newRow["CountSum"] = 0;
//					rev.Rows.Add(newRow);
//				}
//				else
//				{
//					newRow = rev.NewRow();
//					newRow.ItemArray = row.ItemArray;					
//					rev.Rows.Add(newRow);
//				}
//			}
//			return rev;
//		}


		public static DataTable GetApInvoiceActive801List
		{
			get
			{
				DataAccess dba = new DataAccess();
				return dba.dxGetSPData("Invoice801List");
			}
		}

        public static DataTable GetApInvoiceExportList(string brCode)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@BranchCode", brCode);
            return dba.dxGetSPData("APInvoiceExportList");
        }
		
		public static string SaveApInvoiceComments(SqlCommand command, int WfId, int DetId, int UserId, string Comments,
			string LogType, string VendorName, string InvoiceNo, decimal amount, int SubProcessId)
		{
			command.CommandType = CommandType.StoredProcedure ;
			command.Parameters.Clear();
			command.CommandText = "InvoiceAddComments";
			command.Parameters.Add("@wfId", WfId);
			command.Parameters.Add("@DetId", DetId);
			command.Parameters.Add("@UserId", UserId);
			command.Parameters.Add("@LogDesc", Comments);
			command.Parameters.Add("@Type", LogType);
			command.Parameters.Add("@VendorName", VendorName);
			command.Parameters.Add("@InvoiceNo", InvoiceNo);
			command.Parameters.Add("@amount", amount);
			command.Parameters.Add("@SubProcessId", SubProcessId);

			try
			{
				command.ExecuteNonQuery();
				return string.Empty;
			}
			catch(SqlException ex)
			{
				return ex.Message;
			}
			catch(Exception ex)
			{
				return ex.Message;
			}
		}

		public static int GetApInvoice800DetId(int WfId)
		{
			DataAccess dba = new DataAccess();
			//string sql = "Select DetId From wfDet Where ActionId=22650 And wfId = " + WfId;			
			/*string sql = "SELECT APInvoices.InvoiceId, APInvoices.ClaimId, APInvoices.InvoiceDate, " +
							" APInvoices.InvoiceNo, APInvoices.VendorId, APInvoices.VendorName, " +
							" APInvoices.Amount, APInvoices.PONo, APInvoices.SubProcessId, " + 
							" APInvoices.InvoiceStatusId, APInvoices.FileId, APInvoices.CostCodeId " + 
							" FROM 	APInvoices " + 
									" INNER JOIN wfDet ON APInvoices.InvoiceId = wfDet.PIndexId " + 
									" AND APInvoices.InvoiceId IN " +
							"(SELECT PIndexId FROM wfDet WHERE wfid =" + WfId;
							*/
			string sql = "SELECT PIndexId FROM wfDet WHERE wfid =" + WfId;
			return dba.dxGetIntData(sql);
		}

		public static decimal GetDefMarginPOApInvoice()
		{
			DataAccess dba = new DataAccess();
			string sql = "select DefMarginPOApInvoice from wfSysRec";
			return Decimal.Parse(dba.dxGetData(sql).ToString());
		}
		
		
		public static string GetApInvoiceComments(int WfId, int DetId)
		{
			DataAccess dba = new DataAccess();
			string sql = "Select LogDesc From wfLogs Where DetId = " + DetId + " And wfId=" + WfId;			
			return dba.dxGetTextData(sql);
		}

		public static int GetInvoiceId(int detId)
		{
			DataAccess dba = new DataAccess();
			return dba.dxGetIntData("Select PIndexId From wfDet Where DetId = " +detId);			
		}

		public static int GetScannedFileId(int detId)
		{
			DataAccess dba = new DataAccess();
			return dba.dxGetIntData("SELECT  A.FileId From APInvoices A Join wfDet D On A.InvoiceId = D.PIndexId Where D.DetId = " +detId);
		}

		public static DataTable GetVentor
		{
			get
			{
				DataAccess dba = new DataAccess();
				DataTable data = dba.dxGetSPData("InvoiceVendorList");
//				DataRow row = data.NewRow();
//				row["VendorId"] = 0;
//				row["Name"] = "Other";
//				data.Rows.Add(row);
				return data;
			}		   
		}

		public static DataTable GetActiveProjects
		{
			get
			{
				DataAccess dba = new DataAccess();
				return SetTypeData(dba.dxGetSPData("InvoiceActiveApProject"));
			}
		}
        public static DataTable GetActiveProject(int wfid, int subprocessid)
        {

                DataAccess dba = new DataAccess();
                dba.dxAddParameter("@wfid", wfid);
                dba.dxAddParameter("@subprocessid", subprocessid);
                return SetTypeData(dba.dxGetSPData("InvoiceCurrentApProject"));

        }

		private static DataTable SetTypeData(DataTable data)
		{
			DataRow newRow = null;
			DataTable rev = data.Clone();
			foreach(DataRow row in data.Rows)
			{				
				newRow = rev.NewRow();
				newRow.ItemArray = row.ItemArray;
				newRow["ClaimId"] += ":" + row["JobType"].ToString().Trim();
				//newRow["ClaimNo"] = row["ClaimNo"].ToString() + row["JobType"].ToString().Trim();
				newRow["FileNo"] = row["FileNo"].ToString().Trim() + row["JobType"].ToString().Trim() + " " + row["JobName"].ToString().Trim();
				rev.Rows.Add(newRow);				
			}
			newRow = rev.NewRow();
			newRow["FileNo"] = "";
			newRow["ClaimId"] = 0;
			rev.Rows.InsertAt(newRow, 0);			
			return rev;
		}

		public static DataTable GetActiveSplitProjects(int brId, int vendorId)
		{
			DataAccess dba = new DataAccess();			
			dba.dxClearParameters();
			dba.dxAddParameter("@brId", brId);
			dba.dxAddParameter("@VendorId", vendorId);
			return SetTypeDataSplit(dba.dxGetSPData("InvoiceActiveApSplitProject"));
		}

        public static DataTable GetActiveSplitProjectsByBranchByUser(int brId, int userid, int vendorId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            
            dba.dxAddParameter("@brId", brId);
            dba.dxAddParameter("@UserId", userid);
            dba.dxAddParameter("@VendorId", vendorId);
            
            return SetTypeDataSplit(dba.dxGetSPData("InvoiceActiveApSplitProject"));
        }

		private static DataTable SetTypeDataSplit(DataTable data)
		{
			DataRow newRow = null;
			DataTable rev = data.Clone();
			foreach(DataRow row in data.Rows)
			{	
				string strJobType = string.Empty;
				if (Int32.Parse(row["SubProcessId"].ToString().Trim()) == 1)
					strJobType = "E";
				else
					strJobType = "R";
				newRow = rev.NewRow();
				newRow.ItemArray = row.ItemArray;
				newRow["ClaimId"] += ":" + strJobType;
				//newRow["ClaimNo"] = row["ClaimNo"].ToString() + row["JobType"].ToString().Trim();
				newRow["FileNo"] = row["FileNo"].ToString().Trim() + strJobType + " " + row["JobName"].ToString().Trim();
				rev.Rows.Add(newRow);
			}
			newRow = rev.NewRow();
			newRow["FileNo"] = "";
			newRow["ClaimId"] = 0;
			rev.Rows.InsertAt(newRow, 0);			
			return rev;
		}

		#endregion


		#region Atributes
		private bool _hasData = false;
		public bool HasData
		{
			get{return this._hasData;}

		}
		private DateTime _invoiceDate;
		public DateTime InvoiceDate
		{
			get{return this._invoiceDate;}
			set{this._invoiceDate = value;}
		}

		private string _invocieNo;
		public string InvoiceNo
		{
			get{return this._invocieNo;}
			set{this._invocieNo = value;}
		}

		private int _poNo = -1;
		public int PONo
		{
			get{return this._poNo;}
			set{this._poNo= value;}
		}

		private int _crNo = -1;
		public int CRNo
		{
			get{return this._crNo;}
			set{this._crNo = value;}
		}

		private string _vendorName = "";
		public string VendorName
		{
			get{return this._vendorName;}
			set{this._vendorName= value;}
		}

		private int _vendorId;
		public int VendorId
		{
			get{return this._vendorId;}
			set{this._vendorId= value;}
		}

		private int _claimId;
		public int ClaimId
		{
			get{return this._claimId;}
			set{this._claimId= value;}
		}

		private decimal _amount = (decimal) 0;
		public decimal Amount
		{
			get{return this._amount;}
			set{this._amount= value;}
		}
	
		private decimal _gstAmount;
		public decimal GSTAmount
		{
			get{return this._gstAmount;}
			set{this._gstAmount = value;}
		}

		private decimal _pstAmount;
		public decimal PSTAmount
		{
			get{return this._pstAmount;}
			set{this._pstAmount = value;}
		}

        private decimal _qstAmount;
        public decimal QSTAmount
        {
            get { return this._qstAmount; }
            set { this._qstAmount = value; }
        }

        public decimal ProvincialTax
        {
            get
            {
                if (WorkFlow.BLL.Invoice.APInvoice.getProvincialTaxFieldDescription(this.VendorId, WorkFlow.BLL.Claim.Claim.GetBranchId(this.ClaimId, (this.SubProcess == 1 ? "E" : "R"))) == "PST")
                {
                    return this._pstAmount;
                }
                else
                {
                    return this._qstAmount;
                }
            }
        }

		private decimal _totalAmount;
		public decimal TotalAmount
		{
			get{return this._totalAmount;}
			set{this._totalAmount = value;}
		}

		private int _invoiceId;
		public int InvoiceId
		{
			get{return this._invoiceId;}
		}

		private string _fileNo = "";
		public string FileNo
		{
			get{return this._fileNo;}
		}
		private string _jobName = "";
		public string JobName
		{
			get{return this._jobName;}
		}
		private int _subProcess;
		public int SubProcess
		{
			get{return this._subProcess;}
			set{this._subProcess= value;}
		}
		
		private int origal_fileId;
		private int _fileId;
		public int FileId
		{
			get{return this._fileId;}
			set
			{
				this.origal_fileId = this._fileId;
				this._fileId = value;
			}
		}
		private string _filePath = "";
		public string FilePath
		{
			get{return this._filePath;}
		}

		private string _fileName = "";
		public string FileName
		{
			get{return this._fileName;}
		}
												
		private int _CostCodeId;
		public int CostCodeId
		{
			get{return this._CostCodeId;}
			set{this._CostCodeId = value;}
		}

		private bool _history;
		public bool History
		{
			get{return this._history;}
			set{this._history = value;}
		}

        private bool _isVerified;
        public bool IsVerified
        {
            get { return this._isVerified; }
            set { this._isVerified = value; }
        }

		private int _batchId = 0;
		public int BatchId
		{
			get{return this._batchId;}
			set{this._batchId = value;}
		}

		//Added jwalin
		private int _processId = 22;
		public int ProcessId
		{
			get{return this._processId;}
			set{this._processId = value;}
		}

		//Added jwalin
		private int _invoiceIndexId = 1;
		public int InvoiceIndexId
		{
			get{return this._invoiceIndexId;}
			set{this._invoiceIndexId = value;}
		}

		private string _wfid = "";
		public string WfId
		{
			get{return this._wfid;}
			set{this._wfid = value;}
		}

        private int _AdminOrClaims;
        public int AdminOrClaims
        {
            get { return this._AdminOrClaims; }
            set { this._AdminOrClaims = value; }
        }

        private string _Desc;
        public string Desc
        {
            get { return this._Desc; }
            set { this._Desc = value; }
        }

        private string _Abbr;
        public string Abbr
        {
            get { return this._Abbr; }
            set { this._Abbr = value; }
        }

        private string _EmpUserId;
        public string EmpUserId
        {
            get { return this._EmpUserId; }
            set { this._EmpUserId = value; }
        }

        private string _FileNumberProvidedCorrect;
        public string  FileNumberProvidedCorrect
        {
            get { return this._FileNumberProvidedCorrect; }
            set { this._FileNumberProvidedCorrect = value; }
        }
       
		#endregion
		
		public APInvoice()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public APInvoice(int invoiceId)
		{
			this._invoiceId = invoiceId;
			this.Init();		
		}

        public static DataTable GetClaimsForApprovedInvoices(int BranchId)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@BrId", BranchId);
            return SetTypeDataSplit(dba.dxGetSPData("getClaimsByBranch"));
        }

        public static DataTable GetClaimsForApprovedInvoicesByBranchByUser(int branchid, int userid)
        {
            DataAccess dba = new DataAccess();

            dba.dxAddParameter("@BranchId", branchid);
            dba.dxAddParameter("@UserId", userid);

            return SetTypeDataSplit(dba.dxGetSPData("getClaimsByBranchByUser"));
        }

		private void Init()
		{
			DataAccess dba = new DataAccess();
			dba.dxAddParameter("@InvoiceId", this._invoiceId);
			DataTable data = dba.dxGetSPData("InvoiceGetApInvoiceData"); 
			if (data.Rows.Count >0)
			{
				DataRow row = data.Rows[0];
				this._amount = DataHelper.GetMoney(row, "Amount");
				this._gstAmount = DataHelper.GetMoney(row, "GSTAmount");
				this._pstAmount = DataHelper.GetMoney(row, "PSTAmount");
                this._qstAmount = DataHelper.GetMoney(row, "QSTAmount");
				this._totalAmount = DataHelper.GetMoney(row, "TotalAmount");
				this._claimId = DataHelper.GetInt(row, "ClaimId");
				this._invocieNo = DataHelper.GetString(row, "InvoiceNo");
				this._invoiceDate = DataHelper.GetDate(row, "InvoiceDate");
				this._poNo = DataHelper.GetInt(row, "PONo");
				this._crNo = DataHelper.GetInt(row, "CRNo");
				this._vendorId = DataHelper.GetInt(row, "VendorId");
				SetVentorName(row);
				this._fileNo = DataHelper.GetString(row, "FileNo");
				this._jobName = DataHelper.GetString(row, "JobName");
				this.FileId = DataHelper.GetInt(row, "FileId");
				this.SetFilePath();
				this.CostCodeId = DataHelper.GetInt(row, "CostCodeId");
				this.BatchId = DataHelper.GetInt(row, "BatchId");
				this._hasData = true;
				//Added jwalin
				this._processId = DataHelper.GetInt(row, "ProcessId");
				this._invoiceIndexId = DataHelper.GetInt(row, "InvoiceIndexId");
				this._subProcess = DataHelper.GetInt(row, "SubProcessId");
			}
		}

		private void SetFilePath()
		{
			DataAccess dba = new DataAccess();
			DataTable data = dba.dxGetTable("Select FileName, FilePath From StrFilesDet Where KeyId =" + this._fileId);
			if (data.Rows.Count >0)
			{
				this._filePath = data.Rows[0]["FilePath"].ToString();
				this._fileName = data.Rows[0]["FileName"].ToString();
			}
		}

        public static int GetPMforClaims(int WfId, int SubProcessId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@WfId", WfId);
            dba.dxAddParameter("@SubProcessId", SubProcessId);
            return dba.dxGetSPInt("GetPMforClaims");
        }


        public static int GetBMforProjects(int WfId, int ProjectId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@WfId", @WfId);
            dba.dxAddParameter("@ProjectId", @ProjectId);
            return dba.dxGetSPInt("GetBMforProjects");
        }


		private void SetVentorName(DataRow row)
		{
			DataAccess dba = new DataAccess();
			string sql = "Select [Name] From Vendors Where VendorId = " + this.VendorId ;
			string ventorName = dba.dxGetTextData(sql);
			if (!ventorName.Equals(""))
			{
				this._vendorName = ventorName;
			}
			else
			{
				this._vendorName = DataHelper.GetString(row, "VendorName");
			}
		}

		public int AddMaterialCost(ref string message, SqlCommand command, int wfId, int processId, int refTypeId)
		{

			if (!this._hasData)
			{
                message = "The invoice has not been created yet.";
				return -1;
			}
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "InvoiceAddJobCostFromApInvoice";
			command.Parameters.Clear();
			command.Parameters.Add("@invId", this.InvoiceId);
			command.Parameters.Add("@wfId", wfId);
			command.Parameters.Add("@processId", processId);
			command.Parameters.Add("@refTypeId", refTypeId);
			SqlParameter param = command.Parameters.Add("@jobCostId", SqlDbType.Int);
			param.Direction = ParameterDirection.Output;
			try
			{
				command.ExecuteNonQuery();
				message = "";
				return (int)param.Value;
			}
			catch (SqlException ex)
			{
				message = ex.Message;
				return -1;
			}
			catch(Exception ex)
			{
				message = ex.Message;
				return -1;
			}			
		}

		public string UpdateMaterialCost()
		{
			string message = "";

			DataAccess dba = new DataAccess();
			using (SqlConnection conn = dba.dxConnection)
			{
				SqlCommand command = conn.CreateCommand();
				conn.Open();
				this.UpdateMaterialCost(ref message, command, this.ProcessId, (int)WorkFlow.BLL.JobCost.JobCostRefType.AP_Invoices);
				return message;
			}
		}

		public int UpdateMaterialCost(ref string message, SqlCommand command, int processId, int refTypeId)
		{

			if (!this._hasData)
			{
                message = "The invoice has not been created yet.";
				return -1;
			}
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "InvoiceUpdateJobCostFromApInvoice";
			command.Parameters.Clear();
			command.Parameters.Add("@invId", this.InvoiceId);
			command.Parameters.Add("@processId", processId);
			command.Parameters.Add("@refTypeId", refTypeId);
			SqlParameter param = command.Parameters.Add("@jobCostId", SqlDbType.Int);
			param.Direction = ParameterDirection.Output;
			try
			{
				command.ExecuteNonQuery();
				message = "";
				return (int)param.Value;
			}
			catch (SqlException ex)
			{
				message = ex.Message;
				return -1;
			}
			catch(Exception ex)
			{
				message = ex.Message;
				return -1;
			}			
		}

		public string Update()
		{
			DataAccess dba = new DataAccess();
			using (SqlConnection conn = dba.dxConnection)
			{
				SqlCommand command = conn.CreateCommand();
				conn.Open();
				return this.Update(command);
			}
		}

		public string Update( SqlCommand command)
		{
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "InvoiceUpdateApInvoice";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@ClaimId",this._claimId);
			command.Parameters.AddWithValue("@InvoiceDate",this._invoiceDate );
			command.Parameters.AddWithValue("@InvoiceNo",this._invocieNo );
			command.Parameters.AddWithValue("@VendorId",this._vendorId  );
			command.Parameters.AddWithValue("@VendorName",this._vendorName);
			command.Parameters.AddWithValue("@Amount",this._amount);
			command.Parameters.AddWithValue("@GSTAmount",this._gstAmount);
			command.Parameters.AddWithValue("@PSTAmount",this._pstAmount);
            command.Parameters.AddWithValue("@QSTAmount", this._qstAmount);
            command.Parameters.AddWithValue("@TotalAmount",this._totalAmount);
			command.Parameters.AddWithValue("@SubProcess",this._subProcess);
			command.Parameters.AddWithValue("@PONo",this._poNo);
			command.Parameters.AddWithValue("@CRNo",this._crNo);
			command.Parameters.AddWithValue("@CostCodeId", this.CostCodeId);
			//command.Parameters.AddWithValue("@FileId", this.FileId);
			
			command.Parameters.AddWithValue("@invoiceId", this.InvoiceId);	
			try
			{
				command.ExecuteNonQuery();
				return string.Empty;					
			}
			catch(SqlException ex)
			{
				return ex.Message;				

			}
			catch(Exception ex)
			{
				return ex.Message;				
			}
		}

		public string AddToAPInvoiceHistory()
		{
			DataAccess dba = new DataAccess();
			using (SqlConnection conn = dba.dxConnection)
			{
				SqlCommand command = conn.CreateCommand();
				conn.Open();
				return this.AddToAPInvoiceHistory(command);
			}
		}

		public string AddToAPInvoiceHistory( SqlCommand command)
		{
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "InvoiceCreateApInvoiceHistory";
			command.Parameters.Clear();
			command.Parameters.AddWithValue("@ClaimId",this._claimId);
			command.Parameters.AddWithValue("@InvoiceDate",this._invoiceDate);
			command.Parameters.AddWithValue("@InvoiceNo",this._invocieNo);
			command.Parameters.AddWithValue("@VendorId",this._vendorId );
			command.Parameters.AddWithValue("@VendorName",this._vendorName);
			command.Parameters.AddWithValue("@Amount",this._amount);
			command.Parameters.AddWithValue("@GSTAmount",this._gstAmount);
			command.Parameters.AddWithValue("@PSTAmount",this._pstAmount);
            command.Parameters.AddWithValue("@QSTAmount", this._qstAmount);
            command.Parameters.AddWithValue("@TotalAmount",this._totalAmount);
			command.Parameters.AddWithValue("@SubProcess",this._subProcess);
			command.Parameters.AddWithValue("@PONo",this._poNo);
			command.Parameters.AddWithValue("@CRNo",this._crNo);
			command.Parameters.AddWithValue("@CostCodeId", this.CostCodeId);
			
			command.Parameters.AddWithValue("@invoiceId", this.InvoiceId);	
			try
			{
				command.ExecuteNonQuery();
				return string.Empty;					
			}
			catch(SqlException ex)
			{
				return ex.Message;				

			}
			catch(Exception ex)
			{
				return ex.Message;				
			}
		}


        public int AddNew(ref string message, SqlCommand command, int userId)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "InvoiceAddNewApInvoice";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ClaimId", this._claimId);
            command.Parameters.AddWithValue("@InvoiceDate", this._invoiceDate); 
            command.Parameters.AddWithValue("@InvoiceNo", this._invocieNo); 
            command.Parameters.AddWithValue("@VendorId", this._vendorId); 
            command.Parameters.AddWithValue("@VendorName", this._vendorName);
            command.Parameters.AddWithValue("@Amount", this._amount); 
            command.Parameters.AddWithValue("@GSTAmount", this._gstAmount); 
            command.Parameters.AddWithValue("@PSTAmount", this._pstAmount);
            command.Parameters.AddWithValue("@QSTAmount", this._qstAmount);
            command.Parameters.AddWithValue("@TotalAmount", this._totalAmount);


            if (this.FileId > 0)
            {
                command.Parameters.AddWithValue("@FileId", this.FileId);
            }
            command.Parameters.AddWithValue("@SubProcess", this._subProcess);
            if (this.PONo > 0)
            {
                command.Parameters.AddWithValue("@PONo", this._poNo);
            }
            if (this.CRNo > 0)
            {
                command.Parameters.AddWithValue("@CRNo", this._crNo);
            }
            //  if (this.AdminOrClaims == 1)
            //   {
            //       command.Parameters.AddWithValue("@ProcessId", 22);
            //   }
            //   else
            //   {
            //       command.Parameters.AddWithValue("@ProcessId", 24);
            //   }

            command.Parameters.AddWithValue("@CostCodeId", this.CostCodeId);
            command.Parameters.AddWithValue("@SubmitDate", DateTime.Now);
            command.Parameters.AddWithValue("@History", this.History);
            command.Parameters.AddWithValue("@IsVerified", this.IsVerified);
            command.Parameters.AddWithValue("@BatchId", this.BatchId);
            command.Parameters.AddWithValue("@Type", this.AdminOrClaims);
            command.Parameters.AddWithValue("@UserId", userId);

            SqlParameter invIdPara = command.Parameters.AddWithValue("@invoiceId", SqlDbType.Int);
            invIdPara.Direction = ParameterDirection.Output;
            try
            {
                command.ExecuteNonQuery();
                if (invIdPara.Value != DBNull.Value)
                {
                    this._invoiceId = (int)invIdPara.Value;
                    message = string.Empty;
                    this._hasData = true;
                    return this._invoiceId;
                }
                else
                {
                    message = "Adding invoice fails.";
                    return 0;
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

        // AddWithNewProcessId
        public int AddNewWithProcessId(ref string message, SqlCommand command, int userId)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "InvoiceAddNewApInvoiceWithProcessId";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ClaimId", this._claimId);
            command.Parameters.AddWithValue("@InvoiceDate", this._invoiceDate);
            command.Parameters.AddWithValue("@InvoiceNo", this._invocieNo);
            command.Parameters.AddWithValue("@VendorId", this._vendorId);
            command.Parameters.AddWithValue("@VendorName", this._vendorName);
            command.Parameters.AddWithValue("@Amount", this._amount);
            command.Parameters.AddWithValue("@GSTAmount", this._gstAmount);
            command.Parameters.AddWithValue("@PSTAmount", this._pstAmount);
            command.Parameters.AddWithValue("@QSTAmount", this._qstAmount);
            command.Parameters.AddWithValue("@TotalAmount", this._totalAmount);
            command.Parameters.AddWithValue("@ProcessId", this._processId);

            if (this.FileId > 0)
            {
                command.Parameters.AddWithValue("@FileId", this.FileId);
            }

            command.Parameters.AddWithValue("@SubProcess", this._subProcess);

            if (this.PONo > 0)
            {
                command.Parameters.AddWithValue("@PONo", this._poNo);
            }
            if (this.CRNo > 0)
            {
                command.Parameters.AddWithValue("@CRNo", this._crNo);
            }

            command.Parameters.AddWithValue("@CostCodeId", this.CostCodeId);
            command.Parameters.AddWithValue("@SubmitDate", DateTime.Now);
            command.Parameters.AddWithValue("@History", this.History);
            command.Parameters.AddWithValue("@BatchId", this.BatchId);

            if ((this._subProcess == 1) || (this._subProcess == 2))
                command.Parameters.AddWithValue("@type", 1); //this.AdminOrClaims);
            else
                command.Parameters.AddWithValue("@type", 2);
            
            command.Parameters.AddWithValue("@UserId", userId);

            SqlParameter invIdPara = command.Parameters.AddWithValue("@invoiceId", SqlDbType.Int);
            invIdPara.Direction = ParameterDirection.Output;

            try
            {
                command.ExecuteNonQuery();
                if (invIdPara.Value != DBNull.Value)
                {
                    this._invoiceId = (int)invIdPara.Value;
                    message = string.Empty;
                    this._hasData = true;
                    return this._invoiceId;
                }
                else
                {
                    message = "Adding invoice fails.";
                    return 0;
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


        // AddWithNewProcessIdCompleted
        public int AddNewWithProcessIdCompleted(ref string message, SqlCommand command, int userId)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "InvoiceAddNewApInvoiceWithProcessIdCompleted";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ClaimId", this._claimId);
            command.Parameters.AddWithValue("@InvoiceDate", this._invoiceDate);
            command.Parameters.AddWithValue("@InvoiceNo", this._invocieNo);
            command.Parameters.AddWithValue("@VendorId", this._vendorId);
            command.Parameters.AddWithValue("@VendorName", this._vendorName);
            command.Parameters.AddWithValue("@Amount", this._amount);
            command.Parameters.AddWithValue("@GSTAmount", this._gstAmount);
            command.Parameters.AddWithValue("@PSTAmount", this._pstAmount);
            command.Parameters.AddWithValue("@QSTAmount", this._qstAmount);
            command.Parameters.AddWithValue("@TotalAmount", this._totalAmount);
            command.Parameters.AddWithValue("@ProcessId", this._processId);

            if (this.FileId > 0)
            {
                command.Parameters.AddWithValue("@FileId", this.FileId);
            }

            command.Parameters.AddWithValue("@SubProcess", this._subProcess);

            if (this.PONo > 0)
            {
                command.Parameters.AddWithValue("@PONo", this._poNo);
            }
            if (this.CRNo > 0)
            {
                command.Parameters.AddWithValue("@CRNo", this._crNo);
            }

            command.Parameters.AddWithValue("@CostCodeId", this.CostCodeId);
            command.Parameters.AddWithValue("@SubmitDate", DateTime.Now);
            command.Parameters.AddWithValue("@History", this.History);
            command.Parameters.AddWithValue("@BatchId", this.BatchId);

            if ((this._subProcess == 1) || (this._subProcess == 2))
                command.Parameters.AddWithValue("@type", 1); //this.AdminOrClaims);
            else
                command.Parameters.AddWithValue("@type", 2);

            command.Parameters.AddWithValue("@UserId", userId);

            SqlParameter invIdPara = command.Parameters.AddWithValue("@invoiceId", SqlDbType.Int);
            invIdPara.Direction = ParameterDirection.Output;

            try
            {
                command.ExecuteNonQuery();
                if (invIdPara.Value != DBNull.Value)
                {
                    this._invoiceId = (int)invIdPara.Value;
                    message = string.Empty;
                    this._hasData = true;
                    return this._invoiceId;
                }
                else
                {
                    message = "Adding invoice fails.";
                    return 0;
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

		public int SetToCompleted(ref string message, SqlCommand command)
		{
			command.CommandType = CommandType.Text;
			command.CommandText = "UPDATE APInvoices SET InvoiceStatusId = 3 WHERE InvoiceId = @InvoiceId";

			command.Parameters.Clear();
			command.Parameters.Add("@InvoiceId", this.InvoiceId);

			try
			{
				command.ExecuteNonQuery();
				return 0;
			}
			catch(SqlException ex)
			{
				message = ex.Message;
				return -1;
			}
			catch(Exception ex)
			{
				message = ex.Message;
				return -1;
			}
		}

		public int SetToCompleted(ref string message)
		{
			DataAccess dba = new DataAccess();
			dba.dxClearParameters();
			dba.dxAddParameter("@InvoiceId", this.InvoiceId);

			try
			{
				dba.dxExecuteNonQuery("UPDATE APInvoices SET InvoiceStatusId = 3 WHERE InvoiceId = @InvoiceId");
				return 0;
			}
			catch(SqlException ex)
			{
				message = ex.Message;
				return -1;

			}
			catch(Exception ex)
			{
				message = ex.Message;
				return -1;
			}
		}

        public void setProvincialTaxField(ref APInvoice apinvoice, int vendorid, int branchid, decimal amount)
        {
            // Sets the PST or QST member of the APInvoice according to the following conditions
            //
            // If branch and vendor are both Quebec, provincial tax is stored as QST and not considered
            // to be part of the job cost
            //
            // If the branch is Quebec and the vendor is not Quebec, provincial tax is stored
            // as PST and therefore added to the job cost
            //
            // If the branch is not Quebec and the vendor is Quebec, provincial tax is stored as
            // PST and there fore added to the job cost
            //
            // If the branch is Ontario (not Quebec) and the vendor is Ontario (not Quebec), provincial
            // tax is stored as PST and added to the job cost.

            // Branch is Quebec
            if (WorkFlow.Objects.BLL.Branches.BranchProvinceId(branchid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
            {
                if (WorkFlow.Objects.BLL.Vendors.VendorProvinceId(vendorid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
                {
                    apinvoice.QSTAmount = amount;
                    apinvoice.PSTAmount = 0.0m;
                }
                else
                {
                    apinvoice.PSTAmount = amount;
                    apinvoice.QSTAmount = 0.0m;
                }

                return;
            }

            // Branch is not Quebec
            if (WorkFlow.Objects.BLL.Branches.BranchProvinceId(branchid) != WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
            {
                apinvoice.PSTAmount = amount;
                apinvoice.QSTAmount = 0.0m;
                return;
            }

            apinvoice.PSTAmount = amount;
            apinvoice.QSTAmount = 0.0m;
        }
	
		//Added by jwalin on 28-Sept-2006

		public static int DeleteIncomingInvoice(SqlCommand command, long InvoiceId, int WfId, int SubProcessId)
		{
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "Invoice_Delete";
			command.Parameters.Clear();
			command.Parameters.Add("@InvoiceID", InvoiceId);
			command.Parameters.Add("@WfId", WfId );
			command.Parameters.Add("@SubProcessId", SubProcessId );
			SqlParameter retPara = command.Parameters.Add("@RETURN_VALUE",SqlDbType.Int);
			retPara.Direction = ParameterDirection.ReturnValue;

			try
			{
				command.ExecuteNonQuery();
				return (int)retPara.Value;
			}
			catch(Exception ex)
			{
				return -7;				
			}

		}
		//End of adding

        public static void AddExtendedInvoiceInfo(int InvoiceId, string Abbr, string Desc, int EmpUserId, int VendorId, string FileNumberProvidedCorrect)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@InvoiceId",InvoiceId);
            dba.dxAddParameter("@VendorId", VendorId);
            dba.dxAddParameter("@UserId", EmpUserId);
            dba.dxAddParameter("@AbbrProvided",Abbr);
            dba.dxAddParameter("@Desc", Desc);
            dba.dxAddParameter("@FileNumberProvidedCorrect", FileNumberProvidedCorrect);

            dba.dxExecuteSP("InsertInvoiceExtendedData");
        }

		
		public static DataTable GetIncomingListByWfId_ProcessId(int wfId, int ProcessId)
		{
			DataAccess dba = new DataAccess();
			dba.dxAddParameter("@wfId", wfId);
			dba.dxAddParameter("@ProcessId", ProcessId);
			return dba.dxGetSPData("InvoiceIncomingListByWfId_processId");
		}

        public static DataTable GetAdminAPInvoiceOutData(DateTime StartDate, int BrId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@StartDate", StartDate);
            dba.dxAddParameter("@BrId", BrId);
            return dba.dxGetSPData("GetAdminAPInvoiceOutStanding");
        }

        public static DataTable GetAdminAPInvoiceCurrData(DateTime StartDate, DateTime EndDate, int BrId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@StartDate", StartDate);
            dba.dxAddParameter("@EndDate", EndDate);
            dba.dxAddParameter("@BrId", BrId);
            return dba.dxGetSPData("GetAdminAPInvoiceCurrent");
        }

        public static string getProvincialTaxFieldDescription(int vendorid, int branchid)
        {
            // Sets the PST or QST member of the APInvoice according to the following conditions
            //
            // If branch and vendor are both Quebec, provincial tax is stored as QST
            //
            // If the branch is Quebec and the vendor is not Quebec, provincial tax is stored as PST
            //
            // If the branch is not Quebec and the vendor is Quebec, provincial tax is stored as PST
            //
            // If the branch is Ontario (not Quebec) and the vendor is Ontario (not Quebec), provincial
            // tax is stored as PST

            if ((vendorid == 0) || (branchid == 0))
            {
                return "PST/QST";
            }

            // Branch is Quebec
            if (WorkFlow.Objects.BLL.Branches.BranchProvinceId(branchid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
            {
                if (WorkFlow.Objects.BLL.Vendors.VendorProvinceId(vendorid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
                {
                    return "QST";
                }
                else
                {
                    return "PST";
                }
            }

            // Branch is not Quebec
            if (WorkFlow.Objects.BLL.Branches.BranchProvinceId(branchid) != WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
            {
                if (WorkFlow.Objects.BLL.Vendors.VendorProvinceId(vendorid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
                {
                    return "PST/QST";
                }
                else
                {
                    return "PST";
                }
            }

            return "PST";
        }


        public static string getProvincialTaxFieldDescription(int branchid)
        {
            // Sets the PST or QST member of the APInvoice according to the following conditions
            //
            // If the branch is Quebec, provincial tax is stored as QST
            //
            // If the branch is Ontario, provincial tax is stored as PST

            if (branchid == 0)
            {
                return "PST/QST";
            }

            // Branch is Quebec
            if (WorkFlow.Objects.BLL.Branches.BranchProvinceId(branchid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
            {
                return "QST";
            }
            else
            {
                return "PST";
            }
        }

        public static bool isQSTJobCost(int vendorid, int branchid)
        {
            if ((vendorid == 0) || (branchid == 0))
            {
                return false;
            }

            // Branch is Quebec
            if (WorkFlow.Objects.BLL.Branches.BranchProvinceId(branchid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
            {
                if (WorkFlow.Objects.BLL.Vendors.VendorProvinceId(vendorid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
                {
                    return false;
                }
                else
                {
                    if (vendorid == 39)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            // Branch is not Quebec
            if (WorkFlow.Objects.BLL.Branches.BranchProvinceId(branchid) != WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
            {
                if (WorkFlow.Objects.BLL.Vendors.VendorProvinceId(vendorid) == WorkFlow.Objects.BLL.ProvinceData.getProvinceIdByProvinceName("Quebec"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        // GST/HST

        public static string getFederalHarmonizedTaxFieldDescription(int branchid)
        {
            if ((branchid == 0))
            {
                return "GST/HST";
            }

            // Branch has HST
            if (WorkFlow.Objects.BLL.Branches.BranchHasHST(branchid))
            {
                return "HST";
            }

            return "GST";
        }

        public static bool isBeforeFirstThursdayOfMonth()
        {
            if (DateTime.Now < WorkFlow.BLL.Invoice.APInvoice.getFirstThursdayOfCurrentMonth())
            {
                return true;
            }

            return false;
        }
        public static bool isBeforeFirstThursdayOfMonthWIP()
        {
            DateTime FirstThursday = WorkFlow.BLL.Invoice.APInvoice.getFirstThursdayOfCurrentMonth();
            if (DateTime.Now.Date <= FirstThursday)
            {
                return true;
            }

            return false;
        }

        public static bool isAfterFirstThursdayOfMonth()
        {
            if (DateTime.Now > WorkFlow.BLL.Invoice.APInvoice.getFirstThursdayOfCurrentMonth())
            {
                return true;
            }

            return false;
        }

        public static DateTime getFirstThursdayOfCurrentMonth()
        {
            DateTime dtFirstOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            for (int counter = 0; counter < 7; counter++)
            {
                //if (dtFirstOfMonth.AddDays(counter).DayOfWeek > DayOfWeek.Thursday
                if (dtFirstOfMonth.AddDays(counter).DayOfWeek == DayOfWeek.Thursday)
                {
                    return dtFirstOfMonth.AddDays(counter);
                }
            }

            return dtFirstOfMonth;
        }

        public static DateTime getInvoiceProcessingDate(DateTime invoicedate)
        {
            DateTime dtFirstOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            if (isBeforeFirstThursdayOfMonth() && (invoicedate < WorkFlow.BLL.Invoice.APInvoice.getFirstThursdayOfCurrentMonth()))
            {
                return invoicedate;
            }
            else if (isAfterFirstThursdayOfMonth() && (invoicedate < WorkFlow.BLL.Invoice.APInvoice.getFirstThursdayOfCurrentMonth()))
            {
                return dtFirstOfMonth;
            }
            else
            {
                return invoicedate;
            }
        }
        public static DateTime getFirstThursdayOfMonth()
        {
            DateTime dtFirstOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            for (int counter = 0; counter < 7; counter++)
            {
                if (dtFirstOfMonth.AddDays(counter).DayOfWeek == DayOfWeek.Thursday)
                {
                    return dtFirstOfMonth.AddDays(counter);
                }
            }

            return dtFirstOfMonth;
        }
        public static decimal GetAPInvoiceAmountSum(int WfId, int SubProcessId)
        {
            decimal str;
            DataAccess dba = new DataAccess();
            using (SqlConnection connection = dba.dxConnection)
            {
                SqlCommand command = connection.CreateCommand();
                try
                {
                    connection.Open();
                    command = connection.CreateCommand();
                    command.CommandText = "GetAPInvoiceAmountSum";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@wfId", WfId);
                    command.Parameters.AddWithValue("@SubProcessId", SubProcessId);
                    object obj2 = command.ExecuteScalar();
                    if (obj2 == null)
                    {
                        return 0;
                    }
                    str = Convert.ToDecimal(obj2);
                }
                catch (SqlException exception)
                {
                    throw exception;
                }
                catch (Exception exception2)
                {
                    throw exception2;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return str;
        }
            /*{
            DataAccess dba = new DataAccess();
            decimal RetValue = Decimal.Parse("0");
            using (SqlConnection conn = dba.dxConnection)
            {
                SqlCommand command = conn.CreateCommand();
                try
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetAPInvoiceAmountSum";

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@wfId", WfId);
                    command.Parameters.AddWithValue("@SubProcessId", SubProcessId);
                    SqlParameter param = command.Parameters.AddWithValue("@RetValue", SqlDbType.Decimal);
                    param.Precision = 18;
                    param.Scale = 2;
                    param.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();
                    if (param.Value != null && param.Value != DBNull.Value)
                    {
                        RetValue = (decimal)param.Value;
                    }
                }
                catch (Exception ex)
                {
                    RetValue = Decimal.Parse("0");
                }
            }
            return RetValue;
        }*/
        public static int GetCarrierDiscountInvoice(int WfId, int SubProcessId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@Wfid", WfId);
            dba.dxAddParameter("@SubProcessId", SubProcessId);
            return dba.dxGetSPInt("GetCarrierDiscountInvoice");
        }
        public static int CheckCarrierDiscount(int WfId, int SubProcessId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@Wfid", WfId);
            dba.dxAddParameter("@SubProcessId", SubProcessId);
            return dba.dxGetIntData("SELECT COUNT(*) FROM APINVOICES INNER JOIN ClaimValues ON APInvoices.ClaimId = ClaimValues.ClaimId AND APInvoices.SubProcessId = ClaimValues.SubProcessId WHERE (APInvoices.VendorName = 'Intact Vendor Discount') and claimvalues.wfid=@wfid and apinvoices.subprocessid=@SubProcessId");
        }
        public static DataTable GetProjectsForCarrierDiscount(int WfId, int SubProcessId)
        {

            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@Wfid", WfId);
            dba.dxAddParameter("@SubProcessId", SubProcessId);
            return dba.dxGetSPData("CarrierDiscountProjectList");
        }
        public static int CarrierDiscountInvoiceUpdateJobCosts(int InvoiceId, decimal Amount)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@InvoiceId", InvoiceId);
            dba.dxAddParameter("@Amount", Amount);
            return dba.dxGetSPInt("CarrierDiscountInvoiceUpdateJobCosts");
        }
        public static int GetPIndexIdbyDetId(int detID)
        {
            DataAccess dba = new DataAccess();
            string sql = "SELECT PIndexId FROM wfDet WHERE detid =" + detID;
            return dba.dxGetIntData(sql);
        }
        public static int GetSIndexIdbyDetId(int detID)
        {
            DataAccess dba = new DataAccess();
            string sql = "SELECT SIndexId FROM wfDet WHERE detid =" + detID;
            return dba.dxGetIntData(sql);
        }
        public static string GetBranchCodebyDetId(int detID)
        {
            DataAccess dba = new DataAccess();
            string sql = "SELECT BrCode FROM StrBranches INNER JOIN wfDet ON StrBranches.BrAccId = dbo.wfDet.PIndexId WHERE detid =" + detID;
            return dba.dxGetTextData(sql);
        }
        public static DataTable GetApInvoiceExportList(string brCode, int SindexId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@BranchCode", brCode);
            dba.dxAddParameter("@SindexId", SindexId);
            return dba.dxGetSPData("APInvoiceExportList");
        }
        public static DataTable RejectInvoice(int InvoiceId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@InvoiceId", InvoiceId);
            return dba.dxGetSPData("APRejectInvoice");
        }
        public static DataTable GetAdminApInvoiceExportList(string brCode, int SindexId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@BranchCode", brCode);
            dba.dxAddParameter("@SindexId", SindexId);
            return dba.dxGetSPData("AdminAPInvoiceExportList");
        }
        public static DataTable GetApInvoiceExport(string brCode, int SindexId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@BranchCode", brCode);
            dba.dxAddParameter("@SindexId", SindexId);
            return dba.dxGetSPData("APInvoiceExport");
        }

        public static DataTable GetApInvoiceExportCompleted(string brCode, int SindexId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@BranchCode", brCode);
            dba.dxAddParameter("@SindexId", SindexId);
            return dba.dxGetSPData("APInvoiceExportCompleted");
        }
        public static DataTable GetApAdminInvoiceExport(string brCode, int SindexId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@BranchCode", brCode);
            dba.dxAddParameter("@SindexId", SindexId);
            return dba.dxGetSPData("APAdminInvoiceExport");
        }
        public static DataTable GetApAdminInvoiceExportCompleted(string brCode, int SindexId)
        {
            DataAccess dba = new DataAccess();
            dba.dxClearParameters();
            dba.dxAddParameter("@BranchCode", brCode);
            dba.dxAddParameter("@SindexId", SindexId);
            return dba.dxGetSPData("APAdminInvoiceExportCompleted");
        }
        public static DataTable GetPayablesTasks()
        {
            DataAccess dba = new DataAccess();
            return dba.dxGetSPData("getPayablesTasks");
        }
 


        public static void CreateVendorExportTask(SqlCommand command, int detid, ref string message)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            command.CommandText = "wf_CreateTask26020";
            command.Parameters.AddWithValue("@detid", detid);
            try
            {
                command.ExecuteNonQuery();
                message = string.Empty;
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
    }
}
