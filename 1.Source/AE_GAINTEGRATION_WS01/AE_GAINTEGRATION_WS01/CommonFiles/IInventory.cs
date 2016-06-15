using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ComponentModel.DataAnnotations;
using System.Data;


namespace AE_GAINTEGRATION_WS01
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IInventory" in both code and config file together.
    [ServiceContract]
    public interface IInventory
    {
       [OperationContract]        
        string SaveInventoryGoodsIssue(string jsonString);


        [OperationContract]
        string SaveInventoryGoodsRecipet(string jsonString);
    }


   
    [DataContract]
    public class InventoryGoodsIssue
    {

        [DataMember]
        public DataSet GoodsIssueDataSet { get; set; }

        string itemCode = string.Empty, whscode = string.Empty, batchCode = string.Empty, reasonCode = string.Empty,
        gLAccount = string.Empty, businessUnit = string.Empty, remarks = string.Empty, journalRemarks = string.Empty, reference = string.Empty;
        double quantity = 0;
        DateTime postingDate = DateTime.Now, documentDate = DateTime.Now;


        //[DataMember(Name = "ItemCode", IsRequired = true)]
        //[StringLength(20, MinimumLength = 0, ErrorMessage = @"ItemCode length should be maximum 20.")]
        //public string ItemCode { get; set; }

        [DataMember(Name = "ItemCode", IsRequired = true)]
        [StringLength(20, MinimumLength = 1, ErrorMessage = @"ItemCode length should be maximum 20.")]
        public string ItemCode
        {
            get { return itemCode; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "ItemCode" });
                itemCode = value;
            }
        }


        [DataMember(Name = "Whscode", IsRequired = true)]
        [StringLength(8, MinimumLength = 1, ErrorMessage = @"Whscode length should be maximum 8.")]
        public string Whscode
        {
            get { return whscode; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "Whscode" });
                whscode = value;
            }
        }


        [DataMember(Name = "BatchCode", IsRequired = false)]
        [StringLength(36, MinimumLength = 1, ErrorMessage = @"BatchCode length should be maximum 36.")]
        public string BatchCode
        {
            get { return batchCode; }
            set
            {
                if (value.Trim() != string.Empty)
                {
                    Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "BatchCode" });}
                    batchCode = value;                
            }
        }


        [DataMember(Name = "ReasonCode", IsRequired = false)]
        [StringLength(5, MinimumLength = 0, ErrorMessage = @"ReasonCode length should be maximum 5.")]
        public string ReasonCode
        {
            get { return reasonCode; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "ReasonCode" });
                reasonCode = value;
            }
        }

        [DataMember(Name = "Quantity", IsRequired = true)]
        //[StringLength(19, MinimumLength = 1, ErrorMessage = @"Quantity length should be maximum 19.")]
        public double Quantity
        {
            get { return quantity; }
            set
            {
                Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "Quantity" });
                quantity = value;
            }
        }

        [DataMember(Name = "PostingDate", IsRequired = true)]
        public DateTime PostingDate
        {
            get { return postingDate; }
            set
            {
                Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "PostingDate" });
                postingDate = value;
            }
        }

        [DataMember(Name = "DocumentDate", IsRequired = true)]
        public DateTime DocumentDate
        {
            get { return documentDate; }
            set
            {
                Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "DocumentDate" });
                documentDate = value;
            }
        }

        [DataMember(Name = "GLAccount", IsRequired = true)]
        [StringLength(15, MinimumLength = 1, ErrorMessage = @"GLAccount length should be maximum 15.")]
        public string GLAccount
        {
            get { return gLAccount; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "GLAccount" });
                gLAccount = value;
            }
        }

        [DataMember(Name = "BusinessUnit", IsRequired = false)]
        [StringLength(8, MinimumLength = 0, ErrorMessage = @"BusinessUnit length should be maximum 8.")]
        public string BusinessUnit
        {
            get { return businessUnit; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "BusinessUnit" });
                businessUnit = value;
            }
        }


        [DataMember(Name = "Remarks", IsRequired = false)]
        [StringLength(254, MinimumLength = 0, ErrorMessage = @"Remarks length should be maximum 254.")]
        public string Remarks
        {
            get { return remarks; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "Remarks" });
                remarks = value;
            }
        }

        [DataMember(Name = "JournalRemarks", IsRequired = false)]
        [StringLength(50, MinimumLength = 0, ErrorMessage = @"JournalRemarks length should be maximum 50.")]
        public string JournalRemarks
        {
            get { return journalRemarks; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "JournalRemarks" });
                journalRemarks = value;
            }
        }

        [DataMember(Name = "Reference", IsRequired = false)]
        [StringLength(11, MinimumLength = 0, ErrorMessage = @"Reference length should be maximum 50.")]
        public string Reference
        {
            get { return reference; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "Reference" });
                reference = value;
            }
        }


    }


    [DataContract]
    public class InventoryGoodsReceipt
    {
        [DataMember]
        public DataSet GoodsReceiptDataSet { get; set; }

        string itemCode = string.Empty, whscode = string.Empty, batchCode = string.Empty, reasonCode = string.Empty,
        gLAccount = string.Empty, businessUnit = string.Empty, remarks = string.Empty, journalRemarks = string.Empty, reference = string.Empty;
        double quantity = 0, price = 0;
        DateTime postingDate = DateTime.Now, documentDate = DateTime.Now;

        [DataMember(Name = "ItemCode", IsRequired = true)]
        [StringLength(20, MinimumLength = 1, ErrorMessage = @"ItemCode length should be maximum 20.")]
        public string ItemCode
        {
            get { return itemCode; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "ItemCode" });
                itemCode = value;
            }
        }


        [DataMember(Name = "Whscode", IsRequired = true)]
        [StringLength(8, MinimumLength = 1, ErrorMessage = @"Whscode length should be maximum 8.")]
        public string Whscode
        {
            get { return whscode; }
            set
            {
                Validator.ValidateProperty(value.Trim().Trim(), new ValidationContext(this, null, null) { MemberName = "Whscode" });
                whscode = value;
            }
        }


        [DataMember(Name = "BatchCode", IsRequired = false)]
        [StringLength(36, MinimumLength = 1, ErrorMessage = @"BatchCode length should be maximum 36.")]
        public string BatchCode
        {
            get { return batchCode; }
            set
            {
                if (value.Trim() != string.Empty)
                { Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "BatchCode" });}
                 batchCode = value;               
            }
        }


        [DataMember(Name = "ReasonCode", IsRequired = true)]
        [StringLength(5, MinimumLength = 1, ErrorMessage = @"ReasonCode length should be maximum 5.")]
        public string ReasonCode
        {
            get { return reasonCode; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "ReasonCode" });
                reasonCode = value;
            }
        }

        [DataMember(Name = "Quantity", IsRequired = true)]
        [StringLength(19, MinimumLength = 1, ErrorMessage = @"Quantity length should be maximum 19.")]
        public double Quantity
        {
            get { return quantity; }
            set
            {
                Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "Quantity" });
                quantity = value;
            }
        }

        [DataMember(Name = "Price", IsRequired = true)]
       // [StringLength(19, MinimumLength = 1, ErrorMessage = @"Price length should be maximum 19.")]
        public double Price
        {
            get { return price; }
            set
            {
                Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "Price" });
                price = value;
            }
        }

        [DataMember(Name = "PostingDate", IsRequired = true)]
        public DateTime PostingDate
        {
            get { return postingDate; }
            set
            {
                Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "PostingDate" });
                postingDate = value;
            }
        }

        [DataMember(Name = "DocumentDate", IsRequired = true)]
        public DateTime DocumentDate
        {
            get { return documentDate; }
            set
            {
                Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "DocumentDate" });
                documentDate = value;
            }
        }

        [DataMember(Name = "GLAccount", IsRequired = true)]
        [StringLength(15, MinimumLength = 1, ErrorMessage = @"GLAccount length should be maximum 15.")]
        public string GLAccount
        {
            get { return gLAccount; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "GLAccount" });
                gLAccount = value;
            }
        }

        [DataMember(Name = "BusinessUnit", IsRequired = false)]
        [StringLength(8, MinimumLength = 1, ErrorMessage = @"BusinessUnit length should be maximum 8.")]
        public string BusinessUnit
        {
            get { return businessUnit; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "BusinessUnit" });
                businessUnit = value;
            }
        }


        [DataMember(Name = "Remarks", IsRequired = false)]
        [StringLength(254, MinimumLength = 0, ErrorMessage = @"Remarks length should be maximum 254.")]
        public string Remarks
        {
            get { return remarks; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "Remarks" });
                remarks = value;
            }
        }

        [DataMember(Name = "JournalRemarks", IsRequired = false)]
        [StringLength(50, MinimumLength = 0, ErrorMessage = @"JournalRemarks length should be maximum 50.")]
        public string JournalRemarks
        {
            get { return journalRemarks; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "JournalRemarks" });
                journalRemarks = value;
            }
        }

        [DataMember(Name = "Reference", IsRequired = false)]
        [StringLength(11, MinimumLength = 0, ErrorMessage = @"Reference length should be maximum 50.")]
        public string Reference
        {
            get { return reference; }
            set
            {
                Validator.ValidateProperty(value.Trim(), new ValidationContext(this, null, null) { MemberName = "Reference" });
                reference = value;
            }
        }
    }
}
