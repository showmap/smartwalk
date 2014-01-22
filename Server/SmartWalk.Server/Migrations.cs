using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Orchard.Data.Migration;
using SmartWalk.Server.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;

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

            SchemaBuilder.CreateTable("EntityRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Name", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.NotNull())
              .Column("Picture", DbType.String, c => c.NotNull())
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
              .Column("Address", DbType.String, c => c.NotNull())              
              );

            SchemaBuilder.CreateTable("ContactRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              .Column("Title", DbType.String, c => c.NotNull())
              .Column("Contact", DbType.String, c => c.NotNull())
              );            

            SchemaBuilder.CreateTable("ShowRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.NotNull())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.NotNull())
              .Column("Picture", DbType.String, c => c.NotNull())
              .Column("DetailsUrl", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("StorageRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("Key", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EntityMappingRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("StorageRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("ExternalEntityId", DbType.Int32, c => c.NotNull())
              .Column("Type", DbType.Byte, c => c.NotNull())
              );

            SchemaBuilder.CreateTable("EventMetaDataRecord", table => table
              .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
              .Column("RegionRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("EntityRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("SmartWalkUserRecord_Id", DbType.Int32, c => c.NotNull())
              .Column("Title", DbType.String, c => c.NotNull())
              .Column("Description", DbType.String, c => c.NotNull())
              .Column("StartTime", DbType.DateTime, c => c.NotNull())
              .Column("EndTime", DbType.DateTime, c => c.NotNull())
              .Column("IsCombined", DbType.Boolean, c => c.NotNull())
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

            SchemaBuilder.CreateForeignKey("EventMetaDataRecord_RegionRecord", "EventMetaDataRecord", new[] { "RegionRecord_Id" }, "RegionRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetaDataRecord_EntityRecord", "EventMetaDataRecord", new[] { "EntityRecord_Id" }, "EntityRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMetaDataRecord_SmartWalkUserRecord", "EventMetaDataRecord", new[] { "SmartWalkUserRecord_Id" }, "SmartWalkUserRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey("EventMappingRecord_EventMetaDataRecord", "EventMappingRecord", new[] { "EventMetaDataRecord_Id" }, "EventMetaDataRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_StorageRecord", "EventMappingRecord", new[] { "StorageRecord_Id" }, "StorageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("EventMappingRecord_ShowRecord", "EventMappingRecord", new[] { "ShowRecord_Id" }, "ShowRecord", new[] { "Id" });            

            ContentDefinitionManager.AlterPartDefinition(typeof(SmartWalkUserPart).Name, u => u
                .Attachable()
            );

            ContentDefinitionManager.AlterTypeDefinition("User", t => t
                .WithPart(typeof(SmartWalkUserPart).Name)
            );

            return 1;
        }
    }
}