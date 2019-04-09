using System;
using System.Data;
using System.Data.SqlClient;
using Doxess.Data;
using WorkFlow.Objects.BLL;

namespace SFLite.EmergencyResponse
{
	/// <summary>
	/// Summary description for APInvoice.
	/// </summary>
	public class EmergencyResponse
	{


		#region Static Methods
		
		public static int GetBatchId()
		{
			DataAccess dba = new DataAccess();
			return dba.dxGetIntData("select Max(IsNull(BatchId, 0)) + 1 from APInvoices");
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

        private int _wfid = 0;
        public int WfId
        {
            get { return this._wfid; }
            set { this._wfid = value; }
        }

		private string _affectedArea="";
        public string AffectedArea
        {
            get { return this._affectedArea; }
            set { this._affectedArea = value; }
        }

        private string _servicesrendered = "";
        public string ServicesRendered
        {
            get { return this._servicesrendered; }
            set { this._servicesrendered = value; }
        }

        private string _airmovers = "";
        public string AirMovers
        {
            get { return this._airmovers; }
            set { this._airmovers = value; }
        }

        private string _dehumidifiers = "";
        public string Dehumidifiers
        {
            get { return this._dehumidifiers; }
            set { this._dehumidifiers = value; }
        }


		#endregion
		
		public EmergencyResponse()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public EmergencyResponse(int wfid)
		{
			this._wfid = wfid;
			//this.Init();		
		}

        public void AddNew(int wfid)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@wfid",wfid);
            dba.dxAddParameter("@AffectedArea",AffectedArea);
            dba.dxAddParameter("@ServicesRendered", ServicesRendered);
            dba.dxAddParameter("@AirMovers", AirMovers);


        }



        
    }
}
