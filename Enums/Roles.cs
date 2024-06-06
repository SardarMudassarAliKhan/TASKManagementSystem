namespace TASKManagementSystem.Enums
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum Roles
    {
        [EnumMember(Value = "Admin")]
        Admin,

        [EnumMember(Value = "Developer")]
        Developer,

        [EnumMember(Value = "Manager")]
        Manager,

        [EnumMember(Value = "TeamLead")]
        TeamLead
    }

}
