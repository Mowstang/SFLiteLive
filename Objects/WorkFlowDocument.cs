namespace SFLite.DocType
{
    using System;

    public class WorkFlowDocument
    {
        public enum DocumentTypes
        {
            Claim_Cover_Sheet = 1,
            Customer_Authorization_Commercial = 7,
            Customer_Authorization_Residential = 6,
            Emergency_Authorization = 2,
            Preliminary_Report = 3,
            Production_Communication_Sheet = 4,
            Production_Completion_Sheet = 5
        }
    }
}

