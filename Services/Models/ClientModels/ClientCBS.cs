namespace NovaAccounts.Services.Models.ClientModels;

    public class ClientCBS
    {
        public int Id { get; set; }
        public int ClientTypeID { get; set; }
        public string ClientType { get; set; } = string.Empty;
        public string ClientNum { get; set; } = string.Empty;
        public int TitleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int GenderID { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Mobile1 { get; set; } = string.Empty;
        public string Mobile2 { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public DateTime RegistrationTimeStamp { get; set; }
        public int IdTypeID { get; set; }
        public string IdType { get; set; } = string.Empty;
        public string IdNum { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostalAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int ProfessionID { get; set; }
        public string Profession { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public DateTime BusinessRegistrationDate { get; set; }
        public DateTime BusinessCommencementDate { get; set; }
        public string TinNumber { get; set; } = string.Empty;
        public string BusinessRegistrationNumber { get; set; } = string.Empty;
        public int RelationshipOfficerID { get; set; }
        public string RelationshipOfficer { get; set; } = string.Empty;
        public string BranchNumber { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public int LinkID { get; set; }
        public int ClientID { get; set; }
        public string UserEntry { get; set; } = string.Empty;
        public int WorkingStatusID { get; set; }
        public string WorkingStatus { get; set; } = string.Empty;
        public string UserApproval { get; set; } = string.Empty;
        public DateTime ApprovalTimestamp { get; set; }
        public bool KycCompleted { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public string GpsAddress { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public int MaritalStatusID { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
        public string? GroupCode { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        
        public List<SavingsAccount> SavingsAccounts { get; set; } = new();

    }