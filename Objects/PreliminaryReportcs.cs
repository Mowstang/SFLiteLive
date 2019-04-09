using System;
using System.Data;
using System.Data.SqlClient;
using Doxess.Data;

namespace Smartflow.BLL.Claim
{
	/// <summary>
	/// Summary description for PreliminaryReportcs.
	/// </summary>
	public class PreliminaryReportsf 
	{
		#region Static method
		//		public static string GetReportPath
		//		{
		//			get
		//			{
		//				DataAccess dba = new DataAccess();
		//				//return dba.dxGetTextData("Select TemplatePath From wfEmail Where EmailId =5");
		//				return dba.dxGetTextData("Select TemplatePath From wfEmail Where EmailId =4");
		//			}
		//		}

		public static DataTable GetEmailData
		{
			get
			{
				DataAccess dba = new DataAccess();
				return dba.dxGetTable("Select TemplatePath, Subject, LogEntry, EmailName From wfEmail Where EmailId = 4");
			}
		}

		#endregion

		#region Private Variable
			
		private int _claimId;
		private int _detId;
		//private bool _hasData;

		private string _message = "";
		private int _ClaimPrelimReportID;

		public int ClaimPrelimReportID
		{
			get
			{
				return _ClaimPrelimReportID;
			}
			set
			{
				_ClaimPrelimReportID = value;
			}
		}

		private string _DateSent;

		public string DateSent
		{
			get
			{
				return _DateSent;
			}set
			 {
				 _DateSent = value;
			 }
		}
		private string _To;
		public string To
		{
			get
			{
				return _To;
			}set
			 {
				 _To = value;
			 }
		}
		private string _Attention;
		public string Attention
		{
			get
			{
				return _Attention ;
			}
			set
			{
				_Attention = value;
			}
		}

		private string _StroneFile;
		public string StroneFile
		{
			get
			{
				return _StroneFile;
			}
			set
			{
				_StroneFile = value;
			}
		}
		private string _InsuraceClaim;
		public string InsuraceClaim
		{
			get
			{
				return _InsuraceClaim;
			}
			set
			{
				_InsuraceClaim = value;
			}
		}
		private string _Insured ;
		public string Insured
		{
			get
			{
				return _Insured;
			}
			set
			{
				_Insured = value;
			}
		}
		private string _LocationLoss ;
		public string LocationLoss 
		{
			get
			{
				return _LocationLoss ;
			}
			set
			{
				_LocationLoss = value;
			}
		}

		private string _ReceivedDate ;
		public string ReceivedDate 
		{
			get
			{
				return _ReceivedDate ;
			}
			set
			{
				_ReceivedDate = value;
			}
		}
		private string _Appointment ;
		public string Appointment 
		{
			get
			{
				return _Appointment ;
			}
			set
			{
				_Appointment = value;
			}
		}

		private string _Property ;
		public string Property 
		{
			get
			{
				return _Property ;
			}
			set
			{
				_Property = value;
			}
		}
		private string _Damage ;
		public string Damage 
		{
			get
			{
				return _Damage ;
			}
			set
			{
				_Damage = value;
			}
		}

		private string _Restoration ;
		public string Restoration 
		{
			get
			{
				return _Restoration ;
			}
			set
			{
				_Restoration = value;
			}
		}
		
		private bool _AdjusterNeeded ;
		public bool AdjusterNeeded 
		{
			get
			{
				return _AdjusterNeeded ;
			}
			set
			{
				_AdjusterNeeded = value;
			}
		}
		private bool _BuildingDamage ;
		public bool BuildingDamage 
		{
			get
			{
				return _BuildingDamage ;
			}
			set
			{
				_BuildingDamage = value;
			}
		}
		private bool _PhotosTaken ;
		public bool PhotosTaken 
		{
			get
			{
				return _PhotosTaken ;
			}
			set
			{
				_PhotosTaken = value;
			}
		}
		private bool _EmergencyService ;
		public bool EmergencyService 
		{
			get
			{
				return _EmergencyService ;
			}
			set
			{
				_EmergencyService = value;
			}
		}
		private bool _ContentDamage ;
		public bool ContentDamage 
		{
			get
			{
				return _ContentDamage ;
			}
			set
			{
				_ContentDamage = value;
			}
		}
		private string _Responded ;
		public string Responded 
		{
			get
			{
				return _Responded ;
			}
			set{_Responded = value;}
		}

		private string  _InspectionBy ;
		public string InspectionBy 
		{
			get
			{
				return _InspectionBy ;
			}
			set{_InspectionBy = value;}
		}
		private int _SendPR;
		public int SendPR
		{
			get{return _SendPR;}
			set{_SendPR = value;}
		}
		//Preliminary Reports
		
		//WorkFlow.BLL.Claim.Claim.wfDocuments
		private int _wfId;
		public int WfID
		{
			get{return _wfId;}
			set{_wfId = value;}
		}

		private int _subProcessId;
		public int subProcessId
		{
			get{return _subProcessId;}
			set{_subProcessId = value;}
		}			

		private int _docId;
		public int DocId
		{
			get{return _docId;}
			set{_docId = value;}
		}
		
		private string _createdDate;
		public string CreatedDate
		{
			get{return _createdDate;}
			set{_createdDate = value;}
		}
        private string _EmergCompDate;
        public string EmergCompDate
        {
            get { return _EmergCompDate; }
            set { _EmergCompDate = value; }
        }

		private int _userId;
		public int UserId
		{
			get{return _userId;}
			set{_userId = value;}
		}

		private string _PreliminaryReportUrl;
		public string PreliminaryReportUrl
		{
			get{return this._PreliminaryReportUrl;}
		}
		
		private int _AdministratorId;
		public int AdministratorId
		{
			get{return this._AdministratorId;}
		}

        private decimal _EmergReserve;
        public decimal EmergReserve
        {
            get { return _EmergReserve; }
            set { _EmergReserve = value; }
        }

        private decimal _RestReserve;
        public decimal RestReserve
        {
            get { return _RestReserve; }
            set { _RestReserve = value; }
        }

        //private decimal _rbbudget;
        //public decimal rbbudget
        //{
        //    get { return _rbbudget; }
        //    set { _rbbudget = value; }
        //}

        private decimal _conbudget;
        public decimal conbudget
        {
            get { return _conbudget; }
            set { _conbudget = value; }
        }

        /*private string _VenNames;
        public string VenNames{
            get { return _VenNames; }
            set { _VenNames = value; }
        }*/

        private string _PMComment;
        public string PMComment
        {
            get { return _PMComment; }
            set { _PMComment = value; }
        }

        private string _DPCTB;
        public string DPCTB
        {
            get { return _DPCTB; }
            set { _DPCTB = value; }
        }

        private string _DRR;
        public string DRR
        {
            get { return _DRR; }
            set { _DRR = value; }
        }

        private string _dryst;
        public string dryst
        {
            get { return _dryst; }
            set { _dryst = value; }
        }

        private bool _vacateprem;
        public bool vacateprem
        {
            get { return _vacateprem; }
            set { _vacateprem = value; }
        }

        private string _toreq;
        public string toreq
        {
            get { return _toreq; }
            set { _toreq = value; }
        }



        private bool _amhazm;
        public bool amhazm
        {
            get { return _amhazm; }
            set { _amhazm = value; }
        }



        private string _AgeOfProperty;
        public string AgeOfProperty
        {
            get { return _AgeOfProperty; }
            set { _AgeOfProperty = value; }
        }

        private string _SFOB;
        public string SFOB
        {
            get { return _SFOB; }
            set { _SFOB = value; }
        }
        private string _COBuild;
        public string COBuild
        {
            get { return _COBuild; }
            set { _COBuild = value; }
        }

        private string _qualoffinish;
        public string qualoffinish
        {
            get { return _qualoffinish; }
            set { _qualoffinish = value; }
        }
        private string _ddlTOBuild;
        public string ddlTOBuild
        {
            get
            {
                return _ddlTOBuild;
            }
            set { _ddlTOBuild = value; }
        }
        private string _studmg;
        public string studmg
        {
            get
            {
                return _studmg;
            }
            set { _studmg = value; }
        }
        private string _SFAA;
        public string SFAA
        {
            get
            {
                return _SFAA;
            }
            set { _SFAA = value; }
        }
        private string _COLoss;
        public string COLoss
        {
            get
            {
                return _COLoss;
            }
            set { _COLoss = value; }
        }
        private string _RoomAffected;
        public string RoomAffected
        {
            get
            {
                return _RoomAffected;
            }
            set { _RoomAffected = value; }
        }
        private string _wattype;
        public string wattype
        {
            get
            {
                return _wattype;
            }
            set { _wattype = value; }
        }
        private bool _kitaffect;
        public bool kitaffect
        {
            get
            {
                return _kitaffect;
            }
            set { _kitaffect = value; }
        }
        private string _firetype;
        public string firetype
        {
            get
            {
                return _firetype;
            }
            set { _firetype = value; }
        }
        private bool _bathaffect;
        public bool bathaffect
        {
            get
            {
                return _bathaffect;
            }
            set { _bathaffect = value; }
        }
        private string _dmLoc;
        public string dmLoc
        {
            get
            {
                return _dmLoc;
            }
            set { _dmLoc = value; }
        }
        private string _IHV;
        public string IHV
        {
            get
            {
                return _IHV;
            }
            set { _IHV = value; }
        }
        

		#endregion

		#region Property
			
		
		public int ClaimID 
		{
			get
			{
				return this._claimId;
			}
			set
			{
				this._claimId = value;
			}
		}

		public int DetId
		{
			get
			{
				return this._detId;
			}
			set
			{
				this._detId = value;
			}
		}

		private int _branchId = -1;
		
		public int BranchID
		{
			get{return this._branchId;}

		}

		//		public bool HasData
		//		{		
		//			get{return this._hasData ;}
		//		}

		public string Message
		{
			get{return this._message;}
		}


		#endregion

		
		public PreliminaryReportsf(int detId) : base() 
		{
			this._detId = detId;			
			//this.Init();
            //this.SetClaimDetails();
            //this.SetPreliminaryDetails();
		}

		

		//		
		//		private void Init()
		//		{
		//			DataAccess dba = new DataAccess();
		//			dba.dxAddParameter("@ClaimId",this._detId);
		//			DataTable tbl = dba.dxGetSPData("wfGetClaimEmergencyAuthorizationData");
		//			if (tbl.Rows.Count ==1)
		//			{
		//				this._hasData = true;
		//				DataRow row = tbl.Rows[0];
		//
		//				this._claimId = (int)row["ClaimId"];
		//				
		//			}			
		//		}
		#region Generate Preliminary Report
		private void SetReportTemplatePath()
		{
			DataAccess dba = new DataAccess();
			string sql = "Select TemplatePath From wfEmail Where EmailId =4";
			this._PreliminaryReportUrl = dba.dxGetTextData(sql);
		}


		#endregion

		#region Get Claim Details


		#endregion

		#region Priliminary Report Data
		
		/*Steps 
		 * [1]. Get the data from ClaimPrelimReport
		 * [2]. Set the data for Component
		 */

        //private void SetPreliminaryDetails()
        //{
			
        //    //Get the Pliliminaty Report
        //    DataTable priliminaryInfo = Claim.GetPriliminaryReport(this.ClaimID);

        //    if (priliminaryInfo.Rows.Count > 0)
        //    {
        //        DataRow row = priliminaryInfo.Rows[0];
        //        int intPriliminaryId = 0;
        //        if( row["ClaimPrelimReportID"] != DBNull.Value )
        //        {
        //            intPriliminaryId = (int)row["ClaimPrelimReportID"];
        //            this.ClaimPrelimReportID = intPriliminaryId;
        //        }
				
        //        if( row["DateSent"] != DBNull.Value )
        //            this.DateSent = (row["DateSent"].ToString());
        //        else
        //        {
        //            DateTime aDate = DateTime.Today;
        //            this.DateSent = aDate.ToString();
        //        }

        //        if (row["EmergCompDate"] != DBNull.Value)
        //            this.EmergCompDate = (row["EmergCompDate"].ToString());
        //        else
        //        {
        //            this.EmergCompDate = "";                    
        //            //DateTime eDate = DateTime.Today;
        //            //this.EmergCompDate = eDate.ToString();
        //        }


        //        if( row["PropertyStructer"] != DBNull.Value )
        //            this.Property = (row["PropertyStructer"].ToString());
        //        else
        //            this.Property = string.Empty;

        //        //if( row["DamageCause"] != DBNull.Value )
        //        //    this.Damage = (row["DamageCause"].ToString());
        //        //else
        //        //    this.Damage = string.Empty;

        //        if( row["RestorationComments"] != DBNull.Value )
        //            this.Restoration = (row["RestorationComments"].ToString());
        //        else
        //            this.Restoration = string.Empty;
				
        //        //ClaimPrelimReportID, ClaimID, DateSent, PropertyStructer, DamageCause, 
        //        //RestorationComments, IsAdjusterNeededOnSite, IsBuildingDamage, 


        //        if( row["IsBuildingDamage"] != DBNull.Value )
        //        {
        //            this.BuildingDamage = (bool)row["IsBuildingDamage"] ;
        //        }
        //        else
        //        {
        //            this.BuildingDamage = true;
        //        }
				
        //        //IsPhotosTaken, IsEmergencyService, IsContentDamage, InspCompletedBy, MethodSend	
        //        if( row["IsPhotosTaken"] != DBNull.Value )
        //        {
        //            this.PhotosTaken = (bool)row["IsPhotosTaken"];
        //        }
        //        else
        //        {
        //            this.PhotosTaken = true;
        //        }

        //        if( row["IsEmergencyService"] != DBNull.Value )
        //        {
        //            this.EmergencyService = (bool)row["IsEmergencyService"];
        //        }
        //        else
        //        {
        //            this.EmergencyService = true;
        //        }

				
        //        if( row["IsContentDamage"] != DBNull.Value )
        //        {
        //            this.ContentDamage = (bool)row["IsContentDamage"];
        //        }
        //        else
        //        {
        //            this.ContentDamage = true;
        //        }

        //        if( row["Responded"] != DBNull.Value )
        //        {
        //            this.Responded = (row["Responded"].ToString());
        //        }
        //        else
        //        {
        //            this.Responded = string.Empty;
        //        }

        //        if( row["InspCompletedBy"] != DBNull.Value )
        //        {
        //            this.InspectionBy = (row["InspCompletedBy"].ToString());
        //        }
        //        else
        //        {
        //            this.InspectionBy = string.Empty;
        //        }

        //        if( row["MethodSend"] != DBNull.Value )
        //        {
        //            int intMethodSend = (int)row["MethodSend"];
        //            this.SendPR = intMethodSend;
        //        }
        //        else
        //        {
        //            this.SendPR = 1;
        //        }

        //        if (row["AppReserve1"] != DBNull.Value)
        //        {
        //            this.EmergReserve = Convert.ToDecimal(row["AppReserve1"]);
        //        }
        //        else
        //            this.EmergReserve = -99.00m;


        //        if (row["AppReserve2"] != DBNull.Value)
        //        {
        //            this.RestReserve = Convert.ToDecimal(row["AppReserve2"]);
        //        }
        //        else
        //            this.RestReserve = -99.00m;
        //        if (row["conbudget"] != DBNull.Value)
        //        {
        //            this.conbudget = Convert.ToDecimal(row["conbudget"]);
        //        }
        //        else
        //            this.conbudget = -99.00m;

        //        if (row["PMComments"] != DBNull.Value)
        //        {
        //            this.PMComment = (row["PMComments"].ToString());
        //        }
        //        else
        //        {
        //            this.PMComment = string.Empty;
        //        }


        //        if (row["DRR"] != DBNull.Value)
        //        {
        //            this.DRR = (row["DRR"].ToString());
        //        }
        //        else
        //        {
        //            this.DRR = string.Empty;
        //        }

        //        if (row["DPCTB"] != DBNull.Value)
        //        {
        //            this.DPCTB = (row["DPCTB"].ToString());
        //        }
        //        else
        //        {
        //            this.DPCTB = string.Empty;
        //        }

        //        if (row["DryStrat"] != DBNull.Value)
        //        {
        //            this.dryst = (row["DryStrat"].ToString());
        //        }
        //        else
        //        {
        //            this.dryst = string.Empty;
        //        }

        //        if (row["TearOutReq"] != DBNull.Value)
        //        {
        //            this.toreq = (row["TearOutReq"].ToString());
        //        }
        //        else
        //        {
        //            this.toreq = string.Empty;
        //        }

        //        //if (row["OthCom"] != DBNull.Value)
        //        //{
        //        //    this.OthCom = (row["OthCom"].ToString());
        //        //}
        //        //else
        //        //{
        //        //    this.OthCom = string.Empty;
        //        //}

        //        if (row["AgeOfProperty"] != DBNull.Value)
        //        {
        //            this.AgeOfProperty = (row["AgeOfProperty"].ToString());
        //        }
        //        else
        //        {
        //            this.AgeOfProperty = string.Empty;
        //        }

        //        if (row["SFOB"] != DBNull.Value)
        //        {
        //            this.SFOB = (row["SFOB"].ToString());
        //        }
        //        else
        //        {
        //            this.SFOB = string.Empty;
        //        }

        //        if (row["COBuild"] != DBNull.Value)
        //        {
        //            this.COBuild = (row["COBuild"].ToString());
        //        }
        //        else
        //        {
        //            this.COBuild = string.Empty;
        //        }

        //        if (row["QualofF"] != DBNull.Value)
        //        {
        //            this.qualoffinish = (row["QualofF"].ToString());
        //        }
        //        else
        //        {
        //            this.qualoffinish = string.Empty;
        //        }
        //        if (row["TOBuild"] != DBNull.Value)
        //        {
        //            this.ddlTOBuild = (row["TOBuild"].ToString());
        //        }
        //        else
        //        {
        //            this.ddlTOBuild = string.Empty;
        //        }
        //        //if (row["StrucDam"] != DBNull.Value)
        //        //{
        //        //    this.studmg = (row["StrucDam"].ToString());
        //        //}
        //        //else
        //        //{
        //        //    this.studmg = string.Empty;
        //        //}
        //        if (row["SFAA"] != DBNull.Value)
        //        {
        //            this.SFAA = (row["SFAA"].ToString());
        //        }
        //        else
        //        {
        //            this.SFAA = string.Empty;
        //        }
        //        //if (row["COLoss"] != DBNull.Value)
        //        //{
        //        //    this.COLoss = (row["COLoss"].ToString());
        //        //}
        //        //else
        //        //{
        //        //    this.COLoss = string.Empty;
        //        //}
        //        if (row["RoomAffected"] != DBNull.Value)
        //        {
        //            this.RoomAffected = (row["RoomAffected"].ToString());
        //        }
        //        else
        //        {
        //            this.RoomAffected = string.Empty;
        //        }
        //        if (row["wattype"] != DBNull.Value)
        //        {
        //            this.wattype = (row["wattype"].ToString());
        //        }
        //        else
        //        {
        //            this.wattype = string.Empty;
        //        }

        //        if (row["firetype"] != DBNull.Value)
        //        {
        //            this.firetype = (row["firetype"].ToString());
        //        }
        //        else
        //        {
        //            this.firetype = string.Empty;
        //        }

        //        this.bathaffect = Convert.ToBoolean(row["isBathroomAffected"].ToString());
        //        this.kitaffect = Convert.ToBoolean(row["isKitchenAffected"].ToString());
        //        this.amhazm = Convert.ToBoolean(row["isHazardousMaterials"].ToString());
        //        this.vacateprem = Convert.ToBoolean(row["VacatePremises"].ToString());
        //        this.AdjusterNeeded = Convert.ToBoolean(row["IsAdjusterNeededOnSite"].ToString());

        //        if (row["damloc"] != DBNull.Value)
        //        {
        //            this.dmLoc = (row["damloc"].ToString());
        //        }
        //        else
        //        {
        //            this.dmLoc = string.Empty;
        //        }
        //        if (row["IHV"] != DBNull.Value)
        //        {
        //            this.IHV = (row["IHV"].ToString());
        //        }
        //        else
        //        {
        //            this.IHV = string.Empty;
        //        }
        //    }
        //}


		#endregion

		#region Insert PreliminaryReports
		
		public int Update(ref string message, SqlCommand transCommand)
		{
			// the update operation should under a transaction
			//if (!this._hasData) return 0;
			transCommand.CommandText = "wfAddPreliminaryReport";
			transCommand.CommandType = CommandType.StoredProcedure;			
			transCommand.Parameters.Clear();

			transCommand.Parameters.Add("@claimId",this._claimId);
			transCommand.Parameters.Add("@DateSent",this._DateSent);
			transCommand.Parameters.Add("@PropertyStructer",this._Property);
			/*transCommand.Parameters.Add("@DamageCause",this._Damage);*/
			transCommand.Parameters.Add("@RestorationComments",this._Restoration);
			transCommand.Parameters.Add("@IsAdjusterNeededOnSite",this._AdjusterNeeded);
			transCommand.Parameters.Add("@IsBuildingDamage",this._BuildingDamage);
			transCommand.Parameters.Add("@IsPhotosTaken",this._PhotosTaken);
			transCommand.Parameters.Add("@IsEmergencyService",this._EmergencyService);
			transCommand.Parameters.Add("@IsContentDamage",this._ContentDamage);
			transCommand.Parameters.Add("@Responded",this._Responded);
			transCommand.Parameters.Add("@InspCompletedBy",this._InspectionBy);
			transCommand.Parameters.Add("@MethodSend",this._SendPR);
            transCommand.Parameters.Add("@EmergCompDate", this._EmergCompDate);
			transCommand.Parameters.Add("@wfId",this._wfId);
			transCommand.Parameters.Add("@DocId",this._docId);
			transCommand.Parameters.Add("@GeneratedDate",this._createdDate);
			transCommand.Parameters.Add("@CreatedById",this._userId);
			transCommand.Parameters.Add("@subProcessId", this._subProcessId);
            transCommand.Parameters.Add("@DryStrat", this._dryst);
            //transCommand.Parameters.Add("@VenName", this._VenNames);
            transCommand.Parameters.Add("@PMComments", this._PMComment);
            transCommand.Parameters.Add("@DRR", this._DRR);
            transCommand.Parameters.Add("@DPCTB", this._DPCTB);
            transCommand.Parameters.Add("@VacPrem", this._vacateprem);
            transCommand.Parameters.Add("@TearOutReq", this._toreq);
            
            transCommand.Parameters.Add("@AMHM", this._amhazm);
            //transCommand.Parameters.Add("@OthCom", this._OthCom);
            transCommand.Parameters.Add("@AgeOfProperty", this._AgeOfProperty);
            transCommand.Parameters.Add("@SFOB", this._SFOB);
            transCommand.Parameters.Add("@COBuild", this._COBuild);
            transCommand.Parameters.Add("@QualofF", this._qualoffinish);
            transCommand.Parameters.Add("@TOBuild", this._ddlTOBuild);
            //transCommand.Parameters.Add("@StrucDam", this._studmg);
            transCommand.Parameters.Add("@SFAA", this._SFAA);
            
            transCommand.Parameters.Add("@RoomAffected", this._RoomAffected);
            transCommand.Parameters.Add("@wattype", this._wattype);
            transCommand.Parameters.Add("@kitaffect", this._kitaffect);
            transCommand.Parameters.Add("@firetype", this._firetype);
            transCommand.Parameters.Add("@bathaffect", this._bathaffect);
            transCommand.Parameters.Add("@damloc", this._dmLoc);
            transCommand.Parameters.Add("@IHV", this._IHV);
            //transCommand.Parameters.Add("@rbbudget", this._rbbudget);
            transCommand.Parameters.Add("@conbudget", this._conbudget);
            transCommand.Parameters.Add("@reserveAmt", this._EmergReserve);
            transCommand.Parameters.Add("@reserveAmt2", this._RestReserve);
			try
			{
				return transCommand.ExecuteNonQuery();
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

			return 0;
		}

        public static void updateReserveAmt(ref string message, SqlCommand transCommand, int clwfId, int IsEmergency, decimal ReserveAmt)
        {
            try
            {
                transCommand.Parameters.Add("@wfId", clwfId);
                transCommand.Parameters.Add("@reserveAmt", ReserveAmt);
                transCommand.CommandType = CommandType.Text;
                if (IsEmergency == 1)
                    transCommand.CommandText = "update ClaimPrelimReport set AppReserve1=@reserveAmt where ClaimId = (select claimId from claims where wfId = @wfId)";
                else
                    transCommand.CommandText = "update ClaimPrelimReport set AppReserve2=@reserveAmt2 where ClaimId = (select claimId from claims where wfId = @wfId)";
                transCommand.ExecuteNonQuery();
                message = string.Empty;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
        public static bool isNewPR(int wfid)
        {
            int bVal = 0;
            DataAccess dba = new DataAccess();

            dba.dxClearParameters();
            dba.dxAddParameter("@wfid", wfid);


            bVal = dba.dxGetIntData("SELECT count(*) FROM wfdet WHERE @wfid=wfid and CompletionDate>'07/15/2017'");

            if (bVal > 0)
            {
                return true;
            }

            return false;
        }
		public static decimal getReserveAmt(int clWfId, int isEmergency)
		{
			DataAccess dba = new DataAccess();
			dba.dxAddParameter("@WfId", clWfId);
			object RetValue;
			if (isEmergency == 1)
				RetValue = dba.dxGetData("select IsNull(AppReserve1, 0) as AppReserve1 from ClaimPrelimReport where ClaimId = (select claimId from claims where wfId = @WfId)");
			else
				RetValue = dba.dxGetData("select IsNull(AppReserve2, 0) as AppReserve2 from ClaimPrelimReport where ClaimId = (select claimId from claims where wfId = @WfId)");			
			if (RetValue != null)
				return Decimal.Parse(RetValue.ToString());
			else
				return Decimal.Parse("0");
		}
        public string CreateTasks(int wfid, int subproc, int action, int User)
        /* 
         * action = 22120 for uploaded preliminary report (3)
         *          22395 for uploaded Scope Documents (26) or Tick Sheets
         *          22052 for uploaded pictures
         *          22051 for uploaded emergency authorization (2 or 24)         */
        {
            try
            {
                string CONNECT_STRING = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
                using (SqlConnection conn = new SqlConnection(CONNECT_STRING))
                {
                    conn.Open();
                    SqlCommand command = conn.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "CreateTasksTablet";
                    command.Parameters.Clear();
                    command.Parameters.Add("@wfId", wfid);
                    command.Parameters.Add("@SubProcessId", subproc);
                    command.Parameters.Add("@ActionId", action);
                    command.Parameters.Add("@UserId", Convert.ToInt32(User));
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return "triggered tasks failed -" + ex.Message;
            }
            return "OK";
        }

        public static void ReAssignToRole(int DetId, int AssignedtoUserId, int RoleId, ref string message)
        {
            // Re-assign a task based on the Detail ID.

            // WfDet
            DataAccess dba = new DataAccess();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                message = string.Empty;
                SqlCommand command = conn.CreateCommand();
                try
                {
                    command.Parameters.AddWithValue("@DetId", DetId);
                    command.Parameters.AddWithValue("@RoleId", RoleId);
                    command.Parameters.AddWithValue("@AssignedtoUserId", AssignedtoUserId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"UPDATE WfDet Set AssignedtoRoleId=@RoleId, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
                    command.ExecuteNonQuery();
                    message = string.Empty;

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }//end of using (SqlConnection conn = dba.dxConnection)

            // WfDetR2G
            dba = new DataAccess();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                message = string.Empty;
                SqlCommand command = conn.CreateCommand();
                try
                {
                    command.Parameters.AddWithValue("@DetId", DetId);
                    command.Parameters.AddWithValue("@RoleId", RoleId);
                    command.Parameters.AddWithValue("@AssignedtoUserId", AssignedtoUserId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"UPDATE WfDetR2G Set AssignedtoRoleId=@RoleId, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
                    command.ExecuteNonQuery();
                    message = string.Empty;

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }//end of using (SqlConnection conn = dba.dxConnection)
        }
			

		public static void ClaimReAssign(int DetId, int AssignedtoUserId, ref string message)
		{
			// Re-assign a task based on the Detail ID.

			// WfDet
			DataAccess dba = new DataAccess();
			using (SqlConnection conn = dba.dxConnection)
			{				
				conn.Open();
				message = string.Empty;
				SqlCommand command = conn.CreateCommand();
				try
				{
					command.Parameters.Add("@DetId", DetId);
					command.Parameters.Add("@AssignedtoUserId", AssignedtoUserId);
					command.CommandType = CommandType.Text;
					command.CommandText = @"UPDATE WfDet Set AssignedtoRoleId=220, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
					command.ExecuteNonQuery();
					message = string.Empty;

				}
				catch(Exception ex)
				{
					message = ex.Message;
				}
			}//end of using (SqlConnection conn = dba.dxConnection)

			// WfDetR2G
			dba = new DataAccess();
			using (SqlConnection conn = dba.dxConnection)
			{				
				conn.Open();
				message = string.Empty;
				SqlCommand command = conn.CreateCommand();
				try
				{
					command.Parameters.Add("@DetId", DetId);
					command.Parameters.Add("@AssignedtoUserId", AssignedtoUserId);
					command.CommandType = CommandType.Text;
					command.CommandText = @"UPDATE WfDetR2G Set AssignedtoRoleId=220, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
					command.ExecuteNonQuery();
					message = string.Empty;

				}
				catch(Exception ex)
				{
					message = ex.Message;
				}
			}//end of using (SqlConnection conn = dba.dxConnection)
		}

        public static void ClaimReAssignToPM(int DetId, int AssignedtoUserId, ref string message)
        {
            DataAccess dba = new DataAccess();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                message = string.Empty;
                SqlCommand command = conn.CreateCommand();
                try
                {
                    command.Parameters.AddWithValue("@DetId", DetId);
                    command.Parameters.AddWithValue("@AssignedtoUserId", AssignedtoUserId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"UPDATE WfDet Set AssignedtoRoleId=211, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
                    command.ExecuteNonQuery();
                    message = string.Empty;

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }//end of using (SqlConnection conn = dba.dxConnection)

            // WfDetR2G
            dba = new DataAccess();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                message = string.Empty;
                SqlCommand command = conn.CreateCommand();
                try
                {
                    command.Parameters.AddWithValue("@DetId", DetId);
                    command.Parameters.AddWithValue("@AssignedtoUserId", AssignedtoUserId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"UPDATE WfDetR2G Set AssignedtoRoleId=211, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
                    command.ExecuteNonQuery();
                    message = string.Empty;

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }//end of using (SqlConnection conn = dba.dxConnection)
        }

        public static void ClaimReAssignToOA(int DetId, int AssignedtoUserId, ref string message)
        {
            DataAccess dba = new DataAccess();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                message = string.Empty;
                SqlCommand command = conn.CreateCommand();
                try
                {
                    command.Parameters.AddWithValue("@DetId", DetId);
                    command.Parameters.AddWithValue("@AssignedtoUserId", AssignedtoUserId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"UPDATE WfDet Set AssignedtoRoleId=220, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
                    command.ExecuteNonQuery();
                    message = string.Empty;

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }//end of using (SqlConnection conn = dba.dxConnection)

            // WfDetR2G
            dba = new DataAccess();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                message = string.Empty;
                SqlCommand command = conn.CreateCommand();
                try
                {
                    command.Parameters.AddWithValue("@DetId", DetId);
                    command.Parameters.AddWithValue("@AssignedtoUserId", AssignedtoUserId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"UPDATE WfDetR2G Set AssignedtoRoleId=220, 
							AssignedtoUserId=@AssignedtoUserId WHERE DetId=@DetId";
                    command.ExecuteNonQuery();
                    message = string.Empty;

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }//end of using (SqlConnection conn = dba.dxConnection)
        }
        public static int AddMoistureReadings(int WfId, int SubProcessId, decimal ATemp, decimal ARH, decimal AGPP, decimal NATemp, decimal NARH, decimal NAGPP, decimal ETemp, decimal ERH, decimal EGPP, decimal Fans, decimal Dehus)
        {
            int message;
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@WfId", WfId);
            dba.dxAddParameter("@SubProcessId", SubProcessId);
            dba.dxAddParameter("@AffectedTemp", ATemp);
            dba.dxAddParameter("@AffectedRH", ARH);
            dba.dxAddParameter("@AffectedGPP", AGPP);
            dba.dxAddParameter("@NonAffectedTemp", NATemp);
            dba.dxAddParameter("@NonAffectedRH", NARH);
            dba.dxAddParameter("@NonAffectedGPP", NAGPP);
            dba.dxAddParameter("@ExternalTemp", ETemp);
            dba.dxAddParameter("@ExternalRH", ERH);
            dba.dxAddParameter("@ExternalGPP", EGPP);
            dba.dxAddParameter("@Fans", Fans);
            dba.dxAddParameter("@Dehus", Dehus);
            return message=dba.dxExecuteSP("MoistureReadingsAdd");


        }
		#endregion

	}
}


