using Jube.Data.Context;
using Jube.Data.Poco;
using Jube.Data.Repository;
using Jube.Data.Security;
using Jube.Service.Dto.Authentication;
using Jube.Service.Exceptions.Authentication;

namespace Jube.Service.Authentication;

public class SandboxRegistration(DbContext dbContext)
{
    public async Task<SandboxRegistrationResponseDto> Register(
        SandboxRegistrationRequestDto sandboxRegistrationRequestDto,
        string passwordHashingKey)
    {
        if (sandboxRegistrationRequestDto.UserName == null) throw new ArgumentException("Missing User Name.");
        if (sandboxRegistrationRequestDto.Password == null) throw new ArgumentException("Missing Password.");

        ValidateUserDoesNotExistAndRaiseExceptionIfExists(sandboxRegistrationRequestDto.UserName);

        var tenantRegistry = CreateTenantRegistry(sandboxRegistrationRequestDto.UserName);
        var roleRegistry = CreateRoleRegistry(sandboxRegistrationRequestDto.UserName, tenantRegistry);

        AllocateAllPermissionSpecificationsToRoleRegistry(sandboxRegistrationRequestDto.UserName, roleRegistry);

        var userRegistry = CreateUserRegistry(sandboxRegistrationRequestDto.UserName,
            sandboxRegistrationRequestDto.Password, passwordHashingKey, roleRegistry);

        AssignToUserRegistryToTenantRegistry(sandboxRegistrationRequestDto.UserName, tenantRegistry);

        var entityAnalysisModelId = InsertEntityAnalysisModel(userRegistry.Name);
        InsertEntityAnalysisModelRequestXPaths(entityAnalysisModelId, userRegistry.Name);
        InsertEntityAnalysisModelInlineScript(entityAnalysisModelId, 1, userRegistry.Name);
        InsertEntityAnalysisModelAbstractionCalculation(entityAnalysisModelId, userRegistry.Name);
        InsertEntityAnalysisModelAbstractionRule(entityAnalysisModelId, userRegistry.Name);

        var entityAnalysisModelDictionaryId =
            InsertEntityAnalysisModelDictionary(entityAnalysisModelId, userRegistry.Name);

        InsertEntityAnalysisModelDictionaryKvp(entityAnalysisModelDictionaryId, userRegistry.Name);
        InsertEntityAnalysisModelGatewayRule(entityAnalysisModelId, userRegistry.Name);
        InsertEntityAnalysisModelHttpAdaptation(entityAnalysisModelId, userRegistry.Name);
        InsertEntityAnalysisModelInlineFunction(entityAnalysisModelId, userRegistry.Name);

        var entityAnalysisModelListId = InsertEntityAnalysisModelList(entityAnalysisModelId, userRegistry.Name);

        InsertEntityAnalysisModelListValue(entityAnalysisModelListId, userRegistry.Name);
        InsertEntityAnalysisModelSanction(entityAnalysisModelId, userRegistry.Name);
        InsertEntityAnalysisModelTag(entityAnalysisModelId, userRegistry.Name);
        InsertEntityAnalysisModelTtlCounter(entityAnalysisModelId, userRegistry.Name);

        var insertVisualisationRegistryExampleVisualisationId =
            InsertVisualisationRegistryExampleVisualisation(userRegistry.Name);

        var insertVisualisationRegistryExampleEmbeddedVisualisationId =
            InsertVisualisationRegistryExampleEmbeddedVisualisation(userRegistry.Name);

        InsertVisualisationRegistryParameterEmbeddedVisualisation(
            insertVisualisationRegistryExampleEmbeddedVisualisationId, userRegistry.Name);

        InsertVisualisationRegistryParameterExampleEmbeddedVisualisation(
            insertVisualisationRegistryExampleVisualisationId, userRegistry.Name);

        await InsertVisualisationRegistryDatasourceExampleEmbeddedVisualisation(
            insertVisualisationRegistryExampleEmbeddedVisualisationId, userRegistry.Name);

        await InsertVisualisationRegistryDatasourceExampleVisualisation(
            insertVisualisationRegistryExampleVisualisationId, userRegistry.Name);

        var caseWorkflowId = InsertCaseWorkflow(entityAnalysisModelId,
            insertVisualisationRegistryExampleEmbeddedVisualisationId, userRegistry.Name);

        InsertCaseWorkflowAction(caseWorkflowId, userRegistry.Name);
        InsertCaseWorkflowDisplay(caseWorkflowId, userRegistry.Name);
        InsertCaseWorkflowFilter(caseWorkflowId, userRegistry.Name);
        InsertCaseWorkflowForm(caseWorkflowId, userRegistry.Name);
        InsertCaseWorkflowMacro(caseWorkflowId, userRegistry.Name);
        InsertCaseWorkflowXPath(caseWorkflowId, userRegistry.Name);

        var caseWorkflowStatusId = InsertCaseWorkflowStatus(caseWorkflowId, userRegistry.Name);

        InsertEntityAnalysisModelActivationRule(entityAnalysisModelId, caseWorkflowId, caseWorkflowStatusId,
            userRegistry.Name);
        InsertEntityAnalysisModelSynchronisationSchedule(userRegistry.Name);

        return new SandboxRegistrationResponseDto
        {
            TenantRegistryId = tenantRegistry.Id,
            RoleRegistryId = roleRegistry.Id,
            UserRegistryId = userRegistry.Id
        };
    }

    private int InsertCaseWorkflowStatus(int caseWorkflowId, string userName)
    {
        var caseWorkflowStatusRepository = new CaseWorkflowStatusRepository(dbContext, userName);

        var returnCaseWorkflowStatusId = caseWorkflowStatusRepository.Insert(new CaseWorkflowStatus
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "First Line Review",
            Active = 1,
            Priority = 5,
            ForeColor = "#260080",
            BackColor = "#abc8f7"
        }).Id;

        caseWorkflowStatusRepository.Insert(new CaseWorkflowStatus
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Supervisor Review",
            Active = 1,
            Priority = 4,
            ForeColor = "#260080",
            BackColor = "#fafcb1"
        });

        caseWorkflowStatusRepository.Insert(new CaseWorkflowStatus
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Restricted",
            Active = 1,
            Priority = 3,
            ForeColor = "#260080",
            BackColor = "#fce9c5"
        });

        caseWorkflowStatusRepository.Insert(new CaseWorkflowStatus
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Fraudulent",
            Active = 1,
            Priority = 4,
            ForeColor = "#260080",
            BackColor = "#facaf4"
        });

        caseWorkflowStatusRepository.Insert(new CaseWorkflowStatus
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Reported",
            Active = 1,
            Priority = 1,
            ForeColor = "#260080",
            BackColor = "#f77781"
        });

        return returnCaseWorkflowStatusId;
    }

    private void InsertCaseWorkflowAction(int caseWorkflowId, string userName)
    {
        var caseWorkflowActionRepository = new CaseWorkflowActionRepository(dbContext, userName);

        caseWorkflowActionRepository.Insert(new CaseWorkflowAction
        {
            Name = "Call Customer",
            CaseWorkflowId = caseWorkflowId,
            Active = 1
        });

        caseWorkflowActionRepository.Insert(new CaseWorkflowAction
        {
            Name = "Email Customer",
            CaseWorkflowId = caseWorkflowId,
            Active = 1
        });

        caseWorkflowActionRepository.Insert(new CaseWorkflowAction
        {
            Name = "Escalation",
            CaseWorkflowId = caseWorkflowId,
            Active = 1
        });

        caseWorkflowActionRepository.Insert(new CaseWorkflowAction
        {
            Name = "General Action",
            CaseWorkflowId = caseWorkflowId,
            Active = 1
        });
    }

    private void InsertCaseWorkflowXPath(int caseWorkflowId, string userName)
    {
        var caseWorkflowXPathRepository = new CaseWorkflowXPathRepository(dbContext, userName);

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AccountId",
            XPath = "payload.AccountId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "TxnDateTime",
            XPath = "payload.TxnDateTime",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Currency",
            XPath = "payload.Currency",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ResponseCode",
            XPath = "payload.ResponseCode",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff",
            RegularExpression = "[^0]+",
            ForeRowColorScope = 1,
            BackRowColorScope = 0
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "CurrencyAmount",
            XPath = "payload.CurrencyAmount",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "SettlementAmount",
            XPath = "payload.SettlementAmount",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AccountCurrency",
            XPath = "payload.AccountCurrency",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ChannelId",
            XPath = "payload.ChannelId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AppVersionCode",
            XPath = "payload.AppVersionCode",
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ServiceCode",
            XPath = "payload.ServiceCode",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "System",
            XPath = "payload.System",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Brand",
            XPath = "payload.Brand",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Model",
            XPath = "payload.Model",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AccountLongitude",
            XPath = "payload.AccountLongitude",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AccountLatitude",
            XPath = "payload.AccountLatitude",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "OS",
            XPath = "payload.OS",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Resolution",
            XPath = "payload.Resolution",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "DebuggerAttached",
            XPath = "payload.DebuggerAttached",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "SimulatorAttached",
            XPath = "payload.SimulatorAttached",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Jailbreak",
            XPath = "payload.Jailbreak",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "MAC",
            XPath = "payload.MAC",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ToAccountId",
            XPath = "payload.ToAccountId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ToAccountExternalRef",
            XPath = "payload.ToAccountExternalRef",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "TwoFATypeId",
            XPath = "payload.TwoFATypeId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "TwoFAResponseId",
            XPath = "payload.TwoFAResponseId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Storage",
            XPath = "payload.Storage",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "TransactionExternalResponseId",
            XPath = "payload.TransactionExternalResponseId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "FingerprintHash",
            XPath = "payload.FingerprintHash",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BusinessModel",
            XPath = "payload.BusinessModel",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AmountEUR",
            XPath = "payload.AmountEUR",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AmountUSD",
            XPath = "payload.AmountUSD",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AmountUSDRate",
            XPath = "payload.AmountUSDRate",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AmountGBP",
            XPath = "payload.AmountGBP",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AmountGBPRate",
            XPath = "payload.AmountGBPRate",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Is3D",
            XPath = "payload.Is3D",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "OriginalAmount",
            XPath = "payload.OriginalAmount",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "OriginalCurrency",
            XPath = "payload.OriginalCurrency",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "Email",
            XPath = "payload.Email",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "CreditCardHash",
            XPath = "payload.CreditCardHash",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AcquirerBankName",
            XPath = "payload.AcquirerBankName",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ActionDate",
            XPath = "payload.ActionDate",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "APMAccountId",
            XPath = "payload.APMAccountId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BankId",
            XPath = "payload.BankId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingAddress",
            XPath = "payload.BillingAddress",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingCity",
            XPath = "payload.BillingCity",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingCountry",
            XPath = "payload.BillingCountry",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingFirstName",
            XPath = "payload.BillingFirstName",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingLastName",
            XPath = "payload.BillingLastName",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingPhone",
            XPath = "payload.BillingPhone",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingState",
            XPath = "payload.BillingState",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "BillingZip",
            XPath = "payload.BillingZip",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IsAPM",
            XPath = "payload.IsAPM",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IsCascaded",
            XPath = "payload.IsCascaded",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IsCredited",
            XPath = "payload.IsCredited",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IsCurrencyConverted",
            XPath = "payload.IsCurrencyConverted",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IsModification",
            XPath = "payload.IsModification",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IsRebill",
            XPath = "payload.IsRebill",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "TransactionTypeId",
            XPath = "payload.TransactionTypeId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "TransactionResultId",
            XPath = "payload.TransactionResultId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IP",
            Drill = 1,
            XPath = "payload.IP",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "AmountEURRate",
            XPath = "payload.AmountEURRate",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "DeviceId",
            Drill = 1,
            XPath = "payload.DeviceId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "IsModified",
            XPath = "payload.IsModified",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "OrderId",
            XPath = "payload.OrderId",
            Active = 1,
            BoldLineFormatForeColor = "#000000",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });

        caseWorkflowXPathRepository.Insert(new CaseWorkflowXPath
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "TxnId",
            XPath = "payload.TxnId",
            Active = 1,
            BoldLineMatched = 1,
            BoldLineFormatForeColor = "#032cfc",
            BoldLineFormatBackColor = "#ffffff",
            ConditionalFormatForeColor = "#000000",
            ConditionalFormatBackColor = "#ffffff"
        });
    }

    private int InsertCaseWorkflow(int entityAnalysisModelId, int visualisationRegistryId, string userName)
    {
        var caseWorkflowRepository = new CaseWorkflowRepository(dbContext, userName);

        return caseWorkflowRepository.Insert(new CaseWorkflow
        {
            Name = "Detailed Account Financial Transactions Cases",
            Active = 1,
            EntityAnalysisModelId = entityAnalysisModelId,
            EnableVisualisation = 1,
            VisualisationRegistryId = visualisationRegistryId
        }).Id;
    }

    private void InsertCaseWorkflowMacro(int caseWorkflowId, string userName)
    {
        var caseWorkflowMacroRepository = new CaseWorkflowMacroRepository(dbContext, userName);

        caseWorkflowMacroRepository.Insert(new CaseWorkflowMacro
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ExampleMacro",
            Active = 1,
            Javascript = "alert('Example Macro Javascript Eval.');",
            ImageLocation = "calculator.png"
        });
    }

    private void InsertCaseWorkflowForm(int caseWorkflowId, string userName)
    {
        var caseWorkflowForm = new CaseWorkflowFormRepository(dbContext, userName);

        caseWorkflowForm.Insert(new CaseWorkflowForm
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ExampleForm",
            Active = 1,
            Html = "<p>This form will support most unstyled elements available to HTML forms.</p>" +
                   Environment.NewLine +
                   "<p>Injection of Javascript to support validation and additional styling " +
                   "is not supported at this time.</p>" + Environment.NewLine +
                   "<p>The submit button is added during rendering.  " +
                   "The submit button will query the name and value attributes and create the " +
                   "payload for processing.</p>" + Environment.NewLine +
                   "Example Textbox Element: " + Environment.NewLine +
                   "<br/>" + Environment.NewLine +
                   "<input id='ExampleTextboxElement'/>"
        });
    }

    private void InsertCaseWorkflowFilter(int caseWorkflowId, string userName)
    {
        var caseWorkflowFilterRepository = new CaseWorkflowFilterRepository(dbContext, userName);

        caseWorkflowFilterRepository.Insert(new CaseWorkflowFilter
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "OpenCasesNotLockedByCreatedDate",
            FilterSql = "\"Case\".\"ClosedStatusId\" = @1 AND \"Case\".\"Locked\" = @2",
            FilterJson = "{\"rules\": [{\"id\": \"ClosedStatusId\", \"type\": \"integer\", \"field\": " +
                         "\"\\\"Case\\\".\\\"ClosedStatusId\\\"\", \"input\": \"select\", " +
                         "\"value\": 0, \"operator\": \"equal\"}, {\"id\": \"Locked\", " +
                         "\"type\": \"integer\", \"field\": \"\\\"Case\\\".\\\"Locked\\\"\", " +
                         "\"input\": \"radio\", \"value\": 0, \"operator\": \"equal\"}], " +
                         "\"valid\": true, \"condition\": \"AND\"}",
            SelectJson = "{\"rules\": [{\"id\": \"CaseKey\", \"type\": \"string\", \"field\": \"\\\"CaseKey\\\"\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"CaseKeyValue\", \"type\": \"string\", " +
                         "\"field\": \"\\\"CaseKeyValue\\\"\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", " +
                         "\"operator\": \"order\"}, {\"id\": \"CaseWorkflowStatusId\", " +
                         "\"type\": \"string\", \"field\": \"\\\"CaseWorkflowStatus\\\".\\\"Id\\\"\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"Payload.CurrencyAmount\", " +
                         "\"type\": \"string\", \"field\": \"(\\\"Json\\\"-> 'payload' -> 'CurrencyAmount')::double precision\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"Payload.BusinessModel\", \"type\": \"string\", " +
                         "\"field\": \"(\\\"Json\\\"-> 'payload' -> 'BusinessModel')\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"Payload.IP\", \"type\": \"string\", " +
                         "\"field\": \"(\\\"Json\\\"-> 'payload' -> 'IP')\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"Payload.Email\", \"type\": \"string\", " +
                         "\"field\": \"(\\\"Json\\\"-> 'payload' -> 'Email')\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"Payload.BusinessModel\", \"type\": \"string\", " +
                         "\"field\": \"(\\\"Json\\\"-> 'payload' -> 'BusinessModel')\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"Payload.BillingAddress\", \"type\": \"string\", " +
                         "\"field\": \"(\\\"Json\\\"-> 'payload' -> 'BillingAddress')\", " +
                         "\"input\": \"radio\", \"value\": \"ASC\", \"operator\": \"order\"}, " +
                         "{\"id\": \"Payload.BillingCountry\", \"type\": \"string\", \"field\": " +
                         "\"(\\\"Json\\\"-> 'payload' -> 'BillingCountry')\", \"input\": \"radio\", " +
                         "\"value\": \"ASC\", \"operator\": \"order\"}, {\"id\": \"CreatedDate\", " +
                         "\"type\": \"string\", \"field\": \"\\\"Case\\\".\\\"CreatedDate\\\"\", " +
                         "\"input\": \"radio\", \"value\": \"DESC\", \"operator\": \"order\"}], " +
                         "\"valid\": true, \"condition\": \"AND\"}",
            FilterTokens = "[0,0]",
            Active = 1
        });
    }

    private void InsertCaseWorkflowDisplay(int caseWorkflowId, string userName)
    {
        var caseWorkflowDisplayRepository = new CaseWorkflowDisplayRepository(dbContext, userName);

        caseWorkflowDisplayRepository.Insert(new CaseWorkflowDisplay
        {
            CaseWorkflowId = caseWorkflowId,
            Name = "ExampleDisplayCurrencyAmount",
            Active = 1,
            Html = "This is an example Cases Workflow Display:" + Environment.NewLine +
                   "The transaction amount is:" + Environment.NewLine +
                   "<br/>" + Environment.NewLine +
                   "<br/>" + Environment.NewLine +
                   "<div style='font-size:30px'>[@CurrencyAmount@]</div>" + Environment.NewLine +
                   "<br/>" + Environment.NewLine +
                   "The tokens are taken from the Cases Workflows XPath and can be laid out in HTML."
        });
    }

    private void InsertVisualisationRegistryParameterEmbeddedVisualisation(int visualisationRegistryId,
        string userName)
    {
        var visualisationRegistryParameterRepository =
            new VisualisationRegistryParameterRepository(dbContext, userName);

        visualisationRegistryParameterRepository.Insert(new VisualisationRegistryParameter
        {
            VisualisationRegistryId = visualisationRegistryId,
            Name = "AccountId",
            Active = 1,
            DataTypeId = 1,
            Required = 1,
            DefaultValue = "Test1"
        });
    }

    private void InsertVisualisationRegistryParameterExampleEmbeddedVisualisation(int visualisationRegistryId,
        string userName)
    {
        var visualisationRegistryParameterRepository =
            new VisualisationRegistryParameterRepository(dbContext, userName);

        visualisationRegistryParameterRepository.Insert(new VisualisationRegistryParameter
        {
            VisualisationRegistryId = visualisationRegistryId,
            Name = "Percentage Contribution Greater Than",
            Active = 1,
            DataTypeId = 3,
            Required = 1,
            DefaultValue = "0"
        });

        visualisationRegistryParameterRepository.Insert(new VisualisationRegistryParameter
        {
            VisualisationRegistryId = visualisationRegistryId,
            Name = "Frequency Greater Than",
            Active = 1,
            DataTypeId = 2,
            Required = 1,
            DefaultValue = "0"
        });
    }

    private async Task InsertVisualisationRegistryDatasourceExampleVisualisation(int visualisationRegistryId,
        string userName)
    {
        var visualisationRegistryDatasourceRepository =
            new VisualisationRegistryDatasourceRepository(dbContext, userName);

        await visualisationRegistryDatasourceRepository.InsertAsync(new VisualisationRegistryDatasource
        {
            VisualisationRegistryId = visualisationRegistryId,
            Name = "ExamplePie",
            Active = 1,
            VisualisationTypeId = 1,
            Command = "select " + Environment.NewLine +
                      "\"ActivationRuleName\"," + Environment.NewLine +
                      "\"Frequency\"," + Environment.NewLine +
                      "\"PercentageContribution\" " + Environment.NewLine +
                      "from \"ExampleCaseVolumeEntry\" " + Environment.NewLine +
                      "where \"Frequency\" > @Frequency_Greater_Than " + Environment.NewLine +
                      "and \"PercentageContribution\" > @Percentage_Contribution_Greater_Than " +
                      Environment.NewLine +
                      "order by \"Frequency\" desc",
            Priority = 1,
            IncludeGrid = 0,
            IncludeDisplay = 1,
            ColumnSpan = 3,
            RowSpan = 2,
            VisualisationText = "({" + Environment.NewLine +
                                "    legend: {" + Environment.NewLine +
                                "        position: \"bottom\"" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    series: [{" + Environment.NewLine +
                                "        type: \"pie\"," + Environment.NewLine +
                                "        field: \"Frequency\"," + Environment.NewLine +
                                "        categoryField: \"ActivationRuleName\"" + Environment.NewLine +
                                "    }]," + Environment.NewLine +
                                "    seriesColors: [\"#03a9f4\", \"#ff9800\", \"#fad84a\", \"#4caf50\"]," +
                                Environment.NewLine +
                                "    tooltip: {" + Environment.NewLine +
                                "        visible: true," + Environment.NewLine +
                                "        template: \"${ category } - ${ value }%\"" + Environment.NewLine +
                                "    }" + Environment.NewLine +
                                "})" + Environment.NewLine
        });

        await visualisationRegistryDatasourceRepository.InsertAsync(new VisualisationRegistryDatasource
        {
            VisualisationRegistryId = visualisationRegistryId,
            Name = "ExampleBar",
            Active = 1,
            VisualisationTypeId = 1,
            Command = "select " + Environment.NewLine +
                      "\"ActivationRuleName\"," + Environment.NewLine +
                      "\"Frequency\"," + Environment.NewLine +
                      "\"PercentageContribution\" " + Environment.NewLine +
                      "from \"ExampleCaseVolumeEntry\" " + Environment.NewLine +
                      "where \"Frequency\" > @Frequency_Greater_Than " + Environment.NewLine +
                      "and \"PercentageContribution\" > @Percentage_Contribution_Greater_Than " +
                      Environment.NewLine +
                      "order by \"Frequency\" desc",
            Priority = 2,
            IncludeGrid = 0,
            IncludeDisplay = 1,
            ColumnSpan = 3,
            RowSpan = 2,
            VisualisationText = "({" + Environment.NewLine +
                                "    legend: {" + Environment.NewLine +
                                "        visible: false" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    seriesDefaults: {" + Environment.NewLine +
                                "        type: \"bar\"" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    series: [" + Environment.NewLine +
                                "        {" + Environment.NewLine +
                                "        field: \"Frequency\"," + Environment.NewLine +
                                "        categoryField: \"ActivationRuleName\"," + Environment.NewLine +
                                "        name: \"Frequency\"" + Environment.NewLine +
                                "    }]," + Environment.NewLine +
                                "    valueAxis: {" + Environment.NewLine +
                                "        max: 140000," + Environment.NewLine +
                                "        line: {" + Environment.NewLine +
                                "            visible: false" + Environment.NewLine +
                                "        }," + Environment.NewLine +
                                "        minorGridLines: {" + Environment.NewLine +
                                "            visible: true" + Environment.NewLine +
                                "        }," + Environment.NewLine +
                                "        labels: {" + Environment.NewLine +
                                "            rotation: \"auto\"" + Environment.NewLine +
                                "        }" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    tooltip: {" + Environment.NewLine +
                                "        visible: true," + Environment.NewLine +
                                "        template: \"#= series.name #: #= value #\"" + Environment.NewLine +
                                "    }" + Environment.NewLine +
                                "})" + Environment.NewLine
        });
    }

    private async Task InsertVisualisationRegistryDatasourceExampleEmbeddedVisualisation(int visualisationRegistryId,
        string userName)
    {
        var visualisationRegistryDatasourceRepository =
            new VisualisationRegistryDatasourceRepository(dbContext, userName);

        await visualisationRegistryDatasourceRepository.InsertAsync(new VisualisationRegistryDatasource
        {
            VisualisationRegistryId = visualisationRegistryId,
            Name = "ExamplePie",
            Active = 1,
            VisualisationTypeId = 1,
            Command = "select " + Environment.NewLine +
                      "\"MCC\"," + Environment.NewLine +
                      "\"Frequency\"," + Environment.NewLine +
                      "\"Sum\" " + Environment.NewLine +
                      "from \"ExampleCustomerCaseManagement\" " + Environment.NewLine +
                      "where \"AccountId\" = @AccountId " + Environment.NewLine +
                      "order by \"Frequency\" desc",
            Priority = 1,
            IncludeGrid = 0,
            IncludeDisplay = 1,
            ColumnSpan = 3,
            RowSpan = 2,
            VisualisationText = "({" + Environment.NewLine +
                                "    legend: {" + Environment.NewLine +
                                "        position: \"bottom\"" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    series: [{" + Environment.NewLine +
                                "        type: \"pie\"," + Environment.NewLine +
                                "        field: \"Frequency\"," + Environment.NewLine +
                                "        categoryField: \"MCC\"" + Environment.NewLine +
                                "    }]," + Environment.NewLine +
                                "    seriesColors: [\"#03a9f4\", \"#ff9800\", \"#fad84a\", \"#4caf50\"]," +
                                Environment.NewLine +
                                "    tooltip: {" + Environment.NewLine +
                                "        visible: true," + Environment.NewLine +
                                "        template: \"${ category } - ${ value }%\"" + Environment.NewLine +
                                "    }" + Environment.NewLine +
                                "})" + Environment.NewLine
        });

        await visualisationRegistryDatasourceRepository.InsertAsync(new VisualisationRegistryDatasource
        {
            VisualisationRegistryId = visualisationRegistryId,
            Name = "ExampleBar",
            Active = 1,
            VisualisationTypeId = 1,
            Command = "select " + Environment.NewLine +
                      "\"MCC\"," + Environment.NewLine +
                      "\"Frequency\"," + Environment.NewLine +
                      "\"Sum\" " + Environment.NewLine +
                      "from \"ExampleCustomerCaseManagement\" " + Environment.NewLine +
                      "where \"AccountId\" = @AccountId " + Environment.NewLine +
                      "order by \"Frequency\" desc",
            Priority = 2,
            IncludeGrid = 0,
            IncludeDisplay = 1,
            ColumnSpan = 3,
            RowSpan = 2,
            VisualisationText = "({" + Environment.NewLine +
                                "    legend: {" + Environment.NewLine +
                                "        visible: false" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    seriesDefaults: {" + Environment.NewLine +
                                "        type: \"bar\"" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    series: [" + Environment.NewLine +
                                "        {" + Environment.NewLine +
                                "        field: \"Sum\"," + Environment.NewLine +
                                "        categoryField: \"MCC\"," + Environment.NewLine +
                                "        name: \"Sum\"" + Environment.NewLine +
                                "    }]," + Environment.NewLine +
                                "    valueAxis: {" + Environment.NewLine +
                                "        max: 140000," + Environment.NewLine +
                                "        line: {" + Environment.NewLine +
                                "            visible: false" + Environment.NewLine +
                                "        }," + Environment.NewLine +
                                "        minorGridLines: {" + Environment.NewLine +
                                "            visible: true" + Environment.NewLine +
                                "        }," + Environment.NewLine +
                                "        labels: {" + Environment.NewLine +
                                "            rotation: \"auto\"" + Environment.NewLine +
                                "        }" + Environment.NewLine +
                                "    }," + Environment.NewLine +
                                "    tooltip: {" + Environment.NewLine +
                                "        visible: true," + Environment.NewLine +
                                "        template: \"#= series.name #: #= value #\"" + Environment.NewLine +
                                "    }" + Environment.NewLine +
                                "})" + Environment.NewLine
        });
    }

    private int InsertVisualisationRegistryExampleVisualisation(string userName)
    {
        var visualisationRegistryRepository = new VisualisationRegistryRepository(dbContext, userName);

        return visualisationRegistryRepository.Insert(new VisualisationRegistry
        {
            Name = "ExampleVisualisation",
            Active = 1,
            ShowInDirectory = 1,
            TenantRegistryId = 1,
            Columns = 6,
            ColumnWidth = 300,
            RowHeight = 300
        }).Id;
    }

    private int InsertVisualisationRegistryExampleEmbeddedVisualisation(string userName)
    {
        var visualisationRegistryRepository = new VisualisationRegistryRepository(dbContext, userName);

        return visualisationRegistryRepository.Insert(new VisualisationRegistry
        {
            Name = "ExampleEmbeddedVisualisation",
            Active = 1,
            ShowInDirectory = 0,
            TenantRegistryId = 1,
            Columns = 6,
            ColumnWidth = 300,
            RowHeight = 300
        }).Id;
    }

    private void InsertEntityAnalysisModelInlineScript(int entityAnalysisModelId, int entityAnalysisInlineScriptId,
        string userName)
    {
        var entityAnalysisModelInlineScriptRepository =
            new EntityAnalysisModelInlineScriptRepository(dbContext, userName);

        entityAnalysisModelInlineScriptRepository.Insert(new EntityAnalysisModelInlineScript
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Issue OTP",
            Active = 1,
            EntityAnalysisInlineScriptId = entityAnalysisInlineScriptId
        });
    }

    private void InsertEntityAnalysisModelTag(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelTagRepository = new EntityAnalysisModelTagRepository(dbContext, userName);

        entityAnalysisModelTagRepository.Insert(new EntityAnalysisModelTag
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Fraud",
            Active = 1
        });
    }

    private void InsertEntityAnalysisModelSynchronisationSchedule(string userName)
    {
        var entityAnalysisModelSynchronisationScheduleRepository =
            new EntityAnalysisModelSynchronisationScheduleRepository(dbContext, userName);

        entityAnalysisModelSynchronisationScheduleRepository.Insert(new EntityAnalysisModelSynchronisationSchedule());
    }

    private void InsertEntityAnalysisModelSanction(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelSanctionRepository = new EntityAnalysisModelSanctionRepository(dbContext, userName);

        entityAnalysisModelSanctionRepository.Insert(new EntityAnalysisModelSanction
        {
            Name = "FuzzyMatchDistance2JoinedName",
            EntityAnalysisModelId = entityAnalysisModelId,
            MultipartStringDataName = "JoinedName",
            Distance = 2,
            Active = 1,
            ResponsePayload = 1,
            CacheValue = 1,
            CacheInterval = 'h'
        });
    }

    private void InsertEntityAnalysisModelListValue(int entityAnalysisModelListId, string userName)
    {
        var entityAnalysisModelListValueRepository = new EntityAnalysisModelListValueRepository(dbContext, userName);

        entityAnalysisModelListValueRepository.Insert(new EntityAnalysisModelListValue
        {
            EntityAnalysisModelListId = entityAnalysisModelListId,
            ListValue = "123.456.789.123"
        });
    }

    private void InsertEntityAnalysisModelHttpAdaptation(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelHttpAdaptationRepository =
            new EntityAnalysisModelHttpAdaptationRepository(dbContext, userName);

        entityAnalysisModelHttpAdaptationRepository.Insert(new EntityAnalysisModelHttpAdaptation
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Active = 0,
            Name = "ExampleFraudScoreLocalEndpoint",
            ResponsePayload = 1,
            HttpEndpoint = "/api/invoke/ExampleFraudScoreLocalEndpoint"
        });
    }

    private void InsertEntityAnalysisModelTtlCounter(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelTtlCounterRepository = new EntityAnalysisModelTtlCounterRepository(dbContext, userName);

        entityAnalysisModelTtlCounterRepository.Insert(new EntityAnalysisModelTtlCounter
        {
            Name = "TtlCounterAll",
            EntityAnalysisModelId = entityAnalysisModelId,
            Active = 1,
            TtlCounterInterval = "h",
            TtlCounterValue = 1,
            ResponsePayload = 1,
            TtlCounterDataName = "AccountId",
            OnlineAggregation = 0,
            EnableLiveForever = 0
        });
    }

    private int InsertEntityAnalysisModelList(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelListRepository = new EntityAnalysisModelListRepository(dbContext, userName);

        return entityAnalysisModelListRepository.Insert(new EntityAnalysisModelList
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IPDenyList",
            Active = 1
        }).Id;
    }

    private void InsertEntityAnalysisModelInlineFunction(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelInlineFunctionRepository =
            new EntityAnalysisModelInlineFunctionRepository(dbContext, userName);

        entityAnalysisModelInlineFunctionRepository.Insert(new EntityAnalysisModelInlineFunction
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "JoinedName",
            FunctionScript = "Return Payload.BillingFirstName & \" \" & Payload.BillingLastName",
            ReturnDataTypeId = 1,
            Active = 1,
            ResponsePayload = 1
        });
    }

    private void InsertEntityAnalysisModelGatewayRule(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelGatewayRuleRepository =
            new EntityAnalysisModelGatewayRuleRepository(dbContext, userName);

        var builderRuleScript = "If (Payload.CurrencyAmount > 0) Then " + Environment.NewLine +
                                "   Return True " + Environment.NewLine +
                                "End If";

        var json = "{\"not\": false, \"rules\": [{\"id\": \"Payload.CurrencyAmount\", \"type\": \"double\", " +
                   "\"field\": \"Payload.CurrencyAmount\", \"input\": \"number\", \"value\": 0, \"operator\": " +
                   "\"greater\"}], \"valid\": true, \"condition\": \"AND\"}";

        entityAnalysisModelGatewayRuleRepository.Insert(new EntityAnalysisModelGatewayRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Priority = 0,
            BuilderRuleScript = builderRuleScript,
            Json = json,
            Name = "CurrencyAmountAllGreaterThan0.",
            MaxResponseElevation = 10,
            CoderRuleScript = "Return = True",
            RuleScriptTypeId = 1,
            GatewaySample = 1,
            Active = 1
        });
    }

    private int InsertEntityAnalysisModelDictionary(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelDictionaryRepository = new EntityAnalysisModelDictionaryRepository(dbContext, userName);

        return entityAnalysisModelDictionaryRepository.Insert(new EntityAnalysisModelDictionary
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "VolumeThresholdByAccountId",
            Active = 1,
            ResponsePayload = 1,
            DataName = "AccountId"
        }).Id;
    }

    private void InsertEntityAnalysisModelDictionaryKvp(int entityAnalysisModelDictionaryId, string userName)
    {
        var entityAnalysisModelDictionaryKvpRepository =
            new EntityAnalysisModelDictionaryKvpRepository(dbContext, userName);

        entityAnalysisModelDictionaryKvpRepository.Insert(new EntityAnalysisModelDictionaryKvp()
        {
            EntityAnalysisModelDictionaryId = entityAnalysisModelDictionaryId,
            KvpKey = "Test1",
            KvpValue = 1000
        });
    }

    private void InsertEntityAnalysisModelAbstractionRule(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelAbstractionRuleRepository =
            new EntityAnalysisModelAbstractionRuleRepository(dbContext, userName);

        var builderRuleScriptApproved = "If (Payload.ResponseCode = \"0\") Then " + Environment.NewLine +
                                        "   Return True " + Environment.NewLine +
                                        "End If";

        var jsonApproved =
            "{\"not\": false, \"rules\": [{\"id\": \"Payload.ResponseCode\", \"type\": \"string\", " +
            "\"field\": \"Payload.ResponseCode\", \"input\": \"text\", \"value\": \"0\", " +
            "\"operator\": \"equal\"}], \"valid\": true, \"condition\": \"AND\"}";

        entityAnalysisModelAbstractionRuleRepository.Insert(new EntityAnalysisModelAbstractionRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            BuilderRuleScript = builderRuleScriptApproved,
            CoderRuleScript = "Return True",
            RuleScriptTypeId = 1,
            Name = "ResponseCodeEqual0Volume",
            Search = 1,
            SearchFunctionTypeId = 3,
            SearchFunctionKey = "CurrencyAmount",
            SearchInterval = "h",
            SearchValue = 1,
            SearchKey = "AccountId",
            Active = 1,
            ResponsePayload = 1,
            Json = jsonApproved
        });

        var builderRuleScriptDeclined = "If (NOT ( Payload.ResponseCode = \"0\" )) Then " + Environment.NewLine +
                                        "   Return True" + Environment.NewLine +
                                        "End If";

        var jsonDeclined = "{\"not\": true, \"rules\": [{\"id\": \"Payload.ResponseCode\", \"type\": \"string\", " +
                           "\"field\": \"Payload.ResponseCode\", \"input\": \"text\", \"value\": \"0\", " +
                           "\"operator\": \"equal\"}], \"valid\": true, \"condition\": \"AND\"}";

        entityAnalysisModelAbstractionRuleRepository.Insert(new EntityAnalysisModelAbstractionRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            BuilderRuleScript = builderRuleScriptDeclined,
            CoderRuleScript = "Return True",
            RuleScriptTypeId = 1,
            Name = "NotResponseCodeEqual0Volume",
            Search = 1,
            SearchFunctionTypeId = 3,
            SearchFunctionKey = "CurrencyAmount",
            SearchInterval = "h",
            SearchValue = 1,
            SearchKey = "AccountId",
            Active = 1,
            ResponsePayload = 1,
            Json = jsonDeclined
        });
    }

    private void InsertEntityAnalysisModelAbstractionCalculation(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelAbstractionCalculationRepository =
            new EntityAnalysisModelAbstractionCalculationRepository(dbContext, userName);

        var functionScript = "Return Abstraction.NotResponseCodeEqual0Volume / " + Environment.NewLine +
                             " (Abstraction.NotResponseCodeEqual0Volume _  " + Environment.NewLine +
                             "+ Abstraction.ResponseCodeEqual0Volume) _ " + Environment.NewLine;

        entityAnalysisModelAbstractionCalculationRepository.Insert(new EntityAnalysisModelAbstractionCalculation
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ResponseCodeVolumeRatio",
            Active = 1,
            AbstractionCalculationTypeId = 5,
            ResponsePayload = 1,
            FunctionScript = functionScript
        });
    }

    private void InsertEntityAnalysisModelActivationRule(int entityAnalysisModelId, int caseWorkflowId,
        int caseWorkflowStatusId, string userName)
    {
        var entityAnalysisModelActivationRuleRepository =
            new EntityAnalysisModelActivationRuleRepository(dbContext, userName);

        entityAnalysisModelActivationRuleRepository.Insert(new EntityAnalysisModelActivationRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IncrementTtlCounterAll",
            BuilderRuleScript = "Return False",
            CoderRuleScript = "Return True",
            RuleScriptTypeId = 2,
            ResponseElevation = 0,
            EnableCaseWorkflow = 0,
            CaseWorkflowId = 0,
            CaseKey = "",
            Active = 1,
            EnableTtlCounter = 1,
            EntityAnalysisModelTtlCounterId = 1,
            EntityAnalysisModelIdTtlCounter = 1,
            ResponsePayload = 0,
            EnableNotification = 0,
            EnableResponseElevation = 0,
            ResponseElevationKey = "",
            EnableBypass = 0,
            Visible = 0,
            EnableReprocessing = 0,
            EnableSuppression = 0,
            ActivationSample = 1,
            ReviewStatusId = 4
        });

        entityAnalysisModelActivationRuleRepository.Insert(new EntityAnalysisModelActivationRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ThresholdTtlCounterAll",
            BuilderRuleScript = "If (TTLCounter.TtlCounterAll > 5) Then " + Environment.NewLine +
                                "  Return True" + Environment.NewLine +
                                "End If",
            CoderRuleScript = "Return False",
            RuleScriptTypeId = 1,
            Json = "{\"not\": false, \"rules\": [{\"id\": \"TTLCounter.TtlCounterAll\", " +
                   "\"type\": \"double\", \"field\": \"TTLCounter.TtlCounterAll\", " +
                   "\"input\": \"number\", \"value\": 5, \"operator\": \"greater\"}], " +
                   "\"valid\": true, \"condition\": \"AND\"}",
            ResponseElevation = 1,
            Active = 1,
            EnableCaseWorkflow = 1,
            CaseWorkflowId = caseWorkflowId,
            CaseKey = "AccountId",
            EnableTtlCounter = 0,
            EntityAnalysisModelTtlCounterId = 1,
            EntityAnalysisModelIdTtlCounter = 1,
            ResponsePayload = 1,
            EnableNotification = 0,
            EnableResponseElevation = 1,
            ResponseElevationKey = "AccountId",
            EnableBypass = 0,
            Visible = 1,
            EnableReprocessing = 1,
            EnableSuppression = 1,
            ActivationSample = 1,
            ReviewStatusId = 4,
            ResponseElevationContent = "Declined for \"ThresholdTtlCounterAll\"",
            ResponseElevationForeColor = "#fb0707",
            ResponseElevationBackColor = "#f5f2cb",
            SendToActivationWatcher = 1,
            CaseWorkflowStatusId = caseWorkflowStatusId,
            ResponseElevationRedirect = "https://www.jube.io"
        });

        entityAnalysisModelActivationRuleRepository.Insert(new EntityAnalysisModelActivationRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ThresholdSanctionsDistance",
            BuilderRuleScript = "If (Sanction.FuzzyMatchDistance2JoinedName < 1) Then " + Environment.NewLine +
                                "  Return True" + Environment.NewLine +
                                "End If",
            CoderRuleScript = "Return False",
            RuleScriptTypeId = 1,
            Json = "{\"not\": false, \"rules\": [{\"id\": \"Sanction.FuzzyMatchDistance2JoinedName\", " +
                   "\"type\": \"double\", \"field\": \"sanction.FuzzyMatchDistance2JoinedName\", " +
                   "\"input\": \"number\", \"value\": 1, \"operator\": \"less\"}], " +
                   "\"valid\": true, \"condition\": \"AND\"}",
            ResponseElevation = 2,
            EnableCaseWorkflow = 1,
            CaseWorkflowId = caseWorkflowId,
            CaseKey = "AccountId",
            EnableTtlCounter = 0,
            EntityAnalysisModelTtlCounterId = 1,
            EntityAnalysisModelIdTtlCounter = 1,
            ResponsePayload = 1,
            EnableNotification = 0,
            EnableResponseElevation = 1,
            ResponseElevationKey = "AccountId",
            EnableBypass = 0,
            Visible = 1,
            EnableReprocessing = 1,
            EnableSuppression = 1,
            ActivationSample = 1,
            Active = 1,
            ReviewStatusId = 4,
            ResponseElevationContent = "Declined for \"ThresholdSanctionsDistance\"",
            ResponseElevationForeColor = "#fb0707",
            ResponseElevationBackColor = "#f5f2cb",
            SendToActivationWatcher = 1,
            CaseWorkflowStatusId = caseWorkflowStatusId,
            ResponseElevationRedirect = "https://www.jube.io"
        });

        entityAnalysisModelActivationRuleRepository.Insert(new EntityAnalysisModelActivationRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AllIPDenyList",
            BuilderRuleScript = "If (( List.IPDenyList.contains(Payload.IP))) Then" + Environment.NewLine +
                                "  Return True" + Environment.NewLine +
                                "End If",
            CoderRuleScript = "Return False",
            RuleScriptTypeId = 1,
            Json = "{\"rules\": [{\"id\": \"List.IPDenyList\", \"type\": \"string\", \"field\": " +
                   "\"List.IPDenyList\", \"input\": \"select\", \"value\": \"Payload.IP\", \"operator\": \"has\"}], " +
                   "\"valid\": true, \"condition\": \"AND\"}",
            ResponseElevation = 2,
            EnableCaseWorkflow = 1,
            CaseWorkflowId = caseWorkflowId,
            CaseKey = "AccountId",
            Active = 1,
            EnableTtlCounter = 0,
            EntityAnalysisModelTtlCounterId = 1,
            EntityAnalysisModelIdTtlCounter = 1,
            ResponsePayload = 1,
            EnableNotification = 0,
            EnableResponseElevation = 1,
            ResponseElevationKey = "AccountId",
            EnableBypass = 0,
            Visible = 1,
            EnableReprocessing = 1,
            EnableSuppression = 1,
            ActivationSample = 1,
            ReviewStatusId = 4,
            ResponseElevationContent = "Declined for \"AllIPDenyList\"",
            ResponseElevationForeColor = "#fb0707",
            ResponseElevationBackColor = "#f5f2cb",
            SendToActivationWatcher = 1,
            CaseWorkflowStatusId = caseWorkflowStatusId,
            ResponseElevationRedirect = "https://www.jube.io"
        });

        entityAnalysisModelActivationRuleRepository.Insert(new EntityAnalysisModelActivationRule
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "VolumeThresholdByAccountId",
            BuilderRuleScript = "Return False",
            CoderRuleScript =
                "If (Abstraction.ResponseCodeEqual0Volume > Dictionary.VolumeThresholdByAccountId) Then" +
                Environment.NewLine +
                "   Return True" + Environment.NewLine +
                "End If",
            RuleScriptTypeId = 2,
            ResponseElevation = 2,
            EnableCaseWorkflow = 1,
            CaseWorkflowId = caseWorkflowId,
            CaseKey = "AccountId",
            EnableTtlCounter = 0,
            EntityAnalysisModelTtlCounterId = 1,
            EntityAnalysisModelIdTtlCounter = 1,
            ResponsePayload = 1,
            EnableNotification = 0,
            EnableResponseElevation = 1,
            ResponseElevationKey = "AccountId",
            EnableBypass = 0,
            Visible = 1,
            EnableReprocessing = 1,
            EnableSuppression = 1,
            ActivationSample = 1,
            Active = 1,
            ReviewStatusId = 4,
            ResponseElevationContent = "Declined for \"VolumeThresholdByAccountId\"",
            ResponseElevationForeColor = "#fb0707",
            ResponseElevationBackColor = "#f5f2cb",
            SendToActivationWatcher = 1,
            CaseWorkflowStatusId = caseWorkflowStatusId,
            ResponseElevationRedirect = "https://www.jube.io"
        });
    }

    private void InsertEntityAnalysisModelRequestXPaths(int entityAnalysisModelId, string userName)
    {
        var entityAnalysisModelRequestXPathRepository =
            new EntityAnalysisModelRequestXPathRepository(dbContext, userName);

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AccountId",
            XPath = "$.AccountId",
            DataTypeId = 1,
            EnableSuppression = 1,
            ResponsePayload = 1,
            SearchKey = 1,
            Active = 1,
            DefaultValue = "Test1"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "TxnDateTime",
            XPath = "$.TxnDateTime",
            DataTypeId = 4,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "2022-08-19T21:41:37.247"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Currency",
            XPath = "$.Currency",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "826"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ResponseCode",
            XPath = "$.ResponseCode",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "CurrencyAmount",
            XPath = "$.CurrencyAmount",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "123.45"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "SettlementAmount",
            XPath = "$.SettlementAmount",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "123.45"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AccountCurrency",
            XPath = "$.AccountCurrency",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "566"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ChannelId",
            XPath = "$.ChannelId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AppVersionCode",
            XPath = "$.AppVersionCode",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "12.34"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ServiceCode",
            XPath = "$.ServiceCode",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "DID"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "System",
            XPath = "$.System",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Android"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Brand",
            XPath = "$.Brand",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "ZTE"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Model",
            XPath = "$.Model",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Barby"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AccountLongitude",
            XPath = "$.AccountLongitude",
            DataTypeId = 6,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "36.1408"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AccountLatitude",
            XPath = "$.AccountLatitude",
            DataTypeId = 7,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "5.3536"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "OS",
            XPath = "$.OS",
            DataTypeId = 7,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Lollypop"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Resolution",
            XPath = "$.Resolution",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "720*1280"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "DebuggerAttached",
            XPath = "$.DebuggerAttached",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "true"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "SimulatorAttached",
            XPath = "$.SimulatorAttached",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "true"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Jailbreak",
            XPath = "$.Jailbreak",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "false"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "MAC",
            XPath = "$.MAC",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "94:23:44f:2:d3"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ToAccountId",
            XPath = "$.ToAccountId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Test2"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ToAccountExternalRef",
            XPath = "$.ToAccountExternalRef",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "ChurchmanR"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "TwoFATypeId",
            XPath = "$.TwoFATypeId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "SMS"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "TwoFAResponseId",
            XPath = "$.TwoFAResponseId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Storage",
            XPath = "$.Storage",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "true"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "TransactionExternalResponseId",
            XPath = "$.TransactionExternalResponseId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "0"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "FingerprintHash",
            XPath = "$.FingerprintHash",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "jhjkhjkhsjh2hjhjkhj2k"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BusinessModel",
            XPath = "$.BusinessModel",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Travel"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AmountEUR",
            XPath = "$.AmountEUR",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "100.00"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AmountUSD",
            XPath = "$.AmountUSD",
            DataTypeId = 3,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "113.05"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AmountUSDRate",
            XPath = "$.AmountUSDRate",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1.1305502954"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AmountGBP",
            XPath = "$.AmountGBP",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "86.5866"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AmountGBPRate",
            XPath = "$.AmountGBPRate",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "0.8658658602"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Is3D",
            XPath = "$.Is3D",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "true"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "OriginalAmount",
            XPath = "$.OriginalAmount",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "100"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "OriginalCurrency",
            XPath = "$.OriginalCurrency",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "EUR"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "Email",
            XPath = "$.Email",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "please@hash.me"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "CreditCardHash",
            XPath = "$.CreditCardHash",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1FDA39A3EE5E6B4HKAJAA890AFD80709"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AcquirerBankName",
            XPath = "$.AcquirerBankName",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Caixa"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "ActionDate",
            XPath = "$.ActionDate",
            DataTypeId = 4,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "2022-08-19T21:41:37.247"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "APMAccountId",
            XPath = "$.APMAccountId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Skrill123456789"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BankId",
            XPath = "$.BankId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "57"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingAddress",
            XPath = "$.BillingAddress",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Address Line 1"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingCity",
            XPath = "$.BillingCity",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Address Line 2"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingCountry",
            XPath = "$.BillingCountry",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "DE"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingFirstName",
            XPath = "$.BillingFirstName",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Richard"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingLastName",
            XPath = "$.BillingLastName",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "Churchman"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingPhone",
            XPath = "$.BillingPhone",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1234567890"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingState",
            XPath = "$.BillingState",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "DE"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "BillingZip",
            XPath = "$.BillingZip",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "123456"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IsAPM",
            XPath = "$.IsAPM",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "true"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IsCascaded",
            XPath = "$.IsCascaded",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "false"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IsCredited",
            XPath = "$.IsCredited",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "false"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IsCurrencyConverted",
            XPath = "$.IsCurrencyConverted",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "true"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IsModification",
            XPath = "$.IsModification",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "false"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IsRebill",
            XPath = "$.IsRebill",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "true"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "TransactionTypeId",
            XPath = "$.TransactionTypeId",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1000"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "TransactionResultId",
            XPath = "$.TransactionResultId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "2000"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IP",
            XPath = "$.IP",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "123.456.789.200"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "AmountEURRate",
            XPath = "$.AmountEURRate",
            DataTypeId = 3,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "1"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "DeviceId",
            XPath = "$.DeviceId",
            DataTypeId = 1,
            EnableSuppression = 1,
            ResponsePayload = 1,
            SearchKey = 1,
            Active = 1,
            DefaultValue = "OlaRoseGoldPhone6"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "IsModified",
            XPath = "$.IsModified",
            DataTypeId = 5,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "false"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "OrderId",
            XPath = "$.OrderId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "10607324128"
        });

        entityAnalysisModelRequestXPathRepository.Insert(new EntityAnalysisModelRequestXpath
        {
            EntityAnalysisModelId = entityAnalysisModelId,
            Name = "TxnId",
            XPath = "$.TxnId",
            DataTypeId = 1,
            EnableSuppression = 0,
            ResponsePayload = 1,
            SearchKey = 0,
            Active = 1,
            DefaultValue = "0987654321"
        });
    }

    private int InsertEntityAnalysisModel(string userName)
    {
        var entityAnalysisModelRepository = new EntityAnalysisModelRepository(dbContext, userName);
        return entityAnalysisModelRepository.Insert(new EntityAnalysisModel
            {
                Name = "Detailed Account Financial Transactions",
                Guid = Guid.NewGuid(),
                EntryXPath = "$.TxnId",
                EntryName = "TxnId",
                ReferenceDateXPath = "$.TxnDateTime",
                ReferenceDatePayloadLocationTypeId = 1,
                Active = 1,
                ReferenceDateName = "TxnDateTime",
                CacheFetchLimit = 100,
                EnableCache = 1,
                EnableRdbmsArchive = 1,
                EnableTtlCounter = 1,
                EnableSanctionCache = 1,
                EnableActivationArchive = 1,
                EnableActivationWatcher = 1,
                MaxActivationWatcherValue = 1,
                MaxActivationWatcherInterval = 'h',
                MaxActivationWatcherThreshold = 100,
                EnableResponseElevationLimit = 0,
                MaxResponseElevation = 10,
                ActivationWatcherSample = 1
            }
        ).Id;
    }

    private void AssignToUserRegistryToTenantRegistry(string userName, TenantRegistry tenantRegistry)
    {
        var userInTenantRepository = new UserInTenantRepository(dbContext, "Administrator");
        userInTenantRepository.Update(userName, tenantRegistry.Id);
    }

    private void AllocateAllPermissionSpecificationsToRoleRegistry(string userName, RoleRegistry roleRegistry)
    {
        var permissionSpecificationRepository = new PermissionSpecificationRepository(dbContext);
        var permissionSpecifications = permissionSpecificationRepository.Get();

        var roleRegistryPermissionRepository = new RoleRegistryPermissionRepository(dbContext, userName);

        foreach (var roleRegistryPermission in permissionSpecifications.ToList().Select(permissionSpecification =>
                     new RoleRegistryPermission
                     {
                         RoleRegistryId = roleRegistry.Id,
                         Active = 1,
                         PermissionSpecificationId = permissionSpecification.Id
                     }))
        {
            roleRegistryPermissionRepository.Insert(roleRegistryPermission);
        }
    }

    private UserRegistry CreateUserRegistry(string userName, string password, string passwordHashingKey,
        RoleRegistry roleRegistry)
    {
        var userRegistry = new UserRegistry
        {
            Name = userName,
            Email = userName,
            Active = 1,
            RoleRegistryId = roleRegistry.Id
        };

        var userRegistryRepository = new UserRegistryRepository(dbContext, roleRegistry);
        userRegistryRepository.Insert(userRegistry);
        userRegistryRepository.SetPassword(userRegistry.Id,
            HashPassword.GenerateHash(password, passwordHashingKey), DateTime.Now.AddDays(90));

        return userRegistry;
    }

    private RoleRegistry CreateRoleRegistry(string userName, TenantRegistry tenantRegistry)
    {
        var roleRegistry = new RoleRegistry
        {
            TenantRegistryId = tenantRegistry.Id,
            Name = userName,
            Active = 1
        };

        var roleRegistryRepository = new RoleRegistryRepository(dbContext, tenantRegistry);
        roleRegistry = roleRegistryRepository.Insert(roleRegistry);
        return roleRegistry;
    }

    private TenantRegistry CreateTenantRegistry(string userName)
    {
        var tenantRegistryRepository = new TenantRegistryRepository(dbContext, "Administrator");

        var tenantRegistry = new TenantRegistry
        {
            Active = 1,
            Name = userName,
            Landlord = 0
        };

        tenantRegistry = tenantRegistryRepository.Insert(tenantRegistry);
        return tenantRegistry;
    }

    private void ValidateUserDoesNotExistAndRaiseExceptionIfExists(string userName)
    {
        var userRegistryRepositoryExisting = new UserRegistryRepository(dbContext);
        var userRegistryExisting = userRegistryRepositoryExisting.GetByUserName(userName);

        if (userRegistryExisting != null) throw new ConflictException();
    }
}