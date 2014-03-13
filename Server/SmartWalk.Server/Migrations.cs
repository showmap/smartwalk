using System;
using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using SmartWalk.Server.Models;

namespace SmartWalk.Server
{
    public class Migrations : DataMigrationImpl
    {
        public int Create() {

            SchemaBuilder.CreateTable("SmartWalkUserRecord", table => table
               .ContentPartRecord()
               .Column("FirstName", DbType.String, c => c.NotNull().WithLength(50))
               .Column("LastName", DbType.String, c => c.NotNull().WithLength(50))
               .Column<DateTime>("CreatedAt", c => c.NotNull())
               .Column<DateTime>("LastLoginAt", c => c.NotNull())
               );

            //SchemaBuilder.DropTable("EventMappingRecord");
            //SchemaBuilder.DropTable("EventMetaDataRecord");
            //SchemaBuilder.DropTable("EntityMappingRecord");
            //SchemaBuilder.DropTable("StorageRecord");
            //SchemaBuilder.DropTable("ShowRecord");
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

            SchemaBuilder.CreateTable("RegionRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("Country", DbType.String, c => c.NotNull())
              .Column("State", DbType.String, c => c.NotNull())
              .Column("City", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
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

            SchemaBuilder.CreateTable("ShowRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("IsReference", DbType.Boolean, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.Nullable())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("Picture", DbType.String, c => c.Nullable())
              .Column("DetailsUrl", DbType.String, c => c.Nullable())
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
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("CombineType", DbType.Byte, c => c.NotNull())
              .Column("IsMobileReady", DbType.Boolean, c => c.NotNull())
              .Column("IsWidgetReady", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ShowRecord_Id", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateForeignKey("EntityRecord_SmartWalkUserRecord", "EntityRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("AddressRecord_EntityRecord", "AddressRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("AddressRecord_RegionRecord", "AddressRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_RegionRecord", "EventMetadataRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            ContentDefinitionManager.AlterPartDefinition(typeof(SmartWalkUserPart).Name, u => u
                .Attachable()
            );

            ContentDefinitionManager.AlterTypeDefinition("User", t => t
                .WithPart(typeof(SmartWalkUserPart).Name)
            );

            return 1;
        }

        public int UpdateFrom1()
        {
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

            SchemaBuilder.CreateTable("RegionRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("Country", DbType.String, c => c.NotNull())
              .Column("State", DbType.String, c => c.NotNull())
              .Column("City", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
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
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
              .Column("CombineType", DbType.Byte, c => c.NotNull())
              .Column("IsMobileReady", DbType.Boolean, c => c.NotNull())
              .Column("IsWidgetReady", DbType.Boolean, c => c.NotNull())
              .Column("DateCreated", DbType.DateTime, c => c.NotNull())
              .Column("DateModified", DbType.DateTime, c => c.NotNull())
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

            SchemaBuilder.CreateTable("EventMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EventMetaDataRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ShowRecord_Id", DbType.Int32, c => c.NotNull())
              );

            SchemaBuilder.CreateForeignKey("EntityRecord_SmartWalkUserRecord", "EntityRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("AddressRecord_EntityRecord", "AddressRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("AddressRecord_RegionRecord", "AddressRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowRecord_EventMetadataRecord", "ShowRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_RegionRecord", "EventMetadataRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            return 2;
        }

        public int UpdateFrom2()
        {
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

            SchemaBuilder.CreateTable("RegionRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("Region", DbType.String, c => c.NotNull())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("AddressRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
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
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
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
            SchemaBuilder.CreateForeignKey("AddressRecord_RegionRecord", "AddressRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ContactRecord_EntityRecord", "ContactRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowRecord_EntityRecord", "ShowRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowRecord_EventMetadataRecord", "ShowRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EntityMappingRecord_EntityRecord", "EntityMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EntityMappingRecord_StorageRecord", "EntityMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_RegionRecord", "EventMetadataRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_EntityRecord", "EventMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowMappingRecord_ShowRecord", "ShowMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowMappingRecord_StorageRecord", "ShowMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            return 3;
        }

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
              );

            SchemaBuilder.CreateTable("RegionRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("Region", DbType.String, c => c.NotNull())
              .Column("Latitude", DbType.Double, c => c.NotNull())
              .Column("Longitude", DbType.Double, c => c.NotNull())
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
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.Nullable())
              .Column("Description", DbType.String, c => c.Nullable().Unlimited())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.Nullable())
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

            SchemaBuilder.CreateForeignKey("EventMetadataRecord_RegionRecord", "EventMetadataRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_EntityRecord", "EventMetadataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetadataRecord_SmartWalkUserRecord", "EventMetadataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetadataRecord", "EventMappingRecord", new[] { "EventMetadataRecord_Id" }, "EventMetadataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_EntityRecord", "EventMappingRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("ShowMappingRecord_ShowRecord", "ShowMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ShowMappingRecord_StorageRecord", "ShowMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });

            return 4;
        }        
    }
}