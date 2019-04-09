namespace SFLite
{
    using Doxess.Data;
    using SFLite.DocType;
    using Doxess.Web.WorkFlow;
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;

    public class WfDocument
    {
        private int actionDocId;
        private int actionId;
        private int detId;
        private ArrayList docList;
        private int userId;
        private int wfId;

        public WfDocument(int actionDocId)
        {
            this.actionDocId = -1;
            this.docList = null;
            this.actionDocId = actionDocId;
        }

        public WfDocument(int actionId, int detId, int userId)
        {
            this.actionDocId = -1;
            this.docList = null;
            this.actionId = actionId;
            this.detId = detId;
            this.userId = userId;
            this.Init();
        }

        private string AddDocument(SqlCommand command, WorkFlowDocument.DocumentTypes type)
        {
            command.Parameters.Clear();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "wfCreateDocument";
            command.Parameters.Add("@wfId", this.wfId);
            command.Parameters.Add("@DocId", (int) type);
            command.Parameters.Add("@CreatedById", this.userId);
            command.Parameters.Add("@ActionDocId", SqlDbType.Int).Direction = ParameterDirection.Output;
            try
            {
                command.ExecuteNonQuery();
                return string.Empty;
            }
            catch (SqlException exception)
            {
                return exception.Message;
            }
            catch (Exception exception2)
            {
                return exception2.Message;
            }
        }

        public string AddDocuments(SqlCommand command)
        {
            this.docList.Sort();
            string str = string.Empty;
            foreach (object obj2 in this.docList)
            {
                try
                {
                    WorkFlowDocument.DocumentTypes type = (WorkFlowDocument.DocumentTypes) obj2;
                    str = this.AddDocument(command, type);
                    if (!str.Equals(string.Empty))
                    {
                        return str;
                    }
                    continue;
                }
                catch (Exception)
                {
                    throw new Exception("Wrong Doc ID.");
                }
            }
            return string.Empty;
        }

        public int AddNewItem(int ActionId, int DocId, ref string message)
        {
            DataAccess access = new DataAccess();
            using (SqlConnection connection = access.dxConnection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                return this.AddNewItem(ActionId, DocId, command, ref message);
            }
        }

        public int AddNewItem(int ActionId, int DocId, SqlCommand command, ref string message)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "wfActionAddNewDocument";
            command.Parameters.Clear();
            command.Parameters.Add("@ActionId", ActionId);
            command.Parameters.Add("@DocId", DocId);
            SqlParameter parameter = command.Parameters.Add("@actionDocId", SqlDbType.Int);
            parameter.Direction = ParameterDirection.Output;
            try
            {
                command.ExecuteNonQuery();
                this.actionDocId = (int) parameter.Value;
                message = string.Empty;
                return this.actionDocId;
            }
            catch (SqlException exception)
            {
                message = exception.Message;
                return -1;
            }
            catch (Exception exception2)
            {
                message = exception2.Message;
                return -1;
            }
        }

        public int AddNewItem(int ActionId, int DocId, string TableName, string ConditionField, string ConditionValue, ValueType valueType, string Operator, ref string message)
        {
            DataAccess access = new DataAccess();
            using (SqlConnection connection = access.dxConnection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                return this.AddNewItem(ActionId, DocId, TableName, ConditionField, ConditionValue, valueType, Operator, command, ref message);
            }
        }

        public int AddNewItem(int ActionId, int DocId, string TableName, string ConditionField, string ConditionValue, ValueType valueType, string Operator, SqlCommand command, ref string message)
        {
            if (!this.CheckConditionValue(ConditionValue, valueType, Operator, ref message))
            {
                return -1;
            }
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "wfActionAddNewDocument";
            command.Parameters.Clear();
            command.Parameters.Add("@ActionId", ActionId);
            command.Parameters.Add("@DocId", DocId);
            command.Parameters.Add("@Always", false);
            command.Parameters.Add("@TableName", TableName);
            command.Parameters.Add("@ConditionField", ConditionField);
            command.Parameters.Add("@ConditionValue", ConditionValue);
            command.Parameters.Add("@ValueType", valueType);
            command.Parameters.Add("@Operator", Operator);
            SqlParameter parameter = command.Parameters.Add("@actionDocId", SqlDbType.Int);
            parameter.Direction = ParameterDirection.Output;
            try
            {
                command.ExecuteNonQuery();
                this.actionDocId = (int) parameter.Value;
                message = string.Empty;
                return this.actionDocId;
            }
            catch (SqlException exception)
            {
                message = exception.Message;
                return -1;
            }
            catch (Exception exception2)
            {
                message = exception2.Message;
                return -1;
            }
        }

        private static bool ChechFieldDataType(string TableName, string FieldName, ValueType valueType, ref string message)
        {
            DataAccess access = new DataAccess();
            message = "";
            try
            {
                string sql = "Select Top 1 " + FieldName + " From " + TableName;
                DataTable table = access.dxGetTable(sql);
                DataColumn column = table.Columns[0];
                string name = table.Columns[0].DataType.Name;
                if (name.IndexOf("Int") >= 0)
                {
                    name = "Int";
                }
                if (name.Equals(valueType.ToString()))
                {
                    return true;
                }
                message = "Field, " + FieldName + " has data type: " + name + ". Please match the data type.";
                return false;
            }
            catch (SqlException exception)
            {
                message = exception.Message;
                return false;
            }
        }

        private static bool ChechFieldName(string TableName, string FieldName, ref string message)
        {
            DataAccess access = new DataAccess();
            message = "";
            try
            {
                string sql = "Select Top 1 " + FieldName + " From " + TableName;
                DataTable table = access.dxGetTable(sql);
                return true;
            }
            catch (SqlException)
            {
                message = "Table, " + TableName + " does not have " + FieldName + " in it.";
                return false;
            }
        }

        private static bool ChechTable(string TableName, ref string message)
        {
            DataAccess access = new DataAccess();
            message = "";
            try
            {
                string sql = "Select Top 1 * From " + TableName;
                DataTable table = access.dxGetTable(sql);
                return true;
            }
            catch (SqlException)
            {
                message = "Table " + TableName + " does not exist in database.";
                return false;
            }
        }

        private static bool ChechValueDataType(string strValue, ValueType valueType, ref string message)
        {
            message = "";
            switch (valueType)
            {
                case ValueType.Int:
                    if (!Util.IsNumeric(strValue) || (strValue.IndexOf(".") >= 0))
                    {
                        message = "Value should be integer.";
                        return false;
                    }
                    return true;

                case ValueType.Decimal:
                    if (!Util.IsNumeric(strValue))
                    {
                        message = "Value should be numeric.";
                        return false;
                    }
                    return true;

                case ValueType.Boolean:
                    strValue = strValue.ToLower();
                    if (!strValue.Equals("0") && !strValue.Equals("1"))
                    {
                        message = "Value should be boolean (1: true or 0: false)";
                        return false;
                    }
                    return true;

                case ValueType.DateTime:
                    if (!Util.IsDate(strValue))
                    {
                        message = "Value should be Date (dd/mm/yyyy)";
                        return false;
                    }
                    return true;
            }
            return true;
        }

        private static bool ChechWfId(string TableName, ref string message)
        {
            DataAccess access = new DataAccess();
            message = "";
            try
            {
                string sql = "Select Top 1 * From " + TableName + " Where wfId is not null";
                DataTable table = access.dxGetTable(sql);
                return true;
            }
            catch (SqlException)
            {
                message = "Table, " + TableName + " does not have wfId field in it. Please enter other table name.";
                return false;
            }
        }

        private bool CheckConditionValue(string ConditionValue, ValueType valueType, string Operator, ref string message)
        {
            message = string.Empty;
            switch (valueType)
            {
                case ValueType.String:
                    if (!Operator.Equals("="))
                    {
                        message = "For string value, only = can be used as operator.";
                        return false;
                    }
                    return true;

                case ValueType.Int:
                    if (!Util.IsNumeric(ConditionValue) || (ConditionValue.IndexOf(".") >= 0))
                    {
                        message = "Condition value should be integer.";
                        return false;
                    }
                    return true;

                case ValueType.Decimal:
                    if (!Util.IsNumeric(ConditionValue))
                    {
                        message = "Condition value should be numeric.";
                        return false;
                    }
                    return true;

                case ValueType.Boolean:
                    if (((ConditionValue.Equals("0") || ConditionValue.Equals("1")) || (ConditionValue.ToLower().Equals("true") || ConditionValue.ToLower().Equals("false"))) && Operator.Equals("="))
                    {
                        return true;
                    }
                    message = "Wrong value or operator for boolean input.";
                    return false;

                case ValueType.DateTime:
                    if (!Util.IsDate(ConditionValue))
                    {
                        message = "Condition value should be date format(dd/mm/yyyy).";
                        return false;
                    }
                    return true;
            }
            message = "Wrong value type";
            return false;
        }

        private static bool CheckOperator(string strOperator, ValueType valueType, ref string message)
        {
            if ((valueType == ValueType.String) || (valueType == ValueType.Boolean))
            {
                if (strOperator.Equals("=") || strOperator.Equals("!="))
                {
                    return true;
                }
                message = valueType.ToString() + " can only apply = or Not = operator.";
                return false;
            }
            return true;
        }

        public string DeleteItem()
        {
            DataAccess access = new DataAccess();
            using (SqlConnection connection = access.dxConnection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                return this.DeleteItem(command);
            }
        }

        public string DeleteItem(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "wfActionDeleteActionDocItem";
            command.Parameters.Clear();
            command.Parameters.Add("@ActionDocId", this.actionDocId);
            try
            {
                command.ExecuteNonQuery();
                return string.Empty;
            }
            catch (SqlException exception)
            {
                return exception.Message;
            }
            catch (Exception exception2)
            {
                return exception2.Message;
            }
        }

        public static DataTable GetActionDocumentList(int processId)
        {
            DataAccess access = new DataAccess();
            access.dxAddParameter("@processId", processId);
            return access.dxGetSPData("wfActionDocumentList");
        }

        private void GetAlwaysDocId()
        {
            DataAccess access = new DataAccess();
            access.dxAddParameter("@actionId", this.actionId);
            foreach (DataRow row in access.dxGetSPData("wfActionGetAlwaysDocId").Rows)
            {
                this.docList.Add((int) row["DocId"]);
            }
        }

        private void GetOtherDocList()
        {
            DataAccess access = new DataAccess();
            string sql = "Select DocId, TableName, ConditionField, ConditionValue, ValueType, Operator From wfActionDoc  Where Always = 0 And  ActionId =" + this.actionId;
            DataTable table = access.dxGetTable(sql);
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            foreach (DataRow row in table.Rows)
            {
                DateTime date;
                DateTime time2;
                string str7;
                ValueType type = (ValueType) row["ValueType"];
                str2 = row["TableName"].ToString().Trim();
                str3 = row["ConditionField"].ToString().Trim();
                str4 = row["ConditionValue"].ToString().Trim();
                str5 = row["Operator"].ToString().Trim().ToLower();
                sql = string.Concat(new object[] { "Select ", str3, " From ", str2, " Where WfId =", this.wfId });
                DataTable table2 = access.dxGetTable(sql);
                if (table2.Rows.Count > 0)
                {
                    switch (type)
                    {
                        case ValueType.String:
                        {
                            string str6 = table2.Rows[0][str3].ToString().Trim();
                            if (!str5.Equals("=") || !str6.Equals(str4))
                            {
                                goto Label_01BB;
                            }
                            this.docList.Add((int) row["DocId"]);
                            continue;
                        }
                        case ValueType.Int:
                            goto Label_0549;

                        case ValueType.Decimal:
                            goto Label_02D5;

                        case ValueType.Boolean:
                        {
                            if (!str5.Equals("=") || (((bool) table2.Rows[0][str3]) != Convert.ToBoolean(Convert.ToInt16(str4))))
                            {
                                goto Label_0257;
                            }
                            this.docList.Add((int) row["DocId"]);
                            continue;
                        }
                        case ValueType.DateTime:
                            date = (DateTime) table2.Rows[0][str3];
                            date = date.Date;
                            time2 = Convert.ToDateTime(str4).Date;
                            goto Label_07C2;
                    }
                }
                continue;
            Label_01BB:
                if (str5.Equals("!=") && !str4.Equals(str4))
                {
                    this.docList.Add((int) row["DocId"]);
                }
                continue;
            Label_0257:
                if (str5.Equals("!=") && (((bool) table2.Rows[0][str3]) != Convert.ToBoolean(Convert.ToInt16(str4))))
                {
                    this.docList.Add((int) row["DocId"]);
                }
                continue;
            Label_02D5:
                if ((str7 = str5) != null)
                {
                    str7 = string.IsInterned(str7);
                    if (str7 == "=")
                    {
                        if (((decimal) table2.Rows[0][str3]) == Convert.ToDecimal(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == "!=")
                    {
                        if (((decimal) table2.Rows[0][str3]) != Convert.ToDecimal(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == ">")
                    {
                        if (((decimal) table2.Rows[0][str3]) > Convert.ToDecimal(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == "<")
                    {
                        if (((decimal) table2.Rows[0][str3]) < Convert.ToDecimal(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == ">=")
                    {
                        if (((decimal) table2.Rows[0][str3]) >= Convert.ToDecimal(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if ((str7 == "<=") && (((decimal) table2.Rows[0][str3]) <= Convert.ToDecimal(str4)))
                    {
                        this.docList.Add((int) row["DocId"]);
                    }
                }
                continue;
            Label_0549:
                if ((str7 = str5) != null)
                {
                    str7 = string.IsInterned(str7);
                    if (str7 == "=")
                    {
                        if (((int) table2.Rows[0][str3]) == Convert.ToInt32(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == "!=")
                    {
                        if (((int) table2.Rows[0][str3]) != Convert.ToInt32(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == ">")
                    {
                        if (((int) table2.Rows[0][str3]) > Convert.ToInt32(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == "<")
                    {
                        if (((int) table2.Rows[0][str3]) < Convert.ToInt32(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if (str7 == ">=")
                    {
                        if (((int) table2.Rows[0][str3]) >= Convert.ToInt32(str4))
                        {
                            this.docList.Add((int) row["DocId"]);
                        }
                    }
                    else if ((str7 == "<=") && (((int) table2.Rows[0][str3]) <= Convert.ToInt32(str4)))
                    {
                        this.docList.Add((int) row["DocId"]);
                    }
                }
                continue;
            Label_07C2:
                switch (str5)
                {
                    case "=":
                        if (!date.Equals(time2))
                        {
                            continue;
                        }
                        this.docList.Add((int) row["DocId"]);
                        break;

                    case "!=":
                        if (date.Equals(time2))
                        {
                            continue;
                        }
                        this.docList.Add((int) row["DocId"]);
                        break;

                    case ">":
                        if (date <= time2)
                        {
                            continue;
                        }
                        this.docList.Add((int) row["DocId"]);
                        break;

                    case "<":
                        if (date >= time2)
                        {
                            continue;
                        }
                        this.docList.Add((int) row["DocId"]);
                        break;

                    case ">=":
                        if (date < time2)
                        {
                            continue;
                        }
                        this.docList.Add((int) row["DocId"]);
                        break;

                    case "<=":
                        if (date > time2)
                        {
                            continue;
                        }
                        this.docList.Add((int) row["DocId"]);
                        break;
                }
            }
        }

        private void Init()
        {
            DataAccess access = new DataAccess();
            string sql = "Select WfId From wfDet Where DetId = " + this.detId;
            this.wfId = access.dxGetIntData(sql);
            this.wfGetDocIdList();
        }

        public int UpdateItem(int ActionId, int DocId, ref string message)
        {
            DataAccess access = new DataAccess();
            using (SqlConnection connection = access.dxConnection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                return this.UpdateItem(ActionId, DocId, command, ref message);
            }
        }

        public int UpdateItem(int ActionId, int DocId, SqlCommand command, ref string message)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "wfActionUpdateDocItem";
            command.Parameters.Clear();
            command.Parameters.Add("@ActionId", ActionId);
            command.Parameters.Add("@DocId", DocId);
            command.Parameters.Add("@actionDocId", this.actionDocId);
            try
            {
                message = string.Empty;
                return command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                message = exception.Message;
                return -1;
            }
            catch (Exception exception2)
            {
                message = exception2.Message;
                return -1;
            }
        }

        public int UpdateItem(int ActionId, int DocId, string TableName, string ConditionField, string ConditionValue, int valueType, string Operator, ref string message)
        {
            DataAccess access = new DataAccess();
            using (SqlConnection connection = access.dxConnection)
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                return this.UpdateItem(ActionId, DocId, TableName, ConditionField, ConditionValue, valueType, Operator, command, ref message);
            }
        }

        public int UpdateItem(int ActionId, int DocId, string TableName, string ConditionField, string ConditionValue, int valueType, string Operator, SqlCommand command, ref string message)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "wfActionUpdateDocItem";
            command.Parameters.Clear();
            command.Parameters.Add("@ActionId", ActionId);
            command.Parameters.Add("@DocId", DocId);
            command.Parameters.Add("@Always", false);
            command.Parameters.Add("@TableName", TableName);
            command.Parameters.Add("@ConditionField", ConditionField);
            command.Parameters.Add("@ConditionValue", ConditionValue);
            command.Parameters.Add("@ValueType", valueType);
            command.Parameters.Add("@Operator", Operator);
            command.Parameters.Add("@actionDocId", this.actionDocId);
            try
            {
                message = string.Empty;
                return command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                message = exception.Message;
                return -1;
            }
            catch (Exception exception2)
            {
                message = exception2.Message;
                return -1;
            }
        }

        public static bool wfCheckInputForDocument(string TableName, string FieldName, string strValue, ValueType valueType, string strOperator, ref string message)
        {
            if (!ChechTable(TableName, ref message))
            {
                return false;
            }
            if (!ChechWfId(TableName, ref message))
            {
                return false;
            }
            if (!ChechFieldName(TableName, FieldName, ref message))
            {
                return false;
            }
            if (!ChechFieldDataType(TableName, FieldName, valueType, ref message))
            {
                return false;
            }
            if (!ChechValueDataType(strValue, valueType, ref message))
            {
                return false;
            }
            if (!CheckOperator(strOperator, valueType, ref message))
            {
                return false;
            }
            return true;
        }

        private void wfGetDocIdList()
        {
            this.docList = new ArrayList();
            this.GetAlwaysDocId();
            this.GetOtherDocList();
        }

        public static DataTable wfGetDocListByActionId(int actionId)
        {
            DataAccess access = new DataAccess();
            access.dxAddParameter("@ActionId", actionId);
            return access.dxGetSPData("wfActionDocListByActionId");
        }

        public ArrayList DocIdList
        {
            get
            {
                return this.docList;
            }
        }

        public enum ValueType
        {
            Boolean = 4,
            DateTime = 5,
            Decimal = 3,
            Int = 2,
            String = 1
        }
    }
}

