using System;
namespace IrmaQuicDashboard.Models.Enums
{
    public enum LogEntryType
    {
        NewSession,
        ServerLogGETIrmaWithToken,
        ServerLogJSONResponseIssuingCredentials,
        RequestIssuancePermission,
        RespondPermission,
        ServerLogPOSTCommitments,
        ServerLogJSONResponseProof,
        Success
    }
}
