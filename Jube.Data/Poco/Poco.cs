/* Copyright (C) 2022-present Jube Holdings Limited.
 *
 * This file is part of Jube™ software.
 *
 * Jube™ is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License 
 * as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Jube™ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty  
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License along with Jube™. If not, 
 * see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using LinqToDB;
using LinqToDB.Mapping;

namespace Jube.Data.Poco
{
    [Table]
    public class ActivationWatcher
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public string Key { get; set; }
        [Column] [Nullable] public string KeyValue { get; set; }
        [Column] [Nullable] public double? Longitude { get; set; }
        [Column] [Nullable] public double? Latitude { get; set; }
        [Column] [Nullable] public string ActivationRuleSummary { get; set; }
        [Column] [Nullable] public string ResponseElevationContent { get; set; }
        [Column] [Nullable] public double? ResponseElevation { get; set; }
        [Column] [Nullable] public string BackColor { get; set; }
        [Column] [Nullable] public string ForeColor { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.ManyToOne,
            BackReferenceName = "ActivationWatcherTenantRegistry")]
        public TenantRegistry TenantRegistry { get; set; }
    }

    [Table]
    public class ArchiveEntityAnalysisModelAbstractionEntry
    {
        [Column] [Identity] public long Id { get; set; }
        [Column] [Nullable] public string SearchKey { get; set; }
        [Column] [Nullable] public string SearchValue { get; set; }
        [Column] [Nullable] public double? Value { get; set; }
        [Column] [Nullable] public long? EntityAnalysisModelSearchKeyDistinctValueCalculationInstanceId { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelAbstractionRuleId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
    }

    [Table]
    public class ArchiveKey
    {
        [Column] [Identity] public long Id { get; set; }
        [Column] [Nullable] public byte? ProcessingTypeId { get; set; }
        [Column] [Nullable] public string Key { get; set; }
        [Column] [Nullable] public string KeyValueString { get; set; }
        [Column] [Nullable] public int? KeyValueInteger { get; set; }
        [Column] [Nullable] public double? KeyValueFloat { get; set; }
        [Column] [Nullable] public byte? KeyValueBoolean { get; set; }
        [Column] [Nullable] public DateTime? KeyValueDate { get; set; }
        [Column] [Nullable] public Guid EntityAnalysisModelInstanceEntryGuid { get; set; }
    }
    
    [Table]
    public class Case
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public Guid EntityAnalysisModelInstanceEntryGuid { get; set; }
        [Column] [Nullable] public DateTime? DiaryDate { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public int CaseWorkflowStatusId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public string LockedUser { get; set; }
        [Column] [Nullable] public DateTime? LockedDate { get; set; }
        [Column] [Nullable] public byte? ClosedStatusId { get; set; }
        [Column] [Nullable] public DateTime? ClosedDate { get; set; }
        [Column] [Nullable] public string ClosedUser { get; set; }
        [Column] [Nullable] public string CaseKey { get; set; }
        [Column] [Nullable] public byte? Diary { get; set; }
        [Column] [Nullable] public string DiaryUser { get; set; }
        [Column] [Nullable] public byte? Rating { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public string CaseKeyValue { get; set; }
        [Column] [Nullable] public byte? LastClosedStatus { get; set; }
        [Column] [Nullable] public DateTime? ClosedStatusMigrationDate { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseEvent> ReprocessingIntervalType { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseNote> Notes { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowFormEntry> WorkflowsFormsEntry { get; set; }
    }

    [Table]
    public class SessionCaseJournal
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class CaseEvent
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseEventTypeId { get; set; }
        [Column] [Nullable] public string Before { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CaseKey { get; set; }
        [Column] [Nullable] public int? CaseId { get; set; }
        [Column] [Nullable] public string After { get; set; }
        [Column] [Nullable] public string CaseKeyValue { get; set; }

        [Association(ThisKey = "CaseId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public Case Case { get; set; }
    }

    [Table]
    public class CaseExecutionLog
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Command { get; set; }
        [Column] [Nullable] public byte? InError { get; set; }
        [Column] [Nullable] public string ErrorStack { get; set; }
        [Column] [Nullable] public int? RowCount { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? ResponseMs { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
    }

    [Table]
    public class CaseFile
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public byte[] Object { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string ContentType { get; set; }
        [Column] [Nullable] public string Extension { get; set; }
        [Column] [Nullable] public long Size { get; set; }
        [Column] [Nullable] public string CaseKey { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public string CaseKeyValue { get; set; }
        [Column] [Nullable] public int? CaseId { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }

        [Association(ThisKey = "CaseId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public Case Case { get; set; }
    }

    [Table]
    public class CaseNote
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Note { get; set; }
        [Column] [Nullable] public int? ActionId { get; set; }
        [Column] [Nullable] public int? PriorityId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CaseKey { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public string CaseKeyValue { get; set; }
        [Column] [Nullable] public int? CaseId { get; set; }

        [Association(ThisKey = "CaseId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public Case Case { get; set; }
    }

    [Table]
    public class SessionCaseSearchCompiledSql
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public Guid Guid { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string FilterJson { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string FilterTokens { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string SelectJson { get; set; }

        [Column] [Nullable] public string FilterSql { get; set; }
        [Column] [Nullable] public string SelectSqlSearch { get; set; }
        [Column] [Nullable] public string SelectSqlDisplay { get; set; }
        [Column] [Nullable] public string WhereSql { get; set; }
        [Column] [Nullable] public string OrderSql { get; set; }
        [Column] [Nullable] public byte? Prepared { get; set; }
        [Column] [Nullable] public string Error { get; set; }
        [Column] [Nullable] public int CaseWorkflowId { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
    }

    public class SessionCaseSearchCompiledSqlExecution
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int SessionCaseSearchCompiledSqlId { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int ResponseTime { get; set; }
        [Column] [Nullable] public int Records { get; set; }
    }

    [Table]
    public class CaseWorkflow
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public int? CaseStatusId { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryId { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public byte? EnableVisualisation { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<SessionCaseJournal> SessionCaseJournalCaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowAction> CaseWorkflowsActionsCaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowDisplay> CaseWorkflowsDisplaysCaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowFilter> CaseWorkflowsFiltersCaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowForm> CaseWorkflowsFormsCaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowMacro> CaseWorkflowsMacrosCaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowStatus> CaseWorkflowsStatusCaseWorkflows { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowXPath> CaseWorkflowsXPathCaseWorkflows { get; set; }

        [Association(ThisKey = "TenantRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public TenantRegistry TenantRegistry { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }
    
    [Table]
    public class CaseWorkflowVersion
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }        
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public int? CaseStatusId { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryId { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public byte? EnableVisualisation { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
    }

    [Table]
    public class CaseWorkflowAction
    {
        [Column] [Identity] public int Id { get; set; }
        [Column("CaseWorkflowId")] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public byte? EnableHttpEndpoint { get; set; }
        [Column] [Nullable] public string HttpEndpoint { get; set; }
        [Column] [Nullable] public byte? HttpEndpointTypeId { get; set; }
        [Column] [Nullable] public byte? EnableNotification { get; set; }
        [Column] [Nullable] public byte? NotificationTypeId { get; set; }
        [Column] [Nullable] public string NotificationDestination { get; set; }
        [Column] [Nullable] public string NotificationSubject { get; set; }
        [Column] [Nullable] public string NotificationBody { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class CaseWorkflowDisplay
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string Html { get; set; }
        
        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class CaseWorkflowFilter
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string FilterJson { get; set; }

        [Column] [Nullable] public string FilterSql { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string SelectJson { get; set; }

        [Column] [Nullable] public string FilterTokens { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryId { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class CaseWorkflowForm
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string Html { get; set; }
        [Column] [Nullable] public byte? EnableHttpEndpoint { get; set; }
        [Column] [Nullable] public string HttpEndpoint { get; set; }
        [Column] [Nullable] public byte? HttpEndpointTypeId { get; set; }
        [Column] [Nullable] public byte? EnableNotification { get; set; }
        [Column] [Nullable] public byte? NotificationTypeId { get; set; }
        [Column] [Nullable] public string NotificationDestination { get; set; }
        [Column] [Nullable] public string NotificationSubject { get; set; }
        [Column] [Nullable] public string NotificationBody { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class CaseWorkflowFormEntryValue
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string Value { get; set; }
        [Column] [Nullable] public int? CaseWorkflowFormEntryId { get; set; }

        [Association(ThisKey = "CaseWorkflowFormEntryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflowFormEntry CaseWorkflowsFormsEntry { get; set; }
    }

    [Table]
    public class CaseWorkflowFormEntry
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Payload { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public string CaseKey { get; set; }
        [Column] [Nullable] public int? CaseId { get; set; }
        [Column] [Nullable] public int? CaseWorkflowFormId { get; set; }
        [Column] [Nullable] public string CaseKeyValue { get; set; }

        [Association(ThisKey = "CaseId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public Case Case { get; set; }

        [Association(ThisKey = "Id", OtherKey = "CaseWorkflowFormEntryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflowFormEntryValue> CaseWorkflowsFormsEntryValuesCaseWorkflowsFormsEntry
        {
            get;
            set;
        }
    }

    [Table]
    public class CaseWorkflowMacro
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string Javascript { get; set; }
        [Column] [Nullable] public string ImageLocation { get; set; }
        [Column] [Nullable] public byte? EnableHttpEndpoint { get; set; }
        [Column] [Nullable] public string HttpEndpoint { get; set; }
        [Column] [Nullable] public byte? HttpEndpointTypeId { get; set; }
        [Column] [Nullable] public byte? EnableNotification { get; set; }
        [Column] [Nullable] public byte? NotificationTypeId { get; set; }
        [Column] [Nullable] public string NotificationDestination { get; set; }
        [Column] [Nullable] public string NotificationSubject { get; set; }
        [Column] [Nullable] public string NotificationBody { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class CaseWorkflowStatus
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string ForeColor { get; set; }
        [Column] [Nullable] public string BackColor { get; set; }
        [Column] [Nullable] public byte? EnableHttpEndpoint { get; set; }
        [Column] [Nullable] public string HttpEndpoint { get; set; }
        [Column] [Nullable] public byte? HttpEndpointTypeId { get; set; }
        [Column] [Nullable] public byte? EnableNotification { get; set; }
        [Column] [Nullable] public byte? NotificationTypeId { get; set; }
        [Column] [Nullable] public string NotificationDestination { get; set; }
        [Column] [Nullable] public string NotificationSubject { get; set; }
        [Column] [Nullable] public string NotificationBody { get; set; }
        [Column] [Nullable] public byte? Priority { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class CaseWorkflowStatusVersion
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseWorkflowStatusId { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string ForeColor { get; set; }
        [Column] [Nullable] public string BackColor { get; set; }
        [Column] [Nullable] public byte? EnableHttpEndpoint { get; set; }
        [Column] [Nullable] public string HttpEndpoint { get; set; }
        [Column] [Nullable] public byte? HttpEndpointTypeId { get; set; }
        [Column] [Nullable] public byte? EnableNotification { get; set; }
        [Column] [Nullable] public byte? NotificationTypeId { get; set; }
        [Column] [Nullable] public string NotificationDestination { get; set; }
        [Column] [Nullable] public string NotificationSubject { get; set; }
        [Column] [Nullable] public string NotificationBody { get; set; }
        [Column] [Nullable] public byte? Priority { get; set; }
    }

    [Table]
    public class CaseWorkflowXPath
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string XPath { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Column] [Nullable] public byte? ConditionalRegularExpressionFormatting { get; set; }

        [Column] [Nullable] public string ConditionalFormatForeColor { get; set; }

        [Column] [Nullable] public string ConditionalFormatBackColor { get; set; }

        [Column] [Nullable] public string RegularExpression { get; set; }

        [Column] [Nullable] public byte? ForeRowColorScope { get; set; }

        [Column] [Nullable] public byte? BackRowColorScope { get; set; }

        [Column] [Nullable] public byte? Drill { get; set; }
        [Column] [Nullable] public string BoldLineFormatForeColor { get; set; }
        [Column] [Nullable] public string BoldLineFormatBackColor { get; set; }
        [Column] [Nullable] public byte? BoldLineMatched { get; set; }

        [Association(ThisKey = "CaseWorkflowId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflow CaseWorkflow { get; set; }
    }

    [Table]
    public class Country
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string Alpha2 { get; set; }
        [Column] [Nullable] public string Alpha3 { get; set; }
        [Column] [Nullable] public int? Numeric { get; set; }
        [Column] [Nullable] public double? Latitude { get; set; }
        [Column] [Nullable] public double? Longitude { get; set; }
    }

    [Table]
    public class EntityAnalysisModelListCsvFileUpload
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelListId { get; set; }
        [Column] [Nullable] public string FileName { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column("CreatedUser")] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int Records { get; set; }
        [Column] [Nullable] public int Errors { get; set; }
        [Column] [Nullable] public long Length { get; set; }
        [Column] [Nullable] public int Version { get; set; }
        [Column] [Nullable] public int InheritedId { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }

        [Association(ThisKey = "EntityAnalysisModelListId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModelList EntityAnalysisModelList { get; set; }
    }

    [Table]
    public class EntityAnalysisModelDictionaryCsvFileUpload
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelDictionaryId { get; set; }
        [Column] [Nullable] public string FileName { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int Records { get; set; }
        [Column] [Nullable] public int Errors { get; set; }
        [Column] [Nullable] public long Length { get; set; }
        [Column] [Nullable] public int Version { get; set; }
        [Column] [Nullable] public int InheritedId { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }

        [Association(ThisKey = "EntityAnalysisModelDictionaryId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModelDictionary EntityAnalysisModelDictionary { get; set; }
    }

    [Table]
    public class Currency
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string Symbol { get; set; }
        [Column] [Nullable] public double? ExchangeRateToBaseCurrency { get; set; }
    }

    [Table]
    public class EntityAnalysisInlineScript
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Code { get; set; }
        [Column] [Nullable] public string Dependency { get; set; }
        [Column] [Nullable] public string ClassName { get; set; }
        [Column] [Nullable] public string MethodName { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisInlineScriptId",
            CanBeNull = true, Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelInlineScript>
            EntityAnalysisModelInlineScriptsEntityAnalysisInlineScripts { get; set; }
    }

    [Table]
    public class EntityAnalysisInstance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public Guid Guid { get; set; }
        [Column] [Nullable] public string Instance { get; set; }
    }


    [Table]
    public class EntityAnalysisModelVersion
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public Guid Guid { get; set; }
        [Column] [Nullable] public string EntryXPath { get; set; }
        [Column] [Nullable] public string ReferenceDateXPath { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string EntryName { get; set; }
        [Column] [Nullable] public string ReferenceDateName { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public int? CacheFetchLimit { get; set; }
        [Column] [Nullable] public byte? ReferenceDatePayloadLocationTypeId { get; set; }
        [Column] [Nullable] public double? MaxResponseElevation { get; set; }
        [Column] [Nullable] public char? MaxResponseElevationInterval { get; set; }
        [Column] [Nullable] public int? MaxResponseElevationValue { get; set; }
        [Column] [Nullable] public int? MaxResponseElevationThreshold { get; set; }
        [Column] [Nullable] public char? MaxActivationWatcherInterval { get; set; }
        [Column] [Nullable] public int? MaxActivationWatcherValue { get; set; }
        [Column] [Nullable] public int? MaxActivationWatcherThreshold { get; set; }
        [Column] [Nullable] public double? ActivationWatcherSample { get; set; }
        [Column] [Nullable] public byte? EnableCache { get; set; }
        [Column] [Nullable] public byte? EnableTtlCounter { get; set; }
        [Column] [Nullable] public byte? EnableSanctionCache { get; set; }
        [Column] [Nullable] public byte? EnableRdbmsArchive { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
    }

    [Table]
    public class EntityAnalysisModel
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public Guid Guid { get; set; }
        [Column] [Nullable] public string EntryXPath { get; set; }
        [Column] [Nullable] public string ReferenceDateXPath { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string EntryName { get; set; }
        [Column] [Nullable] public string ReferenceDateName { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public int? CacheFetchLimit { get; set; }
        [Column] [Nullable] public byte? ReferenceDatePayloadLocationTypeId { get; set; }
        [Column] [Nullable] public double? MaxResponseElevation { get; set; }
        [Column] [Nullable] public char? MaxResponseElevationInterval { get; set; }
        [Column] [Nullable] public int? MaxResponseElevationValue { get; set; }
        [Column] [Nullable] public int? MaxResponseElevationThreshold { get; set; }
        [Column] [Nullable] public char? MaxActivationWatcherInterval { get; set; }
        [Column] [Nullable] public int? MaxActivationWatcherValue { get; set; }
        [Column] [Nullable] public int? MaxActivationWatcherThreshold { get; set; }
        [Column] [Nullable] public double? ActivationWatcherSample { get; set; }
        [Column] [Nullable] public byte? EnableActivationArchive { get; set; }
        [Column] [Nullable] public byte? EnableCache { get; set; }
        [Column] [Nullable] public byte? EnableTtlCounter { get; set; }
        [Column] [Nullable] public byte? EnableSanctionCache { get; set; }
        [Column] [Nullable] public byte? EnableRdbmsArchive { get; set; }
        [Column] [Nullable] public byte? EnableActivationWatcher { get; set; }
        [Column] [Nullable] public byte? EnableResponseElevationLimit { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelVersion> EntityAnalysisModelVersion { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelActivationRuleSuppression>
            EntityAnalysisModelActivationRuleSuppressionsEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelAbstractionCalculation>
            EntityAnalysisModelAbstractionCalculationsEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelAbstractionRule> EntityAnalysisModelAbstractionRulesEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelActivationRule> EntityAnalysisModelActivationRulesEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelHttpAdaptation> EntityAnalysisModelAdaptationsEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelAsynchronousQueueBalance>
            EntityAnalysisModelAsynchronousQueueBalancesEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelDictionary> EntityAnalysisModelDictionaryEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelGatewayRule> EntityAnalysisModelGatewayRulesEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelInlineFunction> EntityAnalysisModelInlineFunctionsEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelInlineScript> EntityAnalysisModelInlineScriptsEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelList> EntityAnalysisModelListsEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelProcessingCounter>
            EntityAnalysisModelProcessingCountersEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelReprocessingRule>
            EntityAnalysisModelReprocessingRulesEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelRequestXpath> EntityAnalysisModelRequestXpathEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelSanction> EntityAnalysisModelSanctionsEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelSearchKeyCalculationInstance>
            EntityAnalysisModelSearchKeyCalculationInstancesEntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelTtlCounter> EntityAnalysisModelTtlCountersEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelSuppression> EntityAnalysisModelSuppressionsEntityAnalysisModel
        {
            get;
            set;
        }

        [Association(ThisKey = "TenantRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public TenantRegistry TenantRegistry { get; set; }
    }

    [Table]
    public class EntityAnalysisModelActivationRuleSuppression
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string SuppressionKey { get; set; }
        [Column] [Nullable] public string SuppressionKeyValue { get; set; }
        [Column] [Nullable] public string EntityAnalysisModelActivationRuleName { get; set; }
        [Column] [Nullable] public int InheritedId { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelInstance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public Guid EntityAnalysisInstanceGuid { get; set; }
        [Column] [Nullable] public Guid EntityAnalysisModelInstanceGuid { get; set; }
    }

    [Table]
    public class EntityAnalysisModelAbstractionCalculation
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string EntityAnalysisModelAbstractionNameLeft { get; set; }
        [Column] [Nullable] public string EntityAnalysisModelAbstractionNameRight { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public int? AbstractionCalculationTypeId { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public string FunctionScript { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelAbstractionRule
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string BuilderRuleScript { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string SearchKey { get; set; }
        [Column] [Nullable] public int? SearchFunctionTypeId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string SearchInterval { get; set; }
        [Column] [Nullable] public string SearchFunctionKey { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public int SearchValue { get; set; }
        [Column] [Nullable] public byte? Search { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public byte? Offset { get; set; }
        [Column] [Nullable] public byte? OffsetTypeId { get; set; }
        [Column] [Nullable] public int? OffsetValue { get; set; }
        [Column] [Nullable] public string CoderRuleScript { get; set; }
        [Column] [Nullable] public byte? RuleScriptTypeId { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelActivationRule
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string BuilderRuleScript { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public double? ResponseElevation { get; set; }
        [Column] [Nullable] public int? CaseWorkflowId { get; set; }
        [Column] [Nullable] public int? EnableCaseWorkflow { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelTtlCounterId { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelIdTtlCounter { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? EnableTtlCounter { get; set; }
        [Column] [Nullable] public string ResponseElevationContent { get; set; }
        [Column] [Nullable] public byte? SendToActivationWatcher { get; set; }
        [Column] [Nullable] public string ResponseElevationForeColor { get; set; }
        [Column] [Nullable] public string ResponseElevationBackColor { get; set; }
        [Column] [Nullable] public int? CaseWorkflowStatusId { get; set; }
        [Column] [Nullable] public double? ActivationSample { get; set; }
        [Column] [Nullable] public long? ActivationCounter { get; set; }
        [Column] [Nullable] public DateTime? ActivationCounterDate { get; set; }
        [Column] [Nullable] public string ResponseElevationRedirect { get; set; }
        [Column] [Nullable] public byte? ReviewStatusId { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public byte? EnableNotification { get; set; }
        [Column] [Nullable] public byte? NotificationTypeId { get; set; }
        [Column] [Nullable] public string NotificationDestination { get; set; }
        [Column] [Nullable] public string NotificationSubject { get; set; }
        [Column] [Nullable] public string NotificationBody { get; set; }
        [Column] [Nullable] public string CoderRuleScript { get; set; }
        [Column] [Nullable] public byte? RuleScriptTypeId { get; set; }
        [Column] [Nullable] public byte? EnableResponseElevation { get; set; }
        [Column] [Nullable] public string CaseKey { get; set; }
        [Column] [Nullable] public string ResponseElevationKey { get; set; }
        [Column] [Nullable] public byte? EnableBypass { get; set; }
        [Column] [Nullable] public char? BypassSuspendInterval { get; set; }
        [Column] [Nullable] public int? BypassSuspendValue { get; set; }
        [Column] [Nullable] public double? BypassSuspendSample { get; set; }
        [Column] [Nullable] public byte? Visible { get; set; }
        [Column] [Nullable] public byte? EnableReprocessing { get; set; }
        [Column] [Nullable] public byte? EnableSuppression { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
        
        [Association(ThisKey = "CaseWorkflowStatusId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public CaseWorkflowStatus CaseWorkflowStatus { get; set; }
    }

    [Table]
    public class EntityAnalysisModelHttpAdaptation
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public string HttpEndpoint { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class UserRegistry
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int RoleRegistryId { get; set; }
        [Column] [Nullable] public string Email { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string Password { get; set; }
        [Column] [Nullable] public int FailedPasswordCount { get; set; }
        [Column] [Nullable] public DateTime? PasswordCreatedDate { get; set; }
        [Column] [Nullable] public DateTime? PasswordLockedDate { get; set; }
        [Column] [Nullable] public DateTime? PasswordExpiryDate { get; set; }
        [Column] [Nullable] public DateTime? LastLoginDate { get; set; }
        [Column] [Nullable] public byte? PasswordLocked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }

        [Association(ThisKey = "RoleRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public RoleRegistry RoleRegistry { get; set; }
    }

    [Table]
    public class EntityAnalysisAsynchronousQueueBalance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? AsynchronousInvoke { get; set; }
        [Column] [Nullable] public int? AsynchronousCallback { get; set; }
        [Column] [Nullable] public int? AsynchronousCallbackTimeout { get; set; }
        [Column] [Nullable] public string Instance { get; set; }
    }

    [Table]
    public class EntityAnalysisModelAsynchronousQueueBalance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? Archive { get; set; }
        [Column] [Nullable] public int? ActivationWatcher { get; set; }
        [Column] [Nullable] public string Instance { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelDictionary
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public string DataName { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelDictionaryId",
            CanBeNull = true, Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelDictionaryKvp>
            EntityAnalysisModelDictionaryKvpEntityAnalysisModelDictionary { get; set; }
    }

    [Table]
    public class EntityAnalysisModelDictionaryKvp
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelDictionaryId { get; set; }
        [Column] [Nullable] public string KvpKey { get; set; }
        [Column] [Nullable] public double? KvpValue { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column("CreatedUser")] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "EntityAnalysisModelDictionaryId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModelDictionary EntityAnalysisModelDictionary { get; set; }
    }

    [Table]
    public class EntityAnalysisModelGatewayRule
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public int? Priority { get; set; }
        [Column] [Nullable] public string BuilderRuleScript { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public double? MaxResponseElevation { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public int? ActivationCounter { get; set; }
        [Column] [Nullable] public DateTime? ActivationCounterDate { get; set; }
        [Column] [Nullable] public string CoderRuleScript { get; set; }
        [Column] [Nullable] public byte? RuleScriptTypeId { get; set; }
        [Column] [Nullable] public double? GatewaySample { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelInlineFunction
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string FunctionScript { get; set; }
        [Column] [Nullable] public int? ReturnDataTypeId { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelInlineScript
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public int? EntityAnalysisInlineScriptId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }

        [Association(ThisKey = "EntityAnalysisInlineScriptId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public EntityAnalysisInlineScript EntityAnalysisInlineScript { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelTag
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelList
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "EntityAnalysisModelListId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelListValue> EntityAnalysisModelListValuesEntityAnalysisModelLists
        {
            get;
            set;
        }
    }

    [Table]
    public class EntityAnalysisModelListValue
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelListId { get; set; }
        [Column] [Nullable] public string ListValue { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "EntityAnalysisModelListId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModelList EntityAnalysisModelList { get; set; }
    }

    [Table]
    public class EntityAnalysisModelProcessingCounter
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string Instance { get; set; }
        [Column] [Nullable] public int? ModelInvoke { get; set; }
        [Column] [Nullable] public int? GatewayMatch { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public int? ResponseElevation { get; set; }
        [Column] [Nullable] public double? ResponseElevationSum { get; set; }
        [Column] [Nullable] public double? ActivationWatcher { get; set; }
        [Column] [Nullable] public int? ResponseElevationValueLimit { get; set; }
        [Column] [Nullable] public int? ResponseElevationLimit { get; set; }
        [Column] [Nullable] public int? ResponseElevationValueGatewayLimit { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelReprocessingRule
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public int? Priority { get; set; }
        [Column] [Nullable] public string BuilderRuleScript { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public double? ReprocessingSample { get; set; }
        [Column] [Nullable] public string CoderRuleScript { get; set; }
        [Column] [Nullable] public byte? RuleScriptTypeId { get; set; }
        [Column] [Nullable] public int? ReprocessingValue { get; set; }
        [Column] [Nullable] public string ReprocessingInterval { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "EntityAnalysisModelReprocessingRuleId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelReprocessingRuleInstance>
            EntityAnalysisModelReprocessingRuleInstancesEntityAnalysisModelReprocessingRules { get; set; }
    }

    [Table]
    public class EntityAnalysisModelReprocessingRuleInstance
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelReprocessingRuleId { get; set; }
        [Column] [Nullable] public byte? StatusId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public DateTime? StartedDate { get; set; }
        [Column] [Nullable] public long? AvailableCount { get; set; }
        [Column] [Nullable] public long? SampledCount { get; set; }
        [Column] [Nullable] public long? MatchedCount { get; set; }
        [Column] [Nullable] public long? ProcessedCount { get; set; }
        [Column] [Nullable] public DateTime? CompletedDate { get; set; }
        [Column] [Nullable] public long? ErrorCount { get; set; }
        [Column] [Nullable] public DateTime? ReferenceDate { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int Version { get; set; }
        [Column] [Nullable] public int InheritedId { get; set; }

        [Association(ThisKey = "EntityAnalysisModelReprocessingRuleId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne,
            KeyName = "FKEntityAnalysisModelReprocessingRuleInstancesEntityAnalysisModelReprocessingRules")]
        public EntityAnalysisModelReprocessingRule EntityAnalysisModelReprocessingRule { get; set; }
    }

    [Table]
    public class EntityAnalysisModelRequestXpath
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public int? DataTypeId { get; set; }
        [Column] [Nullable] public string XPath { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? SearchKey { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public byte? SearchKeyCache { get; set; }
        [Column] [Nullable] public string SearchKeyCacheInterval { get; set; }
        [Column] [Nullable] public int? SearchKeyCacheValue { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public string SearchKeyCacheTtlInterval { get; set; }
        [Column] [Nullable] public int? SearchKeyCacheTtlValue { get; set; }
        [Column] [Nullable] public int? SearchKeyCacheFetchLimit { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public byte? SearchKeyCacheSample { get; set; }
        [Column] [Nullable] public byte? EnableSuppression { get; set; }
        [Column] [Nullable] public string DefaultValue { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelSanction
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public string MultipartStringDataName { get; set; }
        [Column] [Nullable] public byte? Distance { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public int? CacheValue { get; set; }
        [Column] [Nullable] public char? CacheInterval { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelSearchKeyCalculationInstance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string SearchKey { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public byte? Completed { get; set; }
        [Column] [Nullable] public DateTime? CompletedDate { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public int? DistinctValuesCount { get; set; }
        [Column] [Nullable] public DateTime? DistinctValuesUpdatedDate { get; set; }
        [Column] [Nullable] public int? DistinctValuesProcessedValuesCount { get; set; }
        [Column] [Nullable] public DateTime? DistinctValuesProcessedValuesUpdatedDate { get; set; }
        [Column] [Nullable] public DateTime? DistinctFetchToDate { get; set; }
        [Column] [Nullable] public DateTime? ExpiredSearchKeyCacheDate { get; set; }
        [Column] [Nullable] public int? ExpiredSearchKeyCacheCount { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "EntityAnalysisModelSearchKeyCalculationInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelSearchKeyDistinctValueCalculationInstance>
            EntityAnalysisModelSearchKeyDistinctValueCalculationEntityAnalysisModelSearchKeyCalculationInstances
        {
            get;
            set;
        }
    }

    [Table]
    public class EntityAnalysisModelSearchKeyDistinctValueCalculationInstance
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelSearchKeyCalculationInstanceId { get; set; }
        [Column] [Nullable] public string SearchKeyValue { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? EntryCount { get; set; }
        [Column] [Nullable] public DateTime? EntryCountUpdatedDate { get; set; }
        [Column] [Nullable] public DateTime? AbstractionRulesMatchesUpdatedDate { get; set; }
        [Column] [Nullable] public DateTime? CompletedDate { get; set; }

        [Association(ThisKey = "EntityAnalysisModelSearchKeyCalculationInstanceId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModelSearchKeyCalculationInstance EntityAnalysisModelSearchKeyCalculationInstance
        {
            get;
            set;
        }
    }

    [Table]
    public class EntityAnalysisModelSynchronisationNodeStatusEntry
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Instance { get; set; }
        [Column] [Nullable] public DateTime? HeartbeatDate { get; set; }
        [Column] [Nullable] public DateTime? SynchronisedDate { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }

        [Association(ThisKey = "TenantRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public TenantRegistry TenantRegistry { get; set; }
    }

    [Table]
    public class EntityAnalysisModelSynchronisationSchedule
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public DateTime? ScheduleDate { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }

        [Association(ThisKey = "TenantRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public TenantRegistry TenantRegistry { get; set; }
    }

    [Table]
    public class EntityAnalysisModelTtlCounter
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column("CreatedDate")] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string TtlCounterInterval { get; set; }
        [Column] [Nullable] public int? TtlCounterValue { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public string TtlCounterDataName { get; set; }
        [Column] [Nullable] public byte? OnlineAggregation { get; set; }
        [Column] [Nullable] public byte? EnableLiveForever { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class EntityAnalysisModelTtlCounterVersion
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelTtlCounterId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column("CreatedDate")] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string TtlCounterInterval { get; set; }
        [Column] [Nullable] public int? TtlCounterValue { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public string TtlCounterDataName { get; set; }
        [Column] [Nullable] public byte? OnlineAggregation { get; set; }
        [Column] [Nullable] public byte? EnableLiveForever { get; set; }
    }

    [Table]
    public class EntityAnalysisModelSuppression
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string SuppressionKeyValue { get; set; }
        [Column] [Nullable] public string SuppressionKey { get; set; }
        [Column] [Nullable] public int InheritedId { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? ReportTable { get; set; }
        [Column] [Nullable] public byte? ResponsePayload { get; set; }
        [Column] [Nullable] public int? ModelsSinceBest { get; set; }
        [Column] [Nullable] public Guid Guid { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? StatusId { get; set; }
        [Column] [Nullable] public int? Models { get; set; }
        [Column] [Nullable] public double? Score { get; set; }
        [Column] [Nullable] public int? TopologyComplexity { get; set; }
        [Column] [Nullable] public DateTime? CompletedDate { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public byte[] Object { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime DeletedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public byte? Anomaly { get; set; }
        [Column] [Nullable] public double AnomalyProbability { get; set; }
        [Column] [Nullable] public byte? Filter { get; set; }
        [Column] [Nullable] public string FilterSql { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string FilterJson { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string FilterTokens { get; set; }

        [Association(ThisKey = "EntityAnalysisModelId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public EntityAnalysisModel EntityAnalysisModel { get; set; }

        [Association(ThisKey = "Id", OtherKey = "ExhaustiveSearchInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceTrialInstance>
            ExhaustiveSearchInstanceTrialInstancesExhaustiveSearchInstances { get; set; }

        [Association(ThisKey = "Id", OtherKey = "ExhaustiveSearchInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceVariable> ExhaustiveSearchInstanceVariablesExhaustiveSearchInstances
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "ExhaustiveSearchInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceVariable>
            FkExhaustiveSearchInstanceVariablesExhaustiveSearchInstances1BackReferences { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstancePromotedTrialInstancePredictedActual
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public double? Predicted { get; set; }
        [Column] [Nullable] public double? Actual { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceId { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstance ExhaustiveSearchInstanceTrialInstance { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstancePromotedTrialInstanceRoc
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? TruePositive { get; set; }
        [Column] [Nullable] public int? TrueNegative { get; set; }
        [Column] [Nullable] public int? FalsePositive { get; set; }
        [Column] [Nullable] public int? FalseNegative { get; set; }
        [Column] [Nullable] public double? Score { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceId { get; set; }
        [Column] [Nullable] public double? Threshold { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstance ExhaustiveSearchInstanceTrialInstance { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstancePromotedTrialInstanceSensitivity
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public double? Sensitivity { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceVariableId { get; set; }
        [Column] [Nullable] public int? SensitivityRank { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceVariableId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstanceVariable ExhaustiveSearchInstanceTrialInstanceVariable { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstancePromotedTrialInstance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public double? Score { get; set; }
        [Column] [Nullable] public int? TopologyComplexity { get; set; }
        [Column] [Nullable] public int? TruePositive { get; set; }
        [Column] [Nullable] public int? TrueNegative { get; set; }
        [Column] [Nullable] public int? FalsePositive { get; set; }
        [Column] [Nullable] public int? FalseNegative { get; set; }
        [Column] [Nullable] public byte[] Object { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstance ExhaustiveSearchInstanceTrialInstance { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceTrialInstance
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstance ExhaustiveSearchInstance { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstancePromotedTrialInstancePredictedActual>
            ExhaustiveSearchInstancePromotedTrialInstancePredictedActualExhaustiveSearchInstanceTrialInstances
        {
            get;
            set;
        }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstancePromotedTrialInstanceRoc>
            ExhaustiveSearchInstancePromotedTrialInstanceRocExhaustiveSearchInstanceTrialInstances { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceTrialInstanceActivationFunctionTrial>
            ExhaustiveSearchInstanceTrialInstanceActivationFunctionTrialsExhaustiveSearchInstanceTrialInstances
        {
            get;
            set;
        }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceTrialInstanceSensitivity>
            ExhaustiveSearchInstanceTrialInstanceSensitivityExhaustiveSearchInstanceTrialInstances { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceTrialInstanceTopologyTrial>
            ExhaustiveSearchInstanceTrialInstanceTopologyTrialsExhaustiveSearchInstanceTrialInstances { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceTrialInstanceVariable>
            ExhaustiveSearchInstanceTrialInstanceVariablesExhaustiveSearchInstanceTrialInstances { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceTrialInstanceActivationFunctionTrial
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ActivationFunctionId { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstance ExhaustiveSearchInstanceTrialInstance { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceTrialInstanceSensitivity
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public double? Sensitivity { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceVariableId { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceId { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceVariableId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstance ExhaustiveSearchInstanceTrialInstance { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceTrialInstanceTopologyTrial
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceId { get; set; }
        [Column] [Nullable] public int? TrialsSinceImprovement { get; set; }
        [Column] [Nullable] public int? TopologyTrials { get; set; }
        [Column] [Nullable] public int? Layer { get; set; }
        [Column] [Nullable] public int? Neurons { get; set; }
        [Column] [Nullable] public double? Score { get; set; }
        [Column] [Nullable] public byte? Finalisation { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstance ExhaustiveSearchInstanceTrialInstance { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceTrialInstanceVariable
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceVariableId { get; set; }
        [Column] [Nullable] public int? Removed { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceId { get; set; }
        [Column] [Nullable] public int? VariableSequence { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceVariableId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstancePromotedTrialInstanceSensitivity>
            ExhaustiveSearchInstancePromotedTrialInstanceSensitivityExhaustiveSearchInstanceTrialInstanceVariables
        {
            get;
            set;
        }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstance ExhaustiveSearchInstanceTrialInstance { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstanceTrialInstanceVariableId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstancePromotedTrialInstanceVariable>
            ExhaustiveSearchInstanceTrialInstanceVariablePrescriptionsExhaustiveSearchInstanceTrialInstanceVariables
        {
            get;
            set;
        }
    }

    [Table]
    public class ExhaustiveSearchInstancePromotedTrialInstanceVariable
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceTrialInstanceVariableId { get; set; }
        [Column] [Nullable] public double? Mean { get; set; }
        [Column] [Nullable] public double? StandardDeviation { get; set; }
        [Column] [Nullable] public double? Kurtosis { get; set; }
        [Column] [Nullable] public double? Skewness { get; set; }
        [Column] [Nullable] public double? Maximum { get; set; }
        [Column] [Nullable] public double? Minimum { get; set; }
        [Column] [Nullable] public double? Iqr { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceTrialInstanceVariableId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceTrialInstanceVariable ExhaustiveSearchInstanceTrialInstanceVariable { get; set; }

        [Association(ThisKey = "Id",
            OtherKey = "ExhaustiveSearchInstancePromotedTrialInstanceVariableId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceTrialInstanceVariablePrescriptionHistogram>
            ExhaustiveSearchInstanceTrialInstanceVariablePrescriptExhaustiveSearchInstanceTrialInstanceVariablePrescripts
        {
            get;
            set;
        }
    }

    [Table]
    public class ExhaustiveSearchInstanceTrialInstanceVariablePrescriptionHistogram
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstancePromotedTrialInstanceVariableId { get; set; }
        [Column] [Nullable] public int? BinIndex { get; set; }
        [Column] [Nullable] public double? BinRangeStart { get; set; }
        [Column] [Nullable] public double? BinRangeEnd { get; set; }
        [Column] [Nullable] public int? Frequency { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstancePromotedTrialInstanceVariableId",
            OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstancePromotedTrialInstanceVariable
            ExhaustiveSearchInstanceTrialInstanceVariablePrescription { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceVariable
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? ProcessingTypeId { get; set; }
        [Column] [Nullable] public double? Mode { get; set; }
        [Column] [Nullable] public double? Mean { get; set; }
        [Column] [Nullable] public double? StandardDeviation { get; set; }
        [Column] [Nullable] public double? Kurtosis { get; set; }
        [Column] [Nullable] public double? Skewness { get; set; }
        [Column] [Nullable] public double? Maximum { get; set; }
        [Column] [Nullable] public double? Minimum { get; set; }
        [Column] [Nullable] public double? Iqr { get; set; }
        [Column] [Nullable] public byte? PrescriptionSimulation { get; set; }
        [Column] [Nullable] public byte? NormalisationTypeId { get; set; }
        [Column] [Nullable] public int? DistinctValues { get; set; }
        [Column] [Nullable] public double? Correlation { get; set; }
        [Column] [Nullable] public int? CorrelationAbsRank { get; set; }
        [Column] [Nullable] public int? Bins { get; set; }
        [Column] [Nullable] public int? VariableSequence { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstance ExhaustiveSearchInstance { get; set; }

        [Association(ThisKey = "Id", OtherKey = "ExhaustiveSearchInstanceVariableId",
            CanBeNull = true, Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceVariableHistogram>
            ExhaustiveSearchInstanceVariableHistogramsExhaustiveSearchInstanceVariables { get; set; }

        [Association(ThisKey = "Id", OtherKey = "ExhaustiveSearchInstanceVariableId",
            CanBeNull = true, Relationship = Relationship.OneToMany)]
        public IEnumerable<ExhaustiveSearchInstanceVariableMultiCollinearity>
            ExhaustiveSearchInstanceVariableMultiCollinearityExhaustiveSearchInstanceVariables { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstance ExhaustiveSearchInstanceVariablesExhaustiveSearchInstance { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceVariableHistogram
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceVariableId { get; set; }
        [Column] [Nullable] public int? BinSequence { get; set; }
        [Column] [Nullable] public double? BinRangeStart { get; set; }
        [Column] [Nullable] public double? BinRangeEnd { get; set; }
        [Column] [Nullable] public int? Frequency { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceVariableId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceVariable ExhaustiveSearchInstanceVariable { get; set; }
    }

    [Table]
    public class ExhaustiveSearchInstanceVariableMultiCollinearity
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? ExhaustiveSearchInstanceVariableId { get; set; }
        [Column] [Nullable] public int? TestExhaustiveSearchInstanceVariableId { get; set; }
        [Column] [Nullable] public double? Correlation { get; set; }
        [Column] [Nullable] public int? CorrelationAbsRank { get; set; }

        [Association(ThisKey = "ExhaustiveSearchInstanceVariableId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceVariable ExhaustiveSearchInstanceVariable { get; set; }

        [Association(ThisKey = "TestExhaustiveSearchInstanceVariableId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public ExhaustiveSearchInstanceVariable TestExhaustiveSearchInstanceVariable { get; set; }
    }

    [Table]
    public class HttpProcessingCounter
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Instance { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? All { get; set; }
        [Column] [Nullable] public int? Model { get; set; }
        [Column] [Nullable] public int? AsynchronousModel { get; set; }
        [Column] [Nullable] public int? Tag { get; set; }
        [Column] [Nullable] public int? Error { get; set; }
        [Column] [Nullable] public int? Exhaustive { get; set; }
        [Column] [Nullable] public int? Sanction { get; set; }
        [Column] [Nullable] public int? Callback { get; set; }
    }

    [Table]
    public class Archive
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public Guid EntityAnalysisModelInstanceEntryGuid { get; set; }
        [Column] [Nullable] public string EntryKeyValue { get; set; }
        [Column] [Nullable] public double? ResponseElevation { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelActivationRuleId { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public int? ActivationRuleCount { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public DateTime? ReferenceDate { get; set; }
    }
    
    [Table]
    public class MockArchive
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }

        [Column]
        [DataType(DataType.BinaryJson)]
        [Nullable]
        public string Json { get; set; }

        [Column] [Nullable] public Guid EntityAnalysisModelInstanceEntryGuid { get; set; }
        [Column] [Nullable] public string EntryKeyValue { get; set; }
        [Column] [Nullable] public double? ResponseElevation { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelActivationRuleId { get; set; }
        [Column] [Nullable] public int? EntityAnalysisModelId { get; set; }
        [Column] [Nullable] public int? ActivationRuleCount { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public DateTime? ReferenceDate { get; set; }
    }

    [Table]
    public class Language
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }

        [Association(ThisKey = "Id", OtherKey = "LanguageId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<LanguageString> LanguageStrings { get; set; }
    }

    [Table]
    public class LanguageString
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string LanguageStringKey { get; set; }
        [Column] [Nullable] public string LanguageStringColumn { get; set; }
        [Column] [Nullable] public int? LanguageId { get; set; }

        [Association(ThisKey = "LanguageId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public Language Language { get; set; }
    }

    [Table]
    public class PermissionSpecification
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }

        [Association(ThisKey = "Id", OtherKey = "PermissionSpecificationId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<RoleRegistryPermission> RoleRegistryPermissionsPermissionSpecifications { get; set; }

        [Association(ThisKey = "Id", OtherKey = "PermissionSpecificationId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<UserPermissionSpecificationValidation>
            UserPermissionSpecificationValidationsPermissionSpecifications { get; set; }
    }

    [Table]
    public class RoleRegistry
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "Id", OtherKey = "RoleRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<RoleRegistryPermission> RoleRegistryPermissionsRoleRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "RoleRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<UserRoleRegistry> UserRoleRegistryRoleRegistry { get; set; }
    }

    [Table]
    public class RoleRegistryPermission
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? PermissionSpecificationId { get; set; }
        [Column] [Nullable] public int? RoleRegistryId { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "PermissionSpecificationId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public PermissionSpecification PermissionSpecification { get; set; }

        [Association(ThisKey = "RoleRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public RoleRegistry RoleRegistry { get; set; }
    }

    [Table]
    public class RuleScriptToken
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Token { get; set; }
    }

    [Table]
    public class SanctionEntrySource
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Severity { get; set; }
        [Column] [Nullable] public string DirectoryLocation { get; set; }
        [Column] [Nullable] public char? Delimiter { get; set; }
        [Column] [Nullable] public string MultiPartStringIndex { get; set; }
        [Column] [Nullable] public byte? ReferenceIndex { get; set; }
        [Column] [Nullable] public byte? EnableDirectoryLocation { get; set; }
        [Column] [Nullable] public byte? EnableHttpLocation { get; set; }
        [Column] [Nullable] public string HttpLocation { get; set; }
        [Column] [Nullable] public byte? Skip { get; set; }

        [Association(ThisKey = "Id", OtherKey = "SanctionEntrySourceId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<SanctionEntry> SanctionEntrySanctionEntrySources { get; set; }
    }

    [Table]
    public class SanctionEntry
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string SanctionEntryElementValue { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? SanctionEntrySourceId { get; set; }
        [Column] [Nullable] public string SanctionEntryReference { get; set; }
        [Column] [Nullable] public string SanctionPayload { get; set; }
        [Column] [Nullable] public string SanctionEntryHash { get; set; }

        [Association(ThisKey = "SanctionEntrySourceId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public SanctionEntrySource SanctionEntrySource { get; set; }
    }

    [Table]
    public class TenantRegistryVersion
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int TenantRegistryId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public byte? Landlord { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
    }

    [Table]
    public class TenantRegistry
    {
        [Column("Id")] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public byte? Landlord { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<ActivationWatcher> ActivationWatcherTenantRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<CaseWorkflow> CaseWorkflowsTenantRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelSynchronisationNodeStatusEntry>
            EntityAnalysisModelSyncronisationNodeStatusEntryTenantRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModelSynchronisationSchedule>
            EntityAnalysisModelSyncronisationSchedulesTenantRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<EntityAnalysisModel> EntityAnalysisModelTenantRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<UserInTenant> UserInTenantTenantRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "TenantRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<VisualisationRegistry> VisualisationRegistryTenantRegistry { get; set; }
    }

    [Table]
    public class UserLogin
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public string RemoteIp { get; set; }
        [Column] [Nullable] public string LocalIp { get; set; }
        [Column] [Nullable] public string UserAgent { get; set; }
        [Column] [Nullable] public int FailureTypeId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public byte Failed { get; set; }
    }

    [Table]
    public class UserPermissionSpecificationValidation
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public string Ip { get; set; }
        [Column] [Nullable] public string UserAgent { get; set; }
        [Column] [Nullable] public string SessionId { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public int? PermissionSpecificationId { get; set; }
        [Column] [Nullable] public byte Failed { get; set; }
        [Column] [Nullable] public DateTime? TestDate { get; set; }

        [Association(ThisKey = "PermissionSpecificationId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public PermissionSpecification PermissionSpecification { get; set; }
    }

    [Table]
    public class UserInTenant
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string User { get; set; }
        [Column] [Nullable] public int TenantRegistryId { get; set; }
        [Column] [Nullable] public string SwitchedUser { get; set; }
        [Column] [Nullable] public DateTime SwitchedDate { get; set; }

        [Association(ThisKey = "TenantRegistryId", OtherKey = "Id", CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public TenantRegistry TenantRegistry { get; set; }
    }
    
    [Table]
    public class UserInTenantSwitchLog
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int UserInTenantId { get; set; }
        [Column] [Nullable] public int TenantRegistryId { get; set; }
        [Column] [Nullable] public string SwitchedUser { get; set; }
        [Column] [Nullable] public DateTime SwitchedDate { get; set; }

        [Association(ThisKey = "TenantRegistryId", OtherKey = "Id", CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public TenantRegistry TenantRegistry { get; set; }
        
        [Association(ThisKey = "UserInTenantId", OtherKey = "Id", CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public UserInTenant UserInTenant { get; set; }
    }

    [Table]
    public class UserRoleRegistry
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string User { get; set; }
        [Column] [Nullable] public int RoleRegistryId { get; set; }

        [Association(ThisKey = "RoleRegistryId", OtherKey = "Id", CanBeNull = false,
            Relationship = Relationship.ManyToOne)]
        public RoleRegistry RoleRegistry { get; set; }
    }

    [Table]
    public class VisualisationRegistryVersion
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int VisualisationRegistryId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public byte? ShowInDirectory { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public int? Columns { get; set; }
        [Column] [Nullable] public int? ColumnWidth { get; set; }
        [Column] [Nullable] public int? RowHeight { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
    }

    [Table]
    public class VisualisationRegistry
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public byte? ShowInDirectory { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public int? TenantRegistryId { get; set; }
        [Column] [Nullable] public int? Columns { get; set; }
        [Column] [Nullable] public int? ColumnWidth { get; set; }
        [Column] [Nullable] public int? RowHeight { get; set; }
        [Column] [Nullable] public int? Version { get; set; }

        [Association(ThisKey = "TenantRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public TenantRegistry TenantRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "VisualisationRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<VisualisationRegistryDatasource> VisualisationRegistryDatasourcesVisualisationRegistry
        {
            get;
            set;
        }

        [Association(ThisKey = "Id", OtherKey = "VisualisationRegistryId", CanBeNull = true,
            Relationship = Relationship.OneToMany)]
        public IEnumerable<VisualisationRegistryParameter> VisualisationRegistryParametersVisualisationRegistry
        {
            get;
            set;
        }
    }

    [Table]
    public class VisualisationRegistryDatasource
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? VisualisationTypeId { get; set; }
        [Column] [Nullable] public string Command { get; set; }
        [Column] [Nullable] public string VisualisationText { get; set; }
        [Column] [Nullable] public double? Priority { get; set; }
        [Column] [Nullable] public byte? IncludeGrid { get; set; }
        [Column] [Nullable] public byte? IncludeDisplay { get; set; }
        [Column] [Nullable] public int? ColumnSpan { get; set; }
        [Column] [Nullable] public int? RowSpan { get; set; }

        [Association(ThisKey = "VisualisationRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public VisualisationRegistry VisualisationRegistry { get; set; }
    }

    [Table]
    public class VisualisationRegistryDatasourceSeries
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryDatasourceId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public int DataTypeId { get; set; }

        [Association(ThisKey = "VisualisationRegistryDatasourceId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public VisualisationRegistryDatasource VisualisationRegistryDatasource { get; set; }
    }

    [Table]
    public class VisualisationRegistryDatasourceExecutionLog
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public string Error { get; set; }
        [Column] [Nullable] public int? Records { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public int? ResponseTime { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryDatasourceId { get; set; }
    }

    [Table]
    public class VisualisationRegistryDatasourceExecutionLogParameter
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryParameterId { get; set; }
        [Column] [Nullable] public string Value { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryDatasourceExecutionLogId { get; set; }

        [Association(ThisKey = "VisualisationRegistryParameterId", OtherKey = "Id",
            CanBeNull = true, Relationship = Relationship.ManyToOne)]
        public VisualisationRegistryParameter VisualisationRegistryParameter { get; set; }
    }

    [Table]
    public class VisualisationRegistryParameter
    {
        [Column] [PrimaryKey] [Identity] public int Id { get; set; }
        [Column] [Nullable] public int? VisualisationRegistryId { get; set; }
        [Column] [Nullable] public string Name { get; set; }
        [Column] [Nullable] public byte? Active { get; set; }
        [Column] [Nullable] public byte? Locked { get; set; }
        [Column] [Nullable] public DateTime? CreatedDate { get; set; }
        [Column] [Nullable] public string CreatedUser { get; set; }
        [Column] [Nullable] public DateTime? UpdatedDate { get; set; }
        [Column] [Nullable] public string UpdatedUser { get; set; }
        [Column] [Nullable] public byte? Deleted { get; set; }
        [Column] [Nullable] public string DeletedUser { get; set; }
        [Column] [Nullable] public DateTime? DeletedDate { get; set; }
        [Column] [Nullable] public int? InheritedId { get; set; }
        [Column] [Nullable] public int? Version { get; set; }
        [Column] [Nullable] public int? DataTypeId { get; set; }
        [Column] [Nullable] public string DefaultValue { get; set; }
        [Column] [Nullable] public byte? Required { get; set; }

        [Association(ThisKey = "VisualisationRegistryId", OtherKey = "Id", CanBeNull = true,
            Relationship = Relationship.ManyToOne)]
        public VisualisationRegistry VisualisationRegistry { get; set; }

        [Association(ThisKey = "Id", OtherKey = "VisualisationRegistryParameterId",
            CanBeNull = true, Relationship = Relationship.OneToMany)]
        public IEnumerable<VisualisationRegistryDatasourceExecutionLogParameter>
            VisualisationRegistryDatasourcesExecutionLogParametersVisualisationRegistryParameters { get; set; }
    }
}