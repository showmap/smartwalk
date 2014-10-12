using System;
using System.Data;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Core.Settings.Models;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Environment.Configuration;
using Orchard.Users.Models;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Shared;

namespace SmartWalk.Server
{
    [UsedImplicitly]
    public class Migrations : DataMigrationImpl {

        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<SmartWalkUserRecord> _userRepository;
        private readonly ShellSettings _shellSettings;

        public Migrations(
            IRepository<SmartWalkUserRecord> userRepository,
            IOrchardServices orchardServices,
            ShellSettings shellSettings)
        {
            _orchardServices = orchardServices;
            _userRepository = userRepository;
            _shellSettings = shellSettings;
        }

        private string TablePrefix
        {
            get
            {
                return !string.IsNullOrEmpty(_shellSettings.DataTablePrefix) 
                    ? _shellSettings.DataTablePrefix + "_" 
                    : "";
            }
        }

        private string GetFullTableName(string tableName)
        {
            return string.Format("{0}SmartWalk_Server_{1}", TablePrefix, tableName);
        }

        private void PopulateData() {
            var siteSettings = _orchardServices.WorkContext.CurrentSite.As<SiteSettingsPart>();
            var user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName == siteSettings.SuperUser).List().FirstOrDefault();

            if (user != null) {
                _userRepository.Create(new SmartWalkUserRecord {
                    ContentItemRecord = user.ContentItem.Record,
                    CreatedAt = DateTime.Now,
                    LastLoginAt = DateTime.Now,
                    FirstName = user.UserName,
                    LastName = user.UserName
                });
            }
        }

        [UsedImplicitly]
        public int Create()
        {
            SchemaBuilder.CreateTable("SmartWalkUserRecord", table => table
               .ContentPartRecord()
               .Column("FirstName", DbType.String, c => c.NotNull().WithLength(50))
               .Column("LastName", DbType.String, c => c.NotNull().WithLength(50))
               .Column<DateTime>("CreatedAt", c => c.NotNull())
               .Column<DateTime>("LastLoginAt", c => c.NotNull())
               );

            //SchemaBuilder.DropTable("ShowMappingRecord");
            //SchemaBuilder.DropTable("EventMappingRecord");
            //SchemaBuilder.DropTable("ShowRecord");
            //SchemaBuilder.DropTable("EventMetaDataRecord");
            //SchemaBuilder.DropTable("EntityMappingRecord");
            //SchemaBuilder.DropTable("StorageRecord");
            //SchemaBuilder.DropTable("ContactRecord");
            //SchemaBuilder.DropTable("AddressRecord");
            //SchemaBuilder.DropTable("RegionRecord");
            //SchemaBuilder.DropTable("EntityRecord");

            SchemaBuilder.CreateTable("EntityRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Name", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("Picture", DbType.String, c => c.Nullable())
              );

            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
              .Column("Address", DbType.String, c => c.Nullable())
              );

            SchemaBuilder.CreateTable("ContactRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Contact", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("StorageRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("StorageKey", DbType.String, c => c.NotNull().WithLength(3))
              .Column("Description", DbType.String, c => c.Nullable())
             );

            SchemaBuilder.CreateTable("EntityMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEntityId", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMetadataRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
              .Column("CombineType", DbType.Byte, c => c.NotNull())
              .Column("IsPublic", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("IsReference", DbType.Boolean, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.Nullable())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("DetailsUrl", DbType.String, c => c.Nullable())
              );

            SchemaBuilder.CreateTable("ShowMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("ShowRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateForeignKey("EntityRecord_SmartWalkUserRecord", "EntityRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("AddressRecord_EntityRecord", "AddressRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowRecord_EventMetadataRecord", "ShowRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_EntityRecord", "EventMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowMappingRecord_ShowRecord", "ShowMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowMappingRecord_StorageRecord", "ShowMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            ContentDefinitionManager.AlterPartDefinition(typeof(SmartWalkUserPart).Name, u => u
                .Attachable()
            );

            ContentDefinitionManager.AlterTypeDefinition("User", t => t
                .WithPart(typeof(SmartWalkUserPart).Name)
            );            

            PopulateData();

            return 1;
        }

        [UsedImplicitly]
        public int UpdateFrom1()
        {
            SchemaBuilder.DropTable("ShowMappingRecord");
            SchemaBuilder.DropTable("EventMappingRecord");
            SchemaBuilder.DropTable("ShowRecord");
            SchemaBuilder.DropTable("EventMetaDataRecord");
            SchemaBuilder.DropTable("EntityMappingRecord");
            SchemaBuilder.DropTable("StorageRecord");
            SchemaBuilder.DropTable("ContactRecord");
            SchemaBuilder.DropTable("AddressRecord");
            SchemaBuilder.DropTable("RegionRecord");
            SchemaBuilder.DropTable("EntityRecord");

            SchemaBuilder.CreateTable("EntityRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Name", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("Picture", DbType.String, c => c.Nullable())
              );
            
            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
              .Column("Address", DbType.String, c => c.Nullable())
              );

            SchemaBuilder.CreateTable("ContactRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Contact", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("StorageRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("StorageKey", DbType.String, c => c.NotNull().WithLength(3))
              .Column("Description", DbType.String, c => c.Nullable())
             );

            SchemaBuilder.CreateTable("EntityMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEntityId", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMetadataRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())              
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
              .Column("CombineType", DbType.Byte, c => c.NotNull())
              .Column("IsPublic", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("IsReference", DbType.Boolean, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.Nullable())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("DetailsUrl", DbType.String, c => c.Nullable())
              );

            SchemaBuilder.CreateTable("ShowMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("ShowRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateForeignKey("EntityRecord_SmartWalkUserRecord", "EntityRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("AddressRecord_EntityRecord", "AddressRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowRecord_EventMetadataRecord", "ShowRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_EntityRecord", "EventMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowMappingRecord_ShowRecord", "ShowMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowMappingRecord_StorageRecord", "ShowMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            return 2;
        }

        [UsedImplicitly]
        public int UpdateFrom2()
        {
            SchemaBuilder.DropTable("ShowMappingRecord");
            SchemaBuilder.DropTable("EventMappingRecord");
            SchemaBuilder.DropTable("ShowRecord");
            SchemaBuilder.DropTable("EventMetaDataRecord");
            SchemaBuilder.DropTable("EntityMappingRecord");
            SchemaBuilder.DropTable("StorageRecord");
            SchemaBuilder.DropTable("ContactRecord");
            SchemaBuilder.DropTable("AddressRecord");
            SchemaBuilder.DropTable("RegionRecord");
            SchemaBuilder.DropTable("EntityRecord");

            SchemaBuilder.CreateTable("EntityRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Name", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
              .Column("Address", DbType.String, c => c.Nullable())
              );

            SchemaBuilder.CreateTable("ContactRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Contact", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("StorageRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("StorageKey", DbType.String, c => c.NotNull().WithLength(3))
              .Column("Description", DbType.String, c => c.Nullable())
             );

            SchemaBuilder.CreateTable("EntityMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEntityId", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMetadataRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
              .Column("CombineType", DbType.Byte, c => c.NotNull())
              .Column("IsPublic", DbType.Boolean, c => c.NotNull())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("IsReference", DbType.Boolean, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.Nullable())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("DetailsUrl", DbType.String, c => c.Nullable())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("ShowRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateForeignKey("EntityRecord_SmartWalkUserRecord", "EntityRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("AddressRecord_EntityRecord", "AddressRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowRecord_EventMetadataRecord", "ShowRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_EntityRecord", "EventMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowMappingRecord_ShowRecord", "ShowMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowMappingRecord_StorageRecord", "ShowMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            return 3;
        }

        [UsedImplicitly]
        public int UpdateFrom3()
        {
            SchemaBuilder.DropTable("ShowMappingRecord");
            SchemaBuilder.DropTable("EventMappingRecord");
            SchemaBuilder.DropTable("ShowRecord");
            SchemaBuilder.DropTable("EventMetaDataRecord");
            SchemaBuilder.DropTable("EntityMappingRecord");
            SchemaBuilder.DropTable("StorageRecord");
            SchemaBuilder.DropTable("ContactRecord");
            SchemaBuilder.DropTable("AddressRecord");
            SchemaBuilder.DropTable("RegionRecord");
            SchemaBuilder.DropTable("EntityRecord");

            SchemaBuilder.CreateTable("EntityRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Name", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Latitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("Longitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("Address", DbType.String, c => c.Nullable())
              );

            SchemaBuilder.CreateTable("ContactRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Contact", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("StorageRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("StorageKey", DbType.String, c => c.NotNull().WithLength(3))
              .Column("Description", DbType.String, c => c.Nullable())
             );

            SchemaBuilder.CreateTable("EntityMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEntityId", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMetadataRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("Latitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("Longitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("CombineType", DbType.Byte, c => c.NotNull())
              .Column("IsPublic", DbType.Boolean, c => c.NotNull())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("IsReference", DbType.Boolean, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.Nullable())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("DetailsUrl", DbType.String, c => c.Nullable())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("ShowRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateForeignKey("EntityRecord_SmartWalkUserRecord", "EntityRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("AddressRecord_EntityRecord", "AddressRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowRecord_EventMetadataRecord", "ShowRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_EntityRecord", "EventMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowMappingRecord_ShowRecord", "ShowMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowMappingRecord_StorageRecord", "ShowMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            return 4;
        }

        [UsedImplicitly]
        public int UpdateFrom4()
        {
            SchemaBuilder.DropTable("ShowMappingRecord");
            SchemaBuilder.DropTable("EventMappingRecord");
            SchemaBuilder.DropTable("ShowRecord");
            SchemaBuilder.DropTable("EventMetaDataRecord");
            SchemaBuilder.DropTable("EntityMappingRecord");
            SchemaBuilder.DropTable("StorageRecord");
            SchemaBuilder.DropTable("ContactRecord");
            SchemaBuilder.DropTable("AddressRecord");
            SchemaBuilder.DropTable("RegionRecord");
            SchemaBuilder.DropTable("EntityRecord");

            SchemaBuilder.CreateTable("EntityRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Name", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Latitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("Longitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("Address", DbType.String, c => c.Nullable())
              .Column("Tip", DbType.String, c => c.WithLength(100).Nullable())
              );

            SchemaBuilder.CreateTable("ContactRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Contact", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("StorageRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("StorageKey", DbType.String, c => c.NotNull().WithLength(3))
              .Column("Description", DbType.String, c => c.Nullable())
             );

            SchemaBuilder.CreateTable("EntityMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEntityId", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMetadataRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("Latitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("Longitude", DbType.Decimal, c => c.WithPrecision(9).WithScale(6).NotNull())
              .Column("CombineType", DbType.Byte, c => c.NotNull())
              .Column("IsPublic", DbType.Boolean, c => c.NotNull())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("IsReference", DbType.Boolean, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.Nullable())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("DetailsUrl", DbType.String, c => c.Nullable())
              .Column("IsDeleted", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("ShowMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("ShowRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEventId", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateForeignKey("EntityRecord_SmartWalkUserRecord", "EntityRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("AddressRecord_EntityRecord", "AddressRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowRecord_EventMetadataRecord", "ShowRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_EntityRecord", "EventMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowMappingRecord_ShowRecord", "ShowMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowMappingRecord_StorageRecord", "ShowMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            return 5;
        }

        [UsedImplicitly]
        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(
                "ContactRecord",
                table => table.AddColumn("IsDeleted", DbType.Boolean, c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(
                "AddressRecord",
                table => table.AddColumn("IsDeleted", DbType.Boolean, c => c.NotNull().WithDefault(false)));

            return 6;
        }

        [UsedImplicitly]
        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(
                "EventMetadataRecord",
                table => table.AddColumn("Status", DbType.Byte, c => c.NotNull().WithDefault(0)));

            SchemaBuilder.ExecuteSql(
                @"UPDATE emr SET emr.Status = emr.IsPublic FROM SmartWalk_Server_EventMetadataRecord as emr");

            SchemaBuilder.AlterTable(
                "EventMetadataRecord",
                table => table.DropColumn("IsPublic"));

            return 7;
        }

        [UsedImplicitly]
        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(
                "SmartWalkUserRecord",
                table => table.AddColumn("IsVerificationRequested", DbType.Boolean, c => c.NotNull().WithDefault(false)));

            return 8;
        }

        [UsedImplicitly]
        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(
                "SmartWalkUserRecord",
                table => table.AlterColumn(
                    "FirstName",
                    c => c.WithType(DbType.String).WithLength(50).WithDefault(null)));

            SchemaBuilder.AlterTable(
                "SmartWalkUserRecord",
                table => table.AlterColumn(
                    "LastName",
                    c => c.WithType(DbType.String).WithLength(50).WithDefault(null)));

            return 9;
        }

        [UsedImplicitly]
        public int UpdateFrom9()
        {
            SchemaBuilder.CreateTable("EventEntityDetailRecord", table => table
             .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
             .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
             .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
             .Column("Order", DbType.Int32, c => c.Nullable())
             .Column("Description", DbType.String, c => c.Nullable().WithLength(255))
             );            

            SchemaBuilder.CreateForeignKey("EventEntityDetailRecord_EntityRecord", "EventEntityDetailRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventEntityDetailRecord_EventMetadataRecord", "EventEntityDetailRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(
                "EventMetadataRecord",
                table => table.AddColumn(
                    "VenueOrderType", DbType.Byte,
                    c => c.NotNull().WithDefault(0)));

            var sSql = string.Format("ALTER TABLE {0} ALTER COLUMN StartTime datetime NULL", GetFullTableName("EventMetadataRecord"));
            SchemaBuilder.ExecuteSql(sSql);

            return 10;
        }

        [UsedImplicitly]
        public int UpdateFrom10()
        {
            var sSql = string.Format("EXEC sp_RENAME '{0}.[Order]', 'SortOrder', 'COLUMN'", 
                GetFullTableName("EventEntityDetailRecord"));
            SchemaBuilder.ExecuteSql(sSql);

            return 11;
        }

        [UsedImplicitly]
        public int UpdateFrom11()
        {
            SchemaBuilder.AlterTable(
                "EventEntityDetailRecord",
                table => table.AlterColumn("Description", c => c.WithType(DbType.String).Unlimited().WithDefault(null)));

            return 12;
        }

        [UsedImplicitly]
        public int UpdateFrom12()
        {
            SchemaBuilder.AlterTable(
                "EventEntityDetailRecord",
                table => table.AddColumn("IsDeleted", DbType.Boolean, c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(
                "EventMetadataRecord",
                table => table.AddColumn(
                    "VenueTitleFormatType", DbType.Byte,
                    c => c.NotNull().WithDefault(0)));

            return 13;
        }
    }
}